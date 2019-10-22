// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Db2
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="Db2ISeriesProcessor.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
//
// Copyright (c) 2007-2018, Sean Chambers <schambers80@gmail.com>
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
using System.Linq;

using FluentMigrator.Expressions;
using FluentMigrator.Runner.Generators;
using FluentMigrator.Runner.Generators.DB2;
using FluentMigrator.Runner.Generators.DB2.iSeries;
using FluentMigrator.Runner.Helpers;
using FluentMigrator.Runner.Initialization;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FluentMigrator.Runner.Processors.DB2.iSeries
{
    /// <summary>
    /// Class Db2ISeriesProcessor.
    /// Implements the <see cref="FluentMigrator.Runner.Processors.GenericProcessorBase" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Processors.GenericProcessorBase" />
    public class Db2ISeriesProcessor : GenericProcessorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Db2ISeriesProcessor"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="generator">The generator.</param>
        /// <param name="announcer">The announcer.</param>
        /// <param name="options">The options.</param>
        /// <param name="factory">The factory.</param>
        [Obsolete]
        public Db2ISeriesProcessor(IDbConnection connection, IMigrationGenerator generator, IAnnouncer announcer, IMigrationProcessorOptions options, IDbFactory factory)
            : base(connection, factory, generator, announcer, options)
        {
            Quoter = new Db2Quoter();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Db2ISeriesProcessor"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="generator">The generator.</param>
        /// <param name="quoter">The quoter.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="options">The options.</param>
        /// <param name="connectionStringAccessor">The connection string accessor.</param>
        public Db2ISeriesProcessor(
            [NotNull] Db2ISeriesDbFactory factory,
            [NotNull] Db2ISeriesGenerator generator,
            [NotNull] Db2ISeriesQuoter quoter,
            [NotNull] ILogger<Db2ISeriesProcessor> logger,
            [NotNull] IOptionsSnapshot<ProcessorOptions> options,
            [NotNull] IConnectionStringAccessor connectionStringAccessor)
            : base(() => factory.Factory, generator, logger, options.Value, connectionStringAccessor)
        {
            Quoter = quoter;
        }

        /// <summary>
        /// Gets the database type
        /// </summary>
        /// <value>The type of the database.</value>
        public override string DatabaseType => "DB2 iSeries";

        /// <summary>
        /// Gets the database type aliases
        /// </summary>
        /// <value>The database type aliases.</value>
        public override IList<string> DatabaseTypeAliases { get; } = new List<string> { "IBM DB2 iSeries", "DB2" };

        /// <summary>
        /// Gets or sets the quoter.
        /// </summary>
        /// <value>The quoter.</value>
        public IQuoter Quoter
        {
            get;
            set;
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
            var schema = string.IsNullOrEmpty(schemaName) ? string.Empty : "TABLE_SCHEMA = '" + FormatToSafeName(schemaName) + "' AND ";

            var doesExist = Exists("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE {0} TABLE_NAME = '{1}' AND COLUMN_NAME='{2}'", schema, FormatToSafeName(tableName), FormatToSafeName(columnName));
            return doesExist;
        }

        /// <summary>
        /// Tests if a constraint exists
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <param name="tableName">The table name</param>
        /// <param name="constraintName">The constraint name</param>
        /// <returns><c>true</c> when it exists</returns>
        public override bool ConstraintExists(string schemaName, string tableName, string constraintName)
        {
            var schema = string.IsNullOrEmpty(schemaName) ? string.Empty : "TABLE_SCHEMA = '" + FormatToSafeName(schemaName) + "' AND ";

            return Exists("SELECT CONSTRAINT_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE {0} TABLE_NAME = '{1}' AND CONSTRAINT_NAME='{2}'", schema, FormatToSafeName(tableName), FormatToSafeName(constraintName));
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
            var schema = string.IsNullOrEmpty(schemaName) ? string.Empty : "TABLE_SCHEMA = '" + FormatToSafeName(schemaName) + "' AND ";
            var defaultValueAsString = string.Format("%{0}%", FormatHelper.FormatSqlEscape(defaultValue.ToString()));

            return Exists("SELECT COLUMN_DEFAULT FROM INFORMATION_SCHEMA.COLUMNS WHERE {0} TABLE_NAME = '{1}' AND COLUMN_NAME = '{2}' AND COLUMN_DEFAULT LIKE '{3}'", schema, FormatToSafeName(tableName), columnName.ToUpper(), defaultValueAsString);
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
        /// Tests if an index exists
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <param name="tableName">The table name</param>
        /// <param name="indexName">The index name</param>
        /// <returns><c>true</c> when it exists</returns>
        public override bool IndexExists(string schemaName, string tableName, string indexName)
        {
            var schema = string.IsNullOrEmpty(schemaName) ? string.Empty : "INDEX_SCHEMA = '" + FormatToSafeName(schemaName) + "' AND ";

            var doesExist = Exists(
                "SELECT NAME FROM INFORMATION_SCHEMA.SYSINDEXES WHERE {0}TABLE_NAME = '{1}' AND NAME = '{2}'",
                schema,
                FormatToSafeName(tableName),
                FormatToSafeName(indexName));

            return doesExist;
        }

        /// <summary>
        /// Executes a DB operation
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public override void Process(PerformDBOperationExpression expression)
        {
            Logger.LogSay("Performing DB Operation");

            if (Options.PreviewOnly)
            {
                return;
            }

            EnsureConnectionIsOpen();

            expression.Operation?.Invoke(Connection, Transaction);
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
        /// Reads all data from all rows from a table
        /// </summary>
        /// <param name="schemaName">The schema name of the table</param>
        /// <param name="tableName">The table name</param>
        /// <returns>The data from the specified table</returns>
        public override DataSet ReadTableData(string schemaName, string tableName)
        {
            return Read("SELECT * FROM {0}", Quoter.QuoteTableName(tableName, schemaName));
        }

        /// <summary>
        /// Tests if the schema exists
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <returns><c>true</c> when it exists</returns>
        public override bool SchemaExists(string schemaName)
        {
            return Exists("SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = '{0}'", FormatToSafeName(schemaName));
        }

        /// <summary>
        /// Tests if a sequence exists
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <param name="sequenceName">The sequence name</param>
        /// <returns><c>true</c> when it exists</returns>
        public override bool SequenceExists(string schemaName, string sequenceName)
        {
            return false;
        }

        /// <summary>
        /// Tests if the table exists
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <param name="tableName">The table name</param>
        /// <returns><c>true</c> when it exists</returns>
        public override bool TableExists(string schemaName, string tableName)
        {
            var schema = string.IsNullOrEmpty(schemaName) ? string.Empty : "TABLE_SCHEMA = '" + FormatToSafeName(schemaName) + "' AND ";

            return Exists("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE {0}TABLE_NAME = '{1}'", schema, FormatToSafeName(tableName));
        }

        /// <summary>
        /// Processes the specified SQL.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        protected override void Process(string sql)
        {
            Logger.LogSql(sql);

            if (Options.PreviewOnly || string.IsNullOrEmpty(sql))
            {
                return;
            }

            EnsureConnectionIsOpen();

            using (var command = CreateCommand(sql))
            {
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Formats the name of to safe.
        /// </summary>
        /// <param name="sqlName">Name of the SQL.</param>
        /// <returns>System.String.</returns>
        private string FormatToSafeName(string sqlName)
        {
            var rawName = Quoter.UnQuote(sqlName);

            return rawName.Contains('\'') ? FormatHelper.FormatSqlEscape(rawName) : rawName.ToUpper();
        }
    }
}
