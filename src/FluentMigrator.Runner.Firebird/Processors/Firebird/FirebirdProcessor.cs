// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Firebird
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="FirebirdProcessor.cs" company="FluentMigrator Project">
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
using System.Linq;
using System.Text.RegularExpressions;

using FluentMigrator.Expressions;
using FluentMigrator.Model;
using FluentMigrator.Runner.Generators.Firebird;
using FluentMigrator.Runner.Helpers;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Models;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FluentMigrator.Runner.Processors.Firebird
{
    /// <summary>
    /// Class FirebirdProcessor.
    /// Implements the <see cref="FluentMigrator.Runner.Processors.GenericProcessorBase" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Processors.GenericProcessorBase" />
    public class FirebirdProcessor : GenericProcessorBase
    {
        // ReSharper disable once InconsistentNaming
        /// <summary>
        /// The truncator
        /// </summary>
        [Obsolete("Use the Truncator property")]
        protected readonly FirebirdTruncator truncator;

        /// <summary>
        /// The firebird version function
        /// </summary>
        private readonly Lazy<Version> _firebirdVersionFunc;
        /// <summary>
        /// The quoter
        /// </summary>
        private readonly FirebirdQuoter _quoter;

        /// <summary>
        /// The DDL created tables
        /// </summary>
        protected List<string> DDLCreatedTables;
        /// <summary>
        /// The DDL created columns
        /// </summary>
        protected Dictionary<string, List<string>> DDLCreatedColumns;
        /// <summary>
        /// The DDL touched tables
        /// </summary>
        protected List<string> DDLTouchedTables;
        /// <summary>
        /// The DDL touched columns
        /// </summary>
        protected Dictionary<string, List<string>> DDLTouchedColumns;

        /// <summary>
        /// Initializes a new instance of the <see cref="FirebirdProcessor"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="generator">The generator.</param>
        /// <param name="announcer">The announcer.</param>
        /// <param name="options">The options.</param>
        /// <param name="factory">The factory.</param>
        /// <param name="fbOptions">The fb options.</param>
        /// <exception cref="ArgumentNullException">fbOptions</exception>
        [Obsolete]
        public FirebirdProcessor(IDbConnection connection, IMigrationGenerator generator, IAnnouncer announcer, IMigrationProcessorOptions options, IDbFactory factory, FirebirdOptions fbOptions)
            : base(connection, factory, generator, announcer, options)
        {
            FBOptions = fbOptions ?? throw new ArgumentNullException(nameof(fbOptions));
            _firebirdVersionFunc = new Lazy<Version>(GetFirebirdVersion);
            _quoter = new FirebirdQuoter(fbOptions.ForceQuote);
            truncator = new FirebirdTruncator(FBOptions.TruncateLongNames, FBOptions.PackKeyNames);
            ClearLocks();
            ClearDDLFollowers();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FirebirdProcessor"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="generator">The generator.</param>
        /// <param name="quoter">The quoter.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="options">The options.</param>
        /// <param name="connectionStringAccessor">The connection string accessor.</param>
        /// <param name="fbOptions">The fb options.</param>
        /// <exception cref="ArgumentNullException">fbOptions</exception>
        public FirebirdProcessor(
            [NotNull] FirebirdDbFactory factory,
            [NotNull] FirebirdGenerator generator,
            [NotNull] FirebirdQuoter quoter,
            [NotNull] ILogger<FirebirdProcessor> logger,
            [NotNull] IOptionsSnapshot<ProcessorOptions> options,
            [NotNull] IConnectionStringAccessor connectionStringAccessor,
            [NotNull] FirebirdOptions fbOptions)
            : base(() => factory.Factory, generator, logger, options.Value, connectionStringAccessor)
        {
            FBOptions = fbOptions ?? throw new ArgumentNullException(nameof(fbOptions));
            _firebirdVersionFunc = new Lazy<Version>(GetFirebirdVersion);
            _quoter = quoter;
#pragma warning disable 618
            truncator = new FirebirdTruncator(FBOptions.TruncateLongNames, FBOptions.PackKeyNames);
#pragma warning restore 618
            ClearLocks();
            ClearDDLFollowers();
        }

        /// <summary>
        /// Gets the database type
        /// </summary>
        /// <value>The type of the database.</value>
        public override string DatabaseType => "Firebird";

        /// <summary>
        /// Gets the database type aliases
        /// </summary>
        /// <value>The database type aliases.</value>
        public override IList<string> DatabaseTypeAliases { get; } = new List<string>();

        /// <summary>
        /// Gets the fb options.
        /// </summary>
        /// <value>The fb options.</value>
        public FirebirdOptions FBOptions { get; }
        /// <summary>
        /// Gets a value indicating whether this instance is firebird3.
        /// </summary>
        /// <value><c>true</c> if this instance is firebird3; otherwise, <c>false</c>.</value>
        public bool IsFirebird3 => _firebirdVersionFunc.Value >= new Version(3, 0);
        /// <summary>
        /// Gets the generator.
        /// </summary>
        /// <value>The generator.</value>
        public new IMigrationGenerator Generator => base.Generator;

        /// <summary>
        /// Gets the announcer.
        /// </summary>
        /// <value>The announcer.</value>
        [Obsolete]
        public new IAnnouncer Announcer => base.Announcer;

#pragma warning disable 618
        /// <summary>
        /// Gets the truncator.
        /// </summary>
        /// <value>The truncator.</value>
        public FirebirdTruncator Truncator => truncator;
#pragma warning restore 618

        /// <summary>
        /// Gets the firebird version.
        /// </summary>
        /// <returns>Version.</returns>
        private Version GetFirebirdVersion()
        {
            EnsureConnectionIsOpen();
            try
            {
                var result = Read("SELECT rdb$get_context('SYSTEM', 'ENGINE_VERSION') from rdb$database");
                var version = (string)result.Tables[0].Rows[0].ItemArray[0];
                var versionRegEx = new Regex(@"\d+\.\d+(\.\d+)?");
                var match = versionRegEx.Match(version);
                if (match.Success)
                {
                    return new Version(match.Value);
                }

                return new Version(2, 1);
            }
            catch
            {
                // Fehler ignorieren - Älter als Version 2.1
                return new Version(2, 0);
            }
        }

        /// <summary>
        /// Tests if the schema exists
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <returns><c>true</c> when it exists</returns>
        public override bool SchemaExists(string schemaName)
        {
            //No schema support in firebird
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
            CheckTable(schemaName);
            return Exists("select rdb$relation_name from rdb$relations where (rdb$flags IS NOT NULL) and (lower(rdb$relation_name) = lower('{0}'))", FormatToSafeName(tableName));
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
            CheckTable(tableName);
            CheckColumn(tableName, columnName);
            return Exists("select rdb$field_name from rdb$relation_fields where (lower(rdb$relation_name) = lower('{0}')) and (lower(rdb$field_name) = lower('{1}'))", FormatToSafeName(tableName), FormatToSafeName(columnName));
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
            CheckTable(tableName);
            return Exists("select rdb$constraint_name from rdb$relation_constraints where (lower(rdb$relation_name) = lower('{0}')) and (lower(rdb$constraint_name) = lower('{1}'))", FormatToSafeName(tableName), FormatToSafeName(constraintName));
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
            CheckTable(tableName);
            return Exists("select rdb$index_name from rdb$indices where (lower(rdb$relation_name) = lower('{0}')) and (lower(rdb$index_name) = lower('{1}')) and (rdb$system_flag <> 1 OR rdb$system_flag IS NULL) and (rdb$foreign_key IS NULL)", FormatToSafeName(tableName), FormatToSafeName(indexName));
        }

        /// <summary>
        /// Tests if a sequence exists
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <param name="sequenceName">The sequence name</param>
        /// <returns><c>true</c> when it exists</returns>
        public override bool SequenceExists(string schemaName, string sequenceName)
        {
            return Exists("select rdb$generator_name from rdb$generators where lower(rdb$generator_name) = lower('{0}')", FormatToSafeName(sequenceName));
        }

        /// <summary>
        /// Triggers the exists.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="triggerName">Name of the trigger.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool TriggerExists(string schemaName, string tableName, string triggerName)
        {
            CheckTable(tableName);
            return Exists("select rdb$trigger_name from rdb$triggers where (lower(rdb$relation_name) = lower('{0}')) and (lower(rdb$trigger_name) = lower('{1}'))", FormatToSafeName(tableName), FormatToSafeName(triggerName));
        }

        /// <summary>
        /// Reads all data from all rows from a table
        /// </summary>
        /// <param name="schemaName">The schema name of the table</param>
        /// <param name="tableName">The table name</param>
        /// <returns>The data from the specified table</returns>
        public override DataSet ReadTableData(string schemaName, string tableName)
        {
            CheckTable(tableName);
            return Read("SELECT * FROM {0}", _quoter.QuoteTableName(tableName, schemaName));
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

            //Announcer.Sql(String.Format(template,args));

            using (var command = CreateCommand(string.Format(template, args)))
            using (var reader = command.ExecuteReader())
            {
                return reader.ReadDataSet();
            }
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
        /// Begins the transaction.
        /// </summary>
        public override void BeginTransaction()
        {
            base.BeginTransaction();
            ClearLocks();
            ClearDDLFollowers();
        }

        /// <summary>
        /// Commits the transaction.
        /// </summary>
        public override void CommitTransaction()
        {
            base.CommitTransaction();
            EnsureConnectionIsClosed();
            ClearLocks();
        }

        /// <summary>
        /// Rollbacks the transaction.
        /// </summary>
        public override void RollbackTransaction()
        {
            base.RollbackTransaction();
            EnsureConnectionIsClosed();
            ClearLocks();
        }

        /// <summary>
        /// Commits the retaining.
        /// </summary>
        public virtual void CommitRetaining()
        {
            if (IsRunningOutOfMigrationScope())
            {
                EnsureConnectionIsClosed();
                return;
            }

            Logger.LogSay("Committing and Retaining Transaction");

            CommitTransaction();
            BeginTransaction();
        }

        /// <summary>
        /// Automatics the commit.
        /// </summary>
        public virtual void AutoCommit()
        {
            if (FBOptions.TransactionModel == FirebirdTransactionModel.AutoCommit)
                CommitRetaining();
        }

        /// <summary>
        /// Determines whether [is running out of migration scope].
        /// </summary>
        /// <returns><c>true</c> if [is running out of migration scope]; otherwise, <c>false</c>.</returns>
        public bool IsRunningOutOfMigrationScope()
        {
            return Transaction == null;
        }

        /// <summary>
        /// Clears the DDL followers.
        /// </summary>
        protected void ClearDDLFollowers()
        {
            DDLCreatedTables = new List<string>();
            DDLCreatedColumns = new Dictionary<string, List<string>>();
        }

        /// <summary>
        /// Registers the table creation.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        protected void RegisterTableCreation(string tableName)
        {
            if (!DDLCreatedTables.Contains(tableName))
                DDLCreatedTables.Add(tableName);
        }

        /// <summary>
        /// Registers the column creation.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columnName">Name of the column.</param>
        protected void RegisterColumnCreation(string tableName, string columnName)
        {
            if (!DDLCreatedColumns.ContainsKey(tableName))
            {
                DDLCreatedColumns.Add(tableName, new List<string>() { columnName });
            }
            else if (!DDLCreatedColumns[tableName].Contains(columnName))
            {
                DDLCreatedColumns[tableName].Add(columnName);
            }
        }

        /// <summary>
        /// Determines whether [is table created] [the specified table name].
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns><c>true</c> if [is table created] [the specified table name]; otherwise, <c>false</c>.</returns>
        protected bool IsTableCreated(string tableName)
        {
            return DDLCreatedTables.Contains(tableName);
        }

        /// <summary>
        /// Determines whether [is column created] [the specified table name].
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns><c>true</c> if [is column created] [the specified table name]; otherwise, <c>false</c>.</returns>
        protected bool IsColumnCreated(string tableName, string columnName)
        {
            return DDLCreatedColumns.ContainsKey(tableName) && DDLCreatedColumns[tableName].Contains(columnName);
        }

        /// <summary>
        /// Clears the locks.
        /// </summary>
        protected void ClearLocks()
        {
            DDLTouchedTables = new List<string>();
            DDLTouchedColumns = new Dictionary<string, List<string>>();
        }

        /// <summary>
        /// Locks the table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        public void LockTable(string tableName)
        {
            if (!DDLTouchedTables.Contains(tableName))
                DDLTouchedTables.Add(tableName);
        }

        /// <summary>
        /// Locks the column.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columns">The columns.</param>
        public void LockColumn(string tableName, IEnumerable<string> columns)
        {
            columns.ToList().ForEach(x => LockColumn(tableName, x));
        }

        /// <summary>
        /// Locks the column.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columnName">Name of the column.</param>
        public void LockColumn(string tableName, string columnName)
        {
            if (!DDLTouchedColumns.ContainsKey(tableName))
            {
                DDLTouchedColumns.Add(tableName, new List<string>() { columnName });
            }
            else if (!DDLTouchedColumns[tableName].Contains(columnName))
            {
                DDLTouchedColumns[tableName].Add(columnName);
            }
        }

        /// <summary>
        /// Checks the table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <exception cref="InvalidOperationException"></exception>
        public void CheckTable(string tableName)
        {
            if (DDLTouchedTables.Contains(tableName))
            {
                if (FBOptions.TransactionModel == FirebirdTransactionModel.AutoCommitOnCheckFail)
                {
                    CommitRetaining();
                    return;
                }

                if (FBOptions.VirtualLock)
                    throw new InvalidOperationException(string.Format("Table {0} is locked", tableName));
            }
        }

        /// <summary>
        /// Checks the column.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columns">The columns.</param>
        public void CheckColumn(string tableName, IEnumerable<string> columns)
        {
            columns.ToList().ForEach(x => CheckColumn(tableName, x));
        }

        /// <summary>
        /// Checks the column.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <exception cref="InvalidOperationException"></exception>
        public void CheckColumn(string tableName, string columnName)
        {
            CheckTable(tableName);
            if (DDLTouchedColumns.Any(x => x.Key == tableName && x.Value.Contains(columnName)))
            {
                if (FBOptions.TransactionModel == FirebirdTransactionModel.AutoCommitOnCheckFail)
                {
                    CommitRetaining();
                    return;
                }

                if (FBOptions.VirtualLock)
                    throw new InvalidOperationException(string.Format("Column {0} in table {1} is locked", columnName, tableName));
            }
        }

        /// <summary>
        /// Executes a <c>ALTER TABLE ADD COLUMN</c> SQL expression
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public override void Process(CreateColumnExpression expression)
        {
            Truncator.Truncate(expression);
            CheckColumn(expression.TableName, expression.Column.Name);
            LockColumn(expression.TableName, expression.Column.Name);
            RegisterColumnCreation(expression.TableName, expression.Column.Name);
            InternalProcess(Generator.Generate(expression));

            if (expression.Column.IsIdentity)
            {
                CreateSequenceForIdentity(expression.TableName, expression.Column.Name);
            }

            /*if (FBOptions.TransactionModel == FirebirdTransactionModel.AutoCommitOnCheckFail)
                CommitRetaining();*/
            if (FBOptions.TransactionModel != FirebirdTransactionModel.None)
                CommitRetaining();
        }

        /// <summary>
        /// Executes a <c>ALTER TABLE ALTER COLUMN</c> SQL expression
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public override void Process(AlterColumnExpression expression)
        {
            Truncator.Truncate(expression);
            CheckColumn(expression.TableName, expression.Column.Name);
            FirebirdSchemaProvider schema = new FirebirdSchemaProvider(this, _quoter);
            FirebirdTableSchema table = schema.GetTableSchema(expression.TableName);
            ColumnDefinition colDef = table.Definition.Columns.FirstOrDefault(x => x.Name == _quoter.ToFbObjectName(expression.Column.Name));

            var generator = (FirebirdGenerator) Generator;

            var tableName = expression.Column.TableName ?? expression.TableName;

            //Change nullable constraint
            if (colDef == null || colDef.IsNullable != expression.Column.IsNullable)
            {
                string nullConstraintCommand;
                if (IsFirebird3)
                {
                    nullConstraintCommand = generator.GenerateSetNull3(tableName, expression.Column);
                }
                else
                {
                    nullConstraintCommand = generator.GenerateSetNullPre3(tableName, expression.Column);
                }

                InternalProcess(nullConstraintCommand);
            }

            //Change default value
            if (colDef == null || !FirebirdGenerator.DefaultValuesMatch(colDef, expression.Column))
            {
                IMigrationExpression defaultConstraint;
                if (expression.Column.DefaultValue is ColumnDefinition.UndefinedDefaultValue)
                {
                    defaultConstraint = new DeleteDefaultConstraintExpression()
                    {
                        SchemaName = expression.SchemaName,
                        TableName = expression.TableName,
                        ColumnName = expression.Column.Name
                    };
                }
                else
                {
                    defaultConstraint = new AlterDefaultConstraintExpression()
                    {
                        ColumnName = expression.Column.Name,
                        DefaultValue = expression.Column.DefaultValue,
                        TableName = expression.TableName,
                        SchemaName = expression.SchemaName
                    };
                }

                if (defaultConstraint is DeleteDefaultConstraintExpression deleteDefaultConstraintExpression)
                {
                    InternalProcess(Generator.Generate(deleteDefaultConstraintExpression));
                }
                else
                {
                    InternalProcess(Generator.Generate((AlterDefaultConstraintExpression) defaultConstraint));
                }
            }

            //Change type
            if (colDef == null || !FirebirdGenerator.ColumnTypesMatch(colDef, expression.Column))
            {
                InternalProcess(generator.GenerateSetType(tableName, expression.Column));
            }

            bool identitySequenceExists;
            try
            {
                identitySequenceExists = SequenceExists(string.Empty, GetSequenceName(expression.TableName, expression.Column.Name));
            }
            catch (ArgumentException)
            {
                identitySequenceExists = false;
            }


            //Adjust identity generators
            if (expression.Column.IsIdentity)
            {
                if (!identitySequenceExists)
                    CreateSequenceForIdentity(expression.TableName, expression.Column.Name);
            }
            else
            {
                if (identitySequenceExists)
                    DeleteSequenceForIdentity(expression.TableName, expression.Column.Name);
            }

        }

        /// <summary>
        /// Executes an SQL expression to rename a column
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public override void Process(RenameColumnExpression expression)
        {
            Truncator.Truncate(expression);
            CheckColumn(expression.TableName, expression.OldName);
            CheckColumn(expression.TableName, expression.NewName);
            LockColumn(expression.TableName, expression.OldName);
            LockColumn(expression.TableName, expression.NewName);
            InternalProcess(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes a <c>ALTER TABLE DROP COLUMN</c> SQL expression
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public override void Process(DeleteColumnExpression expression)
        {
            Truncator.Truncate(expression);
            CheckColumn(expression.TableName, expression.ColumnNames);
            LockColumn(expression.TableName, expression.ColumnNames);
            foreach (string columnName in expression.ColumnNames)
            {
                try
                {
                    if (SequenceExists(string.Empty, GetSequenceName(expression.TableName, columnName)))
                    {
                        DeleteSequenceForIdentity(expression.TableName, columnName);
                    }
                }
                catch (ArgumentException)
                {
                    // Ignore argument exception???
                }
            }

            InternalProcess(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes a <c>CREATE TABLE</c> SQL expression
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public override void Process(CreateTableExpression expression)
        {
            Truncator.Truncate(expression);
            LockTable(expression.TableName);
            RegisterTableCreation(expression.TableName);
            InternalProcess(Generator.Generate(expression));
            foreach (ColumnDefinition colDef in expression.Columns)
            {
                if (colDef.IsIdentity)
                    CreateSequenceForIdentity(expression.TableName, colDef.Name);
            }

            /*if(FBOptions.TransactionModel == FirebirdTransactionModel.AutoCommitOnCheckFail)
                CommitRetaining();*/
            if (FBOptions.TransactionModel != FirebirdTransactionModel.None)
                CommitRetaining();

        }

        /// <summary>
        /// Executes a <c>ALTER TABLE</c> SQL expression
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public override void Process(AlterTableExpression expression)
        {
            Truncator.Truncate(expression);
            CheckTable(expression.TableName);
            LockTable(expression.TableName);
            InternalProcess(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes an SQL expression to rename a table
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public override void Process(RenameTableExpression expression)
        {
            Truncator.Truncate(expression);
            FirebirdSchemaProvider schema = new FirebirdSchemaProvider(this, _quoter);
            FirebirdTableDefinition firebirdTableDef = schema.GetTableDefinition(expression.OldName);
            firebirdTableDef.Name = expression.NewName;
            CreateTableExpression createNew = new CreateTableExpression()
            {
                TableName = expression.NewName,
                SchemaName = string.Empty
            };

            //copy column definitions (nb: avoid to copy key names, because in firebird they must be globally unique, so let it rename)
            firebirdTableDef.Columns.ToList().ForEach(x => createNew.Columns.Add(new ColumnDefinition()
            {
                Name = x.Name,
                DefaultValue = x.DefaultValue,
                IsForeignKey = x.IsForeignKey,
                IsIdentity = x.IsIdentity,
                IsIndexed = x.IsIndexed,
                IsNullable = x.IsNullable,
                IsPrimaryKey = x.IsPrimaryKey,
                IsUnique = x.IsUnique,
                ModificationType = x.ModificationType,
                Precision = x.Precision,
                Size = x.Size,
                Type = x.Type,
                CustomType = x.CustomType
            }));

            Process(createNew);

            int columnCount = firebirdTableDef.Columns.Count;
            string[] columns = firebirdTableDef.Columns.Select(x => x.Name).ToArray();
            InsertDataExpression data = new InsertDataExpression();
            data.TableName = firebirdTableDef.Name;
            data.SchemaName = firebirdTableDef.SchemaName;
            using (DataSet ds = ReadTableData(string.Empty, expression.OldName))
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    InsertionDataDefinition insert = new InsertionDataDefinition();
                    for (int i = 0; i < columnCount; i++)
                    {
                        insert.Add(new KeyValuePair<string, object>(columns[i], dr.ItemArray[i]));
                    }
                    data.Rows.Add(insert);
                }
            }
            Process(data);

            DeleteTableExpression delTable = new DeleteTableExpression()
            {
                TableName = expression.OldName,
                SchemaName = string.Empty
            };
            Process(delTable);
        }

        /// <summary>
        /// Executes a <c>DROP TABLE</c> SQL expression
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public override void Process(DeleteTableExpression expression)
        {
            Truncator.Truncate(expression);
            CheckTable(expression.TableName);
            LockTable(expression.TableName);
            InternalProcess(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes an SQL expression to alter a default constraint
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public override void Process(AlterDefaultConstraintExpression expression)
        {
            Truncator.Truncate(expression);
            CheckColumn(expression.TableName, expression.ColumnName);
            LockColumn(expression.TableName, expression.ColumnName);
            InternalProcess(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes an SQL expression to drop a default constraint
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public override void Process(DeleteDefaultConstraintExpression expression)
        {
            Truncator.Truncate(expression);
            CheckColumn(expression.TableName, expression.ColumnName);
            LockColumn(expression.TableName, expression.ColumnName);
            InternalProcess(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes an SQL expression to create an index
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public override void Process(CreateIndexExpression expression)
        {
            Truncator.Truncate(expression);
            CheckTable(expression.Index.TableName);
            CheckColumn(expression.Index.TableName, expression.Index.Columns.Select(x => x.Name));
            InternalProcess(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes an SQL expression to drop an index
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public override void Process(DeleteIndexExpression expression)
        {
            Truncator.Truncate(expression);
            CheckTable(expression.Index.TableName);
            CheckColumn(expression.Index.TableName, expression.Index.Columns.Select(x => x.Name));
            InternalProcess(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes a <c>CREATE SCHEMA</c> SQL expression
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public override void Process(CreateSchemaExpression expression)
        {
            Truncator.Truncate(expression);
            InternalProcess(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes a <c>ALTER SCHEMA</c> SQL expression
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public override void Process(AlterSchemaExpression expression)
        {
            Truncator.Truncate(expression);
            InternalProcess(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes a <c>DROP SCHEMA</c> SQL expression
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public override void Process(DeleteSchemaExpression expression)
        {
            Truncator.Truncate(expression);
            InternalProcess(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes an SQL expression to create a constraint
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public override void Process(CreateConstraintExpression expression)
        {
            Truncator.Truncate(expression);
            InternalProcess(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes an SQL expression to drop a constraint
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public override void Process(DeleteConstraintExpression expression)
        {
            Truncator.Truncate(expression);
            InternalProcess(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes an SQL expression to create a foreign key
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public override void Process(CreateForeignKeyExpression expression)
        {
            Truncator.Truncate(expression);
            InternalProcess(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes an SQL expression to drop a foreign key
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public override void Process(DeleteForeignKeyExpression expression)
        {
            Truncator.Truncate(expression);
            InternalProcess(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes a <c>CREATE SEQUENCE</c> SQL expression
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public override void Process(CreateSequenceExpression expression)
        {
            Truncator.Truncate(expression);
            InternalProcess(Generator.Generate(expression));

            if (expression.Sequence.StartWith != null)
                InternalProcess(((FirebirdGenerator) Generator).GenerateAlterSequence(expression.Sequence));

        }

        /// <summary>
        /// Executes a <c>DROP SEQUENCE</c> SQL expression
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public override void Process(DeleteSequenceExpression expression)
        {
            Truncator.Truncate(expression);
            InternalProcess(Generator.Generate(expression));
        }

        /// <summary>
        /// Executes an SQL expression to INSERT data
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public override void Process(InsertDataExpression expression)
        {
            Truncator.Truncate(expression);
            CheckTable(expression.TableName);
            expression.Rows.ForEach(x => x.ForEach(y => CheckColumn(expression.TableName, y.Key)));
            var subExpression = new InsertDataExpression() { SchemaName = expression.SchemaName, TableName = expression.TableName };
            foreach (var row in expression.Rows)
            {
                subExpression.Rows.Clear();
                subExpression.Rows.Add(row);
                InternalProcess(Generator.Generate(subExpression));
            }
        }

        /// <summary>
        /// Executes an SQL expression to DELETE data
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public override void Process(DeleteDataExpression expression)
        {
            Truncator.Truncate(expression);
            CheckTable(expression.TableName);
            var subExpression = new DeleteDataExpression()
            {
                SchemaName = expression.SchemaName,
                TableName = expression.TableName,
                IsAllRows = expression.IsAllRows
            };
            if (expression.IsAllRows)
            {
                InternalProcess(Generator.Generate(expression));
            }
            else
            {
                foreach (var row in expression.Rows)
                {
                    subExpression.Rows.Clear();
                    subExpression.Rows.Add(row);
                    InternalProcess(Generator.Generate(subExpression));
                }
            }
        }

        /// <summary>
        /// Executes an SQL expression to UPDATE data
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public override void Process(UpdateDataExpression expression)
        {
            Truncator.Truncate(expression);
            CheckColumn(expression.TableName, expression.Set.Select(x => x.Key));
            InternalProcess(Generator.Generate(expression));
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
        /// Executes a DB operation
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        public override void Process(PerformDBOperationExpression expression)
        {
            Logger.LogSay("Performing DB Operation");

            if (Options.PreviewOnly)
                return;

            EnsureConnectionIsOpen();

            if (expression.Operation != null)
            {
                expression.Operation(Connection, Transaction);

                if (FBOptions.TransactionModel == FirebirdTransactionModel.AutoCommit)
                    CommitRetaining();

            }
        }

        /// <summary>
        /// Internals the process.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <exception cref="Exception"></exception>
        protected void InternalProcess(string sql)
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

            if (FBOptions.TransactionModel == FirebirdTransactionModel.AutoCommit)
                CommitRetaining();

        }

        /// <summary>
        /// Processes the specified SQL.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        protected override void Process(string sql)
        {
            InternalProcess(sql);
        }

        /// <summary>
        /// Formats the name of to safe.
        /// </summary>
        /// <param name="sqlName">Name of the SQL.</param>
        /// <returns>System.String.</returns>
        private string FormatToSafeName(string sqlName)
        {
            if (_quoter.IsQuoted(sqlName))
                return FormatHelper.FormatSqlEscape(_quoter.UnQuote(sqlName));
            else
                return FormatHelper.FormatSqlEscape(sqlName).ToUpper();
        }

        /// <summary>
        /// Gets the name of the sequence.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>System.String.</returns>
        private string GetSequenceName(string tableName, string columnName)
        {
            return Truncator.Truncate($"gen_{tableName}_{columnName}");
        }

        /// <summary>
        /// Gets the name of the identity trigger.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>System.String.</returns>
        private string GetIdentityTriggerName(string tableName, string columnName)
        {
            return Truncator.Truncate($"gen_id_{tableName}_{columnName}");
        }

        /// <summary>
        /// Creates the sequence for identity.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columnName">Name of the column.</param>
        private void CreateSequenceForIdentity(string tableName, string columnName)
        {
            CheckTable(tableName);
            LockTable(tableName);
            string sequenceName = GetSequenceName(tableName, columnName);
            if (!SequenceExists(string.Empty, sequenceName))
            {
                CreateSequenceExpression sequence = new CreateSequenceExpression()
                {
                    Sequence = new SequenceDefinition() { Name = sequenceName }
                };
                Process(sequence);
            }
            string triggerName = GetIdentityTriggerName(tableName, columnName);
            string quotedColumn = _quoter.Quote(columnName);
            string trigger = string.Format("as begin if (NEW.{0} is NULL) then NEW.{1} = GEN_ID({2}, 1); end", quotedColumn, quotedColumn, _quoter.QuoteSequenceName(sequenceName, string.Empty));

            PerformDBOperationExpression createTrigger = CreateTriggerExpression(tableName, triggerName, true, TriggerEvent.Insert, trigger);
            Process(createTrigger);
        }

        /// <summary>
        /// Deletes the sequence for identity.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columnName">Name of the column.</param>
        private void DeleteSequenceForIdentity(string tableName, string columnName)
        {
            CheckTable(tableName);
            LockTable(tableName);

            string sequenceName;
            try{
                sequenceName = GetSequenceName(tableName, columnName);
            }
            catch (ArgumentException)
            {
                return;
            }

            DeleteSequenceExpression deleteSequence = null;
            if (SequenceExists(string.Empty, sequenceName))
            {
                deleteSequence = new DeleteSequenceExpression() { SchemaName = string.Empty, SequenceName = sequenceName };
            }
            string triggerName = GetIdentityTriggerName(tableName, columnName);
            PerformDBOperationExpression deleteTrigger = DeleteTriggerExpression(tableName, triggerName);
            Process(deleteTrigger);

            if (deleteSequence != null)
                Process(deleteSequence);

        }

        /// <summary>
        /// Creates the trigger expression.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="trigger">The trigger.</param>
        /// <returns>PerformDBOperationExpression.</returns>
        public PerformDBOperationExpression CreateTriggerExpression(string tableName, TriggerInfo trigger)
        {
            return CreateTriggerExpression(tableName, trigger.Name, trigger.Before, trigger.Event, trigger.Body);
        }

        /// <summary>
        /// Creates the trigger expression.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="triggerName">Name of the trigger.</param>
        /// <param name="onBefore">if set to <c>true</c> [on before].</param>
        /// <param name="onEvent">The on event.</param>
        /// <param name="triggerBody">The trigger body.</param>
        /// <returns>PerformDBOperationExpression.</returns>
        public PerformDBOperationExpression CreateTriggerExpression(string tableName, string triggerName, bool onBefore, TriggerEvent onEvent, string triggerBody)
        {
            tableName = Truncator.Truncate(tableName);
            triggerName = Truncator.Truncate(triggerName);
            CheckTable(tableName);
            LockTable(tableName);
            PerformDBOperationExpression createTrigger = new PerformDBOperationExpression();
            createTrigger.Operation = (connection, transaction) =>
            {
                string triggerSql = string.Format(@"CREATE TRIGGER {0} FOR {1} ACTIVE {2} {3} POSITION 0
                    {4}
                    ", _quoter.Quote(triggerName), _quoter.Quote(tableName),
                     onBefore ? "before" : "after",
                     onEvent.ToString().ToLower(),
                     triggerBody
                     );
                Logger.LogSql(triggerSql);
                using (var cmd = CreateCommand(triggerSql, connection, transaction))
                {
                    cmd.ExecuteNonQuery();
                }
            };
            return createTrigger;
        }

        /// <summary>
        /// Deletes the trigger expression.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="triggerName">Name of the trigger.</param>
        /// <returns>PerformDBOperationExpression.</returns>
        public PerformDBOperationExpression DeleteTriggerExpression(string tableName, string triggerName)
        {
            tableName = Truncator.Truncate(tableName);
            triggerName = Truncator.Truncate(triggerName);
            CheckTable(tableName);
            LockTable(tableName);
            PerformDBOperationExpression deleteTrigger = new PerformDBOperationExpression();
            deleteTrigger.Operation = (connection, transaction) =>
            {
                string triggerSql = string.Format("DROP TRIGGER {0}", _quoter.Quote(triggerName));
                Logger.LogSql(triggerSql);
                using (var cmd = CreateCommand(triggerSql, connection, transaction))
                {
                    cmd.ExecuteNonQuery();
                }
            };
            return deleteTrigger;
        }
    }
}
