// ***********************************************************************
// Assembly         : FluentMigrator.Runner
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="ConnectionlessVersionLoader.cs" company="FluentMigrator Project">
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

using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using FluentMigrator.Model;
using FluentMigrator.Runner.Conventions;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Runner.Versioning;
using FluentMigrator.Runner.VersionTableInfo;

using JetBrains.Annotations;

using Microsoft.Extensions.Options;

namespace FluentMigrator.Runner
{
    /// <summary>
    /// Class ConnectionlessVersionLoader.
    /// Implements the <see cref="FluentMigrator.Runner.IVersionLoader" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.IVersionLoader" />
    public class ConnectionlessVersionLoader : IVersionLoader
    {
        /// <summary>
        /// The processor
        /// </summary>
        [NotNull]
        private readonly IMigrationProcessor _processor;

        /// <summary>
        /// The migration information loader
        /// </summary>
        [NotNull]
        private readonly IMigrationInformationLoader _migrationInformationLoader;

        /// <summary>
        /// The versions loaded
        /// </summary>
        private bool _versionsLoaded;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionlessVersionLoader"/> class.
        /// </summary>
        /// <param name="runner">The runner.</param>
        /// <param name="assemblies">The assemblies.</param>
        /// <param name="conventionSet">The convention set.</param>
        /// <param name="conventions">The conventions.</param>
        /// <param name="runnerContext">The runner context.</param>
        /// <param name="versionTableMetaData">The version table meta data.</param>
        [Obsolete]
        internal ConnectionlessVersionLoader(
            IMigrationRunner runner,
            IAssemblyCollection assemblies,
            IConventionSet conventionSet,
            IMigrationRunnerConventions conventions,
            IRunnerContext runnerContext,
            IVersionTableMetaData versionTableMetaData = null)
        {
            _migrationInformationLoader = runner.MigrationLoader;
            _processor = runner.Processor;

            Runner = runner;
            Assemblies = assemblies;
            Conventions = conventions;
            StartVersion = runnerContext.StartVersion;
            TargetVersion = runnerContext.Version;

            VersionInfo = new VersionInfo();
            VersionTableMetaData = versionTableMetaData ??
                (IVersionTableMetaData)Activator.CreateInstance(assemblies.Assemblies.GetVersionTableMetaDataType(
                    Conventions, runnerContext));
            VersionMigration = new VersionMigration(VersionTableMetaData);
            VersionSchemaMigration = new VersionSchemaMigration(VersionTableMetaData);
            VersionUniqueMigration = new VersionUniqueMigration(VersionTableMetaData);
            VersionDescriptionMigration = new VersionDescriptionMigration(VersionTableMetaData);

            if (VersionTableMetaData is DefaultVersionTableMetaData defaultMetaData)
            {
                conventionSet.SchemaConvention?.Apply(defaultMetaData);
            }

            LoadVersionInfo();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionlessVersionLoader"/> class.
        /// </summary>
        /// <param name="processorAccessor">The processor accessor.</param>
        /// <param name="conventions">The conventions.</param>
        /// <param name="runnerOptions">The runner options.</param>
        /// <param name="migrationInformationLoader">The migration information loader.</param>
        /// <param name="versionTableMetaData">The version table meta data.</param>
        public ConnectionlessVersionLoader(
            [NotNull] IProcessorAccessor processorAccessor,
            [NotNull] IMigrationRunnerConventions conventions,
            [NotNull] IOptions<RunnerOptions> runnerOptions,
            [NotNull] IMigrationInformationLoader migrationInformationLoader,
            [NotNull] IVersionTableMetaData versionTableMetaData)
        {
            _processor = processorAccessor.Processor;
            _migrationInformationLoader = migrationInformationLoader;
            Conventions = conventions;
            StartVersion = runnerOptions.Value.StartVersion;
            TargetVersion = runnerOptions.Value.Version;

            VersionInfo = new VersionInfo();
            VersionTableMetaData = versionTableMetaData;
            VersionMigration = new VersionMigration(VersionTableMetaData);
            VersionSchemaMigration = new VersionSchemaMigration(VersionTableMetaData);
            VersionUniqueMigration = new VersionUniqueMigration(VersionTableMetaData);
            VersionDescriptionMigration = new VersionDescriptionMigration(VersionTableMetaData);

            LoadVersionInfo();
        }

        /// <summary>
        /// Gets or sets the assemblies.
        /// </summary>
        /// <value>The assemblies.</value>
        [Obsolete]
        [CanBeNull]
        protected IAssemblyCollection Assemblies { get; set; }

        /// <summary>
        /// Gets or sets the conventions.
        /// </summary>
        /// <value>The conventions.</value>
        public IMigrationRunnerConventions Conventions { get; set; }
        /// <summary>
        /// Gets or sets the start version.
        /// </summary>
        /// <value>The start version.</value>
        public long StartVersion { get; set; }
        /// <summary>
        /// Gets or sets the target version.
        /// </summary>
        /// <value>The target version.</value>
        public long TargetVersion { get; set; }
        /// <summary>
        /// Gets the version schema migration.
        /// </summary>
        /// <value>The version schema migration.</value>
        public VersionSchemaMigration VersionSchemaMigration { get; }
        /// <summary>
        /// Gets the version migration.
        /// </summary>
        /// <value>The version migration.</value>
        public IMigration VersionMigration { get; }
        /// <summary>
        /// Gets the version unique migration.
        /// </summary>
        /// <value>The version unique migration.</value>
        public IMigration VersionUniqueMigration { get; }
        /// <summary>
        /// Gets the version description migration.
        /// </summary>
        /// <value>The version description migration.</value>
        public IMigration VersionDescriptionMigration { get; }

        /// <summary>
        /// The runner this version loader belongs to
        /// </summary>
        /// <value>The runner.</value>
        [Obsolete]
        [CanBeNull]
        public IMigrationRunner Runner { get; set; }
        /// <summary>
        /// Gets an interface to query/update the status of migrations
        /// </summary>
        /// <value>The version information.</value>
        public IVersionInfo VersionInfo { get; set; }
        /// <summary>
        /// Gets or sets the version table meta data.
        /// </summary>
        /// <value>The version table meta data.</value>
        public IVersionTableMetaData VersionTableMetaData { get; set; }

        /// <summary>
        /// Gets a value indicating whether the schema for the version table has been created (or already exited)
        /// </summary>
        /// <value><c>true</c> if [already created version schema]; otherwise, <c>false</c>.</value>
        public bool AlreadyCreatedVersionSchema
        {
            get
            {
                return string.IsNullOrEmpty(VersionTableMetaData.SchemaName) ||
                       _processor.SchemaExists(VersionTableMetaData.SchemaName);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the version table has been created (or already exited)
        /// </summary>
        /// <value><c>true</c> if [already created version table]; otherwise, <c>false</c>.</value>
        public bool AlreadyCreatedVersionTable
        {
            get { return _processor.TableExists(VersionTableMetaData.SchemaName, VersionTableMetaData.TableName); }
        }

        /// <summary>
        /// Deletes a version from the version table
        /// </summary>
        /// <param name="version">The version to delete from the version table</param>
        public void DeleteVersion(long version)
        {
            var expression = new DeleteDataExpression {TableName = VersionTableMetaData.TableName, SchemaName = VersionTableMetaData.SchemaName};
            expression.Rows.Add(new DeletionDataDefinition
            {
                new KeyValuePair<string, object>(VersionTableMetaData.ColumnName, version)
            });
            expression.ExecuteWith(_processor);
        }

        /// <summary>
        /// Get the version table metadata
        /// </summary>
        /// <returns>The version table metadata</returns>
        public IVersionTableMetaData GetVersionTableMetaData()
        {
            return VersionTableMetaData;
        }

        /// <summary>
        /// Loads all version data stored in the version table
        /// </summary>
        public void LoadVersionInfo()
        {
            if (_versionsLoaded)
            {
                return;
            }

            foreach (var migration in _migrationInformationLoader.LoadMigrations())
            {
                if (migration.Key < StartVersion)
                {
                    VersionInfo.AddAppliedMigration(migration.Key);
                }
            }

            _versionsLoaded = true;
        }

        /// <summary>
        /// Removes the version table
        /// </summary>
        public void RemoveVersionTable()
        {
            var expression = new DeleteTableExpression {TableName = VersionTableMetaData.TableName, SchemaName = VersionTableMetaData.SchemaName};
            expression.ExecuteWith(_processor);

            if (!string.IsNullOrEmpty(VersionTableMetaData.SchemaName))
            {
                var schemaExpression = new DeleteSchemaExpression {SchemaName = VersionTableMetaData.SchemaName};
                schemaExpression.ExecuteWith(_processor);
            }
        }

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
            var dataExpression = new InsertDataExpression();
            dataExpression.Rows.Add(CreateVersionInfoInsertionData(version, description));
            dataExpression.TableName = VersionTableMetaData.TableName;
            dataExpression.SchemaName = VersionTableMetaData.SchemaName;

            dataExpression.ExecuteWith(_processor);
        }

        /// <summary>
        /// Creates the version information insertion data.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="description">The description.</param>
        /// <returns>InsertionDataDefinition.</returns>
        protected virtual InsertionDataDefinition CreateVersionInfoInsertionData(long version, string description)
        {
            return new InsertionDataDefinition
            {
                new KeyValuePair<string, object>(VersionTableMetaData.ColumnName, version),
                new KeyValuePair<string, object>(VersionTableMetaData.AppliedOnColumnName, DateTime.UtcNow),
                new KeyValuePair<string, object>(VersionTableMetaData.DescriptionColumnName, description)
            };
        }
    }
}
