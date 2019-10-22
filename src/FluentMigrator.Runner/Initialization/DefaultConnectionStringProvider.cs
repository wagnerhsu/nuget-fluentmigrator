// ***********************************************************************
// Assembly         : FluentMigrator.Runner
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="DefaultConnectionStringProvider.cs" company="FluentMigrator Project">
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
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using JetBrains.Annotations;

#if NETFRAMEWORK
using FluentMigrator.Runner.Initialization.NetFramework;
using Microsoft.Extensions.Options;
#endif

namespace FluentMigrator.Runner.Initialization
{
    /// <summary>
    /// Class DefaultConnectionStringProvider.
    /// Implements the <see cref="FluentMigrator.Runner.Initialization.IConnectionStringProvider" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Initialization.IConnectionStringProvider" />
    [Obsolete]
    public class DefaultConnectionStringProvider : IConnectionStringProvider
    {
        /// <summary>
        /// The accessors
        /// </summary>
        [CanBeNull]
        [ItemNotNull]
        private readonly IReadOnlyCollection<IConnectionStringReader> _accessors;

        /// <summary>
        /// The synchronize root
        /// </summary>
        private readonly object _syncRoot = new object();
        /// <summary>
        /// The connection string
        /// </summary>
        private string _connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultConnectionStringProvider"/> class.
        /// </summary>
        [Obsolete]
        public DefaultConnectionStringProvider()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultConnectionStringProvider"/> class.
        /// </summary>
        /// <param name="accessors">The accessors.</param>
        public DefaultConnectionStringProvider([NotNull, ItemNotNull] IEnumerable<IConnectionStringReader> accessors)
        {
            _accessors = accessors.ToList();
        }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <param name="announcer">The announcer.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="configPath">The configuration path.</param>
        /// <param name="assemblyLocation">The assembly location.</param>
        /// <param name="database">The database.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="InvalidOperationException">No connection string specified</exception>
        public string GetConnectionString(IAnnouncer announcer, string connection, string configPath, string assemblyLocation,
            string database)
        {
            if (_connectionString == null)
            {
                lock (_syncRoot)
                {
                    if (_connectionString == null)
                    {
                        var accessors = _accessors ?? CreateAccessors(assemblyLocation, announcer, configPath).ToList();
                        var result = GetConnectionString(accessors, connection, database);
                        if (string.IsNullOrEmpty(result))
                            result = connection;
                        if (string.IsNullOrEmpty(result))
                            throw new InvalidOperationException("No connection string specified");
                        return _connectionString = result;
                    }
                }
            }

            return _connectionString;
        }

        /// <summary>
        /// Creates the accessors.
        /// </summary>
        /// <param name="assemblyLocation">The assembly location.</param>
        /// <param name="announcer">The announcer.</param>
        /// <param name="configPath">The configuration path.</param>
        /// <returns>IEnumerable&lt;IConnectionStringReader&gt;.</returns>
        [SuppressMessage("ReSharper", "UnusedParameter.Local", Justification = "Parameters are required for the full .NET Framework")]
        private static IEnumerable<IConnectionStringReader> CreateAccessors(string assemblyLocation, IAnnouncer announcer, string configPath)
        {
#if NETFRAMEWORK
#pragma warning disable 612
            var options = new AppConfigConnectionStringAccessorOptions()
            {
                ConnectionStringConfigPath = configPath,
            };

            yield return new AppConfigConnectionStringReader(
                new NetConfigManager(),
                assemblyLocation,
                announcer,
                new OptionsWrapper<AppConfigConnectionStringAccessorOptions>(options));
#pragma warning restore 612
#else
            yield break;
#endif
        }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <param name="accessors">The accessors.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="database">The database.</param>
        /// <returns>System.String.</returns>
        private static string GetConnectionString(IReadOnlyCollection<IConnectionStringReader> accessors, string connection, string database)
        {
            var result = GetConnectionString(accessors, connection);
            if (result == null)
                result = GetConnectionString(accessors, database);
            if (result == null)
                result = GetConnectionString(accessors, Environment.MachineName);
            return result;
        }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <param name="accessors">The accessors.</param>
        /// <param name="connectionStringOrName">Name of the connection string or.</param>
        /// <returns>System.String.</returns>
        private static string GetConnectionString(IReadOnlyCollection<IConnectionStringReader> accessors, string connectionStringOrName)
        {
            if (string.IsNullOrEmpty(connectionStringOrName))
                return null;

            foreach (var accessor in accessors.OrderByDescending(x => x.Priority))
            {
                var connectionString = accessor.GetConnectionString(connectionStringOrName);
                if (connectionString != null)
                    return connectionString;
            }

            return null;
        }
    }
}
