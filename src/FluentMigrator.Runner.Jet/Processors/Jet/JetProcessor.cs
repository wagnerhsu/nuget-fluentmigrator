// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Jet
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="JetProcessor.cs" company="FluentMigrator Project">
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
using System.Data.OleDb;
using System.Diagnostics;

using FluentMigrator.Expressions;
using FluentMigrator.Runner.Generators.Jet;
using FluentMigrator.Runner.Initialization;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FluentMigrator.Runner.Processors.Jet
{
    /// <summary>
    /// Class JetProcessor.
    /// Implements the <see cref="FluentMigrator.Runner.Processors.ProcessorBase" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Processors.ProcessorBase" />
    public class JetProcessor : ProcessorBase
    {
        /// <summary>
        /// The connection
        /// </summary>
        private readonly Lazy<OleDbConnection> _connection;
        /// <summary>
        /// The transaction
        /// </summary>
        private OleDbTransaction _transaction;
        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <value>The connection.</value>
        public OleDbConnection Connection => _connection.Value;
        /// <summary>
        /// Gets the transaction.
        /// </summary>
        /// <value>The transaction.</value>
        public OleDbTransaction Transaction => _transaction;
        /// <summary>
        /// The disposed
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="JetProcessor"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="generator">The generator.</param>
        /// <param name="announcer">The announcer.</param>
        /// <param name="options">The options.</param>
        [Obsolete]
        public JetProcessor(IDbConnection connection, IMigrationGenerator generator, IAnnouncer announcer, IMigrationProcessorOptions options)
            : base(generator, announcer, options)
        {
            _connection = new Lazy<OleDbConnection>(() => (OleDbConnection) connection);

            // Prefetch connectionstring as after opening the security info could no longer be present
            // for instance on sql server
            ConnectionString = connection.ConnectionString;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JetProcessor"/> class.
        /// </summary>
        /// <param name="generator">The generator.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="options">The options.</param>
        /// <param name="connectionStringAccessor">The connection string accessor.</param>
        public JetProcessor(
            [NotNull] JetGenerator generator,
            [NotNull] ILogger<JetProcessor> logger,
            [NotNull] IOptionsSnapshot<ProcessorOptions> options,
            [NotNull] IConnectionStringAccessor connectionStringAccessor)
            : base(generator, logger, options.Value)
        {
            var factory = OleDbFactory.Instance;
            var connectionString = connectionStringAccessor.ConnectionString ?? options.Value.ConnectionString;
            if (factory != null)
            {
                _connection = new Lazy<OleDbConnection>(
                    () =>
                    {
                        var conn = (OleDbConnection) factory.CreateConnection();
                        Debug.Assert(conn != null, nameof(conn) + " != null");
                        conn.ConnectionString = connectionString;
                        return conn;
                    });
            }

#pragma warning disable 612
            ConnectionString = connectionString;
#pragma warning restore 612
        }

        /// <summary>
        /// Gets the connection string
        /// </summary>
        /// <value>The connection string.</value>
        [Obsolete]
        public override string ConnectionString { get; }

        /// <summary>
        /// Gets the database type
        /// </summary>
        /// <value>The type of the database.</value>
        public override string DatabaseType { get; } = "Jet";

        /// <summary>
        /// Gets the database type aliases
        /// </summary>
        /// <value>The database type aliases.</value>
        public override IList<string> DatabaseTypeAliases { get; } = new List<string>();

        /// <summary>
        /// Ensures the connection is open.
        /// </summary>
        protected void EnsureConnectionIsOpen()
        {
            if (Connection.State != ConnectionState.Open)
                Connection.Open();
        }

        /// <summary>
        /// Ensures the connection is closed.
        /// </summary>
        protected void EnsureConnectionIsClosed()
        {
            if (Connection.State != ConnectionState.Closed)
                Connection.Close();
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

            expression.Operation?.Invoke(Connection, _transaction);
        }

        /// <summary>
        /// Processes the specified SQL.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <exception cref="Exception"></exception>
        protected override void Process(string sql)
        {
            Logger.LogSql(sql);

            if (Options.PreviewOnly || string.IsNullOrEmpty(sql))
                return;

            EnsureConnectionIsOpen();

            using (var command = new OleDbCommand(sql, Connection, Transaction))
            {
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (OleDbException ex)
                {
                    throw new Exception(string.Format("Exception while processing \"{0}\"", sql), ex);
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

            var ds = new DataSet();
            using (var command = new OleDbCommand(string.Format(template, args), Connection, Transaction))
            using (var adapter = new OleDbDataAdapter(command))
            {
                adapter.Fill(ds);
                return ds;
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

            using (var command = new OleDbCommand(string.Format(template, args), Connection, Transaction))
            using (var reader = command.ExecuteReader())
            {
                Debug.Assert(reader != null, nameof(reader) + " != null");
                return reader.Read();
            }
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
        /// Schemas the exists.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public override bool SchemaExists(string tableName)
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
            EnsureConnectionIsOpen();

            var restrict = new object[] { null, null, tableName, "TABLE" };
            using (var tables = Connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, restrict))
            {
                Debug.Assert(tables != null, nameof(tables) + " != null");
                for (int i = 0; i < tables.Rows.Count; i++)
                {
                    var name = tables.Rows[i].ItemArray[2].ToString();
                    if (name == tableName)
                    {
                        return true;
                    }
                }
                return false;
            }
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
            EnsureConnectionIsOpen();

            var restrict = new object[] { null, null, tableName, null };
            using (var columns = Connection.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, restrict))
            {
                Debug.Assert(columns != null, nameof(columns) + " != null");
                for (int i = 0; i < columns.Rows.Count; i++)
                {
                    var name = columns.Rows[i].ItemArray[3].ToString();
                    if (name == columnName)
                    {
                        return true;
                    }
                }
                return false;
            }
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
            EnsureConnectionIsOpen();

            var restrict = new object[] { null, null, constraintName, null, null, tableName };
            using (var constraints = Connection.GetOleDbSchemaTable(OleDbSchemaGuid.Table_Constraints, restrict))
            {
                Debug.Assert(constraints != null, nameof(constraints) + " != null");
                return constraints.Rows.Count > 0;
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
            EnsureConnectionIsOpen();

            var restrict = new object[] { null, null, indexName, null, tableName };
            using (var indexes = Connection.GetOleDbSchemaTable(OleDbSchemaGuid.Indexes, restrict))
            {
                Debug.Assert(indexes != null, nameof(indexes) + " != null");
                return indexes.Rows.Count > 0;
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
        /// Begins a transaction
        /// </summary>
        public override void BeginTransaction()
        {
            if (_transaction != null) return;

            EnsureConnectionIsOpen();

            Logger.LogSay("Beginning Transaction");
            _transaction = Connection.BeginTransaction();
        }

        /// <summary>
        /// Rollback of a transaction
        /// </summary>
        public override void RollbackTransaction()
        {
            if (_transaction == null) return;

            Logger.LogSay("Rolling back transaction");
            _transaction.Rollback();
            WasCommitted = true;
            _transaction = null;
        }

        /// <summary>
        /// Commits a transaction
        /// </summary>
        public override void CommitTransaction()
        {
            if (_transaction == null) return;

            Logger.LogSay("Committing Transaction");
            _transaction.Commit();
            WasCommitted = true;
            _transaction = null;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="isDisposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool isDisposing)
        {
            if (!isDisposing || _disposed)
                return;

            _disposed = true;

            RollbackTransaction();
            EnsureConnectionIsClosed();
        }
    }
}
