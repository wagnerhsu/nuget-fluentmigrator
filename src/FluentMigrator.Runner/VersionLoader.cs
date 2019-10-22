// ***********************************************************************
// Assembly         : FluentMigrator.Runner
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="VersionLoader.cs" company="FluentMigrator Project">
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
using System.Data;
using System.Reflection;

using FluentMigrator.Expressions;
using FluentMigrator.Model;
using FluentMigrator.Runner.Versioning;
using FluentMigrator.Runner.VersionTableInfo;
using FluentMigrator.Infrastructure;
using FluentMigrator.Runner.Conventions;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;

using JetBrains.Annotations;

namespace FluentMigrator.Runner
{
    /// <summary>
    /// Class VersionLoader.
    /// Implements the <see cref="FluentMigrator.Runner.IVersionLoader" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.IVersionLoader" />
    public class VersionLoader : IVersionLoader
    {
        /// <summary>
        /// The processor
        /// </summary>
        [NotNull]
        private readonly IMigrationProcessor _processor;

        /// <summary>
        /// The convention set
        /// </summary>
        private readonly IConventionSet _conventionSet;
        /// <summary>
        /// The version schema migration already run
        /// </summary>
        private bool _versionSchemaMigrationAlreadyRun;
        /// <summary>
        /// The version migration already run
        /// </summary>
        private bool _versionMigrationAlreadyRun;
        /// <summary>
        /// The version unique migration already run
        /// </summary>
        private bool _versionUniqueMigrationAlreadyRun;
        /// <summary>
        /// The version description migration already run
        /// </summary>
        private bool _versionDescriptionMigrationAlreadyRun;
        /// <summary>
        /// The version information
        /// </summary>
        private IVersionInfo _versionInfo;
        /// <summary>
        /// Gets or sets the conventions.
        /// </summary>
        /// <value>The conventions.</value>
        private IMigrationRunnerConventions Conventions { get; set; }

        /// <summary>
        /// Gets or sets the assemblies.
        /// </summary>
        /// <value>The assemblies.</value>
        [CanBeNull]
        [Obsolete]
        protected IAssemblyCollection Assemblies { get; set; }

        /// <summary>
        /// Gets the version table meta data
        /// </summary>
        /// <value>The version table meta data.</value>
        public IVersionTableMetaData VersionTableMetaData { get; }

