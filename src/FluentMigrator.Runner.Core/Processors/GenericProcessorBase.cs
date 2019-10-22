// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Core
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="GenericProcessorBase.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License

// Copyright (c) 2007-2018, Sean Chambers <schambers80@gmail.com>
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
using System.Data;
using System.Data.Common;
using System.Diagnostics;

using FluentMigrator.Runner.Initialization;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;

namespace FluentMigrator.Runner.Processors
{
    /// <summary>
    /// Class GenericProcessorBase.
    /// Implements the <see cref="FluentMigrator.Runner.Processors.ProcessorBase" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Processors.ProcessorBase" />
    public abstract class GenericProcessorBase : ProcessorBase
    {
        /// <summary>
        /// The connection string
        /// </summary>
        [Obsolete]
        private readonly string _connectionString;

        /// <summary>
        /// The database provider factory
        /// </summary>
        [NotNull, ItemCanBeNull]
        private readonly Lazy<DbProviderFactory> _dbProviderFactory;

        /// <summary>
        /// The lazy connection
        /// </summary>
        [NotNull, ItemCanBeNull]
        private readonly Lazy<IDbConnection> _lazyConnection;

        /// <summary>
        /// The connection
        /// </summary>
        [CanBeNull]
        private IDbConnection _connection;

        /// <summary>
        /// The disposed
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericProcessorBase"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="factory">The factory.</param>
        /// <param name="generator">The generator.</param>
        /// <param name="announcer">The announcer.</param>
        /// <param name="options">The options.</param>
        [Obsolete]
        protected GenericProcessorBase(
            IDbConnection connection,
            IDbFactory factory,
            IMigrationGenerator generator,
            IAnnouncer announcer,
            [NotNull] IMigrationProcessorOptions options)
            : base(generator, announcer, options)
        {
            _dbProviderFactory = new Lazy<DbProviderFactory>(() => (factory as DbFactoryBase)?.Factory);

            // Set the connection string, because it cannot be set by
            // the base class (due to the missing information)
            Options.ConnectionString = connection?.ConnectionString;

            // Prefetch connectionstring as after opening the security info could no longer be present
            // for instance on sql server
            _connectionString = connection?.ConnectionString;

            Factory = factory;

            _lazyConnection = new Lazy<IDbConnection>(() => connection);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericProcessorBase"/> class.
        /// </summary>
        /// <param name="factoryAccessor">The factory accessor.</param>
        /// <param name="generator">The generator.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="options">The options.</param>
        /// <param name="connectionStringAccessor">The connection string accessor.</param>
        protected GenericProcessorBase(
            [CanBeNull] Func<DbProviderFactory> factoryAccessor,
            [NotNull] IMigrationGenerator generator,
            [NotNull] ILogger logger,
            [NotNull] ProcessorOptions options,
            [NotNull] IConnectionStringAccessor connectionStringAccessor)
            : base(generator, logger, options)
        {
            _dbProviderFactory = new Lazy<DbProviderFactory>(() => factoryAccessor?.Invoke());

            var connectionString = connectionStringAccessor.ConnectionString;

#pragma warning disable 612
            var legacyFactory = new DbFactoryWrapper(this);

            // Prefetch connectionstring as after opening the security info could no longer be present
            // for instance on sql server
            _connectionString = connectionString;

            Factory = legacyFactory;
#pragma warning restore 612

            _lazyConnection = new Lazy<IDbConnection>(
                () =>
                {
                    if (DbProviderFactory == null)
                        return null;
                    var connection = DbProviderFactory.CreateConnection();
                    Debug.Assert(connection != null, nameof(Connection) + " != null");
                    connection.ConnectionString = connectionString;
                    connection.Open();
                    return connection;
                });
        }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        [Obsolete("Will change from public to protected")]
        public override string ConnectionString => _connectionString;

        /// <summary>
        /// Gets or sets the connection.
        /// </summary>
        /// <value>The connection.</value>
        public IDbConnection Connection
        {
            get => _connection ?? _lazyConnection.Value;
            protected set => _connection = value;
        }

        /// <summary>
        /// Gets or sets the factory.
        /// </summary>
        /// <value>The factory.</value>
        [Obsolete]
        [NotNull]
        public IDbFactory Factory { get; protected set; }

        /// <summary>
        /// Gets or sets the transaction.
        /// </summary>
        /// <value>The transaction.</value>
        [CanBeNull]
        public IDbTransaction Transaction { get; protected set; }

        /// <summary>
        /// Gets the database provider factory.
        /// </summary>
        /// <value>The database provider factory.</value>
        [CanBeNull]
        protected DbProviderFactory DbProviderFactory => _dbProviderFactory.Value;

        /// <summary>
        /// Ensures the connection is open.
        /// </summary>
        protected virtual void EnsureConnectionIsOpen()
        {
            if (Connection != null && Connection.State != ConnectionState.Open)
            {
                Connection.Open();
            }
        }

        /// <summary>
        /// Ensures the connection is closed.
        /// </summary>
        protected virtual void EnsureConnectionIsClosed()
        {
            if ((_connection != null || (_lazyConnection.IsValueCreated && Connection != null)) && Connection.State != ConnectionState.Closed)
            {
                Connection.Close();
            }
        }

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        public override void BeginTransaction()
        {
            if (Transaction != null) return;

            EnsureConnectionIsOpen();

            Logger.LogSay("Beginning Transaction");

            Transaction = Connection?.BeginTransaction();
        }

        /// <summary>
        /// Rollbacks the transaction.
        /// </summary>
        public override void RollbackTransaction()
        {
            if (Transaction == null) return;

            Logger.LogSay("Rolling back transaction");
            Transaction.Rollback();
            Transaction.Dispose();
            WasCommitted = true;
            Transaction = null;
        }

        /// <summary>
        /// Commits the transaction.
        /// </summary>
        public override void CommitTransaction()
        {
            if (Transaction == null) return;

            Logger.LogSay("Committing Transaction");
            Transaction.Commit();
            Transaction.Dispose();
            WasCommitted = true;
            Transaction = null;
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
            if ((_connection != null || (_lazyConnection.IsValueCreated && Connection != null)))
            {
                Connection.Dispose();
            }
        }

        /// <summary>
        /// Creates the command.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns>IDbCommand.</returns>
        protected virtual IDbCommand CreateCommand(string commandText)
        {
            return CreateCommand(commandText, Connection, Transaction);
        }

        /// <summary>
        /// Creates the command.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="transaction">The transaction.</param>
        /// <returns>IDbCommand.</returns>
        protected virtual IDbCommand CreateCommand(string commandText, IDbConnection connection, IDbTransaction transaction)
        {
            IDbCommand result;
            if (DbProviderFactory != null)
            {
                result = DbProviderFactory.CreateCommand();
                Debug.Assert(result != null, nameof(result) + " != null");
                result.Connection = connection;
                if (transaction != null)
                    result.Transaction = transaction;
                result.CommandText = commandText;
            }
            else
            {
#pragma warning disable 612
                result = Factory.CreateCommand(commandText, connection, transaction, Options);
#pragma warning restore 612
            }

            if (Options.Timeout != null)
            {
                result.CommandTimeout = (int) Options.Timeout.Value.TotalSeconds;
            }

            return result;
        }

        /// <summary>
        /// Class DbFactoryWrapper.
        /// Implements the <see cref="FluentMigrator.Runner.Processors.IDbFactory" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Runner.Processors.IDbFactory" />
        [Obsolete]
        private class DbFactoryWrapper : IDbFactory
        {
            /// <summary>
            /// The processor
            /// </summary>
            private readonly GenericProcessorBase _processor;

            /// <summary>
            /// Initializes a new instance of the <see cref="DbFactoryWrapper"/> class.
            /// </summary>
            /// <param name="processor">The processor.</param>
            public DbFactoryWrapper(GenericProcessorBase processor)
            {
                _processor = processor;
            }

            /// <inheritdoc />
            public IDbConnection CreateConnection(string connectionString)
            {
                Debug.Assert(_processor.DbProviderFactory != null, "_processor.DbProviderFactory != null");
                var result = _processor.DbProviderFactory.CreateConnection();
                Debug.Assert(result != null, nameof(result) + " != null");
                result.ConnectionString = connectionString;
                return result;
            }

            /// <inheritdoc />
            [Obsolete]
            public IDbCommand CreateCommand(
                string commandText,
                IDbConnection connection,
                IDbTransaction transaction,
                IMigrationProcessorOptions options)
            {
                return _processor.CreateCommand(commandText);
            }
        }
    }
}
