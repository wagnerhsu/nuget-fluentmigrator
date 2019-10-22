// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Core
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="DbFactoryBase.cs" company="FluentMigrator Project">
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

namespace FluentMigrator.Runner.Processors
{
#pragma warning disable 612
    /// <summary>
    /// Class DbFactoryBase.
    /// Implements the <see cref="FluentMigrator.Runner.Processors.IDbFactory" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Processors.IDbFactory" />
    public abstract class DbFactoryBase : IDbFactory
#pragma warning restore 612
    {
        /// <summary>
        /// The lock
        /// </summary>
        private readonly object _lock = new object();
        /// <summary>
        /// The factory
        /// </summary>
        private volatile DbProviderFactory _factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbFactoryBase"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        protected DbFactoryBase(DbProviderFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbFactoryBase"/> class.
        /// </summary>
        protected DbFactoryBase()
        {
        }

        /// <summary>
        /// Gets the DB provider factory
        /// </summary>
        /// <value>The factory.</value>
        public virtual DbProviderFactory Factory
        {
            get
            {
                if (_factory == null)
                {
                    lock (_lock)
                    {
                        if (_factory == null)
                        {
                            _factory = CreateFactory();
                        }
                    }
                }
                return _factory;
            }
        }

        /// <summary>
        /// Creates the factory.
        /// </summary>
        /// <returns>DbProviderFactory.</returns>
        protected abstract DbProviderFactory CreateFactory();

        /// <summary>
        /// Creates the connection.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>IDbConnection.</returns>
        [Obsolete]
        public IDbConnection CreateConnection(string connectionString)
        {
            var connection = Factory.CreateConnection();
            Debug.Assert(connection != null, nameof(connection) + " != null");
            connection.ConnectionString = connectionString;
            return connection;
        }

        /// <summary>
        /// Creates the command.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="transaction">The transaction.</param>
        /// <param name="options">The options.</param>
        /// <returns>IDbCommand.</returns>
        [Obsolete]
        public virtual IDbCommand CreateCommand(string commandText, IDbConnection connection, IDbTransaction transaction, IMigrationProcessorOptions options)
        {
            var command = connection.CreateCommand();
            command.CommandText = commandText;
            if (options?.Timeout != null) command.CommandTimeout = options.Timeout.Value;
            if (transaction != null) command.Transaction = transaction;
            return command;
        }
    }
}
