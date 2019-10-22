// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="IntegrationTestOptions.cs" company="FluentMigrator Project">
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.Configuration;

namespace FluentMigrator.Tests
{
    /// <summary>
    /// Class IntegrationTestOptions.
    /// </summary>
    public static class IntegrationTestOptions
    {
        /// <summary>
        /// The platform identifiers
        /// </summary>
        private static readonly ISet<string> _platformIdentifiers;

        /// <summary>
        /// Initializes static members of the <see cref="IntegrationTestOptions"/> class.
        /// </summary>
        static IntegrationTestOptions()
        {
            var asmPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var config = new ConfigurationBuilder()
                .SetBasePath(asmPath)
                .AddJsonFile("appsettings.json")
                .AddUserSecrets("FluentMigrator.Tests")
                .Build();
            DatabaseServers = config
                .GetSection("TestConnectionStrings")
                .Get<IReadOnlyDictionary<string, DatabaseServerOptions>>();
            if (Environment.Is64BitProcess)
            {
                _platformIdentifiers = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "amd64", "x64", "x86-64"
                };
            }
            else
            {
                _platformIdentifiers = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "x86", "x86-32", "x32"
                };
            }
        }

        /// <summary>
        /// Gets the database servers.
        /// </summary>
        /// <value>The database servers.</value>
        private static IReadOnlyDictionary<string, DatabaseServerOptions> DatabaseServers { get;}

        /// <summary>
        /// Gets the SQL server2005.
        /// </summary>
        /// <value>The SQL server2005.</value>
        public static DatabaseServerOptions SqlServer2005 => GetOptions("SqlServer2005");

        /// <summary>
        /// Gets the SQL server2008.
        /// </summary>
        /// <value>The SQL server2008.</value>
        public static DatabaseServerOptions SqlServer2008 => GetOptions("SqlServer2008");

        /// <summary>
        /// Gets the SQL server2012.
        /// </summary>
        /// <value>The SQL server2012.</value>
        public static DatabaseServerOptions SqlServer2012 => GetOptions("SqlServer2012");

        /// <summary>
        /// Gets the SQL server2014.
        /// </summary>
        /// <value>The SQL server2014.</value>
        public static DatabaseServerOptions SqlServer2014 => GetOptions("SqlServer2014");

        /// <summary>
        /// Gets the SQL server2016.
        /// </summary>
        /// <value>The SQL server2016.</value>
        public static DatabaseServerOptions SqlServer2016 => GetOptions("SqlServer2016");

        /// <summary>
        /// Gets the SQL server ce.
        /// </summary>
        /// <value>The SQL server ce.</value>
        public static DatabaseServerOptions SqlServerCe => GetOptions("SqlServerCe");

        /// <summary>
        /// Gets the SQL anywhere16.
        /// </summary>
        /// <value>The SQL anywhere16.</value>
        public static DatabaseServerOptions SqlAnywhere16 => GetOptions("SqlAnywhere16").GetOptionsForPlatform();

        /// <summary>
        /// Gets the jet.
        /// </summary>
        /// <value>The jet.</value>
        public static DatabaseServerOptions Jet => GetOptions("Jet");

        // ReSharper disable once InconsistentNaming
        /// <summary>
        /// Gets the sq lite.
        /// </summary>
        /// <value>The sq lite.</value>
        public static DatabaseServerOptions SQLite => GetOptions("SQLite");

        /// <summary>
        /// Gets my SQL.
        /// </summary>
        /// <value>My SQL.</value>
        public static DatabaseServerOptions MySql => GetOptions("MySql");

        /// <summary>
        /// Gets the postgres.
        /// </summary>
        /// <value>The postgres.</value>
        public static DatabaseServerOptions Postgres => GetOptions("Postgres");

        /// <summary>
        /// Gets the firebird.
        /// </summary>
        /// <value>The firebird.</value>
        public static DatabaseServerOptions Firebird => GetOptions("Firebird").GetOptionsForPlatform();

        /// <summary>
        /// Gets the oracle.
        /// </summary>
        /// <value>The oracle.</value>
        public static DatabaseServerOptions Oracle => GetOptions("Oracle");

        /// <summary>
        /// Gets the DB2.
        /// </summary>
        /// <value>The DB2.</value>
        public static DatabaseServerOptions Db2 => Environment.Is64BitProcess ? GetOptions("Db2") : DatabaseServerOptions.Empty;

        /// <summary>
        /// Gets the DB2 i series.
        /// </summary>
        /// <value>The DB2 i series.</value>
        public static DatabaseServerOptions Db2ISeries => GetOptions("Db2ISeries");

        /// <summary>
        /// Gets the hana.
        /// </summary>
        /// <value>The hana.</value>
        public static DatabaseServerOptions Hana => Environment.Is64BitProcess ? GetOptions("Hana") : DatabaseServerOptions.Empty;

        /// <summary>
        /// Class DatabaseServerOptions.
        /// </summary>
        public class DatabaseServerOptions
        {
            /// <summary>
            /// The supported platforms
            /// </summary>
            private ISet<string> _supportedPlatforms;
            /// <summary>
            /// The supported platforms value
            /// </summary>
            private string _supportedPlatformsValue;

            /// <summary>
            /// Gets the empty.
            /// </summary>
            /// <value>The empty.</value>
            public static DatabaseServerOptions Empty { get; } = new DatabaseServerOptions() { IsEnabled = false };

            /// <summary>
            /// Gets or sets the connection string.
            /// </summary>
            /// <value>The connection string.</value>
            [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Set by JSON serializer")]
            public string ConnectionString { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether this instance is enabled.
            /// </summary>
            /// <value><c>true</c> if this instance is enabled; otherwise, <c>false</c>.</value>
            [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Set by JSON serializer")]
            public bool IsEnabled { get; set; }

            /// <summary>
            /// Gets or sets the supported platforms.
            /// </summary>
            /// <value>The supported platforms.</value>
            [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Set by JSON serializer")]
            public string SupportedPlatforms
            {
                get => _supportedPlatformsValue;
                set
                {
                    _supportedPlatformsValue = value;
                    var items = value.Split(',', ';')
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Select(x => x.Trim());
                    _supportedPlatforms = new HashSet<string>(items, StringComparer.OrdinalIgnoreCase);
                }
            }

            /// <summary>
            /// Gets the options for platform.
            /// </summary>
            /// <returns>DatabaseServerOptions.</returns>
            public DatabaseServerOptions GetOptionsForPlatform()
            {
                return GetOptionsForPlatform(_platformIdentifiers);
            }

            /// <summary>
            /// Gets the options for platform.
            /// </summary>
            /// <param name="platforms">The platforms.</param>
            /// <returns>DatabaseServerOptions.</returns>
            private DatabaseServerOptions GetOptionsForPlatform(ISet<string> platforms)
            {
                if (_supportedPlatforms == null || _supportedPlatforms.Count == 0 || !IsEnabled)
                    return this;
                if (_supportedPlatforms.Any(platforms.Contains))
                    return this;
                return Empty;
            }
        }

        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>DatabaseServerOptions.</returns>
        private static DatabaseServerOptions GetOptions(string key)
        {
            if (DatabaseServers.TryGetValue(key, out var options))
                return options;
            return DatabaseServerOptions.Empty;
        }
    }
}
