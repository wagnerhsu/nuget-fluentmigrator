// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="TemporaryDatabase.cs" company="FluentMigrator Project">
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
using System.IO;

using FirebirdSql.Data.FirebirdClient;

using NUnit.Framework;

namespace FluentMigrator.Tests.Integration.Processors.Firebird
{
    /// <summary>
    /// Class TemporaryDatabase.
    /// Implements the <see cref="System.IDisposable" />
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class TemporaryDatabase : IDisposable
    {
        /// <summary>
        /// The connection string
        /// </summary>
        private readonly Lazy<string> _connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemporaryDatabase"/> class.
        /// </summary>
        /// <param name="connectionOptions">The connection options.</param>
        /// <param name="prober">The prober.</param>
        public TemporaryDatabase(IntegrationTestOptions.DatabaseServerOptions connectionOptions, FirebirdLibraryProber prober)
        {
            if (!connectionOptions.IsEnabled)
                Assert.Ignore();

            DbFileName = Path.GetTempFileName();
            _connectionString = new Lazy<string>(() =>
            {
                var csb = new FbConnectionStringBuilder(connectionOptions.ConnectionString)
                {
                    Pooling = false,
                    Database = DbFileName,
                };

                return prober.CreateDb(csb);
            });
        }

        /// <summary>
        /// Gets the name of the database file.
        /// </summary>
        /// <value>The name of the database file.</value>
        public string DbFileName { get; }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString => _connectionString.Value;

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        public void Dispose()
        {
            try
            {
                File.Delete(DbFileName);
            }
            catch
            {
                // Ignore it, remaining temporary files
                // are annoying, but no security threat
                // in this context!
            }
        }
    }
}
