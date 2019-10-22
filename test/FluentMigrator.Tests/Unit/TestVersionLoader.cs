// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="TestVersionLoader.cs" company="FluentMigrator Project">
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

using System.Collections.Generic;

using FluentMigrator.Runner;
using FluentMigrator.Runner.Versioning;
using FluentMigrator.Runner.VersionTableInfo;

namespace FluentMigrator.Tests.Unit
{
    /// <summary>
    /// Class TestVersionLoader.
    /// Implements the <see cref="FluentMigrator.Runner.IVersionLoader" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.IVersionLoader" />
    public class TestVersionLoader : IVersionLoader
    {
        /// <summary>
        /// The version table meta data
        /// </summary>
        private readonly IVersionTableMetaData _versionTableMetaData;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestVersionLoader"/> class.
        /// </summary>
        /// <param name="runner">The runner.</param>
        /// <param name="versionTableMetaData">The version table meta data.</param>
        public TestVersionLoader(IMigrationRunner runner, IVersionTableMetaData versionTableMetaData)
        {
            _versionTableMetaData = versionTableMetaData;
            Runner = runner;
            VersionInfo = new VersionInfo();
            Versions = new List<long>();
        }

        /// <summary>
        /// Gets a value indicating whether the schema for the version table has been created (or already exited)
        /// </summary>
        /// <value><c>true</c> if [already created version schema]; otherwise, <c>false</c>.</value>
        public bool AlreadyCreatedVersionSchema { get; set; }

        /// <summary>
        /// Gets a value indicating whether the version table has been created (or already exited)
        /// </summary>
        /// <value><c>true</c> if [already created version table]; otherwise, <c>false</c>.</value>
        public bool AlreadyCreatedVersionTable { get; set; }

        /// <summary>
        /// Deletes a version from the version table
        /// </summary>
        /// <param name="version">The version to delete from the version table</param>
        public void DeleteVersion(long version)
        {
            Versions.Remove(version);
        }

        /// <summary>
        /// Get the version table metadata
        /// </summary>
        /// <returns>The version table metadata</returns>
        public IVersionTableMetaData GetVersionTableMetaData()
        {
            return _versionTableMetaData;
        }

        /// <summary>
        /// Loads all version data stored in the version table
        /// </summary>
        public void LoadVersionInfo()
        {
            VersionInfo = new VersionInfo();

            foreach (var version in Versions)
            {
                VersionInfo.AddAppliedMigration(version);
            }

            DidLoadVersionInfoGetCalled = true;
        }

        /// <summary>
        /// Gets a value indicating whether [did load version information get called].
        /// </summary>
        /// <value><c>true</c> if [did load version information get called]; otherwise, <c>false</c>.</value>
        public bool DidLoadVersionInfoGetCalled { get; private set; }

        /// <summary>
        /// Removes the version table
        /// </summary>
        public void RemoveVersionTable()
        {
            DidRemoveVersionTableGetCalled = true;
        }

        /// <summary>
        /// Gets a value indicating whether [did remove version table get called].
        /// </summary>
        /// <value><c>true</c> if [did remove version table get called]; otherwise, <c>false</c>.</value>
        public bool DidRemoveVersionTableGetCalled { get; private set; }

        /// <summary>
        /// The runner this version loader belongs to
        /// </summary>
        /// <value>The runner.</value>
        public IMigrationRunner Runner { get; set; }

        /// <summary>
        /// Adds the version information
        /// </summary>
        /// <param name="version">The version number</param>
        public void UpdateVersionInfo(long version)
        {
            UpdateVersionInfo(version, null);
        }

        /// <summary>
        /// Adds the version information
        /// </summary>
        /// <param name="version">The version number</param>
        /// <param name="description">The version description</param>
        public void UpdateVersionInfo(long version, string description)
        {
            Versions.Add(version);

            DidUpdateVersionInfoGetCalled = true;
        }

        /// <summary>
        /// Gets a value indicating whether [did update version information get called].
        /// </summary>
        /// <value><c>true</c> if [did update version information get called]; otherwise, <c>false</c>.</value>
        public bool DidUpdateVersionInfoGetCalled { get; private set; }

        /// <summary>
        /// Gets an interface to query/update the status of migrations
        /// </summary>
        /// <value>The version information.</value>
        public IVersionInfo VersionInfo { get; set; }

        /// <summary>
        /// Gets the version table meta data
        /// </summary>
        /// <value>The version table meta data.</value>
        public IVersionTableMetaData VersionTableMetaData => _versionTableMetaData;

        /// <summary>
        /// Gets the versions.
        /// </summary>
        /// <value>The versions.</value>
        public List<long> Versions { get; private set; }
    }
}
