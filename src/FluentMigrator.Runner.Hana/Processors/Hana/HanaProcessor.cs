// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Hana
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="HanaProcessor.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

using FluentMigrator.Expressions;
using FluentMigrator.Runner.Generators;
using FluentMigrator.Runner.Generators.Hana;
using FluentMigrator.Runner.Helpers;
using FluentMigrator.Runner.Initialization;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FluentMigrator.Runner.Processors.Hana
{
    /// <summary>
    /// Class HanaProcessor.
    /// Implements the <see cref="FluentMigrator.Runner.Processors.GenericProcessorBase" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Processors.GenericProcessorBase" />
    public class HanaProcessor : GenericProcessorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HanaProcessor"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="generator">The generator.</param>
        /// <param name="announcer">The announcer.</param>
        /// <param name="options">The options.</param>
        /// <param name="factory">The factory.</param>
        [Obsolete]
        public HanaProcessor(IDbConnection connection, IMigrationGenerator generator, IAnnouncer announcer, IMigrationProcessorOptions options, IDbFactory factory)
            : base(connection, factory, generator, announcer, options)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HanaProcessor"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="generator">The generator.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="options">The options.</param>
        /// <param name="connectionStringAccessor">The connection string accessor.</param>
        public HanaProcessor(
            [NotNull] HanaDbFactory factory,
            [NotNull] HanaGenerator generator,
            [NotNull] ILogger<HanaProcessor> logger,
            [NotNull] IOptionsSnapshot<ProcessorOptions> options,
            [NotNull] IConnectionStringAccessor connectionStringAccessor)
            : base(() => factory.Factory, generator, logger, options.Value, connectionStringAccessor)
        {
        }

        /// <summary>
        /// Gets the database type
        /// </summary>
        /// <value>The type of the database.</value>
        public override string DatabaseType => "Hana";

        /// <summary>
        /// Gets the database type aliases
        /// </summary>
        /// <value>The database type aliases.</value>
        public override IList<string> DatabaseTypeAliases { get; } = new List<string>();

        /// <summary>
        /// Gets the quoter.
        /// </summary>
        /// <value>The quoter.</value>
        public IQuoter Quoter => ((HanaGenerator)Generator).Quoter;

        /// <summary>
        /// Tests if the schema exists
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <returns><c>true</c> when it exists</returns>
        public override bool SchemaExists(string schemaName)
        {
            return false;
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
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentNullException(nameof(tableName));

            return Exists(
                "SELECT 1 FROM TABLES WHERE SCHEMA_NAME = CURRENT_SCHEMA AND TABLE_NAME = '{0}'",
                FormatHelper.FormatSqlEscape(Quoter.UnQuote(tableName)));
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
                throw new ArgumentNullException(nameof(tableName));
            if (columnName == null)
                throw new ArgumentNullException(nameof(columnName));

            return Exists("SELECT 1 FROM TABLE_COLUMNS WHERE SCHEMA_NAME = CURRENT_SCHEMA AND upper(TABLE_NAME) = '{0}' AND upper(COLUMN_NAME) = '{1}'",
                FormatHelper.FormatSqlEscape(Quoter.UnQuote(tableName).ToUpper()),
                FormatHelper.FormatSqlEscape(Quoter.UnQuote(columnName).ToUpper()));
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
                throw new ArgumentNullException(nameof(tableName));
            if (constraintName == null)
                throw new ArgumentNullException(nameof(constraintName));

            if (constraintName.Length == 0)
                return false;

            return Exists("SELECT 1 FROM CONSTRAINTS WHERE SCHEMA_NAME = CURRENT_SCHEMA and upper(CONSTRAINT_NAME) = '{0}'",
                FormatHelper.FormatSqlEscape(Quoter.UnQuote(constraintName).ToUpper()));
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
                throw new ArgumentNullException(nameof(tableName));
            if (indexName == null)
                throw new ArgumentNullException(nameof(indexName));

            if (indexName.Length == 0)
                return false;

            return Exists("SELECT 1 FROM INDEXES WHERE SCHEMA_NAME = CURRENT_SCHEMA AND upper(INDEX_NAME) = '{0}'",
                    FormatHelper.FormatSqlEscape(Quoter.UnQuote(indexName).ToUpper()));
        }

        /// <summary>
        /// Tests if a sequence exists
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <param name="sequenceName">The sequence name</param>
        /// <returns><c>true</c> when it exists</returns>
        /// <exception cref="ArgumentNullException">sequenceName</exception>
        public override bool SequenceExists(string schemaName, string sequenceName)
        {
            if (sequenceName == null)
                throw new ArgumentNullException(nameof(sequenceName));

            if (string.IsNullOrEmpty(sequenceName))
                return false;

            return Exists("SELECT 1 FROM SEQUENCES WHERE SCHEMA_NAME = CURRENT_SCHEMA and upper(SEQUENCE_NAME) = '{0}'",
                FormatHelper.FormatSqlEscape(Quoter.UnQuote(sequenceName).ToUpper()));
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
                throw new ArgumentNullException(nameof(template));

            EnsureConnectionIsOpen();

            var querySql = string.Format(template, args);

            Logger.LogSql($"{querySql};");

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
                throw new ArgumentNullException(nameof(tableName));

            return Read("SELECT * FROM {0}", Quoter.QuoteTableName(tableName, schemaName));
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
                throw new ArgumentNullException(nameof(template));

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
                return;

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
                return;

            EnsureConnectionIsOpen();

            var batches = Regex.Split(sql, @"^\s*;\s*$", RegexOptions.Multiline)
                .Where(x => !string.IsNullOrEmpty(x))
                .Select(c => c.Trim());

            foreach (var batch in batches)
            {
                var batchCommand = batch.EndsWith(";")
                    ? batch.Remove(batch.Length - 1)
                    : batch;

                using (var command = CreateCommand(batchCommand))
                    command.ExecuteNonQuery();
            }
        }

    }
}
