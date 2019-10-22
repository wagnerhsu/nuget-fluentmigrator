// ***********************************************************************
// Assembly         : FluentMigrator.Runner
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="RunnerContext.cs" company="FluentMigrator Project">
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

namespace FluentMigrator.Runner.Initialization
{
    /// <summary>
    /// Class RunnerContext.
    /// Implements the <see cref="FluentMigrator.Runner.Initialization.IRunnerContext" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Initialization.IRunnerContext" />
    [Obsolete]
    public class RunnerContext : IRunnerContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RunnerContext"/> class.
        /// </summary>
        /// <param name="announcer">The announcer.</param>
        public RunnerContext(IAnnouncer announcer)
        {
            Announcer = announcer;
        }

        /// <summary>
        /// Gets or sets the identifier of the processor to use
        /// </summary>
        /// <value>The database.</value>
        public string Database { get; set; }
        /// <summary>
        /// Gets or sets the connection string (or name)
        /// </summary>
        /// <value>The connection.</value>
        /// <remarks>Will not be used when <see cref="P:FluentMigrator.Runner.Initialization.IRunnerContext.PreviewOnly" /> is active.
        /// The option is now directly stored in <see cref="P:FluentMigrator.Runner.Processors.ProcessorOptions.ConnectionString" /></remarks>
        public string Connection { get; set; }
        /// <summary>
        /// Gets or sets the assembly names
        /// </summary>
        /// <value>The targets.</value>
        /// <remarks>The option is now stored in <see cref="P:FluentMigrator.Runner.Initialization.AssemblySourceOptions.AssemblyNames" /></remarks>
        public string[] Targets { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether a preview-only mode is active
        /// </summary>
        /// <value><c>true</c> if [preview only]; otherwise, <c>false</c>.</value>
        /// <remarks>The option is now directly stored in <see cref="P:FluentMigrator.Runner.Processors.ProcessorOptions.PreviewOnly" /></remarks>
        public bool PreviewOnly { get; set; }
        /// <summary>
        /// Gets or sets the root namespace for filtering
        /// </summary>
        /// <value>The namespace.</value>
        /// <remarks>The option is now directly stored in <see cref="P:FluentMigrator.Runner.Initialization.TypeFilterOptions.Namespace" />.</remarks>
        public string Namespace { get; set; }
        /// <summary>
        /// Gets or sets the value indicating whether all sub-namespaces should be included
        /// </summary>
        /// <value><c>true</c> if [nested namespaces]; otherwise, <c>false</c>.</value>
        /// <remarks>The option is now directly stored in <see cref="P:FluentMigrator.Runner.Initialization.TypeFilterOptions.NestedNamespaces" />.</remarks>
        public bool NestedNamespaces { get; set; }
        /// <summary>
        /// Gets or sets the task to execute
        /// </summary>
        /// <value>The task.</value>
        /// <remarks>The option is now stored in <see cref="P:FluentMigrator.Runner.Initialization.RunnerOptions.Task" />.</remarks>
        public string Task { get; set; }
        /// <summary>
        /// Gets or sets the target version
        /// </summary>
        /// <value>The version.</value>
        /// <remarks>The option is now stored in <see cref="P:FluentMigrator.Runner.Initialization.RunnerOptions.Version" />.</remarks>
        public long Version { get; set; }
        /// <summary>
        /// Gets or sets the start version
        /// </summary>
        /// <value>The start version.</value>
        /// <remarks>The option is now stored in <see cref="P:FluentMigrator.Runner.Initialization.RunnerOptions.StartVersion" />.</remarks>
        public long StartVersion { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether no connection should be used
        /// </summary>
        /// <value><c>true</c> if [no connection]; otherwise, <c>false</c>.</value>
        /// <remarks>The difference between this and <see cref="P:FluentMigrator.Runner.Initialization.IRunnerContext.PreviewOnly" /> is, that
        /// the preview-only mode uses the connection to determine the current
        /// state of the database.</remarks>
        public bool NoConnection { get; set; }
        /// <summary>
        /// Gets or sets the number of versions to apply
        /// </summary>
        /// <value>The steps.</value>
        /// <remarks>The option is now stored in <see cref="P:FluentMigrator.Runner.Initialization.RunnerOptions.Steps" />.</remarks>
        public int Steps { get; set; }
        /// <summary>
        /// Gets or sets the working directory
        /// </summary>
        /// <value>The working directory.</value>
        public string WorkingDirectory { get; set; }
        /// <summary>
        /// Gets or sets the profile migrations to apply
        /// </summary>
        /// <value>The profile.</value>
        /// <remarks>The option is now stored in <see cref="P:FluentMigrator.Runner.Initialization.RunnerOptions.Profile" />.</remarks>
        public string Profile { get; set; }
        /// <summary>
        /// Gets or sets the default command timeout in seconds
        /// </summary>
        /// <value>The timeout.</value>
        public int? Timeout { get; set; }
        /// <summary>
        /// Gets or sets the path to an app.config/web.config to load the connection string from
        /// </summary>
        /// <value>The connection string configuration path.</value>
        public string ConnectionStringConfigPath { get; set; }
        /// <summary>
        /// Gets or sets the tags the migrations must match
        /// </summary>
        /// <value>The tags.</value>
        /// <remarks>All migrations are matched when no tags were specified.
        /// The option is now stored in <see cref="P:FluentMigrator.Runner.Initialization.RunnerOptions.Tags" />.</remarks>
        public IEnumerable<string> Tags { get; set; }
        /// <summary>
        /// Use one transaction for the whole session
        /// </summary>
        /// <value><c>true</c> if [transaction per session]; otherwise, <c>false</c>.</value>
        /// <remarks>The default transaction behavior is to use one transaction per migration.
        /// The option is now stored in <see cref="P:FluentMigrator.Runner.Initialization.RunnerOptions.TransactionPerSession" />.</remarks>
        public bool TransactionPerSession { get; set; }

        /// <inheritdoc />
        public bool AllowBreakingChange { get; set; }
        /// <summary>
        /// Gets or sets the provider switches
        /// </summary>
        /// <value>The provider switches.</value>
        /// <remarks>The option is now stored in <see cref="P:FluentMigrator.Runner.Processors.ProcessorOptions.PreviewOnly" />.</remarks>
        public string ProviderSwitches { get; set; }

        /// <summary>
        /// Gets or sets the announcer to use
        /// </summary>
        /// <value>The announcer.</value>
        public IAnnouncer Announcer { get; }

        /// <summary>
        /// Gets or sets the stopwatch to use
        /// </summary>
        /// <value>The stop watch.</value>
        public IStopWatch StopWatch { get; } = new StopWatch();

        /// <inheritdoc />
        public object ApplicationContext { get; set; }

        /// <inheritdoc />
        public string DefaultSchemaName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [strip comments].
        /// </summary>
        /// <value><c>true</c> if [strip comments]; otherwise, <c>false</c>.</value>
        public bool StripComments { get; set; }
    }
}
