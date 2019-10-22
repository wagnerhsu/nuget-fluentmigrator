// ***********************************************************************
// Assembly         : FluentMigrator.Runner.SqlServer
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SqlServerProcessor.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
// Copyright (c) 2018, Fluent Migrator Project
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
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

using FluentMigrator.Expressions;
using FluentMigrator.Runner.BatchParser;
using FluentMigrator.Runner.BatchParser.Sources;
using FluentMigrator.Runner.BatchParser.SpecialTokenSearchers;
using FluentMigrator.Runner.Generators;
using FluentMigrator.Runner.Generators.Generic;
using FluentMigrator.Runner.Helpers;
using FluentMigrator.Runner.Initialization;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FluentMigrator.Runner.Processors.SqlServer
{
    /// <summary>
    /// Class SqlServerProcessor.
    /// Implements the <see cref="FluentMigrator.Runner.Processors.GenericProcessorBase" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Processors.GenericProcessorBase" />
    public class SqlServerProcessor : GenericProcessorBase
    {
        /// <summary>
        /// The service provider
        /// </summary>
        [CanBeNull]
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// The SQL schema exists
        /// </summary>
        private const string SqlSchemaExists = "SELECT 1 WHERE EXISTS (SELECT * FROM sys.schemas WHERE NAME = '{0}') ";
        /// <summary>
        /// The table exists
        /// </summary>
        private const string TABLE_EXISTS = "SELECT 1 WHERE EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '{0}' AND TABLE_NAME = '{1}')";
        /// <summary>
        /// The column exists
        /// </summary>
        private const string COLUMN_EXISTS = "SELECT 1 WHERE EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = '{0}' AND TABLE_NAME = '{1}' AND COLUMN_NAME = '{2}')";
        /// <summary>
        /// The constraint exists
        /// </summary>
        private const string CONSTRAINT_EXISTS = "SELECT 1 WHERE EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_CATALOG = DB_NAME() AND TABLE_SCHEMA = '{0}' AND TABLE_NAME = '{1}' AND CONSTRAINT_NAME = '{2}')";
        /// <summary>
        /// The index exists
        /// </summary>
        private const string INDEX_EXISTS = "SELECT 1 WHERE EXISTS (SELECT * FROM sys.indexes WHERE name = '{0}' and object_id=OBJECT_ID('{1}.{2}'))";
        /// <summary>
        /// The sequences exists
        /// </summary>
        private const string SEQUENCES_EXISTS = "SELECT 1 WHERE EXISTS (SELECT * FROM INFORMATION_SCHEMA.SEQUENCES WHERE SEQUENCE_SCHEMA = '{0}' AND SEQUENCE_NAME = '{1}' )";
        /// <summary>
        /// The defaultvalue exists
        /// </summary>
        private const string DEFAULTVALUE_EXISTS = "SELECT 1 WHERE EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = '{0}' AND TABLE_NAME = '{1}' AND COLUMN_NAME = '{2}' AND COLUMN_DEFAULT LIKE '{3}')";

        /// <summary>
        /// Gets the database type
        /// </summary>
        /// <value>The type of the database.</value>
        public override string DatabaseType { get;}

        /// <summary>
        /// Gets the database type aliases
        /// </summary>
        /// <value>The database type aliases.</value>
        public override IList<string> DatabaseTypeAliases { get; }

        /// <summary>
        /// Gets the quoter.
        /// </summary>
        /// <value>The quoter.</value>
        public IQuoter Quoter { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerProcessor"/> class.
        /// </summary>
        /// <param name="databaseTypes">The database types.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="generator">The generator.</param>
        /// <param name="announcer">The announcer.</param>
        /// <param name="options">The options.</param>
        /// <param name="factory">The factory.</param>
        [Obsolete]
        public SqlServerProcessor(
            IEnumerable<string> databaseTypes,
            IDbConnection connection,
            GenericGenerator generator,
            IAnnouncer announcer,
            [NotNull] IMigrationProcessorOptions options,
            IDbFactory factory)
            : base(connection, factory, generator, announcer, options)
        {
            var dbTypes = databaseTypes.ToList();
            DatabaseType = dbTypes.First();
            DatabaseTypeAliases = dbTypes.Skip(1).ToList();
            Quoter = generator?.Quoter;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerProcessor"/> class.
        /// </summary>
        /// <param name="databaseTypes">The database types.</param>
        /// <param name="generator">The generator.</param>
        /// <param name="quoter">The quoter.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="options">The options.</param>
        /// <param name="connectionStringAccessor">The connection string accessor.</param>
        /// <param name="serviceProvider">The service provider.</param>
        protected SqlServerProcessor(
            [NotNull, ItemNotNull] IEnumerable<string> databaseTypes,
            [NotNull] IMigrationGenerator generator,
            [NotNull] IQuoter quoter,
            [NotNull] ILogger logger,
            [NotNull] IOptionsSnapshot<ProcessorOptions> options,
            [NotNull] IConnectionStringAccessor connectionStringAccessor,
            [NotNull] IServiceProvider serviceProvider)
            : this(databaseTypes, SqlClientFactory.Instance, generator, quoter, logger, options, connectionStringAccessor, serviceProvider)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerProcessor"/> class.
        /// </summary>
        /// <param name="databaseTypes">The database types.</param>
        /// <param name="factory">The factory.</param>
        /// <param name="generator">The generator.</param>
        /// <param name="quoter">The quoter.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="options">The options.</param>
        /// <param name="connectionStringAccessor">The connection string accessor.</param>
        /// <param name="serviceProvider">The service provider.</param>
        protected SqlServerProcessor(
            [NotNull, ItemNotNull] IEnumerable<string> databaseTypes,
            [NotNull] DbProviderFactory factory,
            [NotNull] IMigrationGenerator generator,
            [NotNull] IQuoter quoter,
            [NotNull] ILogger logger,
            [NotNull] IOptionsSnapshot<ProcessorOptions> options,
            [NotNull] IConnectionStringAccessor connectionStringAccessor,
            [NotNull] IServiceProvider serviceProvider)
            : base(() => factory, generator, logger, options.Value, connectionStringAccessor)
        {
            _serviceProvider = serviceProvider;
            var dbTypes = databaseTypes.ToList();
            DatabaseType = dbTypes.First();
            DatabaseTypeAliases = dbTypes.Skip(1).ToList();
            Quoter = quoter;
        }

        /// <summary>
        /// Safes the name of the schema.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <returns>System.String.</returns>
        private static string SafeSchemaName(string schemaName)
        {
            return string.IsNullOrEmpty(schemaName) ? "dbo" : FormatHelper.FormatSqlEscape(schemaName);
        }

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        public override void BeginTransaction()
        {
            base.BeginTransaction();
            Logger.LogSql("BEGIN TRANSACTION");
        }

        /// <summary>
        /// Commits the transaction.
        /// </summary>
        public override void CommitTransaction()
        {
            base.CommitTransaction();
            Logger.LogSql("COMMIT TRANSACTION");
        }

        /// <summary>
        /// Rollbacks the transaction.
        /// </summary>
        public override void RollbackTransaction()
        {
            if (Transaction == null)
            {
                return;
            }

            base.RollbackTransaction();
            Logger.LogSql("ROLLBACK TRANSACTION");
        }

        /// <summary>
        /// Tests if the schema exists
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <returns><c>true</c> when it exists</returns>
        public override bool SchemaExists(string schemaName)
        {
            return Exists(SqlSchemaExists, SafeSchemaName(schemaName));
        }

        /// <summary>
        /// Tests if the table exists
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <param name="tableName">The table name</param>
        /// <returns><c>true</c> when it exists</returns>
        public override bool TableExists(string schemaName, string tableName)
        {
            try
            {
                return Exists(TABLE_EXISTS, SafeSchemaName(schemaName),
                    FormatHelper.FormatSqlEscape(tableName));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return false;
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
            return Exists(COLUMN_EXISTS, SafeSchemaName(schemaName),
                FormatHelper.FormatSqlEscape(tableName), FormatHelper.FormatSqlEscape(columnName));
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
            return Exists(CONSTRAINT_EXISTS, SafeSchemaName(schemaName),
                FormatHelper.FormatSqlEscape(tableName), FormatHelper.FormatSqlEscape(constraintName));
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
            return Exists(INDEX_EXISTS,
                FormatHelper.FormatSqlEscape(indexName), SafeSchemaName(schemaName), FormatHelper.FormatSqlEscape(tableName));
        }

        /// <summary>
        /// Tests if a sequence exists
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <param name="sequenceName">The sequence name</param>
        /// <returns><c>true</c> when it exists</returns>
        public override bool SequenceExists(string schemaName, string sequenceName)
        {
            return Exists(SEQUENCES_EXISTS, SafeSchemaName(schemaName),
                FormatHelper.FormatSqlEscape(sequenceName));
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
            return Exists(DEFAULTVALUE_EXISTS, SafeSchemaName(schemaName),
                FormatHelper.FormatSqlEscape(tableName),
                FormatHelper.FormatSqlEscape(columnName), defaultValueAsString);
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
            {
                var result = command.ExecuteScalar();
                return DBNull.Value != result && Convert.ToInt32(result) == 1;
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
            return Read("SELECT * FROM [{0}].[{1}]", SafeSchemaName(schemaName), tableName);
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

            if (ContainsGo(sql))
            {
                ExecuteBatchNonQuery(sql);
            }
            else
            {
                ExecuteNonQuery(sql);
            }
        }

        /// <summary>
        /// Determines whether the specified SQL contains go.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns><c>true</c> if the specified SQL contains go; otherwise, <c>false</c>.</returns>
        private bool ContainsGo(string sql)
        {
            var containsGo = false;
            var parser = _serviceProvider?.GetService<SqlServerBatchParser>() ?? new SqlServerBatchParser();
            parser.SpecialToken += (sender, args) => containsGo = true;
            using (var source = new TextReaderSource(new StringReader(sql), true))
            {
                parser.Process(source);
            }

            return containsGo;
        }

        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <exception cref="Exception"></exception>
        private void ExecuteNonQuery(string sql)
        {
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
                        message.WriteLine("An error occured executing the following sql:");
                        message.WriteLine(sql);
                        message.WriteLine("The error was {0}", ex.Message);

                        throw new Exception(message.ToString(), ex);
                    }
                }
            }
        }

        /// <summary>
        /// Executes the batch non query.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <exception cref="Exception"></exception>
        private void ExecuteBatchNonQuery(string sql)
        {
            var sqlBatch = string.Empty;

            try
            {
                var parser = _serviceProvider?.GetService<SqlServerBatchParser>() ?? new SqlServerBatchParser();
                parser.SqlText += (sender, args) => sqlBatch = args.SqlText.Trim();
                parser.SpecialToken += (sender, args) =>
                {
                    if (string.IsNullOrEmpty(sqlBatch))
                    {
                        return;
                    }

                    if (args.Opaque is GoSearcher.GoSearcherParameters goParams)
                    {
                        using (var command = CreateCommand(sqlBatch))
                        {
                            for (var i = 0; i != goParams.Count; ++i)
                            {
                                command.ExecuteNonQuery();
                            }
                        }
                    }

                    sqlBatch = null;
                };

                using (var source = new TextReaderSource(new StringReader(sql), true))
                {
                    parser.Process(source, stripComments: Options.StripComments);
                }

                if (!string.IsNullOrEmpty(sqlBatch))
                {
                    using (var command = CreateCommand(sqlBatch))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                using (var message = new StringWriter())
                {
                    message.WriteLine("An error occured executing the following sql:");
                    message.WriteLine(string.IsNullOrEmpty(sqlBatch) ? sql : sqlBatch);
                    message.WriteLine("The error was {0}", ex.Message);

                    throw new Exception(message.ToString(), ex);
                }
            }
        }

        /// <summary>
        /// Processes the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
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
    }
}
