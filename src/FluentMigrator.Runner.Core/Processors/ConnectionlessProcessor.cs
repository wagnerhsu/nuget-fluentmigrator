// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Core
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="ConnectionlessProcessor.cs" company="FluentMigrator Project">
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
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Generators;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Logging;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FluentMigrator.Runner.Processors
{
    /// <summary>
    /// Class ConnectionlessProcessor.
    /// Implements the <see cref="FluentMigrator.IMigrationProcessor" />
    /// </summary>
    /// <seealso cref="FluentMigrator.IMigrationProcessor" />
    public class ConnectionlessProcessor: IMigrationProcessor
    {
        /// <summary>
        /// The logger
        /// </summary>
        [NotNull] private readonly ILogger _logger;
#pragma warning disable 612
        /// <summary>
        /// The legacy options
        /// </summary>
        [Obsolete]
        private readonly IMigrationProcessorOptions _legacyOptions;
#pragma warning restore 612

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionlessProcessor"/> class.
        /// </summary>
        /// <param name="generator">The generator.</param>
        /// <param name="context">The context.</param>
        /// <param name="options">The options.</param>
        [Obsolete]
        public ConnectionlessProcessor(
            IMigrationGenerator generator,
            IRunnerContext context,
            IMigrationProcessorOptions options)
        {
            _logger = new AnnouncerFluentMigratorLogger(context.Announcer);
            _legacyOptions = options;
            DatabaseType = context.Database;
            Generator = generator;
            Announcer = context.Announcer;
            Options = options.GetProcessorOptions(connectionString: null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionlessProcessor"/> class.
        /// </summary>
        /// <param name="generatorAccessor">The generator accessor.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="options">The options.</param>
        /// <param name="accessorOptions">The accessor options.</param>
        public ConnectionlessProcessor(
            [NotNull] IGeneratorAccessor generatorAccessor,
            [NotNull] ILogger logger,
            [NotNull] IOptionsSnapshot<ProcessorOptions> options,
            [NotNull] IOptions<SelectingProcessorAccessorOptions> accessorOptions)
        {
            _logger = logger;
            var generator = generatorAccessor.Generator;
            DatabaseType = string.IsNullOrEmpty(accessorOptions.Value.ProcessorId) ? generator.GetName() : accessorOptions.Value.ProcessorId;
            Generator = generator;
            Options = options.Value;
#pragma warning disable 612
            Announcer = new LoggerAnnouncer(logger, new AnnouncerOptions() { ShowElapsedTime = true, ShowSql = true });
            _legacyOptions = options.Value;
#pragma warning restore 612
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionlessProcessor"/> class.
        /// </summary>
        /// <param name="generatorAccessor">The generator accessor.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="options">The options.</param>
        /// <param name="processorIds">The processor ids.</param>
        public ConnectionlessProcessor(
            [NotNull] IGeneratorAccessor generatorAccessor,
            [NotNull] ILogger logger,
            [NotNull] IOptionsSnapshot<ProcessorOptions> options,
            [NotNull] IReadOnlyCollection<string> processorIds)
        {
            _logger = logger;
            var generator = generatorAccessor.Generator;
            DatabaseType = processorIds.FirstOrDefault() ?? generator.GetName();
            DatabaseTypeAliases = processorIds.Count == 0 ? Array.Empty<string>() : processorIds.Skip(1).ToArray();
            Generator = generator;
            Options = options.Value;
#pragma warning disable 612
            Announcer = new LoggerAnnouncer(logger, AnnouncerOptions.AllEnabled);
            _legacyOptions = options.Value;
#pragma warning restore 612
        }

        /// <summary>
        /// Gets the connection string
        /// </summary>
        /// <value>The connection string.</value>
        [Obsolete("Will change from public to protected")]
        public string ConnectionString { get; } = "No connection";

        /// <summary>
        /// Gets or sets the generator.
        /// </summary>
        /// <value>The generator.</value>
        public IMigrationGenerator Generator { get; set; }

        /// <summary>
        /// Gets or sets the announcer.
        /// </summary>
        /// <value>The announcer.</value>
        [Obsolete]
        public IAnnouncer Announcer { get; set; }
        /// <summary>
        /// Gets the migration processor options
        /// </summary>
        /// <value>The options.</value>
        public ProcessorOptions Options {get; set;}

        /// <summary>
        /// Gets the migration processor options
        /// </summary>
        /// <value>The options.</value>
        [Obsolete]
        IMigrationProcessorOptions IMigrationProcessor.Options => _legacyOptions;

        /// <inheritdoc />
        public void Execute(string sql)
        {
            Process(sql);
        }

        /// <summary>
        /// Execute an SQL statement
        /// </summary>
        /// <param name="template">The SQL statement</param>
        /// <param name="args">The arguments to replace in the SQL statement</param>
        public void Execute(string template, params object[] args)
        {
            Process(string.Format(template, args));
        }

        /// <summary>
        /// Reads all data from all rows from a table
        /// </summary>
        /// <param name="schemaName">The schema name of the table</param>
        /// <param name="tableName">The table name</param>
        /// <returns>The data from the specified table</returns>
        /// <exception cref="NotImplementedException">Method is not supported by the connectionless processor</exception>
        public DataSet ReadTableData(string schemaName, string tableName)
        {
            throw new NotImplementedException("Method is not supported by the connectionless processor");
        }

        /// <summary>
        /// Executes and returns the result of an SQL query
        /// </summary>
        /// <param name="template">The SQL query</param>
        /// <param name="args">The arguments of the SQL query</param>
        /// <returns>The data from the specified SQL query</returns>
        /// <exception cref="NotImplementedException">Method is not supported by the connectionless processor</exception>
        public DataSet Read(string template, params object[] args)
        {
            throw new NotImplementedException("Method is not supported by the connectionless processor");
        }

        /// <summary>
        /// Returns <c>true</c> if data could be found for the given SQL query
        /// </summary>
        /// <param name="template">The SQL query</param>
        /// <param name="args">The arguments of the SQL query</param>
        /// <returns><c>true</c> when the SQL query returned data</returns>
        /// <exception cref="NotImplementedException">Method is not supported by the connectionless processor</exception>
        public bool Exists(string template, params object[] args)
        {
            throw new NotImplementedException("Method is not supported by the connectionless processor");
        }

        /// <summary>
        /// Begins a transaction
        /// </summary>
        public void BeginTransaction()
        {

        }

        /// <summary>
        /// Commits a transaction
        /// </summary>
        public void CommitTransaction()
        {

        }

        /// <summary>
        /// Rollback of a transaction
        /// </summary>
        public void RollbackTransaction()
        {

        }

        /// <summary>
        /// Processes the specified SQL.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        protected void Process(string sql)
        {
            _logger.LogSql(sql);
        }

        /// <summary>
        /// Executes a <c>CREATE SCHEMA</c> SQL expression
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public void Process(CreateSchemaExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes a <c>DROP SCHEMA</c> SQL expression
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public void Process(DeleteSchemaExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes a <c>ALTER TABLE</c> SQL expression
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public void Process(AlterTableExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes a <c>ALTER TABLE ALTER COLUMN</c> SQL expression
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public void Process(AlterColumnExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes a <c>CREATE TABLE</c> SQL expression
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public void Process(CreateTableExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes a <c>ALTER TABLE ADD COLUMN</c> SQL expression
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public void Process(CreateColumnExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes a <c>DROP TABLE</c> SQL expression
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public void Process(DeleteTableExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes a <c>ALTER TABLE DROP COLUMN</c> SQL expression
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public void Process(DeleteColumnExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes an SQL expression to create a foreign key
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public void Process(CreateForeignKeyExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes an SQL expression to drop a foreign key
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public void Process(DeleteForeignKeyExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes an SQL expression to create an index
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public void Process(CreateIndexExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes an SQL expression to drop an index
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public void Process(DeleteIndexExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes an SQL expression to rename a table
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public void Process(RenameTableExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes an SQL expression to rename a column
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public void Process(RenameColumnExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes an SQL expression to INSERT data
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public void Process(InsertDataExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes an SQL expression to alter a default constraint
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public void Process(AlterDefaultConstraintExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes a DB operation
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public void Process(PerformDBOperationExpression expression)
        {
            _logger.LogSay("Performing DB Operation");
        }

        /// <summary>
        /// Executes an SQL expression to DELETE data
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public void Process(DeleteDataExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes an SQL expression to UPDATE data
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public void Process(UpdateDataExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes a <c>ALTER SCHEMA</c> SQL expression
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public void Process(AlterSchemaExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes a <c>CREATE SEQUENCE</c> SQL expression
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public void Process(CreateSequenceExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes a <c>DROP SEQUENCE</c> SQL expression
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public void Process(DeleteSequenceExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes an SQL expression to create a constraint
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public void Process(CreateConstraintExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes an SQL expression to drop a constraint
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public void Process(DeleteConstraintExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes an SQL expression to drop a default constraint
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public void Process(DeleteDefaultConstraintExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        /// <summary>
        /// Tests if the schema exists
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <returns><c>true</c> when it exists</returns>
        /// <exception cref="NotImplementedException">Method is not supported by the connectionless processor</exception>
        public bool SchemaExists(string schemaName)
        {
            throw new NotImplementedException("Method is not supported by the connectionless processor");
        }

        /// <summary>
        /// Tests if the table exists
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <param name="tableName">The table name</param>
        /// <returns><c>true</c> when it exists</returns>
        /// <exception cref="NotImplementedException">Method is not supported by the connectionless processor</exception>
        public bool TableExists(string schemaName, string tableName)
        {
            throw new NotImplementedException("Method is not supported by the connectionless processor");
        }

        /// <summary>
        /// Tests if a column exists
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <param name="tableName">The table name</param>
        /// <param name="columnName">The column name</param>
        /// <returns><c>true</c> when it exists</returns>
        /// <exception cref="NotImplementedException">Method is not supported by the connectionless processor</exception>
        public bool ColumnExists(string schemaName, string tableName, string columnName)
        {
            throw new NotImplementedException("Method is not supported by the connectionless processor");
        }

        /// <summary>
        /// Tests if a constraint exists
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <param name="tableName">The table name</param>
        /// <param name="constraintName">The constraint name</param>
        /// <returns><c>true</c> when it exists</returns>
        /// <exception cref="NotImplementedException">Method is not supported by the connectionless processor</exception>
        public bool ConstraintExists(string schemaName, string tableName, string constraintName)
        {
            throw new NotImplementedException("Method is not supported by the connectionless processor");
        }

        /// <summary>
        /// Tests if an index exists
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <param name="tableName">The table name</param>
        /// <param name="indexName">The index name</param>
        /// <returns><c>true</c> when it exists</returns>
        /// <exception cref="NotImplementedException">Method is not supported by the connectionless processor</exception>
        public bool IndexExists(string schemaName, string tableName, string indexName)
        {
            throw new NotImplementedException("Method is not supported by the connectionless processor");
        }

        /// <summary>
        /// Tests if a sequence exists
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <param name="sequenceName">The sequence name</param>
        /// <returns><c>true</c> when it exists</returns>
        /// <exception cref="NotImplementedException">Method is not supported by the connectionless processor</exception>
        public bool SequenceExists(string schemaName, string sequenceName)
        {
            throw new NotImplementedException("Method is not supported by the connectionless processor");
        }

        /// <summary>
        /// Tests if a default value for a column exists
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <param name="tableName">The table name</param>
        /// <param name="columnName">The column name</param>
        /// <param name="defaultValue">The default value</param>
        /// <returns><c>true</c> when it exists</returns>
        /// <exception cref="NotImplementedException">Method is not supported by the connectionless processor</exception>
        public bool DefaultValueExists(string schemaName, string tableName, string columnName, object defaultValue)
        {
            throw new NotImplementedException("Method is not supported by the connectionless processor");
        }

#pragma warning disable 618
        /// <summary>
        /// Gets the database type
        /// </summary>
        /// <value>The type of the database.</value>
        public string DatabaseType { get; }
#pragma warning restore 618

        /// <summary>
        /// Gets the database type aliases
        /// </summary>
        /// <value>The database type aliases.</value>
        public IList<string> DatabaseTypeAliases { get; } = new List<string>();

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        public void Dispose()
        {
        }
    }
}
