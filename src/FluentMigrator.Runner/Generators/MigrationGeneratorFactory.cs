// ***********************************************************************
// Assembly         : FluentMigrator.Runner
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="MigrationGeneratorFactory.cs" company="FluentMigrator Project">
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
using System.Linq;

using FluentMigrator.Runner.Extensions;
using FluentMigrator.Runner.Processors;

namespace FluentMigrator.Runner.Generators
{
    /// <summary>
    /// Class MigrationGeneratorFactory.
    /// </summary>
    [Obsolete]
    public class MigrationGeneratorFactory
    {
        /// <summary>
        /// The migration generators
        /// </summary>
        private static readonly IDictionary<string, IMigrationGenerator> _migrationGenerators;

        /// <summary>
        /// Initializes static members of the <see cref="MigrationGeneratorFactory"/> class.
        /// </summary>
        static MigrationGeneratorFactory()
        {
            var assemblies = MigrationProcessorFactoryProvider.RegisteredFactories.Select(x => x.GetType().Assembly);

            var types = assemblies
                .SelectMany(a => a.GetExportedTypes())
                .Where(type => type.IsConcrete() && type.Is<IMigrationGenerator>())
                .ToList();

            var available = new SortedDictionary<string, IMigrationGenerator>();
            foreach (Type type in types)
            {
                try
                {
                    var factory = (IMigrationGenerator) Activator.CreateInstance(type);
                    available.Add(type.Name.Replace("Generator", ""), factory);
                }
                catch (Exception)
                {
                    //can't add generators that require construtor parameters
                }
            }

            _migrationGenerators = available;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationGeneratorFactory"/> class.
        /// </summary>
        [Obsolete("Ony the statically provided generators are accessed")]
        public MigrationGeneratorFactory()
        {
        }

        /// <summary>
        /// Gets the registered generators.
        /// </summary>
        /// <value>The registered generators.</value>
        public static IEnumerable<IMigrationGenerator> RegisteredGenerators
            => _migrationGenerators.Values;

        /// <summary>
        /// Gets the generator.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>IMigrationGenerator.</returns>
        [Obsolete("Ony the statically provided generators are accessed")]
        public virtual IMigrationGenerator GetGenerator(string name)
        {
            return _migrationGenerators
                   .Where(pair => pair.Key.Equals(name, StringComparison.OrdinalIgnoreCase))
                   .Select(pair => pair.Value)
                   .FirstOrDefault();
        }

        /// <summary>
        /// Lists the available generator types.
        /// </summary>
        /// <returns>System.String.</returns>
        [Obsolete("Ony the statically provided generators are accessed")]
        public string ListAvailableGeneratorTypes()
        {
            return string.Join(", ", _migrationGenerators.Keys.ToArray());
        }
    }
}
