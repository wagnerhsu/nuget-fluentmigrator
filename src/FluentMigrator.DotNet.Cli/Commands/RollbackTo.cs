// ***********************************************************************
// Assembly         : FluentMigrator.DotNet.Cli
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="RollbackTo.cs" company="FluentMigrator Project">
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

using System.ComponentModel.DataAnnotations;

using McMaster.Extensions.CommandLineUtils;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnassignedGetOnlyAutoProperty

namespace FluentMigrator.DotNet.Cli.Commands
{
    /// <summary>
    /// Class RollbackTo.
    /// Implements the <see cref="FluentMigrator.DotNet.Cli.Commands.BaseCommand" />
    /// </summary>
    /// <seealso cref="FluentMigrator.DotNet.Cli.Commands.BaseCommand" />
    [HelpOption]
    [Command("to", Description = "Rollback migrations up to a given version")]
    public class RollbackTo : BaseCommand
    {
        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public Rollback Parent { get; }

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <value>The version.</value>
        [Argument(0, "version", "The target version to rollback to.")]
        [Required]
        public long Version { get; }

        /// <summary>
        /// Called when [execute].
        /// </summary>
        /// <param name="console">The console.</param>
        /// <returns>System.Int32.</returns>
        private int OnExecute(IConsole console)
        {
            var options = MigratorOptions.CreateRollbackTo(this);
            return ExecuteMigrations(options, console);
        }
    }
}
