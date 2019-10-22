// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Core
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="IRunnerContext.cs" company="FluentMigrator Project">
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

using FluentMigrator.Runner.Conventions;
using FluentMigrator.Runner.Processors;

namespace FluentMigrator.Runner.Initialization
{
    /// <summary>
    /// Interface IRunnerContext
    /// </summary>
    [Obsolete]
    public interface IRunnerContext
    {
        /// <summary>
        /// Gets or sets the identifier of the processor to use
        /// </summary>
        /// <value>The database.</value>
        [Obsolete("A preselection must happen during the migration runner configuration")]
        string Database { get; set; }

        /// <summary>
        /// Gets or sets the connection string (or name)
        /// </summary>
        /// <value>The connection.</value>
        /// <remarks>Will not be used when <see cref="PreviewOnly" /> is active.
        /// The option is now directly stored in <see cref="ProcessorOptions.ConnectionString" /></remarks>
        string Connection { get; set; }

        /// <summary>
        /// Gets or sets the assembly names
        /// </summary>
        /// <value>The targets.</value>
        /// <remarks>The option is now stored in <see cref="AssemblySourceOptions.AssemblyNames" /></remarks>
        string[] Targets { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a preview-only mode is active
        /// </summary>
        /// <value><c>true</c> if [preview only]; otherwise, <c>false</c>.</value>
        /// <remarks>The option is now directly stored in <see cref="ProcessorOptions.PreviewOnly" /></remarks>
        bool PreviewOnly { get; set; }

        /// <summary>
        /// Gets or sets the root namespace for filtering
        /// </summary>
        /// <value>The namespace.</value>
        /// <remarks>The option is now directly stored in <see cref="TypeFilterOptions.Namespace" />.</remarks>
        string Namespace { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether all sub-namespaces should be included
        /// </summary>
        /// <value><c>true</c> if [nested namespaces]; otherwise, <c>false</c>.</value>
        /// <remarks>The option is now directly stored in <see cref="TypeFilterOptions.NestedNamespaces" />.</remarks>
        bool NestedNamespaces { get; set; }

        /// <summary>
        /// Gets or sets the task to execute
        /// </summary>
        /// <value>The task.</value>
        /// <remarks>The option is now stored in <see cref="RunnerOptions.Task" />.</remarks>
        string Task { get; set; }

        /// <summary>
        /// Gets or sets the target version
        /// </summary>
        /// <value>The version.</value>
        /// <remarks>The option is now stored in <see cref="RunnerOptions.Version" />.</remarks>
        long Version { get; set; }

        /// <summary>
        /// Gets or sets the start version
        /// </summary>
        /// <value>The start version.</value>
        /// <remarks>The option is now stored in <see cref="RunnerOptions.StartVersion" />.</remarks>
        long StartVersion { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether no connection should be used
        /// </summary>
        /// <value><c>true</c> if [no connection]; otherwise, <c>false</c>.</value>
        /// <remarks>The difference between this and <see cref="PreviewOnly" /> is, that
        /// the preview-only mode uses the connection to determine the current
        /// state of the database.</remarks>
        bool NoConnection { get; set; }

        /// <summary>
        /// Gets or sets the number of versions to apply
        /// </summary>
        /// <value>The steps.</value>
        /// <remarks>The option is now stored in <see cref="RunnerOptions.Steps" />.</remarks>
        int Steps { get; set; }

        /// <summary>
        /// Gets or sets the working directory
        /// </summary>
        /// <value>The working directory.</value>
        string WorkingDirectory { get; set; }

        /// <summary>
        /// Gets or sets the profile migrations to apply
        /// </summary>
        /// <value>The profile.</value>
        /// <remarks>The option is now stored in <see cref="RunnerOptions.Profile" />.</remarks>
        string Profile { get; set; }

        /// <summary>
        /// Gets or sets the announcer to use
        /// </summary>
        /// <value>The announcer.</value>
        IAnnouncer Announcer { get; }

        /// <summary>
        /// Gets or sets the stopwatch to use
        /// </summary>
        /// <value>The stop watch.</value>
        IStopWatch StopWatch { get; }

        /// <summary>
        /// Gets or sets the default command timeout in seconds
        /// </summary>
        /// <value>The timeout.</value>
        int? Timeout { get; set; }

        /// <summary>
        /// Gets or sets the path to an app.config/web.config to load the connection string from
        /// </summary>
        /// <value>The connection string configuration path.</value>
        string ConnectionStringConfigPath { get; set; }

        /// <summary>
        /// Gets or sets the tags the migrations must match
        /// </summary>
        /// <value>The tags.</value>
        /// <remarks>All migrations are matched when no tags were specified.
        /// The option is now stored in <see cref="RunnerOptions.Tags" />.</remarks>
        IEnumerable<string> Tags { get; set; }

        /// <summary>
        /// Gets or sets the provider switches
        /// </summary>
        /// <value>The provider switches.</value>
        /// <remarks>The option is now stored in <see cref="ProcessorOptions.PreviewOnly" />.</remarks>
        string ProviderSwitches { get; set; }

        /// <summary>
        /// Use one transaction for the whole session
        /// </summary>
        /// <value><c>true</c> if [transaction per session]; otherwise, <c>false</c>.</value>
        /// <remarks>The default transaction behavior is to use one transaction per migration.
        /// The option is now stored in <see cref="RunnerOptions.TransactionPerSession" />.</remarks>
        bool TransactionPerSession { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the migration runner is allowed to apply breaking changes
        /// </summary>
        /// <value><c>true</c> if [allow breaking change]; otherwise, <c>false</c>.</value>
        /// <remarks>The option is now stored in <see cref="RunnerOptions.AllowBreakingChange" />.</remarks>
        bool AllowBreakingChange { get; set; }

        /// <summary>
        /// Gets or sets the arbitrary application context passed to the task runner
        /// </summary>
        /// <value>The application context.</value>
        object ApplicationContext { get; set; }

        /// <summary>
        /// Gets or sets the default schema name
        /// </summary>
        /// <value>The default name of the schema.</value>
        /// <remarks>The default schema name must be set using the <see cref="IConventionSet.SchemaConvention" /></remarks>
        string DefaultSchemaName { get;set; }

        /// <summary>
        /// Gets or sets a value indicating whether the comments should be stripped
        /// </summary>
        /// <value><c>true</c> if [strip comments]; otherwise, <c>false</c>.</value>
        bool StripComments { get; set; }
    }
}
