// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Db2
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="Db2Processor.cs" company="FluentMigrator Project">
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

using FluentMigrator.Expressions;
using FluentMigrator.Runner.Generators;
using FluentMigrator.Runner.Generators.DB2;
using FluentMigrator.Runner.Helpers;
using FluentMigrator.Runner.Initialization;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FluentMigrator.Runner.Processors.DB2
{
    /// <summary>
    /// Class Db2Processor.
    /// Implements the <see cref="FluentMigrator.Runner.Processors.GenericProcessorBase" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Processors.GenericProcessorBase" />
    public class Db2Processor : GenericProcessorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Db2Processor"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="generator">The generator.</param>
        /// <param name="announcer">The announcer.</param>
        /// <param name="options">The options.</param>
        /// <param name="factory">The factory.</param>
        [Obsolete]
        public Db2Processor(IDbConnection connection, IMigrationGenerator generator, IAnnouncer announcer, IMigrationProcessorOptions options, IDbFactory factory)
            : base(connection, factory, generator, announcer, options)
        {
            Quoter = new Db2Quoter();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Db2Processor"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="generator">The generator.</param>
        /// <param name="quoter">The quoter.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="options">The options.</param>
        /// <param name="connectionStringAccessor">The connection string accessor.</param>
        public Db2Processor(
            [NotNull] Db2DbFactory factory,
            [NotNull] Db2Generator generator,
            [NotNull] Db2Quoter quoter,
            [NotNull] ILogger<Db2Processor> logger,
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
        public override string DatabaseType => "DB2";

        /// <summary>
        /// Gets the database type aliases
        /// </summary>
        /// <value>The database type aliases.</value>
        public override IList<string> DatabaseTypeAliases { get; } = new List<string> { "IBM DB2" };

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
            var conditions = new List<string>
            {
                BuildEqualityComparison("TABNAME", tableName),
                BuildEqualityComparison("COLNAME", columnName),
            };

            if (!string.IsNullOrEmpty(schemaName))
                conditions.Add(BuildEqualityComparison("TABSCHEMA", schemaName));

            var condition = string.Join(" AND ", conditions);

            var doesExist = Exists("SELECT COLNAME FROM SYSCAT.COLUMNS WHERE {0}", condition);
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
            var conditions = new List<string>
            {
                BuildEqualityComparison("TABNAME", tableName),
                BuildEqualityComparison("CONSTNAME", constraintName),
            };

            if (!string.IsNullOrEmpty(schemaName))
                conditions.Add(BuildEqualityComparison("TABSCHEMA", schemaName));

            var condition = string.Join(" AND ", conditions);

            return Exists("SELECT CONSTNAME FROM SYSCAT.TABCONST WHERE {0}", condition);
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
            var defaultValueAsString = string.Format("%{0}%", FormatHelper.FormatSqlEscape(defaultValue.ToString()));

            var conditions = new List<string>
            {
                BuildEqualityComparison("TABNAME", tableName),
                BuildEqualityComparison("COLNAME", columnName),
                $"\"DEFAULT\" LIKE '{defaultValueAsString}'",
            };

            if (!string.IsNullOrEmpty(schemaName))
                conditions.Add(BuildEqualityComparison("TABSCHEMA", schemaName));

            var condition = string.Join(" AND ", conditions);

            return Exists("SELECT \"DEFAULT\" FROM SYSCAT.COLUMNS WHERE {0}", condition);
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
            var conditions = new List<string>
            {
                BuildEqualityComparison("TABNAME", tableName),
                BuildEqualityComparison("INDNAME", indexName),
            };

            if (!string.IsNullOrEmpty(schemaName))
                conditions.Add(BuildEqualityComparison("INDSCHEMA", schemaName));

            var condition = string.Join(" AND ", conditions);

            var doesExist = Exists("SELECT INDNAME FROM SYSCAT.INDEXES WHERE {0}", condition);

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
            var conditions = new List<string>
            {
                BuildEqualityComparison("SCHEMANAME", schemaName),
            };

            var condition = string.Join(" AND ", conditions);

            return Exists("SELECT SCHEMANAME FROM SYSCAT.SCHEMATA WHERE {0}", condition);
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
            var conditions = new List<string>
            {
                BuildEqualityComparison("TABNAME", tableName),
            };

            if (!string.IsNullOrEmpty(schemaName))
                conditions.Add(BuildEqualityComparison("TABSCHEMA", schemaName));

            var condition = string.Join(" AND ", conditions);

            return Exists("SELECT TABNAME FROM SYSCAT.TABLES WHERE {0}", condition);
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
        /// Builds the equality comparison.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        private string BuildEqualityComparison(string columnName, string value)
        {
            if (Quoter.IsQuoted(value))
            {
                return $"{Quoter.QuoteColumnName(columnName)}='{FormatHelper.FormatSqlEscape(Quoter.UnQuote(value))}'";
            }

            return $"LCASE({Quoter.QuoteColumnName(columnName)})=LCASE('{FormatHelper.FormatSqlEscape(Quoter.UnQuote(value))}')";
        }
    }
}
