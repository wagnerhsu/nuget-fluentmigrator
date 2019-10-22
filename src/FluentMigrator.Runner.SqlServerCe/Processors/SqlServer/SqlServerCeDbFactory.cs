// ***********************************************************************
// Assembly         : FluentMigrator.Runner.SqlServerCe
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SqlServerCeDbFactory.cs" company="FluentMigrator Project">
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
using System.Data;

namespace FluentMigrator.Runner.Processors.SqlServer
{
    /// <summary>
    /// Class SqlServerCeDbFactory.
    /// Implements the <see cref="FluentMigrator.Runner.Processors.ReflectionBasedDbFactory" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Processors.ReflectionBasedDbFactory" />
    public class SqlServerCeDbFactory : ReflectionBasedDbFactory
    {
        /// <summary>
        /// The entries
        /// </summary>
        private static readonly TestEntry[] _entries =
        {
            new TestEntry("System.Data.SqlServerCe", "System.Data.SqlServerCe.SqlCeProviderFactory"),
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerCeDbFactory"/> class.
        /// </summary>
        [Obsolete]
        public SqlServerCeDbFactory()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerCeDbFactory"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public SqlServerCeDbFactory(IServiceProvider serviceProvider)
            : base(serviceProvider, _entries)
        {
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
        public override IDbCommand CreateCommand(string commandText, IDbConnection connection, IDbTransaction transaction, IMigrationProcessorOptions options)
        {
            var command = connection.CreateCommand();
            command.CommandText = commandText;
            // SQL Server CE does not support non-zero command timeout values!! :/
            if (transaction != null) command.Transaction = transaction;
            return command;
        }
    }
}
