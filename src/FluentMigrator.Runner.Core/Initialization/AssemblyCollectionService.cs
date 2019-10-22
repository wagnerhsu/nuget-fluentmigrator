// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Core
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="AssemblyCollectionService.cs" company="FluentMigrator Project">
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
using System.Linq;
using System.Reflection;

using FluentMigrator.Infrastructure;

using JetBrains.Annotations;

namespace FluentMigrator.Runner.Initialization
{
    /// <summary>
    /// A compatibility service to get the assembly collection from the found migrations
    /// </summary>
    [Obsolete("Exists only to simplify the migration to the new FluentMigration version")]
    public class AssemblyCollectionService : IAssemblyCollection
    {
        /// <summary>
        /// The lazy assemblies
        /// </summary>
        private readonly Lazy<Assembly[]> _lazyAssemblies;

        /// <summary>
        /// The exported types
        /// </summary>
        private readonly Lazy<Type[]> _exportedTypes;

        /// <summary>
        /// The ressource entries
        /// </summary>
        private readonly Lazy<ManifestResourceNameWithAssembly[]> _ressourceEntries;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyCollectionService" /> class.
        /// </summary>
        /// <param name="source">The source assemblies used to search for types with given traits</param>
        public AssemblyCollectionService([NotNull] IAssemblySource source)
        {
            _lazyAssemblies = new Lazy<Assembly[]>(() => source.Assemblies.ToArray());
            _exportedTypes = new Lazy<Type[]>(() => _lazyAssemblies.Value.SelectMany(a => a.GetExportedTypes()).ToArray());
            _ressourceEntries = new Lazy<ManifestResourceNameWithAssembly[]>(
                () => _lazyAssemblies.Value.SelectMany(a => a.GetManifestResourceNames(), (asm, name) => new ManifestResourceNameWithAssembly(name, asm)).ToArray());
        }

        /// <inheritdoc />
        public Assembly[] Assemblies => _lazyAssemblies.Value;

        /// <inheritdoc />
        public Type[] GetExportedTypes() => _exportedTypes.Value;

        /// <inheritdoc />
        public ManifestResourceNameWithAssembly[] GetManifestResourceNames() => _ressourceEntries.Value;
    }
}
