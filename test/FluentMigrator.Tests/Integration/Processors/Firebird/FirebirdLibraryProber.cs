// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="FirebirdLibraryProber.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
// Copyright (c) 2007-2018, Sean Chambers and the FluentMigrator Project
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
using System.Linq;

using FirebirdSql.Data.FirebirdClient;

namespace FluentMigrator.Tests.Integration.Processors.Firebird
{
    /// <summary>
    /// Class FirebirdLibraryProber.
    /// </summary>
    public class FirebirdLibraryProber
    {
        /// <summary>
        /// The synchronize
        /// </summary>
        private readonly object _sync = new object();
        /// <summary>
        /// The client libraries
        /// </summary>
        private readonly IReadOnlyCollection<string> _clientLibraries;
        /// <summary>
        /// The probe exception
        /// </summary>
        private Exception _probeException;
        /// <summary>
        /// The found client library
        /// </summary>
        private bool _foundClientLibrary;
        /// <summary>
        /// The client library
        /// </summary>
        private string _clientLibrary;

        /// <summary>
        /// Initializes a new instance of the <see cref="FirebirdLibraryProber"/> class.
        /// </summary>
        public FirebirdLibraryProber()
            : this(DefaultLibraries)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FirebirdLibraryProber"/> class.
        /// </summary>
        /// <param name="clientLibraries">The client libraries.</param>
        public FirebirdLibraryProber(IEnumerable<string> clientLibraries)
        {
            _clientLibraries = clientLibraries.ToList();
        }

        /// <summary>
        /// Gets the default libraries.
        /// </summary>
        /// <value>The default libraries.</value>
        public static IEnumerable<string> DefaultLibraries
        {
            get
            {
                if (Environment.OSVersion.Platform == PlatformID.MacOSX)
                {
                    yield return "libfbclient.dylib";
                    yield return "fbclient";
                }
                else if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    // Try V3.0 first
                    yield return "/opt/firebird/lib/libfbclient.so.3";
                    yield return "/opt/firebird/lib/libfbclient.so";
                    yield return "libfbclient.so.3.0";
                    yield return "libfbclient.so.3";

                    // Now probe for 2.5
                    yield return "/usr/lib/x86_64-linux-gnu/libfbembed.so.2.5";
                    yield return "/usr/lib/x86_64-linux-gnu/libfbclient.so.2";
                    yield return "libfbembed.so.2.5";
                    yield return "fbembed";
                    yield return "libfbclient.so.2";

                    // Last chance
                    yield return "fbclient";
                }
                else
                {
                    // Windows-style OS?
                    yield return "fbembed.dll";
                    yield return "gds.dll";
                    yield return "fbclient.dll";
                }
            }
        }

        /// <summary>
        /// Creates the database.
        /// </summary>
        /// <param name="csb">The CSB.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="Exception">Failed to find suitable client library</exception>
        public string CreateDb(FbConnectionStringBuilder csb)
        {
            if (!_foundClientLibrary)
            {
                lock (_sync)
                {
                    if (!_foundClientLibrary)
                    {
                        var clientLibs = new List<string>();
                        if (!string.IsNullOrEmpty(csb.ClientLibrary))
                        {
                            clientLibs.Add(csb.ClientLibrary);
                        }

                        clientLibs.AddRange(_clientLibraries);

                        foreach (var clientLibrary in clientLibs)
                        {
                            csb.ClientLibrary = clientLibrary;
                            try
                            {
                                FbConnection.CreateDatabase(csb.ConnectionString, true);
                                _clientLibrary = clientLibrary;
                                _foundClientLibrary = true;
                                return csb.ConnectionString;
                            }
                            catch (Exception ex)
                            {
                                _probeException = ex;
                            }
                        }

                        _foundClientLibrary = true;
                    }
                }
            }

            if (_clientLibrary != null)
            {
                csb.ClientLibrary = _clientLibrary;
                FbConnection.CreateDatabase(csb.ConnectionString, true);
                return csb.ConnectionString;
            }

            throw new Exception("Failed to find suitable client library", _probeException);
        }
    }
}
