// ***********************************************************************
// Assembly         : FluentMigrator.DotNet.Cli
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="MigrationCommand.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
// Copyright (c) 2007-2018, Sean Chambers and the FluentMigrator Project
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

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using McMaster.Extensions.CommandLineUtils;

// ReSharper disable UnassignedGetOnlyAutoProperty

namespace FluentMigrator.DotNet.Cli.Commands
{
    /// <summary>
    /// Class MigrationCommand.
    /// Implements the <see cref="FluentMigrator.DotNet.Cli.Commands.BaseCommand" />
    /// </summary>
    /// <seealso cref="FluentMigrator.DotNet.Cli.Commands.BaseCommand" />
    public class MigrationCommand : BaseCommand
    {
        /// <summary>
        /// Gets the target assemblies.
        /// </summary>
        /// <value>The target assemblies.</value>
        [Option("-a|--assembly <ASSEMBLY_NAME>", Description = "The assemblies containing the migrations you want to execute.")]
        [Required]
        public IEnumerable<string> TargetAssemblies { get; }

        /// <summary>
        /// Gets the namespace.
        /// </summary>
        /// <value>The namespace.</value>
        [Option("-n|--namespace <NAMESPACE>", Description = "The namespace contains the migrations you want to run. Default is all migrations found within the Target Assembly will be run.")]
        public string Namespace { get; }

        /// <summary>
        /// Gets a value indicating whether [nested namespaces].
        /// </summary>
        /// <value><c>true</c> if [nested namespaces]; otherwise, <c>false</c>.</value>
        [Option("--nested", Description = "Whether migrations in nested namespaces should be included. Used in conjunction with the namespace option.")]
        public bool NestedNamespaces { get; }

        /// <summary>
        /// Gets the start version.
        /// </summary>
        /// <value>The start version.</value>
        [Option("--start-version", Description = "The specific version to start migrating from. Only used when NoConnection is true. Default is 0.")]
        public long? StartVersion { get; }

        /// <summary>
        /// Gets the working directory.
        /// </summary>
        /// <value>The working directory.</value>
        [Option("--working-directory <WORKING_DIRECTORY>", Description = "The directory to load SQL scripts specified by migrations from.")]
        public string WorkingDirectory { get; }

        /// <summary>
        /// Gets the tags.
        /// </summary>
        /// <value>The tags.</value>
        [Option("-t|--tag", Description = "Filters the migrations to be run by tag.")]
        public IEnumerable<string> Tags { get; }

        /// <summary>
        /// Gets a value indicating whether [allow breaking changes].
        /// </summary>
        /// <value><c>true</c> if [allow breaking changes]; otherwise, <c>false</c>.</value>
        [Option("-b|--allow-breaking-changes", Description = "Allows execution of migrations marked as breaking changes.")]
        public bool AllowBreakingChanges { get; }

        /// <summary>
        /// Gets the name of the schema.
        /// </summary>
        /// <value>The name of the schema.</value>
        [Option("--default-schema-name", Description = "Set default schema name for VersionInfo table and the migrations.")]
        public string SchemaName { get; internal set; } = null;

        /// <summary>
        /// Gets or sets the strip comments.
        /// </summary>
        /// <value>The strip comments.</value>
        [Option("--strip", "Strip comments from the SQL scripts. Default is true.", CommandOptionType.SingleOrNoValue)]
        public (bool hasValue, bool? value) StripComments { get; set; }

        /// <summary>
        /// Gets or sets the include untagged migrations.
        /// </summary>
        /// <value>The include untagged migrations.</value>
        [Option("--include-untagged-migrations", "Include untagged migrations.", CommandOptionType.SingleOrNoValue)]
        public (bool hasValue, bool? value) IncludeUntaggedMigrations { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [include untagged maintenances].
        /// </summary>
        /// <value><c>true</c> if [include untagged maintenances]; otherwise, <c>false</c>.</value>
        [Option("--include-untagged-maintenances", Description = "Include untagged maintenances.")]
        public bool IncludeUntaggedMaintenances { get; set; }
    }
}
