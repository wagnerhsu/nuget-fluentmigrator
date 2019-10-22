// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Core
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="IDbFactory.cs" company="FluentMigrator Project">
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

namespace FluentMigrator.Runner.Processors
{
    /// <summary>
    /// Interface IDbFactory
    /// </summary>
    [Obsolete]
    public interface IDbFactory
    {
        /// <summary>
        /// Creates the connection.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>IDbConnection.</returns>
        [Obsolete]
        IDbConnection CreateConnection(string connectionString);

        /// <summary>
        /// Creates the command.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="transaction">The transaction.</param>
        /// <param name="options">The options.</param>
        /// <returns>IDbCommand.</returns>
        [Obsolete]
        IDbCommand CreateCommand(string commandText, IDbConnection connection, IDbTransaction transaction, IMigrationProcessorOptions options);
    }
}
