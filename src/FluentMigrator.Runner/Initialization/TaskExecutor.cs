// ***********************************************************************
// Assembly         : FluentMigrator.Runner
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="TaskExecutor.cs" company="FluentMigrator Project">
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
using System.Reflection;

using FluentMigrator.Infrastructure;
using FluentMigrator.Runner.Initialization.AssemblyLoader;
using FluentMigrator.Runner.Logging;
using FluentMigrator.Runner.Processors;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FluentMigrator.Runner.Initialization
{
    /// <summary>
    /// Class TaskExecutor.
    /// </summary>
    public class TaskExecutor
    {
        /// <summary>
        /// The logger
        /// </summary>
        [NotNull]
        private readonly ILogger _logger;

        /// <summary>
        /// The assembly source
        /// </summary>
        [NotNull]
        private readonly IAssemblySource _assemblySource;

        /// <summary>
        /// The runner options
        /// </summary>
        private readonly RunnerOptions _runnerOptions;

        /// <summary>
        /// The lazy service provider
        /// </summary>
        [NotNull, ItemNotNull]
        private readonly Lazy<IServiceProvider> _lazyServiceProvider;

        /// <summary>
        /// The assemblies
        /// </summary>
        private IReadOnlyCollection<Assembly> _assemblies;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskExecutor"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="assemblySource">The assembly source.</param>
        /// <param name="runnerOptions">The runner options.</param>
        /// <param name="serviceProvider">The service provider.</param>
        public TaskExecutor(
            [NotNull] ILogger<TaskExecutor> logger,
            [NotNull] IAssemblySource assemblySource,
            [NotNull] IOptions<RunnerOptions> runnerOptions,
            [NotNull] IServiceProvider serviceProvider)
        {
            _logger = logger;
            _assemblySource = assemblySource;
            _runnerOptions = runnerOptions.Value;
#pragma warning disable 612
            ConnectionStringProvider = serviceProvider.GetService<IConnectionStringProvider>();
#pragma warning restore 612
            _lazyServiceProvider = new Lazy<IServiceProvider>(() => serviceProvider);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskExecutor"/> class.
        /// </summary>
        /// <param name="runnerContext">The runner context.</param>
        /// <exception cref="ArgumentNullException">runnerContext</exception>
        [Obsolete]
        public TaskExecutor([NotNull] IRunnerContext runnerContext)
        {
            var runnerCtxt = runnerContext ?? throw new ArgumentNullException(nameof(runnerContext));
            _logger = new AnnouncerFluentMigratorLogger(runnerCtxt.Announcer);
            _runnerOptions = new RunnerOptions(runnerCtxt);
            var asmLoaderFactory = new AssemblyLoaderFactory();
            _assemblySource = new AssemblySource(() => new AssemblyCollection(asmLoaderFactory.GetTargetAssemblies(runnerCtxt.Targets)));
            ConnectionStringProvider = new DefaultConnectionStringProvider();
            _lazyServiceProvider = new Lazy<IServiceProvider>(
                () => runnerContext
                    .CreateServices(
                        ConnectionStringProvider,
                        asmLoaderFactory)
                    .BuildServiceProvider(validateScopes: true));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskExecutor"/> class.
        /// </summary>
        /// <param name="runnerContext">The runner context.</param>
        /// <param name="connectionStringProvider">The connection string provider.</param>
        /// <param name="assemblyLoaderFactory">The assembly loader factory.</param>
        /// <param name="factoryProvider">The factory provider.</param>
        [Obsolete("Ony the statically provided factories are accessed")]
        public TaskExecutor(
            [NotNull] IRunnerContext runnerContext,
            [CanBeNull] IConnectionStringProvider connectionStringProvider,
            [NotNull] AssemblyLoaderFactory assemblyLoaderFactory,
            // ReSharper disable once UnusedParameter.Local
            MigrationProcessorFactoryProvider factoryProvider)
            : this(
                runnerContext,
                assemblyLoaderFactory,
                connectionStringProvider)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskExecutor"/> class.
        /// </summary>
        /// <param name="runnerContext">The runner context.</param>
        /// <param name="assemblyLoaderFactory">The assembly loader factory.</param>
        /// <param name="connectionStringProvider">The connection string provider.</param>
        /// <exception cref="ArgumentNullException">runnerContext</exception>
        /// <exception cref="ArgumentNullException">assemblyLoaderFactory</exception>
        [Obsolete]
        public TaskExecutor(
            [NotNull] IRunnerContext runnerContext,
            [NotNull] AssemblyLoaderFactory assemblyLoaderFactory,
            [CanBeNull] IConnectionStringProvider connectionStringProvider = null)
        {
            var runnerCtxt = runnerContext ?? throw new ArgumentNullException(nameof(runnerContext));
            _logger = new AnnouncerFluentMigratorLogger(runnerCtxt.Announcer);
            _runnerOptions = new RunnerOptions(runnerCtxt);
            ConnectionStringProvider = connectionStringProvider;
            var asmLoaderFactory = assemblyLoaderFactory ?? throw new ArgumentNullException(nameof(assemblyLoaderFactory));
            _assemblySource = new AssemblySource(() => new AssemblyCollection(asmLoaderFactory.GetTargetAssemblies(runnerCtxt.Targets)));
            _lazyServiceProvider = new Lazy<IServiceProvider>(
                () => runnerContext
                    .CreateServices(
                        connectionStringProvider,
                        asmLoaderFactory)
                    .BuildServiceProvider(validateScopes: true));
        }

        /// <summary>
        /// Gets the current migration runner
        /// </summary>
        /// <value>The runner.</value>
        /// <remarks>This will only be set during a migration operation</remarks>
        [CanBeNull]
        protected IMigrationRunner Runner { get; set; }

        /// <summary>
        /// Gets the connection string provider
        /// </summary>
        /// <value>The connection string provider.</value>
        [CanBeNull]
        [Obsolete]
        protected IConnectionStringProvider ConnectionStringProvider { get; }

        /// <summary>
        /// Gets the service provider used to instantiate all migration services
        /// </summary>
        /// <value>The service provider.</value>
        [NotNull]
        protected IServiceProvider ServiceProvider => _lazyServiceProvider.Value;

        /// <summary>
        /// Gets the target assemblies.
        /// </summary>
        /// <returns>IEnumerable&lt;Assembly&gt;.</returns>
        [Obsolete]
        protected virtual IEnumerable<Assembly> GetTargetAssemblies()
        {
            return _assemblies ?? (_assemblies = _assemblySource.Assemblies);
        }

        /// <summary>
        /// Will be called durin the runner scope intialization
        /// </summary>
        /// <remarks>The <see cref="Runner" /> isn't initialized yet.</remarks>
        protected virtual void Initialize()
        {
        }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        public void Execute()
        {
            using (var scope = new RunnerScope(this))
            {
                switch (_runnerOptions.Task)
                {
                    case null:
                    case "":
                    case "migrate":
                    case "migrate:up":
                        if (_runnerOptions.Version != 0)
                            scope.Runner.MigrateUp(_runnerOptions.Version);
                        else
                            scope.Runner.MigrateUp();
                        break;
                    case "rollback":
                        if (_runnerOptions.Steps == 0)
                            _runnerOptions.Steps = 1;
                        scope.Runner.Rollback(_runnerOptions.Steps);
                        break;
                    case "rollback:toversion":
                        scope.Runner.RollbackToVersion(_runnerOptions.Version);
                        break;
                    case "rollback:all":
                        scope.Runner.RollbackToVersion(0);
                        break;
                    case "migrate:down":
                        scope.Runner.MigrateDown(_runnerOptions.Version);
                        break;
                    case "validateversionorder":
                        scope.Runner.ValidateVersionOrder();
                        break;
                    case "listmigrations":
                        scope.Runner.ListMigrations();
                        break;
                }
            }

            _logger.LogSay("Task completed.");
        }

        /// <summary>
        /// Checks whether the current task will actually run any migrations.
        /// Can be used to decide whether it's necessary perform a backup before the migrations are executed.
        /// </summary>
        /// <returns><c>true</c> if [has migrations to apply]; otherwise, <c>false</c>.</returns>
        public bool HasMigrationsToApply()
        {
            using (var scope = new RunnerScope(this))
            {
                switch (_runnerOptions.Task)
                {
                    case null:
                    case "":
                    case "migrate":
                    case "migrate:up":
                        if (_runnerOptions.Version != 0)
                            return scope.Runner.HasMigrationsToApplyUp(_runnerOptions.Version);

                        return scope.Runner.HasMigrationsToApplyUp();
                    case "rollback":
                    case "rollback:all":
                        // Number of steps doesn't matter as long as there's at least
                        // one migration applied (at least that one will be rolled back)
                        return scope.Runner.HasMigrationsToApplyRollback();
                    case "rollback:toversion":
                    case "migrate:down":
                        return scope.Runner.HasMigrationsToApplyDown(_runnerOptions.Version);
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Class RunnerScope.
        /// Implements the <see cref="System.IDisposable" />
        /// </summary>
        /// <seealso cref="System.IDisposable" />
        private class RunnerScope : IDisposable
        {
            /// <summary>
            /// The executor
            /// </summary>
            [NotNull]
            private readonly TaskExecutor _executor;

            /// <summary>
            /// The service scope
            /// </summary>
            [CanBeNull]
            private readonly IServiceScope _serviceScope;

            /// <summary>
            /// The has custom runner
            /// </summary>
            private readonly bool _hasCustomRunner;

            /// <summary>
            /// Initializes a new instance of the <see cref="RunnerScope"/> class.
            /// </summary>
            /// <param name="executor">The executor.</param>
            public RunnerScope([NotNull] TaskExecutor executor)
            {
                _executor = executor;

                executor.Initialize();

                if (executor.Runner != null)
                {
                    Runner = executor.Runner;
                    _hasCustomRunner = true;
                }
                else
                {
                    var serviceScope = executor.ServiceProvider.CreateScope();
                    _serviceScope = serviceScope;
                    _executor.Runner = Runner = serviceScope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                }
            }

            /// <summary>
            /// Gets the runner.
            /// </summary>
            /// <value>The runner.</value>
            public IMigrationRunner Runner { get; }

            /// <summary>
            /// Disposes this instance.
            /// </summary>
            public void Dispose()
            {
                if (_hasCustomRunner)
                {
                    Runner.Processor.Dispose();
                }
                else
                {
                    _executor.Runner = null;
                    _serviceScope?.Dispose();
                }
            }
        }
    }
}
