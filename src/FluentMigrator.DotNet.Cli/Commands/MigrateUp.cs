// ***********************************************************************
// Assembly         : FluentMigrator.DotNet.Cli
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="MigrateUp.cs" company="FluentMigrator Project">
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

using McMaster.Extensions.CommandLineUtils;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnassignedGetOnlyAutoProperty

namespace FluentMigrator.DotNet.Cli.Commands
{
    /// <summary>
    /// Class MigrateUp.
    /// Implements the <see cref="FluentMigrator.DotNet.Cli.Commands.BaseCommand" />
    /// </summary>
    /// <seealso cref="FluentMigrator.DotNet.Cli.Commands.BaseCommand" />
    [HelpOption]
    [Command("up", Description = "Apply migrations")]
    public class MigrateUp : BaseCommand
    {
        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public Migrate Parent { get; }

        /// <summary>
        /// Gets the target version.
        /// </summary>
        /// <value>The target version.</value>
        [Option("-t|--target <TARGET_VERSION>", Description = "The specific version to migrate.")]
        public long? TargetVersion { get; }

        /// <summary>
        /// Called when [execute].
        /// </summary>
        /// <param name="console">The console.</param>
        /// <returns>System.Int32.</returns>
        private int OnExecute(IConsole console)
        {
            var options = MigratorOptions.CreateMigrateUp(Parent, TargetVersion);
            return ExecuteMigrations(options, console);
        }
    }
}
