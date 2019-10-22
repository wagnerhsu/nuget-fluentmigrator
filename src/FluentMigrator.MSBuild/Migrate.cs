// ***********************************************************************
// Assembly         : FluentMigrator.MSBuild
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="Migrate.cs" company="FluentMigrator Project">
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
using System.Reflection;

using FluentMigrator.Exceptions;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Conventions;
using FluentMigrator.Runner.Extensions;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Initialization.NetFramework;
using FluentMigrator.Runner.Logging;
using FluentMigrator.Runner.Processors;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FluentMigrator.MSBuild
{
    /// <summary>
    /// Class Migrate.
    /// Implements the <see cref="Microsoft.Build.Utilities.AppDomainIsolatedTask" />
    /// </summary>
    /// <seealso cref="Microsoft.Build.Utilities.AppDomainIsolatedTask" />
    public class Migrate :
#if NETFRAMEWORK
        AppDomainIsolatedTask
#else
        Task
#endif
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Migrate" /> class.
        /// </summary>
        public Migrate()
        {
            AppDomain.CurrentDomain.ResourceResolve += CurrentDomain_ResourceResolve;
        }

        /// <summary>
        /// Handles the ResourceResolve event of the CurrentDomain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="ResolveEventArgs"/> instance containing the event data.</param>
        /// <returns>Assembly.</returns>
        private static Assembly CurrentDomain_ResourceResolve(object sender, ResolveEventArgs args)
        {
            Console.WriteLine(@"Could Not Resolve {0}", args.Name);
            return null;
        }

        /// <summary>
        /// The database type
        /// </summary>
        private string _databaseType;

        /// <summary>
        /// The timeout
        /// </summary>
        private int? _timeout;

        /// <summary>
        /// Gets or sets the application context.
        /// </summary>
        /// <value>The application context.</value>
        public string ApplicationContext { get; set; }

        /// <summary>
        /// Gets or sets the connection.
        /// </summary>
        /// <value>The connection.</value>
        [Required]
        public string Connection { get; set; }

        /// <summary>
        /// Gets or sets the connection string configuration path.
        /// </summary>
        /// <value>The connection string configuration path.</value>
        public string ConnectionStringConfigPath { get; set; }

        /// <summary>
        /// Gets or sets the target.
        /// </summary>
        /// <value>The target.</value>
        public string Target { get { return (Targets != null && Targets.Length == 1) ? Targets[0] : string.Empty; } set { Targets = new[] { value }; } }

        /// <summary>
        /// Gets or sets the targets.
        /// </summary>
        /// <value>The targets.</value>
        public string[] Targets { get; set; }
        /// <summary>
        /// Gets or sets the migration assembly.
        /// </summary>
        /// <value>The migration assembly.</value>
        public string MigrationAssembly { get { return (Targets != null && Targets.Length == 1) ? Targets[0] : string.Empty; } set { Targets = new[] {value}; } }

        /// <summary>
        /// Gets or sets the database.
        /// </summary>
        /// <value>The database.</value>
        public string Database { get { return _databaseType; } set { _databaseType = value; } }

        /// <summary>
        /// Gets or sets the type of the database.
        /// </summary>
        /// <value>The type of the database.</value>
        public string DatabaseType { get { return _databaseType; } set { _databaseType = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Migrate"/> is verbose.
        /// </summary>
        /// <value><c>true</c> if verbose; otherwise, <c>false</c>.</value>
        public bool Verbose { get; set; }

        /// <summary>
        /// Gets or sets the namespace.
        /// </summary>
        /// <value>The namespace.</value>
        public string Namespace { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Migrate"/> is nested.
        /// </summary>
        /// <value><c>true</c> if nested; otherwise, <c>false</c>.</value>
        public bool Nested { get; set; }

        /// <summary>
        /// Gets or sets the task.
        /// </summary>
        /// <value>The task.</value>
        public string Task { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        public long Version { get; set; }

        /// <summary>
        /// Gets or sets the steps.
        /// </summary>
        /// <value>The steps.</value>
        public int Steps { get; set; }

        /// <summary>
        /// Gets or sets the working directory.
        /// </summary>
        /// <value>The working directory.</value>
        public string WorkingDirectory { get; set; }

        /// <summary>
        /// Gets or sets the timeout.
        /// </summary>
        /// <value>The timeout.</value>
        public int Timeout
        {
            get => _timeout ?? 0;
            set => _timeout = value;
        }

        /// <summary>
        /// Gets or sets the profile.
        /// </summary>
        /// <value>The profile.</value>
        public string Profile { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [preview only].
        /// </summary>
        /// <value><c>true</c> if [preview only]; otherwise, <c>false</c>.</value>
        public bool PreviewOnly { get; set; }

        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        /// <value>The tags.</value>
        public string Tags { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Migrate"/> is output.
        /// </summary>
        /// <value><c>true</c> if output; otherwise, <c>false</c>.</value>
        public bool Output { get; set; }

        /// <summary>
        /// Gets or sets the output filename.
        /// </summary>
        /// <value>The output filename.</value>
        public string OutputFilename { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [transaction per session].
        /// </summary>
        /// <value><c>true</c> if [transaction per session]; otherwise, <c>false</c>.</value>
        public bool TransactionPerSession { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [allow breaking change].
        /// </summary>
        /// <value><c>true</c> if [allow breaking change]; otherwise, <c>false</c>.</value>
        public bool AllowBreakingChange { get; set; }

        /// <summary>
        /// Gets or sets the provider switches.
        /// </summary>
        /// <value>The provider switches.</value>
        public string ProviderSwitches { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [strip comments].
        /// </summary>
        /// <value><c>true</c> if [strip comments]; otherwise, <c>false</c>.</value>
        public bool StripComments { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether [include untagged maintenances].
        /// </summary>
        /// <value><c>true</c> if [include untagged maintenances]; otherwise, <c>false</c>.</value>
        public bool IncludeUntaggedMaintenances { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [include untagged migrations].
        /// </summary>
        /// <value><c>true</c> if [include untagged migrations]; otherwise, <c>false</c>.</value>
        public bool IncludeUntaggedMigrations { get; set; } = true;

        /// <summary>
        /// Gets or sets the default name of the schema.
        /// </summary>
        /// <value>The default name of the schema.</value>
        public string DefaultSchemaName { get; set; }

        /// <summary>
        /// Gets a value indicating whether [executing against ms SQL].
        /// </summary>
        /// <value><c>true</c> if [executing against ms SQL]; otherwise, <c>false</c>.</value>
        private bool ExecutingAgainstMsSql => _databaseType.StartsWith("SqlServer", StringComparison.InvariantCultureIgnoreCase);

        /// <summary>
        /// Executes this instance.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public override bool Execute()
        {

            if (string.IsNullOrEmpty(_databaseType))
            {
                Log.LogError("You must specify a database type. i.e. mysql or sqlserver");
                return false;
            }

            if (Targets == null || Targets.Length == 0)
            {
                Log.LogError("You must specify a migration assemblies ");
                return false;
            }

            Log.LogMessage(MessageImportance.Low, "Executing Migration Runner");
            try
            {
                Log.LogMessage(MessageImportance.Low, "Creating Context");

                ExecuteMigrations();
            }
            catch (ProcessorFactoryNotFoundException ex)
            {
                Log.LogError("While executing migrations the following error was encountered: {0}", ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                Log.LogError("While executing migrations the following error was encountered: {0}, {1}", ex.Message, ex.StackTrace);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Executes the migrations.
        /// </summary>
        private void ExecuteMigrations()
        {
            var conventionSet = new DefaultConventionSet(DefaultSchemaName, WorkingDirectory);

            var services = CreateCoreServices()
                .AddSingleton<IConventionSet>(conventionSet)
                .AddSingleton<ILoggerProvider, FluentMigratorConsoleLoggerProvider>()
                .Configure<SelectingProcessorAccessorOptions>(opt => opt.ProcessorId = DatabaseType)
                .Configure<AssemblySourceOptions>(opt => opt.AssemblyNames = Targets)
#pragma warning disable 612
                .Configure<AppConfigConnectionStringAccessorOptions>(
                    opt => opt.ConnectionStringConfigPath = ConnectionStringConfigPath)
#pragma warning restore 612
                .Configure<TypeFilterOptions>(
                    opt =>
                    {
                        opt.Namespace = Namespace;
                        opt.NestedNamespaces = Nested;
                    })
                .Configure<RunnerOptions>(
                    opt =>
                    {
                        opt.Task = Task;
                        opt.Version = Version;
                        opt.Steps = Steps;
                        opt.Profile = Profile;
                        opt.Tags = Tags.ToTags().ToArray();
#pragma warning disable 612
                        opt.ApplicationContext = ApplicationContext;
#pragma warning restore 612
                        opt.TransactionPerSession = TransactionPerSession;
                        opt.AllowBreakingChange = AllowBreakingChange;
                        opt.IncludeUntaggedMigrations = IncludeUntaggedMigrations;
                        opt.IncludeUntaggedMaintenances = IncludeUntaggedMaintenances;
                    })
                .Configure<ProcessorOptions>(
                    opt =>
                    {
                        opt.ConnectionString = Connection;
                        opt.PreviewOnly = PreviewOnly;
                        opt.ProviderSwitches = ProviderSwitches;
                        opt.StripComments = StripComments;
                        opt.Timeout = _timeout == null ? null : (TimeSpan?)TimeSpan.FromSeconds(_timeout.Value);
                    });

            if (Output)
            {
                services
                    .Configure<LogFileFluentMigratorLoggerOptions>(
                        opt =>
                        {
                            opt.ShowSql = true;
                            opt.OutputFileName = OutputFilename;
                            opt.OutputGoBetweenStatements = ExecutingAgainstMsSql;
                        })
                    .AddSingleton<ILoggerProvider, LogFileFluentMigratorLoggerProvider>();
            }

            using (var serviceProvider = services.BuildServiceProvider(validateScopes: false))
            {
                var executor = serviceProvider.GetRequiredService<TaskExecutor>();
                executor.Execute();
            }
        }

        /// <summary>
        /// Creates the core services.
        /// </summary>
        /// <returns>IServiceCollection.</returns>
        private static IServiceCollection CreateCoreServices()
        {
            var services = new ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(
                    builder => builder
                        .AddDb2()
                        .AddDb2ISeries()
                        .AddDotConnectOracle()
                        .AddDotConnectOracle12C()
                        .AddFirebird()
                        .AddHana()
                        .AddMySql4()
                        .AddMySql5()
                        .AddOracle()
                        .AddOracle12C()
                        .AddOracleManaged()
                        .AddOracle12CManaged()
                        .AddPostgres()
                        .AddPostgres92()
                        .AddRedshift()
                        .AddSqlAnywhere()
                        .AddSQLite()
                        .AddSqlServer()
                        .AddSqlServer2000()
                        .AddSqlServer2005()
                        .AddSqlServer2008()
                        .AddSqlServer2012()
                        .AddSqlServer2014()
                        .AddSqlServer2016()
                        .AddSqlServerCe());
            return services;
        }
    }
}
