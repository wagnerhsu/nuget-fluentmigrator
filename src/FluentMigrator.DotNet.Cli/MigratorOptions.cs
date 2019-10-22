// ***********************************************************************
// Assembly         : FluentMigrator.DotNet.Cli
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="MigratorOptions.cs" company="FluentMigrator Project">
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

using System.Collections.Generic;
using System.Linq;

using FluentMigrator.DotNet.Cli.Commands;

namespace FluentMigrator.DotNet.Cli
{
    /// <summary>
    /// Class MigratorOptions.
    /// </summary>
    public class MigratorOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MigratorOptions"/> class.
        /// </summary>
        public MigratorOptions()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MigratorOptions"/> class.
        /// </summary>
        /// <param name="task">The task.</param>
        private MigratorOptions(string task)
        {
            Task = task;
        }

        /// <summary>
        /// Gets the task.
        /// </summary>
        /// <value>The task.</value>
        public string Task { get; }
        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString { get; private set; }
        /// <summary>
        /// Gets the type of the processor.
        /// </summary>
        /// <value>The type of the processor.</value>
        public string ProcessorType { get; private set; }
        /// <summary>
        /// Gets the processor switches.
        /// </summary>
        /// <value>The processor switches.</value>
        public string ProcessorSwitches { get; private set; }
        /// <summary>
        /// Gets the target assemblies.
        /// </summary>
        /// <value>The target assemblies.</value>
        public IReadOnlyCollection<string> TargetAssemblies { get; private set; }
        /// <summary>
        /// Gets the target version.
        /// </summary>
        /// <value>The target version.</value>
        public long? TargetVersion { get; private set; }
        /// <summary>
        /// Gets the steps.
        /// </summary>
        /// <value>The steps.</value>
        public int? Steps { get; private set; }
        /// <summary>
        /// Gets the namespace.
        /// </summary>
        /// <value>The namespace.</value>
        public string Namespace { get; private set; }
        /// <summary>
        /// Gets a value indicating whether [nested namespaces].
        /// </summary>
        /// <value><c>true</c> if [nested namespaces]; otherwise, <c>false</c>.</value>
        public bool NestedNamespaces { get; private set; }
        /// <summary>
        /// Gets the start version.
        /// </summary>
        /// <value>The start version.</value>
        public long? StartVersion { get; private set; }
        /// <summary>
        /// Gets a value indicating whether [no connection].
        /// </summary>
        /// <value><c>true</c> if [no connection]; otherwise, <c>false</c>.</value>
        public bool NoConnection { get; private set; }
        /// <summary>
        /// Gets the working directory.
        /// </summary>
        /// <value>The working directory.</value>
        public string WorkingDirectory { get; private set; }
        /// <summary>
        /// Gets the tags.
        /// </summary>
        /// <value>The tags.</value>
        public IEnumerable<string> Tags { get; private set; }
        /// <summary>
        /// Gets a value indicating whether this <see cref="MigratorOptions"/> is preview.
        /// </summary>
        /// <value><c>true</c> if preview; otherwise, <c>false</c>.</value>
        public bool Preview { get; private set; }
        /// <summary>
        /// Gets a value indicating whether this <see cref="MigratorOptions"/> is verbose.
        /// </summary>
        /// <value><c>true</c> if verbose; otherwise, <c>false</c>.</value>
        public bool Verbose { get; private set; }
        /// <summary>
        /// Gets the profile.
        /// </summary>
        /// <value>The profile.</value>
        public string Profile { get; private set; }
        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        public string Context { get; private set; }
        /// <summary>
        /// Gets the timeout.
        /// </summary>
        /// <value>The timeout.</value>
        public int? Timeout { get; private set; }
        /// <summary>
        /// Gets the transaction mode.
        /// </summary>
        /// <value>The transaction mode.</value>
        public TransactionMode TransactionMode { get; private set; }
        /// <summary>
        /// Gets a value indicating whether this <see cref="MigratorOptions"/> is output.
        /// </summary>
        /// <value><c>true</c> if output; otherwise, <c>false</c>.</value>
        public bool Output { get; private set; }
        /// <summary>
        /// Gets the name of the output file.
        /// </summary>
        /// <value>The name of the output file.</value>
        public string OutputFileName { get; private set; }
        /// <summary>
        /// Gets a value indicating whether [allow breaking changes].
        /// </summary>
        /// <value><c>true</c> if [allow breaking changes]; otherwise, <c>false</c>.</value>
        public bool AllowBreakingChanges { get; private set; }
        /// <summary>
        /// Gets the name of the schema.
        /// </summary>
        /// <value>The name of the schema.</value>
        public string SchemaName { get; private set; }
        /// <summary>
        /// Gets a value indicating whether [strip comments].
        /// </summary>
        /// <value><c>true</c> if [strip comments]; otherwise, <c>false</c>.</value>
        public bool StripComments { get; private set; }
        /// <summary>
        /// Gets a value indicating whether [include untagged maintenances].
        /// </summary>
        /// <value><c>true</c> if [include untagged maintenances]; otherwise, <c>false</c>.</value>
        public bool IncludeUntaggedMaintenances { get; private set; }
        /// <summary>
        /// Gets a value indicating whether [include untagged migrations].
        /// </summary>
        /// <value><c>true</c> if [include untagged migrations]; otherwise, <c>false</c>.</value>
        public bool IncludeUntaggedMigrations { get; private set; } = true;

