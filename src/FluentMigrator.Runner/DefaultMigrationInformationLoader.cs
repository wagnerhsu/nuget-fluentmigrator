// ***********************************************************************
// Assembly         : FluentMigrator.Runner
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="DefaultMigrationInformationLoader.cs" company="FluentMigrator Project">
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using FluentMigrator.Exceptions;
using FluentMigrator.Infrastructure;
using FluentMigrator.Runner.Exceptions;
using FluentMigrator.Runner.Initialization;

using JetBrains.Annotations;

using Microsoft.Extensions.Options;

namespace FluentMigrator.Runner
{
    /// <summary>
    /// Class DefaultMigrationInformationLoader.
    /// Implements the <see cref="FluentMigrator.Runner.IMigrationInformationLoader" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.IMigrationInformationLoader" />
    public class DefaultMigrationInformationLoader : IMigrationInformationLoader
    {
        /// <summary>
        /// The tags to match
        /// </summary>
        [NotNull, ItemNotNull]
        private readonly string[] _tagsToMatch;

        /// <summary>
        /// The include untagged migrations
        /// </summary>
        private readonly bool _includeUntaggedMigrations;

        /// <summary>
        /// The source
        /// </summary>
        [NotNull]
#pragma warning disable 618
        private readonly IMigrationSource _source;
#pragma warning restore 618

