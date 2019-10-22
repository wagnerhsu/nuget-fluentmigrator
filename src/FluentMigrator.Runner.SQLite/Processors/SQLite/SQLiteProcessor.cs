// ***********************************************************************
// Assembly         : FluentMigrator.Runner.SQLite
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SQLiteProcessor.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
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
using System.IO;

using FluentMigrator.Expressions;
using FluentMigrator.Runner.BatchParser;
using FluentMigrator.Runner.BatchParser.Sources;
using FluentMigrator.Runner.BatchParser.SpecialTokenSearchers;
using FluentMigrator.Runner.Generators.SQLite;
using FluentMigrator.Runner.Initialization;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FluentMigrator.Runner.Processors.SQLite
{

    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Class SQLiteProcessor.
    /// Implements the <see cref="FluentMigrator.Runner.Processors.GenericProcessorBase" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Processors.GenericProcessorBase" />
    public class SQLiteProcessor : GenericProcessorBase
    {
        /// <summary>
        /// The service provider
        /// </summary>
        [CanBeNull]
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Gets the database type
        /// </summary>
        /// <value>The type of the database.</value>
        public override string DatabaseType
        {
            get { return "SQLite"; }
        }

        /// <summary>
        /// Gets the database type aliases
        /// </summary>
        /// <value>The database type aliases.</value>
        public override IList<string> DatabaseTypeAliases { get; } = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteProcessor"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="generator">The generator.</param>
        /// <param name="announcer">The announcer.</param>
        /// <param name="options">The options.</param>
        /// <param name="factory">The factory.</param>
        [Obsolete]
        public SQLiteProcessor(
            IDbConnection connection,
            IMigrationGenerator generator,
            IAnnouncer announcer,
            [NotNull] IMigrationProcessorOptions options,
            IDbFactory factory)
            : base(connection, factory, generator, announcer, options)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteProcessor"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="generator">The generator.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="options">The options.</param>
        /// <param name="connectionStringAccessor">The connection string accessor.</param>
        /// <param name="serviceProvider">The service provider.</param>
        public SQLiteProcessor(
            [NotNull] SQLiteDbFactory factory,
            [NotNull] SQLiteGenerator generator,
            [NotNull] ILogger<SQLiteProcessor> logger,
            [NotNull] IOptionsSnapshot<ProcessorOptions> options,
            [NotNull] IConnectionStringAccessor connectionStringAccessor,
            [NotNull] IServiceProvider serviceProvider)
            : base(() => factory.Factory, generator, logger, options.Value, connectionStringAccessor)
        {
            _serviceProvider = serviceProvider;
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
            return Exists("select count(*) from sqlite_master where name=\"{0}\" and type='table'", tableName);
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
            var dataSet = Read("PRAGMA table_info([{0}])", tableName);
            if (dataSet.Tables.Count == 0)
                return false;
            var table = dataSet.Tables[0];
            if (!table.Columns.Contains("Name"))
                return false;
            return table.Select(string.Format("Name='{0}'", columnName.Replace("'", "''"))).Length > 0;
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
            return false;
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
            return Exists("select count(*) from sqlite_master where name='{0}' and tbl_name='{1}' and type='index'", indexName, tableName);
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
                try
                {
                    if (!reader.Read()) return false;
                    if (int.Parse(reader[0].ToString()) <= 0) return false;
                    return true;
                }
                catch
                {
                    return false;
                }
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
            return Read("select * from [{0}]", tableName);
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
            if (string.IsNullOrEmpty(sql))
                return;

            if (Options.PreviewOnly)
            {
                ExecuteBatchNonQuery(
                    sql,
                    (sqlBatch) =>
                    {
                        Logger.LogSql(sqlBatch);
                    },
                    (sqlBatch, goCount) =>
                    {
                        Logger.LogSql(sqlBatch);
                        Logger.LogSql($"GO {goCount}");
                    });
                return;
            }

            Logger.LogSql(sql);

            EnsureConnectionIsOpen();

            if (ContainsGo(sql))
            {
                ExecuteBatchNonQuery(
                    sql,
                    (sqlBatch) =>
                    {
                        using (var command = CreateCommand(sqlBatch))
                        {
                            command.ExecuteNonQuery();
                        }
                    },
                    (sqlBatch, goCount) =>
                    {
                        using (var command = CreateCommand(sqlBatch))
                        {
                            for (var i = 0; i != goCount; ++i)
                            {
                                command.ExecuteNonQuery();
                            }
                        }
                    });
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
            var parser = _serviceProvider?.GetService<SQLiteBatchParser>() ?? new SQLiteBatchParser();
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
                catch (DbException ex)
                {
                    throw new Exception(ex.Message + "\r\nWhile Processing:\r\n\"" + command.CommandText + "\"", ex);
                }
            }
        }

        /// <summary>
        /// Executes the batch non query.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="executeBatch">The execute batch.</param>
        /// <param name="executeGo">The execute go.</param>
        /// <exception cref="Exception"></exception>
        private void ExecuteBatchNonQuery(string sql, Action<string> executeBatch, Action<string, int> executeGo)
        {
            string sqlBatch = string.Empty;

            try
            {
                var parser = _serviceProvider?.GetService<SQLiteBatchParser>() ?? new SQLiteBatchParser();
                parser.SqlText += (sender, args) => { sqlBatch = args.SqlText.Trim(); };
                parser.SpecialToken += (sender, args) =>
                {
                    if (string.IsNullOrEmpty(sqlBatch))
                        return;

                    if (args.Opaque is GoSearcher.GoSearcherParameters goParams)
                    {
                        executeGo(sqlBatch, goParams.Count);
                    }

                    sqlBatch = null;
                };

                using (var source = new TextReaderSource(new StringReader(sql), true))
                {
                    parser.Process(source, stripComments: Options.StripComments);
                }

                if (!string.IsNullOrEmpty(sqlBatch))
                {
                    executeBatch(sqlBatch);
                }
            }
            catch (DbException ex)
            {
                throw new Exception(ex.Message + "\r\nWhile Processing:\r\n\"" + sqlBatch + "\"", ex);
            }
        }

        /// <summary>
        /// Reads the specified template.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>DataSet.</returns>
        public override DataSet Read(string template, params object[] args)
        {
            EnsureConnectionIsOpen();

            using (var command = CreateCommand(string.Format(template, args)))
            using (var reader = command.ExecuteReader())
            {
                return reader.ReadDataSet();
            }
        }
    }
}