        /// <summary>
        /// Creates the list migrations.
        /// </summary>
        /// <param name="cmd">The command.</param>
        /// <returns>MigratorOptions.</returns>
        public static MigratorOptions CreateListMigrations(ListMigrations cmd)
        {
            var result = new MigratorOptions("listmigrations")
                .Init(cmd);
            return result;
        }

        /// <summary>
        /// Creates the migrate up.
        /// </summary>
        /// <param name="cmd">The command.</param>
        /// <param name="targetVersion">The target version.</param>
        /// <returns>MigratorOptions.</returns>
        public static MigratorOptions CreateMigrateUp(Migrate cmd, long? targetVersion = null)
        {
            var result = new MigratorOptions("migrate:up")
                .Init(cmd);
            result.TargetVersion = targetVersion;
            result.TransactionMode = cmd.TransactionMode;
            return result;
        }

        /// <summary>
        /// Creates the migrate down.
        /// </summary>
        /// <param name="cmd">The command.</param>
        /// <returns>MigratorOptions.</returns>
        public static MigratorOptions CreateMigrateDown(MigrateDown cmd)
        {
            var result = new MigratorOptions("migrate:down")
                .Init(cmd.Parent);
            result.TargetVersion = cmd.TargetVersion;
            result.TransactionMode = cmd.Parent.TransactionMode;
            return result;
        }

        /// <summary>
        /// Creates the rollback by.
        /// </summary>
        /// <param name="cmd">The command.</param>
        /// <param name="steps">The steps.</param>
        /// <returns>MigratorOptions.</returns>
        public static MigratorOptions CreateRollbackBy(Rollback cmd, int? steps)
        {
            var result = new MigratorOptions("rollback")
                .Init(cmd);
            result.Steps = steps;
            result.TransactionMode = cmd.TransactionMode;
            return result;
        }

        /// <summary>
        /// Creates the rollback to.
        /// </summary>
        /// <param name="cmd">The command.</param>
        /// <returns>MigratorOptions.</returns>
        public static MigratorOptions CreateRollbackTo(RollbackTo cmd)
        {
            var result = new MigratorOptions("rollback:toversion")
                .Init(cmd.Parent);
            result.TargetVersion = cmd.Version;
            result.TransactionMode = cmd.Parent.TransactionMode;
            return result;
        }

        /// <summary>
        /// Creates the rollback all.
        /// </summary>
        /// <param name="cmd">The command.</param>
        /// <returns>MigratorOptions.</returns>
        public static MigratorOptions CreateRollbackAll(RollbackAll cmd)
        {
            var result = new MigratorOptions("rollback:all")
                .Init(cmd.Parent);
            result.TransactionMode = cmd.Parent.TransactionMode;
            return result;
        }

        /// <summary>
        /// Creates the validate version order.
        /// </summary>
        /// <param name="cmd">The command.</param>
        /// <returns>MigratorOptions.</returns>
        public static MigratorOptions CreateValidateVersionOrder(ValidateVersionOrder cmd)
        {
            return new MigratorOptions("validateversionorder")
                .Init(cmd);
        }

        /// <summary>
        /// Initializes the specified command.
        /// </summary>
        /// <param name="cmd">The command.</param>
        /// <returns>MigratorOptions.</returns>
        private MigratorOptions Init(ConnectionCommand cmd)
        {
            ConnectionString = cmd.ConnectionString;
            NoConnection = cmd.NoConnection;
            ProcessorType = cmd.ProcessorType;
            ProcessorSwitches = cmd.ProcessorSwitches;
            Preview = cmd.Preview;
            Verbose = cmd.Verbose;
            Profile = cmd.Profile;
            Context = cmd.Context;
            Timeout = cmd.Timeout;
            StripComments = !cmd.StripComments.hasValue || (cmd.StripComments.value ?? true);
            (Output, OutputFileName) = cmd.Output;
            return Init((MigrationCommand)cmd);
        }

        /// <summary>
        /// Initializes the specified command.
        /// </summary>
        /// <param name="cmd">The command.</param>
        /// <returns>MigratorOptions.</returns>
        private MigratorOptions Init(MigrationCommand cmd)
        {
            TargetAssemblies = cmd.TargetAssemblies.ToList();
            Namespace = cmd.Namespace;
            NestedNamespaces = cmd.NestedNamespaces;
            StartVersion = cmd.StartVersion;
            WorkingDirectory = cmd.WorkingDirectory;
            Tags = cmd.Tags?.ToList() ?? new List<string>();
            AllowBreakingChanges = cmd.AllowBreakingChanges;
            SchemaName = cmd.SchemaName;
            IncludeUntaggedMigrations = !cmd.IncludeUntaggedMigrations.hasValue || (cmd.IncludeUntaggedMigrations.value ?? true);
            IncludeUntaggedMaintenances = cmd.IncludeUntaggedMaintenances;
            return this;
        }
    }
}
