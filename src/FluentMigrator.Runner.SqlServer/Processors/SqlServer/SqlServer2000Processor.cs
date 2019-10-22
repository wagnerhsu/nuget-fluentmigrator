// ***********************************************************************
// Assembly         : FluentMigrator.Runner.SqlServer
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SqlServer2000Processor.cs" company="FluentMigrator Project">
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

using FluentMigrator.Runner.Helpers;

#region License
//
// Copyright (c) 2007-2018, Sean Chambers <schambers80@gmail.com>
// Copyright (c) 2010, Nathan Brown
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
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;

using FluentMigrator.Expressions;
using FluentMigrator.Runner.BatchParser;
using FluentMigrator.Runner.BatchParser.Sources;
using FluentMigrator.Runner.BatchParser.SpecialTokenSearchers;
using FluentMigrator.Runner.Generators.SqlServer;
using FluentMigrator.Runner.Initialization;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FluentMigrator.Runner.Processors.SqlServer
{
    /// <summary>
    /// Class SqlServer2000Processor.
    /// Implements the <see cref="FluentMigrator.Runner.Processors.GenericProcessorBase" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Processors.GenericProcessorBase" />
    public class SqlServer2000Processor : GenericProcessorBase
    {
        /// <summary>
        /// The service provider
        /// </summary>
        [CanBeNull]
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServer2000Processor"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="generator">The generator.</param>
        /// <param name="announcer">The announcer.</param>
        /// <param name="options">The options.</param>
        /// <param name="factory">The factory.</param>
        [Obsolete]
        public SqlServer2000Processor(IDbConnection connection, IMigrationGenerator generator, IAnnouncer announcer, IMigrationProcessorOptions options, IDbFactory factory)
            : base(connection, factory, generator, announcer, options)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServer2000Processor"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="generator">The generator.</param>
        /// <param name="options">The options.</param>
        /// <param name="connectionStringAccessor">The connection string accessor.</param>
        /// <param name="serviceProvider">The service provider.</param>
        public SqlServer2000Processor(
            [NotNull] ILogger<SqlServer2000Processor> logger,
            [NotNull] SqlServer2000Generator generator,
            [NotNull] IOptionsSnapshot<ProcessorOptions> options,
            [NotNull] IConnectionStringAccessor connectionStringAccessor,
            [NotNull] IServiceProvider serviceProvider)
            : this(SqlClientFactory.Instance, logger, generator, options, connectionStringAccessor, serviceProvider)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServer2000Processor"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="generator">The generator.</param>
        /// <param name="options">The options.</param>
        /// <param name="connectionStringAccessor">The connection string accessor.</param>
        /// <param name="serviceProvider">The service provider.</param>
        protected SqlServer2000Processor(
            DbProviderFactory factory,
            [NotNull] ILogger logger,
            [NotNull] SqlServer2000Generator generator,
            [NotNull] IOptionsSnapshot<ProcessorOptions> options,
            [NotNull] IConnectionStringAccessor connectionStringAccessor,
            [NotNull] IServiceProvider serviceProvider)
            : base(() => factory, generator, logger, options.Value, connectionStringAccessor)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Gets the database type
        /// </summary>
        /// <value>The type of the database.</value>
        public override string DatabaseType => "SqlServer2000";

        /// <summary>
        /// Gets the database type aliases
        /// </summary>
        /// <value>The database type aliases.</value>
        public override IList<string> DatabaseTypeAliases { get; } = new List<string>() { "SqlServer" };

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
            return true;
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
                return Exists("SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{0}'", FormatHelper.FormatSqlEscape(tableName));
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
            return Exists("SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{0}' AND COLUMN_NAME = '{1}'",
                FormatHelper.FormatSqlEscape(tableName),
                FormatHelper.FormatSqlEscape(columnName));
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
            return Exists("SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_CATALOG = DB_NAME() AND TABLE_NAME = '{0}' AND CONSTRAINT_NAME = '{1}'",
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
            return Exists("SELECT NULL FROM sysindexes WHERE name = '{0}'", FormatHelper.FormatSqlEscape(indexName));
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
        public override bool DefaultValueExists(string schemaName, string tableName, string columnName, object defaultValue)
        {
            return false;
        }

        /// <summary>
        /// Reads all data from all rows from a table
        /// </summary>
        /// <param name="schemaName">The schema name of the table</param>
        /// <param name="tableName">The table name</param>
        /// <returns>The data from the specified table</returns>
        public override DataSet ReadTableData(string schemaName, string tableName)
        {
            return Read("SELECT * FROM [{0}]", tableName);
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
        /// Execute an SQL statement
        /// </summary>
        /// <param name="template">The SQL statement</param>
        /// <param name="args">The arguments to replace in the SQL statement</param>
        public override void Execute(string template, params object[] args)
        {
            Process(string.Format(template, args));
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
            sql += "\nGO"; // make sure last batch is executed.
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
            EnsureConnectionIsOpen();

            expression.Operation?.Invoke(Connection, Transaction);
        }
    }
}
