// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Oracle
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="OracleProcessorBase.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
// Copyright (c) 2018, FluentMigrator Project
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

using FluentMigrator.Expressions;
using FluentMigrator.Runner.Generators;
using FluentMigrator.Runner.Generators.Oracle;
using FluentMigrator.Runner.Helpers;
using FluentMigrator.Runner.Initialization;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FluentMigrator.Runner.Processors.Oracle
{
    /// <summary>
    /// Class OracleProcessorBase.
    /// Implements the <see cref="FluentMigrator.Runner.Processors.GenericProcessorBase" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Processors.GenericProcessorBase" />
    public class OracleProcessorBase : GenericProcessorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OracleProcessorBase"/> class.
        /// </summary>
        /// <param name="databaseType">Type of the database.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="generator">The generator.</param>
        /// <param name="announcer">The announcer.</param>
        /// <param name="options">The options.</param>
        /// <param name="factory">The factory.</param>
        [Obsolete]
        protected OracleProcessorBase(
            [NotNull] string databaseType,
            IDbConnection connection,
            IMigrationGenerator generator,
            IAnnouncer announcer,
            IMigrationProcessorOptions options,
            IDbFactory factory)
            : base(connection, factory, generator, announcer, options)
        {
            DatabaseType = databaseType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OracleProcessorBase"/> class.
        /// </summary>
        /// <param name="databaseType">Type of the database.</param>
        /// <param name="factory">The factory.</param>
        /// <param name="generator">The generator.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="options">The options.</param>
        /// <param name="connectionStringAccessor">The connection string accessor.</param>
        protected OracleProcessorBase(
            [NotNull] string databaseType,
            [NotNull] OracleBaseDbFactory factory,
            [NotNull] IMigrationGenerator generator,
            [NotNull] ILogger logger,
            [NotNull] IOptionsSnapshot<ProcessorOptions> options,
            [NotNull] IConnectionStringAccessor connectionStringAccessor)
            : base(() => factory.Factory, generator, logger, options.Value, connectionStringAccessor)
        {
            DatabaseType = databaseType;
        }

        /// <summary>
        /// Gets the database type
        /// </summary>
        /// <value>The type of the database.</value>
        public override string DatabaseType { get; }

        /// <summary>
        /// Gets the database type aliases
        /// </summary>
        /// <value>The database type aliases.</value>
        public override IList<string> DatabaseTypeAliases { get; } = new List<string>() { "Oracle" };

        /// <summary>
        /// Gets the quoter.
        /// </summary>
        /// <value>The quoter.</value>
        public IQuoter Quoter => ((OracleGenerator) Generator).Quoter;

        /// <summary>
        /// Tests if the schema exists
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <returns><c>true</c> when it exists</returns>
        /// <exception cref="ArgumentNullException">schemaName</exception>
        public override bool SchemaExists(string schemaName)
        {
            if (schemaName == null)
            {
                throw new ArgumentNullException(nameof(schemaName));
            }

            if (schemaName.Length == 0)
            {
                return false;
            }

            return Exists("SELECT 1 FROM ALL_USERS WHERE USERNAME = '{0}'", schemaName.ToUpper());
        }

        /// <summary>
        /// Tests if the table exists
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <param name="tableName">The table name</param>
        /// <returns><c>true</c> when it exists</returns>
        /// <exception cref="ArgumentNullException">tableName</exception>
        public override bool TableExists(string schemaName, string tableName)
        {
            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            if (tableName.Length == 0)
            {
                return false;
            }

            if (string.IsNullOrEmpty(schemaName))
            {
                return Exists("SELECT 1 FROM USER_TABLES WHERE upper(TABLE_NAME) = '{0}'",
                    FormatHelper.FormatSqlEscape(tableName.ToUpper()));
            }

            return Exists("SELECT 1 FROM ALL_TABLES WHERE upper(OWNER) = '{0}' AND upper(TABLE_NAME) = '{1}'",
                schemaName.ToUpper(), FormatHelper.FormatSqlEscape(tableName.ToUpper()));
        }

        /// <summary>
        /// Tests if a column exists
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <param name="tableName">The table name</param>
        /// <param name="columnName">The column name</param>
        /// <returns><c>true</c> when it exists</returns>
        /// <exception cref="ArgumentNullException">tableName</exception>
        /// <exception cref="ArgumentNullException">columnName</exception>
        public override bool ColumnExists(string schemaName, string tableName, string columnName)
        {
            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            if (columnName == null)
            {
                throw new ArgumentNullException(nameof(columnName));
            }

            if (columnName.Length == 0 || tableName.Length == 0)
            {
                return false;
            }

            if (string.IsNullOrEmpty(schemaName))
            {
                return Exists(
                    "SELECT 1 FROM USER_TAB_COLUMNS WHERE upper(TABLE_NAME) = '{0}' AND upper(COLUMN_NAME) = '{1}'",
                    FormatHelper.FormatSqlEscape(tableName.ToUpper()),
                    FormatHelper.FormatSqlEscape(columnName.ToUpper()));
            }

            return Exists(
                "SELECT 1 FROM ALL_TAB_COLUMNS WHERE upper(OWNER) = '{0}' AND upper(TABLE_NAME) = '{1}' AND upper(COLUMN_NAME) = '{2}'",
                schemaName.ToUpper(), FormatHelper.FormatSqlEscape(tableName.ToUpper()),
                FormatHelper.FormatSqlEscape(columnName.ToUpper()));
        }

        /// <summary>
        /// Tests if a constraint exists
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <param name="tableName">The table name</param>
        /// <param name="constraintName">The constraint name</param>
        /// <returns><c>true</c> when it exists</returns>
        /// <exception cref="ArgumentNullException">tableName</exception>
        /// <exception cref="ArgumentNullException">constraintName</exception>
        public override bool ConstraintExists(string schemaName, string tableName, string constraintName)
        {
            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            if (constraintName == null)
            {
                throw new ArgumentNullException(nameof(constraintName));
            }

            //In Oracle DB constraint name is unique within the schema, so the table name is not used in the query

            if (constraintName.Length == 0)
            {
                return false;
            }

            if (string.IsNullOrEmpty(schemaName))
            {
                return Exists("SELECT 1 FROM USER_CONSTRAINTS WHERE upper(CONSTRAINT_NAME) = '{0}'",
                    FormatHelper.FormatSqlEscape(constraintName.ToUpper()));
            }

            return Exists("SELECT 1 FROM ALL_CONSTRAINTS WHERE upper(OWNER) = '{0}' AND upper(CONSTRAINT_NAME) = '{1}'",
                schemaName.ToUpper(),
                FormatHelper.FormatSqlEscape(constraintName.ToUpper()));
        }

        /// <summary>
        /// Tests if an index exists
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <param name="tableName">The table name</param>
        /// <param name="indexName">The index name</param>
        /// <returns><c>true</c> when it exists</returns>
        /// <exception cref="ArgumentNullException">tableName</exception>
        /// <exception cref="ArgumentNullException">indexName</exception>
        public override bool IndexExists(string schemaName, string tableName, string indexName)
        {
            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            if (indexName == null)
            {
                throw new ArgumentNullException(nameof(indexName));
            }

            //In Oracle DB index name is unique within the schema, so the table name is not used in the query

            if (indexName.Length == 0)
            {
                return false;
            }

            if (string.IsNullOrEmpty(schemaName))
            {
                return Exists("SELECT 1 FROM USER_INDEXES WHERE upper(INDEX_NAME) = '{0}'",
                    FormatHelper.FormatSqlEscape(indexName.ToUpper()));
            }

            return Exists("SELECT 1 FROM ALL_INDEXES WHERE upper(OWNER) = '{0}' AND upper(INDEX_NAME) = '{1}'",
                schemaName.ToUpper(), FormatHelper.FormatSqlEscape(indexName.ToUpper()));
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
        /// Tests if a default value for a column exists
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <param name="tableName">The table name</param>
        /// <param name="columnName">The column name</param>
        /// <param name="defaultValue">The default value</param>
        /// <returns><c>true</c> when it exists</returns>
        public override bool DefaultValueExists(string schemaName, string tableName, string columnName,
            object defaultValue)
        {
            return false;
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
        /// <exception cref="ArgumentNullException">template</exception>
        public override bool Exists(string template, params object[] args)
        {
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }

            EnsureConnectionIsOpen();

            Logger.LogSql(string.Format(template, args));
            using (var command = CreateCommand(string.Format(template, args)))
            using (var reader = command.ExecuteReader())
            {
                return reader.Read();
            }
        }

        /// <summary>
        /// Reads all data from all rows from a table
        /// </summary>
        /// <param name="schemaName">The schema name of the table</param>
        /// <param name="tableName">The table name</param>
        /// <returns>The data from the specified table</returns>
        /// <exception cref="ArgumentNullException">tableName</exception>
        public override DataSet ReadTableData(string schemaName, string tableName)
        {
            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            if (string.IsNullOrEmpty(schemaName))
            {
                return Read("SELECT * FROM {0}", Quoter.QuoteTableName(tableName));
            }

            return Read("SELECT * FROM {0}.{1}", Quoter.QuoteSchemaName(schemaName), Quoter.QuoteTableName(tableName));
        }

        /// <summary>
        /// Executes and returns the result of an SQL query
        /// </summary>
        /// <param name="template">The SQL query</param>
        /// <param name="args">The arguments of the SQL query</param>
        /// <returns>The data from the specified SQL query</returns>
        /// <exception cref="ArgumentNullException">template</exception>
        public override DataSet Read(string template, params object[] args)
        {
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }

            EnsureConnectionIsOpen();

            using (var command = CreateCommand(string.Format(template, args)))
            using (var reader = command.ExecuteReader())
            {
                return reader.ReadDataSet();
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
            {
                return;
            }

            EnsureConnectionIsOpen();

            expression.Operation?.Invoke(Connection, Transaction);
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

            var batches = Regex.Split(sql, @"^\s*;\s*$", RegexOptions.Multiline)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrEmpty(x));

            foreach (var batch in batches)
            {
                using (var command = CreateCommand(batch))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
