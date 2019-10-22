// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Core
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="ReflectionBasedDbFactory.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
// Copyright (c) 2007-2018, Fluent Migrator Project
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
using System.IO;
using System.Linq;
using System.Reflection;

using FluentMigrator.Runner.Infrastructure;

using JetBrains.Annotations;

namespace FluentMigrator.Runner.Processors
{
    using System.Data.Common;

    /// <summary>
    /// Class ReflectionBasedDbFactory.
    /// Implements the <see cref="FluentMigrator.Runner.Processors.DbFactoryBase" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Processors.DbFactoryBase" />
    public class ReflectionBasedDbFactory : DbFactoryBase
    {
        /// <summary>
        /// The service provider
        /// </summary>
        private readonly IServiceProvider _serviceProvider;
        /// <summary>
        /// The test entries
        /// </summary>
        private readonly TestEntry[] _testEntries;

        /// <summary>
        /// The instance
        /// </summary>
        private DbProviderFactory _instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReflectionBasedDbFactory"/> class.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="dbProviderFactoryTypeName">Name of the database provider factory type.</param>
        [Obsolete]
        public ReflectionBasedDbFactory(string assemblyName, string dbProviderFactoryTypeName)
            : this(new TestEntry(assemblyName, dbProviderFactoryTypeName))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReflectionBasedDbFactory"/> class.
        /// </summary>
        /// <param name="testEntries">The test entries.</param>
        /// <exception cref="ArgumentException">At least one test entry must be specified - testEntries</exception>
        [Obsolete]
        protected ReflectionBasedDbFactory(params TestEntry[] testEntries)
        {
            if (testEntries.Length == 0)
            {
                throw new ArgumentException(@"At least one test entry must be specified", nameof(testEntries));
            }

            _testEntries = testEntries;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReflectionBasedDbFactory"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="testEntries">The test entries.</param>
        /// <exception cref="ArgumentException">At least one test entry must be specified - testEntries</exception>
        protected ReflectionBasedDbFactory(IServiceProvider serviceProvider, params TestEntry[] testEntries)
        {
            if (testEntries.Length == 0)
            {
                throw new ArgumentException(@"At least one test entry must be specified", nameof(testEntries));
            }

            _serviceProvider = serviceProvider;
            _testEntries = testEntries;
        }

        /// <summary>
        /// Creates the factory.
        /// </summary>
        /// <returns>DbProviderFactory.</returns>
        /// <exception cref="AggregateException">Unable to load the driver. Attempted to load: {assemblyNames}, with {fullExceptionOutput}</exception>
        protected override DbProviderFactory CreateFactory()
        {
            if (_instance != null)
            {
                return _instance;
            }

            var exceptions = new List<Exception>();
            if (TryCreateFactory(_serviceProvider, _testEntries, exceptions, out var factory))
            {
                _instance = factory;
                return factory;
            }

            var assemblyNames = string.Join(", ", _testEntries.Select(x => x.AssemblyName));
            var fullExceptionOutput = string.Join(Environment.NewLine, exceptions.Select(x => x.ToString()));

            throw new AggregateException($"Unable to load the driver. Attempted to load: {assemblyNames}, with {fullExceptionOutput}", exceptions);
        }

        /// <summary>
        /// Tries the create factory.
        /// </summary>
        /// <param name="entries">The entries.</param>
        /// <param name="exceptions">The exceptions.</param>
        /// <param name="factory">The factory.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [Obsolete]
        protected static bool TryCreateFactory(
            [NotNull, ItemNotNull] IEnumerable<TestEntry> entries,
            [NotNull, ItemNotNull] ICollection<Exception> exceptions,
            out DbProviderFactory factory)
        {
            return TryCreateFactory(serviceProvider: null, entries, exceptions, out factory);
        }

        /// <summary>
        /// Tries the create factory.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="entries">The entries.</param>
        /// <param name="exceptions">The exceptions.</param>
        /// <param name="factory">The factory.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected static bool TryCreateFactory(
            [CanBeNull] IServiceProvider serviceProvider,
            [NotNull, ItemNotNull] IEnumerable<TestEntry> entries,
            [NotNull, ItemNotNull] ICollection<Exception> exceptions,
            out DbProviderFactory factory)
        {
            var entriesCollection = entries.ToList();

            foreach (var entry in entriesCollection)
            {
                if (TryCreateFromCurrentDomain(entry, exceptions, out factory))
                {
                    return true;
                }
            }

            foreach (var entry in entriesCollection)
            {
                if (TryCreateFactoryFromRuntimeHost(entry, exceptions, serviceProvider, out factory))
                {
                    return true;
                }
            }

            foreach (var entry in entriesCollection)
            {
                if (TryCreateFromAppDomainPaths(entry, exceptions, out factory))
                {
                    return true;
                }
            }

            foreach (var entry in entriesCollection)
            {
                if (TryCreateFromGac(entry, exceptions, out factory))
                {
                    return true;
                }
            }

            factory = null;
            return false;
        }

