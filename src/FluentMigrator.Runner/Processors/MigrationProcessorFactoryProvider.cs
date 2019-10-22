// ***********************************************************************
// Assembly         : FluentMigrator.Runner
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="MigrationProcessorFactoryProvider.cs" company="FluentMigrator Project">
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
using System.Linq;
using System.Reflection;

using FluentMigrator.Runner.Extensions;
using FluentMigrator.Runner.Infrastructure;
using FluentMigrator.Runner.Processors.DB2;
using FluentMigrator.Runner.Processors.DB2.iSeries;
using FluentMigrator.Runner.Processors.DotConnectOracle;
using FluentMigrator.Runner.Processors.Firebird;
using FluentMigrator.Runner.Processors.Hana;
using FluentMigrator.Runner.Processors.MySql;
using FluentMigrator.Runner.Processors.Oracle;
using FluentMigrator.Runner.Processors.Postgres;
using FluentMigrator.Runner.Processors.Redshift;
using FluentMigrator.Runner.Processors.SqlAnywhere;
using FluentMigrator.Runner.Processors.SqlServer;
using FluentMigrator.Runner.Processors.SQLite;

namespace FluentMigrator.Runner.Processors
{
    /// <summary>
    /// Class MigrationProcessorFactoryProvider.
    /// </summary>
    [Obsolete]
    public class MigrationProcessorFactoryProvider
    {
        /// <summary>
        /// The lock
        /// </summary>
        private static readonly object _lock = new object();
        /// <summary>
        /// The migration processor factories
        /// </summary>
        private static IDictionary<string, IMigrationProcessorFactory> _migrationProcessorFactories;

        /// <summary>
        /// Initializes static members of the <see cref="MigrationProcessorFactoryProvider"/> class.
        /// </summary>
        [Obsolete]
        static MigrationProcessorFactoryProvider()
        {
            // Register all available processor factories. The library usually tries
            // to find all provider factories by scanning all referenced assemblies,
            // but this fails if we don't have any reference. Adding the package
            // isn't enough. We MUST have a reference to a type, otherwise the
            // assembly reference gets removed by the C# compiler!
            Register(new Db2ProcessorFactory());
            Register(new Db2ISeriesProcessorFactory());
            Register(new DotConnectOracleProcessorFactory());
            Register(new FirebirdProcessorFactory());
            Register(new MySql4ProcessorFactory());
            Register(new MySql5ProcessorFactory());
            Register(new OracleManagedProcessorFactory());
            Register(new OracleProcessorFactory());
            Register(new PostgresProcessorFactory());
            Register(new SQLiteProcessorFactory());
            Register(new SqlServer2000ProcessorFactory());
            Register(new SqlServer2005ProcessorFactory());
            Register(new SqlServer2008ProcessorFactory());
            Register(new SqlServer2012ProcessorFactory());
            Register(new SqlServer2014ProcessorFactory());
            Register(new SqlServer2016ProcessorFactory());
            Register(new SqlServerProcessorFactory());
            Register(new SqlServerCeProcessorFactory());
            Register(new SqlAnywhere16ProcessorFactory());
            Register(new HanaProcessorFactory());
            Register(new RedshiftProcessorFactory());

#if NETFRAMEWORK
            Register(new Jet.JetProcessorFactory());
#endif
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationProcessorFactoryProvider"/> class.
        /// </summary>
        [Obsolete("Ony the statically provided factories are accessed")]
        public MigrationProcessorFactoryProvider()
        {
        }

        /// <summary>
        /// Gets the migration processor factories.
        /// </summary>
        /// <value>The migration processor factories.</value>
        private static IDictionary<string, IMigrationProcessorFactory> MigrationProcessorFactories
        {
            get
            {
                lock (_lock)
                {
                    return _migrationProcessorFactories ?? (_migrationProcessorFactories = FindProcessorFactories());
                }
            }
        }

        /// <summary>
        /// Gets the registered factories.
        /// </summary>
        /// <value>The registered factories.</value>
        public static IEnumerable<IMigrationProcessorFactory> RegisteredFactories
            => MigrationProcessorFactories.Values;

        /// <summary>
        /// Registers the specified factory.
        /// </summary>
        /// <param name="factory">The factory.</param>
        public static void Register(IMigrationProcessorFactory factory)
        {
            lock (_lock)
            {
                if (_migrationProcessorFactories == null)
                {
                    _migrationProcessorFactories = new Dictionary<string, IMigrationProcessorFactory>(StringComparer.OrdinalIgnoreCase);
                }

                _migrationProcessorFactories[factory.Name] = factory;
            }
        }

        /// <summary>
        /// Gets the processor types.
        /// </summary>
        /// <value>The processor types.</value>
        public static IEnumerable<string> ProcessorTypes
            => MigrationProcessorFactories.Keys;

        /// <summary>
        /// Gets the factory.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>IMigrationProcessorFactory.</returns>
        [Obsolete("Ony the statically provided factories are accessed")]
        public virtual IMigrationProcessorFactory GetFactory(string name)
        {
            if (MigrationProcessorFactories.TryGetValue(name, out var result))
                return result;
            return null;
        }

        /// <summary>
        /// Lists the available processor types.
        /// </summary>
        /// <returns>System.String.</returns>
        [Obsolete]
        public string ListAvailableProcessorTypes()
        {
            return string.Join(", ", MigrationProcessorFactories.Keys.ToArray());
        }

        /// <summary>
        /// Finds the processor factories.
        /// </summary>
        /// <returns>IDictionary&lt;System.String, IMigrationProcessorFactory&gt;.</returns>
        private static IDictionary<string, IMigrationProcessorFactory> FindProcessorFactories()
        {
            var availableMigrationProcessorFactories = new SortedDictionary<string, IMigrationProcessorFactory>(StringComparer.OrdinalIgnoreCase);

            foreach (var assembly in GetAssemblies())
            {
                List<Type> types = assembly
                    .GetExportedTypes()
                    .Where(type => type.IsConcrete() && type.Is<IMigrationProcessorFactory>())
                    .ToList();

                foreach (Type type in types)
                {
                    var factory = (IMigrationProcessorFactory)Activator.CreateInstance(type);
                    availableMigrationProcessorFactories.Add(factory.Name, factory);
                }
            }

            return availableMigrationProcessorFactories;
        }

        /// <summary>
        /// Gets the assemblies.
        /// </summary>
        /// <returns>IEnumerable&lt;Assembly&gt;.</returns>
        private static IEnumerable<Assembly> GetAssemblies()
        {
            var initialAssemblies = RuntimeHost.Current.GetLoadedAssemblies()
                .Where(x => x.GetName().Name.StartsWith("FluentMigrator."));
            var remainingAssemblies = new Queue<Assembly>(initialAssemblies);
            var processedAssemblies = new HashSet<string>(remainingAssemblies.Select(x => x.GetName().Name), StringComparer.OrdinalIgnoreCase);

            while (remainingAssemblies.Count != 0)
            {
                var asm = remainingAssemblies.Dequeue();
                yield return asm;

                var refAsms = asm.GetReferencedAssemblies().Where(x => x.Name.StartsWith("FluentMigrator."));
                foreach (var refAsm in refAsms)
                {
                    if (processedAssemblies.Add(refAsm.Name))
                    {
                        remainingAssemblies.Enqueue(Assembly.Load(refAsm));
                    }
                }
            }
        }
    }
}
