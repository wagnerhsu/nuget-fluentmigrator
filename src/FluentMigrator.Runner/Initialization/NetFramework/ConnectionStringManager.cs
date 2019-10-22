// ***********************************************************************
// Assembly         : FluentMigrator.Runner
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="ConnectionStringManager.cs" company="FluentMigrator Project">
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

#if NETFRAMEWORK
using System;
using System.Configuration;
using System.Text.RegularExpressions;

using FluentMigrator.Exceptions;

using Microsoft.Extensions.Logging;

namespace FluentMigrator.Runner.Initialization.NetFramework
{
    /// <summary>
    /// Locates connection strings by name in assembly's config file or machine.config
    /// If no connection matches it uses the specified connection string as valid connection
    /// </summary>
    internal class ConnectionStringManager
    {
        /// <summary>
        /// The match password
        /// </summary>
        private static readonly Regex _matchPwd = new Regex("(PWD=|PASSWORD=)([^;]*);", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger _logger;
        /// <summary>
        /// The assembly location
        /// </summary>
        private readonly string _assemblyLocation;
        /// <summary>
        /// The configuration manager
        /// </summary>
        private readonly INetConfigManager _configManager;
        /// <summary>
        /// The configuration path
        /// </summary>
        private readonly string _configPath;
        /// <summary>
        /// The database
        /// </summary>
        private readonly string _database;
        /// <summary>
        /// The configuration file
        /// </summary>
        private string _configFile;
        /// <summary>
        /// The connection
        /// </summary>
        private string _connection;
        /// <summary>
        /// The machine name provider
        /// </summary>
        private Func<string> _machineNameProvider = () => Environment.MachineName;
        /// <summary>
        /// The not using configuration
        /// </summary>
        private bool _notUsingConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionStringManager"/> class.
        /// </summary>
        /// <param name="configManager">The configuration manager.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="configPath">The configuration path.</param>
        /// <param name="assemblyLocation">The assembly location.</param>
        /// <param name="database">The database.</param>
        public ConnectionStringManager(INetConfigManager configManager, ILogger<ConnectionStringManager> logger, string connection, string configPath, string assemblyLocation,
                                       string database)
        {
            _connection = connection;
            _configPath = configPath;
            _database = database;
            _assemblyLocation = assemblyLocation;
            _notUsingConfig = true;
            _configManager = configManager;
            _logger = logger;
        }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString { get; private set; }

        /// <summary>
        /// Gets or sets the machine name provider.
        /// </summary>
        /// <value>The machine name provider.</value>
        public Func<string> MachineNameProvider
        {
            get { return _machineNameProvider; }
            set { _machineNameProvider = value; }
        }

        /// <summary>
        /// Loads the connection string.
        /// </summary>
        public void LoadConnectionString()
        {
            if (_notUsingConfig && !string.IsNullOrEmpty(_configPath))
                LoadConnectionStringFromConfigurationFile(_configManager.LoadFromFile(_configPath));

            if (_notUsingConfig && !string.IsNullOrEmpty(_assemblyLocation))
            {
                string defaultConfigFile = _assemblyLocation;

                LoadConnectionStringFromConfigurationFile(_configManager.LoadFromFile(defaultConfigFile));
            }

            if (_notUsingConfig)
                LoadConnectionStringFromConfigurationFile(_configManager.LoadFromMachineConfiguration());

            if (_notUsingConfig && !string.IsNullOrEmpty(_connection))
                ConnectionString = _connection;

            OutputResults();
        }

        /// <summary>
        /// Loads the connection string from configuration file.
        /// </summary>
        /// <param name="configurationFile">The configuration file.</param>
        private void LoadConnectionStringFromConfigurationFile(Configuration configurationFile)
        {
            var connections = configurationFile.ConnectionStrings.ConnectionStrings;

            if (connections == null || connections.Count <= 0)
                return;

            ConnectionStringSettings connectionString;

            if (string.IsNullOrEmpty(_connection))
                connectionString = connections[MachineNameProvider()];
            else
                connectionString = connections[_connection];

            ReadConnectionString(connectionString, configurationFile.FilePath);
        }

        /// <summary>
        /// Reads the connection string.
        /// </summary>
        /// <param name="connectionSetting">The connection setting.</param>
        /// <param name="configurationFile">The configuration file.</param>
        private void ReadConnectionString(ConnectionStringSettings connectionSetting, string configurationFile)
        {
            if (connectionSetting == null) return;

            _connection = connectionSetting.Name;
            ConnectionString = connectionSetting.ConnectionString;
            _configFile = configurationFile;
            _notUsingConfig = false;
        }

        /// <summary>
        /// Outputs the results.
        /// </summary>
        /// <exception cref="UndeterminableConnectionException">Unable to resolve any connectionstring using parameters \"/connection\" and \"/configPath\"</exception>
        private void OutputResults()
        {
            if (string.IsNullOrEmpty(ConnectionString))
                throw new UndeterminableConnectionException("Unable to resolve any connectionstring using parameters \"/connection\" and \"/configPath\"");

            _logger.LogSay(
                _notUsingConfig
                    ? $"Using Database {_database} and Connection String {_matchPwd.Replace(ConnectionString, "$1********;")}"
                    : $"Using Connection {_connection} from Configuration file {_configFile}");
        }
    }
}
#endif
