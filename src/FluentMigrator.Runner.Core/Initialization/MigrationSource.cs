// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Core
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="MigrationSource.cs" company="FluentMigrator Project">
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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

namespace FluentMigrator.Runner.Initialization
{
    /// <summary>
    /// The default implementation of a <see cref="IFilteringMigrationSource" />.
    /// </summary>
    public class MigrationSource : IFilteringMigrationSource
    {
        /// <summary>
        /// The source
        /// </summary>
        [NotNull]
        private readonly IAssemblySource _source;

        /// <summary>
        /// The conventions
        /// </summary>
        [NotNull]
        private readonly IMigrationRunnerConventions _conventions;

        /// <summary>
        /// The service provider
        /// </summary>
        [CanBeNull]
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// The instance cache
        /// </summary>
        [NotNull]
        private readonly ConcurrentDictionary<Type, IMigration> _instanceCache = new ConcurrentDictionary<Type, IMigration>();

        /// <summary>
        /// The source items
        /// </summary>
        [NotNull, ItemNotNull]
        private readonly IEnumerable<IMigrationSourceItem> _sourceItems;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileSource" /> class.
        /// </summary>
        /// <param name="source">The assembly source</param>
        /// <param name="conventions">The migration runner conventios</param>
        /// <param name="serviceProvider">The service provider</param>
        /// <param name="sourceItems">The additional migration source items</param>
        public MigrationSource(
            [NotNull] IAssemblySource source,
            [NotNull] IMigrationRunnerConventions conventions,
            [NotNull] IServiceProvider serviceProvider,
            [NotNull, ItemNotNull] IEnumerable<IMigrationSourceItem> sourceItems)
        {
            _source = source;
            _conventions = conventions;
            _serviceProvider = serviceProvider;
            _sourceItems = sourceItems;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileSource" /> class.
        /// </summary>
        /// <param name="source">The assembly source</param>
        /// <param name="conventions">The migration runner conventios</param>
        [Obsolete]
        public MigrationSource(
            [NotNull] IAssemblySource source,
            [NotNull] IMigrationRunnerConventions conventions)
        {
            _source = source;
            _conventions = conventions;
            _sourceItems = Enumerable.Empty<IMigrationSourceItem>();
        }

        /// <inheritdoc />
        public IEnumerable<IMigration> GetMigrations()
        {
            return GetMigrations(_conventions.TypeIsMigration);
        }

        /// <inheritdoc />
        public IEnumerable<IMigration> GetMigrations(Func<Type, bool> predicate)
        {
            var instances =
                from type in GetMigrationTypeCandidates()
                where !type.IsAbstract && typeof(IMigration).IsAssignableFrom(type)
                where predicate == null || predicate(type)
                select _instanceCache.GetOrAdd(type, CreateInstance);
            return instances;
        }

        /// <summary>
        /// Gets the migration type candidates.
        /// </summary>
        /// <returns>IEnumerable&lt;Type&gt;.</returns>
        private IEnumerable<Type> GetMigrationTypeCandidates()
        {
            return _source
                .Assemblies.SelectMany(a => a.GetExportedTypes())
                .Union(_sourceItems.SelectMany(i => i.MigrationTypeCandidates));
        }

        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>IMigration.</returns>
        private IMigration CreateInstance(Type type)
        {
            if (_serviceProvider == null)
            {
                return (IMigration)Activator.CreateInstance(type);
            }

            return (IMigration)ActivatorUtilities.CreateInstance(_serviceProvider, type);
        }
    }
}