        /// <summary>
        /// Tries the create from application domain paths.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="exceptions">The exceptions.</param>
        /// <param name="factory">The factory.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected static bool TryCreateFromAppDomainPaths(
            [NotNull] TestEntry entry,
            [NotNull, ItemNotNull] ICollection<Exception> exceptions,
            out DbProviderFactory factory)
        {
            if (TryLoadAssemblyFromAppDomainDirectories(entry.AssemblyName, exceptions, out var assembly))
            {
                try
                {
                    var type = assembly.GetType(entry.DBProviderFactoryTypeName, true);
                    if (TryGetInstance(type, out factory))
                    {
                        return true;
                    }

                    factory = (DbProviderFactory) Activator.CreateInstance(type);
                    return true;
                }
                catch (Exception ex)
                {
                    // Ignore
                    exceptions.Add(ex);
                }
            }

            factory = null;
            return false;
        }

        /// <summary>
        /// Tries the create factory from runtime host.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="exceptions">The exceptions.</param>
        /// <param name="factory">The factory.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [Obsolete]
        protected static bool TryCreateFactoryFromRuntimeHost(
            [NotNull] TestEntry entry,
            [NotNull, ItemNotNull] ICollection<Exception> exceptions,
            out DbProviderFactory factory)
        {
            return TryCreateFactoryFromRuntimeHost(entry, exceptions, serviceProvider: null, out factory);
        }

