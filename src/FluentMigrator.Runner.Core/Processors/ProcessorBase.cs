// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Core
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="ProcessorBase.cs" company="FluentMigrator Project">
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
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Logging;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;

namespace FluentMigrator.Runner.Processors
{
    /// <summary>
    /// Class ProcessorBase.
    /// Implements the <see cref="FluentMigrator.IMigrationProcessor" />
    /// </summary>
    /// <seealso cref="FluentMigrator.IMigrationProcessor" />
    public abstract class ProcessorBase : IMigrationProcessor
    {
#pragma warning disable 612
        /// <summary>
        /// The legacy options
        /// </summary>
        [Obsolete]
        private readonly IMigrationProcessorOptions _legacyOptions;
#pragma warning restore 612

        /// <summary>
        /// The generator
        /// </summary>
        protected internal readonly IMigrationGenerator Generator;

#pragma warning disable 612
        /// <summary>
        /// The announcer
        /// </summary>
        [Obsolete]
        protected readonly IAnnouncer Announcer;
#pragma warning restore 612

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessorBase"/> class.
        /// </summary>
        /// <param name="generator">The generator.</param>
        /// <param name="announcer">The announcer.</param>
        /// <param name="options">The options.</param>
        [Obsolete]
        protected ProcessorBase(
            IMigrationGenerator generator,
            IAnnouncer announcer,
            [NotNull] IMigrationProcessorOptions options)
        {
            Generator = generator;
            Announcer = announcer;
            Logger = new AnnouncerFluentMigratorLogger(announcer);
            Options = options as ProcessorOptions ?? new ProcessorOptions()
            {
                PreviewOnly = options.PreviewOnly,
                ProviderSwitches = options.ProviderSwitches,
                Timeout = options.Timeout == null ? null : (TimeSpan?) TimeSpan.FromSeconds(options.Timeout.Value),
            };

            _legacyOptions = options;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessorBase"/> class.
        /// </summary>
        /// <param name="generator">The generator.</param>
        /// <param name="announcer">The announcer.</param>
        /// <param name="options">The options.</param>
        [Obsolete]
        protected ProcessorBase(
            [NotNull] IMigrationGenerator generator,
            [NotNull] IAnnouncer announcer,
            [NotNull] ProcessorOptions options)
        {
            Generator = generator;
            Announcer = announcer;
            Options = options;
            _legacyOptions = options;
            Logger = new AnnouncerFluentMigratorLogger(announcer);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessorBase"/> class.
        /// </summary>
        /// <param name="generator">The generator.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="options">The options.</param>
        protected ProcessorBase(
            [NotNull] IMigrationGenerator generator,
            [NotNull] ILogger logger,
            [NotNull] ProcessorOptions options)
        {
            Generator = generator;
            Options = options;
            Logger = logger;
#pragma warning disable 612
            Announcer = new LoggerAnnouncer(
                logger,
                new AnnouncerOptions()
                {
                    ShowSql = true,
                    ShowElapsedTime = true,
                });
            _legacyOptions = options;
#pragma warning restore 612
        }

        /// <summary>
        /// Gets the migration processor options
        /// </summary>
        /// <value>The options.</value>
        [Obsolete]
        IMigrationProcessorOptions IMigrationProcessor.Options => _legacyOptions;

        /// <summary>
        /// Gets the connection string
        /// </summary>
        /// <value>The connection string.</value>
        [Obsolete]
        public abstract string ConnectionString { get; }

        /// <summary>
        /// Gets the database type
        /// </summary>
        /// <value>The type of the database.</value>
        public abstract string DatabaseType { get; }

        /// <summary>
        /// Gets the database type aliases
        /// </summary>
        /// <value>The database type aliases.</value>
        public abstract IList<string> DatabaseTypeAliases { get; }

        /// <summary>
        /// Gets or sets a value indicating whether [was committed].
        /// </summary>
        /// <value><c>true</c> if [was committed]; otherwise, <c>false</c>.</value>
        public bool WasCommitted { get; protected set; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>The logger.</value>
        protected internal ILogger Logger { get; }

        /// <summary>
        /// Gets the migration processor options
        /// </summary>
        /// <value>The options.</value>
        [NotNull]
        protected ProcessorOptions Options { get; }

        /// <summary>
        /// Executes a <c>CREATE SCHEMA</c> SQL expression
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public virtual void Process(CreateSchemaExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes a <c>DROP SCHEMA</c> SQL expression
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public virtual void Process(DeleteSchemaExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes a <c>CREATE TABLE</c> SQL expression
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public virtual void Process(CreateTableExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes a <c>ALTER TABLE</c> SQL expression
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public virtual void Process(AlterTableExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes a <c>ALTER TABLE ALTER COLUMN</c> SQL expression
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public virtual void Process(AlterColumnExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes a <c>ALTER TABLE ADD COLUMN</c> SQL expression
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public virtual void Process(CreateColumnExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes a <c>DROP TABLE</c> SQL expression
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public virtual void Process(DeleteTableExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes a <c>ALTER TABLE DROP COLUMN</c> SQL expression
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public virtual void Process(DeleteColumnExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes an SQL expression to create a foreign key
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public virtual void Process(CreateForeignKeyExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes an SQL expression to drop a foreign key
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public virtual void Process(DeleteForeignKeyExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes an SQL expression to create an index
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public virtual void Process(CreateIndexExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes an SQL expression to drop an index
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public virtual void Process(DeleteIndexExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes an SQL expression to rename a table
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public virtual void Process(RenameTableExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes an SQL expression to rename a column
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public virtual void Process(RenameColumnExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes an SQL expression to INSERT data
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public virtual void Process(InsertDataExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes an SQL expression to DELETE data
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public virtual void Process(DeleteDataExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes an SQL expression to alter a default constraint
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public virtual void Process(AlterDefaultConstraintExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes an SQL expression to UPDATE data
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public virtual void Process(UpdateDataExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes a DB operation
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public abstract void Process(PerformDBOperationExpression expression);

        /// <summary>
        /// Executes a <c>ALTER SCHEMA</c> SQL expression
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public virtual void Process(AlterSchemaExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes a <c>CREATE SEQUENCE</c> SQL expression
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public virtual void Process(CreateSequenceExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes a <c>DROP SEQUENCE</c> SQL expression
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public virtual void Process(DeleteSequenceExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes an SQL expression to create a constraint
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public virtual void Process(CreateConstraintExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes an SQL expression to drop a constraint
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public virtual void Process(DeleteConstraintExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes an SQL expression to drop a default constraint
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public virtual void Process(DeleteDefaultConstraintExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        /// <summary>
        /// Processes the specified SQL.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        protected abstract void Process(string sql);

        /// <summary>
        /// Begins a transaction
        /// </summary>
        public virtual void BeginTransaction()
        {
        }

        /// <summary>
        /// Commits a transaction
        /// </summary>
        public virtual void CommitTransaction()
        {
        }

        /// <summary>
        /// Rollback of a transaction
        /// </summary>
        public virtual void RollbackTransaction()
        {
        }

        /// <summary>
        /// Reads all data from all rows from a table
        /// </summary>
        /// <param name="schemaName">The schema name of the table</param>
        /// <param name="tableName">The table name</param>
        /// <returns>The data from the specified table</returns>
        public abstract DataSet ReadTableData(string schemaName, string tableName);

        /// <summary>
        /// Executes and returns the result of an SQL query
        /// </summary>
        /// <param name="template">The SQL query</param>
        /// <param name="args">The arguments of the SQL query</param>
        /// <returns>The data from the specified SQL query</returns>
        public abstract DataSet Read(string template, params object[] args);

        /// <summary>
        /// Returns <c>true</c> if data could be found for the given SQL query
        /// </summary>
        /// <param name="template">The SQL query</param>
        /// <param name="args">The arguments of the SQL query</param>
        /// <returns><c>true</c> when the SQL query returned data</returns>
        public abstract bool Exists(string template, params object[] args);

        /// <inheritdoc />
        public virtual void Execute(string sql)
        {
            Execute(sql.Replace("{", "{{").Replace("}", "}}"), Array.Empty<object>());
        }

        /// <summary>
        /// Execute an SQL statement
        /// </summary>
        /// <param name="template">The SQL statement</param>
        /// <param name="args">The arguments to replace in the SQL statement</param>
        public abstract void Execute(string template, params object[] args);

        /// <summary>
        /// Tests if the schema exists
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <returns><c>true</c> when it exists</returns>
        public abstract bool SchemaExists(string schemaName);

        /// <summary>
        /// Tests if the table exists
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <param name="tableName">The table name</param>
        /// <returns><c>true</c> when it exists</returns>
        public abstract bool TableExists(string schemaName, string tableName);

        /// <summary>
        /// Tests if a column exists
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <param name="tableName">The table name</param>
        /// <param name="columnName">The column name</param>
        /// <returns><c>true</c> when it exists</returns>
        public abstract bool ColumnExists(string schemaName, string tableName, string columnName);

        /// <summary>
        /// Tests if a constraint exists
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <param name="tableName">The table name</param>
        /// <param name="constraintName">The constraint name</param>
        /// <returns><c>true</c> when it exists</returns>
        public abstract bool ConstraintExists(string schemaName, string tableName, string constraintName);

        /// <summary>
        /// Tests if an index exists
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <param name="tableName">The table name</param>
        /// <param name="indexName">The index name</param>
        /// <returns><c>true</c> when it exists</returns>
        public abstract bool IndexExists(string schemaName, string tableName, string indexName);

        /// <summary>
        /// Tests if a sequence exists
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <param name="sequenceName">The sequence name</param>
        /// <returns><c>true</c> when it exists</returns>
        public abstract bool SequenceExists(string schemaName, string sequenceName);

        /// <summary>
        /// Tests if a default value for a column exists
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <param name="tableName">The table name</param>
        /// <param name="columnName">The column name</param>
        /// <param name="defaultValue">The default value</param>
        /// <returns><c>true</c> when it exists</returns>
        public abstract bool DefaultValueExists(string schemaName, string tableName, string columnName, object defaultValue);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="isDisposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected abstract void Dispose(bool isDisposing);
    }
}