        /// <summary>
        /// The migration infos
        /// </summary>
        [CanBeNull]
        private SortedList<long, IMigrationInfo> _migrationInfos;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultMigrationInformationLoader"/> class.
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <param name="assembly">The assembly.</param>
        /// <param name="namespace">The namespace.</param>
        /// <param name="tagsToMatch">The tags to match.</param>
        [Obsolete]
        public DefaultMigrationInformationLoader(IMigrationRunnerConventions conventions, Assembly assembly, string @namespace,
                                                   IEnumerable<string> tagsToMatch)
          : this(conventions, new SingleAssembly(assembly), @namespace, false, tagsToMatch)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultMigrationInformationLoader"/> class.
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <param name="assemblies">The assemblies.</param>
        /// <param name="namespace">The namespace.</param>
        /// <param name="tagsToMatch">The tags to match.</param>
        [Obsolete]
        public DefaultMigrationInformationLoader(IMigrationRunnerConventions conventions, IAssemblyCollection assemblies, string @namespace,
                                                 IEnumerable<string> tagsToMatch)
            : this(conventions, assemblies, @namespace, false, tagsToMatch)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultMigrationInformationLoader"/> class.
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <param name="assembly">The assembly.</param>
        /// <param name="namespace">The namespace.</param>
        /// <param name="loadNestedNamespaces">if set to <c>true</c> [load nested namespaces].</param>
        /// <param name="tagsToMatch">The tags to match.</param>
        [Obsolete]
        public DefaultMigrationInformationLoader(IMigrationRunnerConventions conventions, Assembly assembly, string @namespace,
                                                  bool loadNestedNamespaces, IEnumerable<string> tagsToMatch)
            : this(conventions, new SingleAssembly(assembly), @namespace, loadNestedNamespaces, tagsToMatch)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultMigrationInformationLoader"/> class.
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <param name="assemblies">The assemblies.</param>
        /// <param name="namespace">The namespace.</param>
        /// <param name="loadNestedNamespaces">if set to <c>true</c> [load nested namespaces].</param>
        /// <param name="tagsToMatch">The tags to match.</param>
        [Obsolete]
        public DefaultMigrationInformationLoader(IMigrationRunnerConventions conventions, IAssemblyCollection assemblies, string @namespace,
                                                 bool loadNestedNamespaces, IEnumerable<string> tagsToMatch)
        {
            Conventions = conventions;
            Assemblies = assemblies;
            Namespace = @namespace;
            LoadNestedNamespaces = loadNestedNamespaces;
            _tagsToMatch = tagsToMatch as string[] ?? tagsToMatch?.ToArray() ?? Array.Empty<string>();
            _source = new MigrationSource(new AssemblySource(() => assemblies), conventions);
            _includeUntaggedMigrations = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultMigrationInformationLoader"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="filterOptions">The filter options.</param>
        /// <param name="conventions">The conventions.</param>
        /// <param name="runnerOptions">The runner options.</param>
        public DefaultMigrationInformationLoader(
#pragma warning disable 618
            [NotNull] IMigrationSource source,
#pragma warning restore 618
            [NotNull] IOptionsSnapshot<TypeFilterOptions> filterOptions,
            [NotNull] IMigrationRunnerConventions conventions,
            [NotNull] IOptions<RunnerOptions> runnerOptions)
        {
            _source = source;
            Namespace = filterOptions.Value.Namespace;
            LoadNestedNamespaces = filterOptions.Value.NestedNamespaces;
            Conventions = conventions;
            _tagsToMatch = runnerOptions.Value.Tags ?? Array.Empty<string>();
            _includeUntaggedMigrations = runnerOptions.Value.IncludeUntaggedMigrations;
        }

        /// <summary>
        /// Gets the conventions.
        /// </summary>
        /// <value>The conventions.</value>
        [NotNull]
        public IMigrationRunnerConventions Conventions { get; }

        /// <summary>
        /// Gets the assemblies.
        /// </summary>
        /// <value>The assemblies.</value>
        [Obsolete]
        [CanBeNull]
        public IAssemblyCollection Assemblies { get; }

        /// <summary>
        /// Gets the namespace.
        /// </summary>
        /// <value>The namespace.</value>
        [CanBeNull]
        public string Namespace { get; }

        /// <summary>
        /// Gets a value indicating whether [load nested namespaces].
        /// </summary>
        /// <value><c>true</c> if [load nested namespaces]; otherwise, <c>false</c>.</value>
        public bool LoadNestedNamespaces { get; }

        /// <summary>
        /// Gets the tags to match.
        /// </summary>
        /// <value>The tags to match.</value>
        [NotNull, ItemNotNull]
        [Obsolete]
        public IEnumerable<string> TagsToMatch => _tagsToMatch;

        /// <summary>
        /// Loads the migrations.
        /// </summary>
        /// <returns>SortedList&lt;System.Int64, IMigrationInfo&gt;.</returns>
        /// <exception cref="FluentMigrator.Runner.Exceptions.MissingMigrationsException"></exception>
        /// <exception cref="FluentMigrator.Runner.Exceptions.MissingMigrationsException"></exception>
        /// <exception cref="DuplicateMigrationException">Duplicate migration version {migrationInfo.Version}.</exception>
        public SortedList<long, IMigrationInfo> LoadMigrations()
        {
            if (_migrationInfos != null)
            {
                if (_migrationInfos.Count == 0)
                    throw new MissingMigrationsException();
                return _migrationInfos;
            }

            _migrationInfos = new SortedList<long, IMigrationInfo>();
            var migrationInfos = FindMigrations(
                _source,
                Conventions,
                Namespace,
                LoadNestedNamespaces,
                _tagsToMatch,
                _includeUntaggedMigrations);
            foreach (var migrationInfo in migrationInfos)
            {
                if (_migrationInfos.ContainsKey(migrationInfo.Version))
                {
                    throw new DuplicateMigrationException($"Duplicate migration version {migrationInfo.Version}.");
                }

                _migrationInfos.Add(migrationInfo.Version, migrationInfo);
            }

            if (_migrationInfos.Count == 0)
                throw new MissingMigrationsException();

            return _migrationInfos;
        }

        /// <summary>
        /// Finds the migrations.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="conventions">The conventions.</param>
        /// <param name="namespace">The namespace.</param>
        /// <param name="loadNestedNamespaces">if set to <c>true</c> [load nested namespaces].</param>
        /// <param name="tagsToMatch">The tags to match.</param>
        /// <param name="includeUntagged">if set to <c>true</c> [include untagged].</param>
        /// <returns>IEnumerable&lt;IMigrationInfo&gt;.</returns>
        /// <exception cref="FluentMigrator.Runner.Exceptions.MissingMigrationsException">No migrations found</exception>
        [NotNull, ItemNotNull]
        private static IEnumerable<IMigrationInfo> FindMigrations(
#pragma warning disable 618
            [NotNull] IMigrationSource source,
#pragma warning restore 618
            [NotNull] IMigrationRunnerConventions conventions,
            [CanBeNull] string @namespace,
            bool loadNestedNamespaces,
            [NotNull, ItemNotNull] string[] tagsToMatch,
            bool includeUntagged)
        {
            bool IsMatchingMigration(Type type)
            {
                if (!type.IsInNamespace(@namespace, loadNestedNamespaces))
                    return false;
                if (!conventions.TypeIsMigration(type))
                    return false;
                return conventions.HasRequestedTags(type, tagsToMatch, includeUntagged);
            }

            IReadOnlyCollection<IMigration> migrations;

            if (source is IFilteringMigrationSource filteringSource)
            {
                migrations = filteringSource.GetMigrations(IsMatchingMigration).ToList();
            }
            else
            {
                migrations =
                    (from migration in source.GetMigrations()
                     where IsMatchingMigration(migration.GetType())
                     select migration).ToList();
            }

            if (migrations.Count == 0)
            {
                throw new MissingMigrationsException("No migrations found");
            }

            var migrationInfos = migrations
                .Select(conventions.GetMigrationInfoForMigration)
                .ToList();

            return migrationInfos;
        }
    }
}
