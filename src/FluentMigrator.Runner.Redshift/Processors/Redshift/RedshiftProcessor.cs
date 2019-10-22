// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Redshift
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="RedshiftProcessor.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
//
// Copyright (c) 2018, Fluent Migrator Project
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

using FluentMigrator.Expressions;
using FluentMigrator.Runner.Generators.Redshift;
using FluentMigrator.Runner.Helpers;
using FluentMigrator.Runner.Initialization;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FluentMigrator.Runner.Processors.Redshift
{
    /// <summary>
    /// Class RedshiftProcessor.
    /// Implements the <see cref="FluentMigrator.Runner.Processors.GenericProcessorBase" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Processors.GenericProcessorBase" />
    public class RedshiftProcessor : GenericProcessorBase
    {
        /// <summary>
        /// The quoter
        /// </summary>
        private readonly RedshiftQuoter _quoter = new RedshiftQuoter();

        /// <summary>
        /// Gets the database type
        /// </summary>
        /// <value>The type of the database.</value>
        public override string DatabaseType => "Redshift";

        /// <summary>
        /// Gets the database type aliases
        /// </summary>
        /// <value>The database type aliases.</value>
        public override IList<string> DatabaseTypeAliases { get; } = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RedshiftProcessor"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="generator">The generator.</param>
        /// <param name="announcer">The announcer.</param>
        /// <param name="options">The options.</param>
        /// <param name="factory">The factory.</param>
        [Obsolete]
        public RedshiftProcessor(IDbConnection connection, IMigrationGenerator generator, IAnnouncer announcer, IMigrationProcessorOptions options, IDbFactory factory)
            : base(connection, factory, generator, announcer, options)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedshiftProcessor"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="generator">The generator.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="options">The options.</param>
        /// <param name="connectionStringAccessor">The connection string accessor.</param>
        public RedshiftProcessor(
            [NotNull] RedshiftDbFactory factory,
            [NotNull] RedshiftGenerator generator,
            [NotNull] ILogger<RedshiftProcessor> logger,
            [NotNull] IOptionsSnapshot<ProcessorOptions> options,
            [NotNull] IConnectionStringAccessor connectionStringAccessor)
            : base(() => factory.Factory, generator, logger, options.Value, connectionStringAccessor)
        {
        }

        /// <summary>
        /// Execute an SQL statement
        /// </summary>
        /// <param name="template">The SQL statement</param>
        /// <param name="args">The arguments to replace in the SQL statement</param>
        public override void Execute(string template, params object[] args)
        {
            Process(string.Format(template, args));
        }

        /// <summary>
        /// Tests if the schema exists
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <returns><c>true</c> when it exists</returns>
        public override bool SchemaExists(string schemaName)
        {
            return Exists("select * from information_schema.schemata where schema_name ilike '{0}'", FormatToSafeSchemaName(schemaName));
        }

        /// <summary>
        /// Tests if the table exists
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <param name="tableName">The table name</param>
        /// <returns><c>true</c> when it exists</returns>
        public override bool TableExists(string schemaName, string tableName)
        {
            return Exists("select * from information_schema.tables where table_schema ilike '{0}' and table_name ilike '{1}'", FormatToSafeSchemaName(schemaName), FormatToSafeName(tableName));
        }

        /// <summary>
        /// Tests if a column exists
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <param name="tableName">The table name</param>
        /// <param name="columnName">The column name</param>
        /// <returns><c>true</c> when it exists</returns>
        public override bool ColumnExists(string schemaName, string tableName, string columnName)
        {
            return Exists("select * from information_schema.columns where table_schema ilike '{0}' and table_name ilike '{1}' and column_name ilike '{2}'", FormatToSafeSchemaName(schemaName), FormatToSafeName(tableName), FormatToSafeName(columnName));
        }

        /// <summary>
        /// Tests if a constraint exists
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <param name="tableName">The table name</param>
        /// <param name="constraintName">The constraint name</param>
        /// <returns><c>true</c> when it exists</returns>
        public override bool ConstraintExists(string schemaName, string tableName, string constraintName)
            => false;

        /// <summary>
        /// Tests if an index exists
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <param name="tableName">The table name</param>
        /// <param name="indexName">The index name</param>
        /// <returns><c>true</c> when it exists</returns>
        public override bool IndexExists(string schemaName, string tableName, string indexName)
            => false;

        /// <summary>
        /// Tests if a sequence exists
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <param name="sequenceName">The sequence name</param>
        /// <returns><c>true</c> when it exists</returns>
        public override bool SequenceExists(string schemaName, string sequenceName)
            => false;

        /// <summary>
        /// Reads all data from all rows from a table
        /// </summary>
        /// <param name="schemaName">The schema name of the table</param>
        /// <param name="tableName">The table name</param>
        /// <returns>The data from the specified table</returns>
        public override DataSet ReadTableData(string schemaName, string tableName)
        {
            return Read("SELECT * FROM {0}", _quoter.QuoteTableName(tableName, schemaName));
        }

        /// <summary>
        /// Tests if a default value for a column exists
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <param name="tableName">The table name</param>
        /// <param name="columnName">The column name</param>
        /// <param name="defaultValue">The default value</param>
        /// <returns><c>true</c> when it exists</returns>
        public override bool DefaultValueExists(string schemaName, string tableName, string columnName, object defaultValue)
        {
            string defaultValueAsString = string.Format("%{0}%", FormatHelper.FormatSqlEscape(defaultValue.ToString()));
            return Exists("select * from information_schema.columns where table_schema ilike '{0}' and table_name ilike '{1}' and column_name ilike '{2}' and column_default like '{3}'", FormatToSafeSchemaName(schemaName), FormatToSafeName(tableName), FormatToSafeName(columnName), defaultValueAsString);
        }

        /// <summary>
        /// Executes and returns the result of an SQL query
        /// </summary>
        /// <param name="template">The SQL query</param>
        /// <param name="args">The arguments of the SQL query</param>
        /// <returns>The data from the specified SQL query</returns>
        public override DataSet Read(string template, params object[] args)
        {
            EnsureConnectionIsOpen();

            using (var command = CreateCommand(string.Format(template, args)))
            using (var reader = command.ExecuteReader())
            {
                return reader.ReadDataSet();
            }
        }

        /// <summary>
        /// Returns <c>true</c> if data could be found for the given SQL query
        /// </summary>
        /// <param name="template">The SQL query</param>
        /// <param name="args">The arguments of the SQL query</param>
        /// <returns><c>true</c> when the SQL query returned data</returns>
        public override bool Exists(string template, params object[] args)
        {
            EnsureConnectionIsOpen();

            using (var command = CreateCommand(string.Format(template, args)))
            using (var reader = command.ExecuteReader())
            {
                return reader.Read();
            }
        }

        /// <summary>
        /// Processes the specified SQL.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <exception cref="Exception"></exception>
        protected override void Process(string sql)
        {
            Logger.LogSql(sql);

            if (Options.PreviewOnly || string.IsNullOrEmpty(sql))
                return;

            EnsureConnectionIsOpen();

            using (var command = CreateCommand(sql))
            {
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    using (var message = new StringWriter())
                    {
                        message.WriteLine("An error occurred executing the following sql:");
                        message.WriteLine(sql);
                        message.WriteLine("The error was {0}", ex.Message);

                        throw new Exception(message.ToString(), ex);
                    }
                }
            }
        }

        /// <summary>
        /// Executes a DB operation
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public override void Process(PerformDBOperationExpression expression)
        {
            Logger.LogSay("Performing DB Operation");

            if (Options.PreviewOnly)
                return;

            EnsureConnectionIsOpen();

            expression.Operation?.Invoke(Connection, Transaction);
        }

        /// <summary>
        /// Formats the name of to safe schema.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <returns>System.String.</returns>
        private string FormatToSafeSchemaName(string schemaName)
        {
            return FormatHelper.FormatSqlEscape(_quoter.UnQuoteSchemaName(schemaName));
        }

        /// <summary>
        /// Formats the name of to safe.
        /// </summary>
        /// <param name="sqlName">Name of the SQL.</param>
        /// <returns>System.String.</returns>
        private string FormatToSafeName(string sqlName)
        {
            return FormatHelper.FormatSqlEscape(_quoter.UnQuote(sqlName));
        }
    }
}