        /// <summary>
        /// The runner this version loader belongs to
        /// </summary>
        /// <value>The runner.</value>
        [NotNull]
        public IMigrationRunner Runner { get; set; }
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
        /// Initializes a new instance of the <see cref="VersionLoader"/> class.
        /// </summary>
        /// <param name="runner">The runner.</param>
        /// <param name="assembly">The assembly.</param>
        /// <param name="conventionSet">The convention set.</param>
        /// <param name="conventions">The conventions.</param>
        /// <param name="runnerContext">The runner context.</param>
        [Obsolete]
        internal VersionLoader(
            [NotNull] IMigrationRunner runner,
            [NotNull] Assembly assembly,
            [NotNull] IConventionSet conventionSet,
            [NotNull] IMigrationRunnerConventions conventions,
            [NotNull] IRunnerContext runnerContext)
            : this(runner, new SingleAssembly(assembly), conventionSet, conventions, runnerContext)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionLoader"/> class.
        /// </summary>
        /// <param name="runner">The runner.</param>
        /// <param name="assemblies">The assemblies.</param>
        /// <param name="conventionSet">The convention set.</param>
        /// <param name="conventions">The conventions.</param>
        /// <param name="runnerContext">The runner context.</param>
        /// <param name="versionTableMetaData">The version table meta data.</param>
        [Obsolete]
        internal VersionLoader(IMigrationRunner runner, IAssemblyCollection assemblies,
            [NotNull] IConventionSet conventionSet,
            [NotNull] IMigrationRunnerConventions conventions,
            [NotNull] IRunnerContext runnerContext,
            [CanBeNull] IVersionTableMetaData versionTableMetaData = null)
        {
            _conventionSet = conventionSet;
            _processor = runner.Processor;

            Runner = runner;
            Assemblies = assemblies;

            Conventions = conventions;
            VersionTableMetaData = versionTableMetaData ?? CreateVersionTableMetaData(runnerContext);
            VersionMigration = new VersionMigration(VersionTableMetaData);
            VersionSchemaMigration = new VersionSchemaMigration(VersionTableMetaData);
            VersionUniqueMigration = new VersionUniqueMigration(VersionTableMetaData);
            VersionDescriptionMigration = new VersionDescriptionMigration(VersionTableMetaData);

            VersionTableMetaData.ApplicationContext = runnerContext.ApplicationContext;

            LoadVersionInfo();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionLoader"/> class.
        /// </summary>
        /// <param name="processorAccessor">The processor accessor.</param>
        /// <param name="conventionSet">The convention set.</param>
        /// <param name="conventions">The conventions.</param>
        /// <param name="versionTableMetaData">The version table meta data.</param>
        /// <param name="runner">The runner.</param>
        public VersionLoader(
            [NotNull] IProcessorAccessor processorAccessor,
            [NotNull] IConventionSet conventionSet,
            [NotNull] IMigrationRunnerConventions conventions,
            [NotNull] IVersionTableMetaData versionTableMetaData,
            [NotNull] IMigrationRunner runner)
        {
            _conventionSet = conventionSet;
            _processor = processorAccessor.Processor;

            Runner = runner;

            Conventions = conventions;
            VersionTableMetaData = versionTableMetaData;
            VersionMigration = new VersionMigration(VersionTableMetaData);
            VersionSchemaMigration = new VersionSchemaMigration(VersionTableMetaData);
            VersionUniqueMigration = new VersionUniqueMigration(VersionTableMetaData);
            VersionDescriptionMigration = new VersionDescriptionMigration(VersionTableMetaData);

            LoadVersionInfo();
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
        /// Get the version table metadata
        /// </summary>
        /// <returns>The version table metadata</returns>
        [NotNull]
        public IVersionTableMetaData GetVersionTableMetaData()
        {
            return VersionTableMetaData;
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
                           new KeyValuePair<string, object>(VersionTableMetaData.DescriptionColumnName, description),
                       };
        }

        /// <summary>
        /// Gets an interface to query/update the status of migrations
        /// </summary>
        /// <value>The version information.</value>
        /// <exception cref="ArgumentException">Cannot set VersionInfo to null</exception>
        public IVersionInfo VersionInfo
        {
            get => _versionInfo;
            set => _versionInfo = value ?? throw new ArgumentException("Cannot set VersionInfo to null");
        }

        /// <summary>
        /// Gets a value indicating whether the schema for the version table has been created (or already exited)
        /// </summary>
        /// <value><c>true</c> if [already created version schema]; otherwise, <c>false</c>.</value>
        public bool AlreadyCreatedVersionSchema => string.IsNullOrEmpty(VersionTableMetaData.SchemaName) ||
            _processor.SchemaExists(VersionTableMetaData.SchemaName);

        /// <summary>
        /// Gets a value indicating whether the version table has been created (or already exited)
        /// </summary>
        /// <value><c>true</c> if [already created version table]; otherwise, <c>false</c>.</value>
        public bool AlreadyCreatedVersionTable => _processor.TableExists(VersionTableMetaData.SchemaName, VersionTableMetaData.TableName);

        /// <summary>
        /// Gets a value indicating whether [already made version unique].
        /// </summary>
        /// <value><c>true</c> if [already made version unique]; otherwise, <c>false</c>.</value>
        public bool AlreadyMadeVersionUnique => _processor.ColumnExists(VersionTableMetaData.SchemaName, VersionTableMetaData.TableName, VersionTableMetaData.AppliedOnColumnName);

        /// <summary>
        /// Gets a value indicating whether [already made version description].
        /// </summary>
        /// <value><c>true</c> if [already made version description]; otherwise, <c>false</c>.</value>
        public bool AlreadyMadeVersionDescription => _processor.ColumnExists(VersionTableMetaData.SchemaName, VersionTableMetaData.TableName, VersionTableMetaData.DescriptionColumnName);

        /// <summary>
        /// Gets a value indicating whether [owns version schema].
        /// </summary>
        /// <value><c>true</c> if [owns version schema]; otherwise, <c>false</c>.</value>
        public bool OwnsVersionSchema => VersionTableMetaData.OwnsSchema;

        /// <summary>
        /// Loads all version data stored in the version table
        /// </summary>
        public void LoadVersionInfo()
        {
            if (!AlreadyCreatedVersionSchema && !_versionSchemaMigrationAlreadyRun)
            {
                Runner.Up(VersionSchemaMigration);
                _versionSchemaMigrationAlreadyRun = true;
            }

            if (!AlreadyCreatedVersionTable && !_versionMigrationAlreadyRun)
            {
                Runner.Up(VersionMigration);
                _versionMigrationAlreadyRun = true;
            }

            if (!AlreadyMadeVersionUnique && !_versionUniqueMigrationAlreadyRun)
            {
                Runner.Up(VersionUniqueMigration);
                _versionUniqueMigrationAlreadyRun = true;
            }

            if (!AlreadyMadeVersionDescription && !_versionDescriptionMigrationAlreadyRun)
            {
                Runner.Up(VersionDescriptionMigration);
                _versionDescriptionMigrationAlreadyRun = true;
            }

            _versionInfo = new VersionInfo();

            if (!AlreadyCreatedVersionTable) return;

            var dataSet = _processor.ReadTableData(VersionTableMetaData.SchemaName, VersionTableMetaData.TableName);
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                _versionInfo.AddAppliedMigration(long.Parse(row[VersionTableMetaData.ColumnName].ToString()));
            }
        }

