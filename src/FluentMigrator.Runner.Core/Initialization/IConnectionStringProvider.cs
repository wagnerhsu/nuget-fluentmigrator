// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Core
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="IConnectionStringProvider.cs" company="FluentMigrator Project">
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

using JetBrains.Annotations;

namespace FluentMigrator.Runner.Initialization
{
    /// <summary>
    /// Interface IConnectionStringProvider
    /// </summary>
    [Obsolete]
    public interface IConnectionStringProvider
    {
        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <param name="announcer">The announcer.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="configPath">The configuration path.</param>
        /// <param name="assemblyLocation">The assembly location.</param>
        /// <param name="database">The database.</param>
        /// <returns>System.String.</returns>
        [CanBeNull]
        string GetConnectionString(IAnnouncer announcer, string connection, string configPath, string assemblyLocation, string database);
    }
}