        /// <summary>
        /// Tries the create factory from runtime host.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="exceptions">The exceptions.</param>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="factory">The factory.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected static bool TryCreateFactoryFromRuntimeHost(
            [NotNull] TestEntry entry,
            [NotNull, ItemNotNull] ICollection<Exception> exceptions,
            [CanBeNull] IServiceProvider serviceProvider,
            out DbProviderFactory factory)
        {
            try
            {
                factory = (DbProviderFactory)RuntimeHost.Current.CreateInstance(
                    serviceProvider,
                    entry.AssemblyName,
                    entry.DBProviderFactoryTypeName);
                return true;
            }
            catch (Exception ex)
            {
                // Ignore, check if we could load the assembly
                exceptions.Add(ex);
            }

            // Try to create from current domain in case of a successfully loaded assembly
            return TryCreateFromCurrentDomain(entry, exceptions, out factory);
        }

        /// <summary>
        /// Tries the load assembly from application domain directories.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="exceptions">The exceptions.</param>
        /// <param name="assembly">The assembly.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected static bool TryLoadAssemblyFromAppDomainDirectories(
            [NotNull] string assemblyName,
            [NotNull, ItemNotNull] ICollection<Exception> exceptions,
            out Assembly assembly)
        {
            return TryLoadAssemblyFromDirectories(
                GetPathsFromAppDomain(),
                assemblyName,
                exceptions,
                out assembly);
        }

        /// <summary>
        /// Tries the load assembly from directories.
        /// </summary>
        /// <param name="directories">The directories.</param>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="exceptions">The exceptions.</param>
        /// <param name="assembly">The assembly.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected static bool TryLoadAssemblyFromDirectories(
            [NotNull, ItemNotNull] IEnumerable<string> directories,
            [NotNull] string assemblyName,
            [NotNull, ItemNotNull] ICollection<Exception> exceptions,
            out Assembly assembly)
        {
            var alreadyTested = new HashSet<string>(StringComparer.InvariantCulture);
            var assemblyFileName = $"{assemblyName}.dll";
            foreach (var directory in directories)
            {
                var path = Path.Combine(directory, assemblyFileName);
                if (!alreadyTested.Add(path))
                {
                    continue;
                }

                try
                {
                    assembly = Assembly.LoadFile(path);
                    return true;
                }
                catch (Exception ex)
                {
                    exceptions.Add(new Exception($"Failed to load file {path}", ex));
                }
            }

            assembly = null;
            return false;
        }

        /// <summary>
        /// Tries the create from gac.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="exceptions">The exceptions.</param>
        /// <param name="factory">The factory.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private static bool TryCreateFromGac(
            [NotNull] TestEntry entry,
            [NotNull, ItemNotNull] ICollection<Exception> exceptions,
            out DbProviderFactory factory)
        {
            var asmNames = FindAssembliesInGac(entry.AssemblyName);
            var asmName = asmNames.OrderByDescending(n => n.Version).FirstOrDefault();

            if (asmName == null)
            {
                factory = null;
                return false;
            }

            try
            {
                var assembly = Assembly.Load(asmName);
                var type = assembly.GetType(entry.DBProviderFactoryTypeName, true);
                if (TryGetInstance(type, out factory))
                {
                    return true;
                }

                factory = (DbProviderFactory) Activator.CreateInstance(type);
                return true;
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
                factory = null;
                return false;
            }
        }

        /// <summary>
        /// Tries the create from current domain.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="exceptions">The exceptions.</param>
        /// <param name="factory">The factory.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private static bool TryCreateFromCurrentDomain(
            [NotNull] TestEntry entry,
            [NotNull, ItemNotNull] ICollection<Exception> exceptions,
            out DbProviderFactory factory)
        {
            if (TryLoadAssemblyFromCurrentDomain(entry.AssemblyName, exceptions, out var assembly))
            {
                try
                {
                    var type = assembly.GetType(entry.DBProviderFactoryTypeName, true);
                    if (TryGetInstance(type, out factory))
                    {
                        return true;
                    }

                    factory = (DbProviderFactory) Activator.CreateInstance(type);
                    return true;
                }
                catch (Exception ex)
                {
                    // Ignore
                    exceptions.Add(ex);
                }
            }

            factory = null;
            return false;
        }

        /// <summary>
        /// Tries the load assembly from current domain.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="exceptions">The exceptions.</param>
        /// <param name="assembly">The assembly.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private static bool TryLoadAssemblyFromCurrentDomain(
            [NotNull] string assemblyName,
            [NotNull, ItemNotNull] ICollection<Exception> exceptions,
            out Assembly assembly)
        {
            try
            {
                assembly = AppDomain.CurrentDomain.Load(new AssemblyName(assemblyName));
                return true;
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
                assembly = null;
                return false;
            }
        }

        /// <summary>
        /// Finds the assemblies in gac.
        /// </summary>
        /// <param name="names">The names.</param>
        /// <returns>IEnumerable&lt;AssemblyName&gt;.</returns>
        [NotNull, ItemNotNull]
        private static IEnumerable<AssemblyName> FindAssembliesInGac([NotNull, ItemNotNull] params string[] names)
        {
            foreach (var name in names)
            {
                foreach (var assemblyName in RuntimeHost.FindAssemblies(name))
                {
                    yield return assemblyName;
                }
            }
        }

        /// <summary>
        /// Tries the get instance.
        /// </summary>
        /// <param name="factoryType">Type of the factory.</param>
        /// <param name="factory">The factory.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private static bool TryGetInstance(
            [NotNull] Type factoryType,
            out DbProviderFactory factory)
        {
            var instanceField = factoryType.GetField(
                "Instance",
                BindingFlags.Static | BindingFlags.Public | BindingFlags.GetField);

            if (instanceField != null && TryCastInstance(instanceField.GetValue(null), out factory))
            {
                return true;
            }

            var instanceProperty = factoryType.GetProperty(
                "Instance",
                BindingFlags.Static | BindingFlags.Public | BindingFlags.GetProperty);
            if (instanceProperty != null && TryCastInstance(instanceProperty.GetValue(null, null), out factory))
            {
                return true;
            }

            factory = null;
            return false;
        }

        /// <summary>
        /// Tries the cast instance.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="factory">The factory.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private static bool TryCastInstance(
            [NotNull] object value,
            out DbProviderFactory factory)
        {
            factory = value as DbProviderFactory;
            return factory != null;
        }

        /// <summary>
        /// Gets the paths from application domain.
        /// </summary>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        [NotNull, ItemNotNull]
        private static IEnumerable<string> GetPathsFromAppDomain()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                string assemblyDirectory;
                try
                {
                    assemblyDirectory = Path.GetDirectoryName(assembly.Location);
                    if (assemblyDirectory == null)
                    {
                        continue;
                    }
                }
                catch
                {
                    // Ignore error caused by dynamic assembly
                    continue;
                }

                yield return assemblyDirectory;
            }
        }

        /// <summary>
        /// Class TestEntry.
        /// </summary>
        protected class TestEntry
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="TestEntry"/> class.
            /// </summary>
            /// <param name="assemblyName">Name of the assembly.</param>
            /// <param name="dbProviderFactoryTypeName">Name of the database provider factory type.</param>
            public TestEntry(
                [NotNull] string assemblyName,
                [NotNull] string dbProviderFactoryTypeName)
            {
                AssemblyName = assemblyName;
                DBProviderFactoryTypeName = dbProviderFactoryTypeName;
            }

            /// <summary>
            /// Gets the name of the assembly.
            /// </summary>
            /// <value>The name of the assembly.</value>
            [NotNull]
            public string AssemblyName { get; }

            /// <summary>
            /// Gets the name of the database provider factory type.
            /// </summary>
            /// <value>The name of the database provider factory type.</value>
            [NotNull]
            public string DBProviderFactoryTypeName { get; }
        }
    }
}
