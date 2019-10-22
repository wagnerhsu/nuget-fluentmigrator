// ***********************************************************************
// Assembly         : FluentMigrator.Runner
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="AppConfigConnectionStringReader.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
// Copyright (c) 2018, FluentMigrator Project
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

#if NETFRAMEWORK

using System;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;

using FluentMigrator.Runner.Logging;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FluentMigrator.Runner.Initialization.NetFramework
{
    /// <summary>
    /// A <see cref="IConnectionStringReader" /> implementation that uses the app or machine config
    /// </summary>
    [Obsolete]
    public class AppConfigConnectionStringReader : IConnectionStringReader
    {
        /// <summary>
        /// The match password
        /// </summary>
        private static readonly Regex _matchPwd = new Regex("(PWD=|PASSWORD=)([^;]*);", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// The configuration manager
        /// </summary>
        [NotNull]
        private readonly INetConfigManager _configManager;

        /// <summary>
        /// The logger
        /// </summary>
        [NotNull]
        private readonly ILogger _logger;

        /// <summary>
        /// The options
        /// </summary>
        [NotNull]
        private readonly AppConfigConnectionStringAccessorOptions _options;

        /// <summary>
        /// The assembly location
        /// </summary>
        [CanBeNull]
        private readonly string _assemblyLocation;

        /// <summary>
        /// The connection information
        /// </summary>
        [CanBeNull]
        private ConnectionInfo _connectionInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppConfigConnectionStringReader"/> class.
        /// </summary>
        /// <param name="configManager">The configuration manager.</param>
        /// <param name="assemblySource">The assembly source.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="options">The options.</param>
        public AppConfigConnectionStringReader(
            [NotNull] INetConfigManager configManager,
            [NotNull] IAssemblySource assemblySource,
            [NotNull] ILogger<AppConfigConnectionStringReader> logger,
            [NotNull] IOptions<AppConfigConnectionStringAccessorOptions> options)
        {
            _configManager = configManager;
            _logger = logger;
            _options = options.Value;
            var assemblies = assemblySource.Assemblies;
            var singleAssembly = assemblies.Count == 1 ? assemblies.Single() : null;
            _assemblyLocation = singleAssembly != null ? singleAssembly.Location : string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppConfigConnectionStringReader"/> class.
        /// </summary>
        /// <param name="configManager">The configuration manager.</param>
        /// <param name="assemblyLocation">The assembly location.</param>
        /// <param name="announcer">The announcer.</param>
        /// <param name="options">The options.</param>
        [Obsolete]
        public AppConfigConnectionStringReader(
            [NotNull] INetConfigManager configManager,
            [NotNull] string assemblyLocation,
            [NotNull] IAnnouncer announcer,
            [NotNull] IOptions<AppConfigConnectionStringAccessorOptions> options)
        {
            _configManager = configManager;
            _logger = new AnnouncerFluentMigratorLogger(announcer);
            _options = options.Value;
            _assemblyLocation = assemblyLocation;
        }

        /// <inheritdoc />
        public int Priority { get; } = 0;

        /// <inheritdoc />
        public string GetConnectionString(string connectionStringOrName)
        {
            if (_connectionInfo != null)
            {
                return _connectionInfo.ConnectionString;
            }

            var result = LoadConnectionString(connectionStringOrName, _assemblyLocation);
            OutputResults(result);

            _connectionInfo = result;

            return result?.ConnectionString ?? connectionStringOrName;
        }

        /// <summary>
        /// Loads the connection string.
        /// </summary>
        /// <param name="connectionStringOrName">Name of the connection string or.</param>
        /// <param name="assemblyLocation">The assembly location.</param>
        /// <returns>ConnectionInfo.</returns>
        [CanBeNull]
        private ConnectionInfo LoadConnectionString([CanBeNull] string connectionStringOrName, [CanBeNull] string assemblyLocation)
        {
            ConnectionInfo result = null;

            if (!string.IsNullOrEmpty(_options.ConnectionStringConfigPath))
            {
                result = LoadConnectionStringFromConfigurationFile(connectionStringOrName, _configManager.LoadFromFile(_options.ConnectionStringConfigPath));
            }

            if (result == null && !string.IsNullOrEmpty(assemblyLocation))
            {
                result = LoadConnectionStringFromConfigurationFile(connectionStringOrName, _configManager.LoadFromFile(assemblyLocation));
            }

            if (result == null)
            {
                result = LoadConnectionStringFromConfigurationFile(connectionStringOrName, _configManager.LoadFromMachineConfiguration());
            }

            if (result == null && !string.IsNullOrEmpty(connectionStringOrName))
            {
                result = new ConnectionInfo(name: null, connectionStringOrName, source: null);
            }

            return result;
        }

        /// <summary>
        /// Loads the connection string from configuration file.
        /// </summary>
        /// <param name="connectionStringName">Name of the connection string.</param>
        /// <param name="configurationFile">The configuration file.</param>
        /// <returns>ConnectionInfo.</returns>
        [CanBeNull]
        private ConnectionInfo LoadConnectionStringFromConfigurationFile([CanBeNull] string connectionStringName, [NotNull] Configuration configurationFile)
        {
            var connections = configurationFile.ConnectionStrings.ConnectionStrings;

            if (connections == null || connections.Count <= 0)
                return null;

            ConnectionStringSettings connectionString;

            if (string.IsNullOrEmpty(connectionStringName))
                connectionString = connections[_options.MachineName ?? Environment.MachineName];
            else
                connectionString = connections[connectionStringName];

            return ReadConnectionString(connectionString, configurationFile.FilePath);
        }

        /// <summary>
        /// Reads the connection string.
        /// </summary>
        /// <param name="connectionSetting">The connection setting.</param>
        /// <param name="configurationFile">The configuration file.</param>
        /// <returns>ConnectionInfo.</returns>
        [Pure]
        [CanBeNull]
        private ConnectionInfo ReadConnectionString(
            [CanBeNull] ConnectionStringSettings connectionSetting,
            string configurationFile)
        {
            if (connectionSetting == null)
                return null;
            return new ConnectionInfo(connectionSetting.Name, connectionSetting.ConnectionString, configurationFile);
        }

        /// <summary>
        /// Outputs the results.
        /// </summary>
        /// <param name="info">The information.</param>
        private void OutputResults(ConnectionInfo info)
        {
            if (info == null)
            {
                _logger.LogError("Unable to resolve any connectionstring using parameters \"/connection\" and \"/configPath\"");
                return;
            }

            var connectionString = _matchPwd.Replace(info.ConnectionString, "$1********;");
            string message;
            if (string.IsNullOrEmpty(info.Source))
            {
                if (string.IsNullOrEmpty(info.Name))
                {
                    message = $"Using connection string {connectionString}";
                }
                else
                {
                    message = $"Using database {info.Name} and connection string {connectionString}";
                }
            }
            else
            {
                message = $"Using connection {info.Name} from configuration file {info.Source}";
            }

            _logger.LogSay(message);
        }

        /// <summary>
        /// Class ConnectionInfo.
        /// </summary>
        private class ConnectionInfo
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ConnectionInfo"/> class.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <param name="connectionString">The connection string.</param>
            /// <param name="source">The source.</param>
            public ConnectionInfo([CanBeNull] string name, [NotNull] string connectionString, [CanBeNull] string source)
            {
                Name = name;
                ConnectionString = connectionString;
                Source = source;
            }

            /// <summary>
            /// Gets the name.
            /// </summary>
            /// <value>The name.</value>
            [CanBeNull]
            public string Name { get; }

            /// <summary>
            /// Gets the connection string.
            /// </summary>
            /// <value>The connection string.</value>
            [NotNull]
            public string ConnectionString { get; }

            /// <summary>
            /// Gets the source.
            /// </summary>
            /// <value>The source.</value>
            [CanBeNull]
            public string Source { get; }
        }
    }
}
#endif