        /// <summary>
        /// Removes the version table
        /// </summary>
        public void RemoveVersionTable()
        {
            var expression = new DeleteTableExpression { TableName = VersionTableMetaData.TableName, SchemaName = VersionTableMetaData.SchemaName };
            expression.ExecuteWith(_processor);

            if (OwnsVersionSchema && !string.IsNullOrEmpty(VersionTableMetaData.SchemaName))
            {
                var schemaExpression = new DeleteSchemaExpression { SchemaName = VersionTableMetaData.SchemaName };
                schemaExpression.ExecuteWith(_processor);
            }
        }

        /// <summary>
        /// Deletes a version from the version table
        /// </summary>
        /// <param name="version">The version to delete from the version table</param>
        public void DeleteVersion(long version)
        {
            var expression = new DeleteDataExpression { TableName = VersionTableMetaData.TableName, SchemaName = VersionTableMetaData.SchemaName };
            expression.Rows.Add(new DeletionDataDefinition
                                    {
                                        new KeyValuePair<string, object>(VersionTableMetaData.ColumnName, version)
                                    });
            expression.ExecuteWith(_processor);
        }

        /// <summary>
        /// Creates the version table meta data.
        /// </summary>
        /// <param name="runnerContext">The runner context.</param>
        /// <returns>IVersionTableMetaData.</returns>
        [Obsolete]
        [NotNull]
        private IVersionTableMetaData CreateVersionTableMetaData(IRunnerContext runnerContext)
        {
            var type = Assemblies?.Assemblies.GetVersionTableMetaDataType(Conventions, runnerContext)
             ?? typeof(DefaultVersionTableMetaData);

            var instance = (IVersionTableMetaData) Activator.CreateInstance(type);
            if (instance is ISchemaExpression schemaExpression)
            {
                _conventionSet.SchemaConvention?.Apply(schemaExpression);
            }

            return instance;
        }
    }
}
