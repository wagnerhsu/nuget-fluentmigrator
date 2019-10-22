// ***********************************************************************
// Assembly         : FluentMigrator.Runner
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="MigrationConstraintContext.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
//
// Copyright (c) 2019, Fluent Migrator Project
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
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Versioning;

namespace FluentMigrator.Runner.Constraints
{
    /// <summary>
    /// Contextual information about the context in which runner will determinate whether a constrained Migration should be run.
    /// </summary>
    /// <seealso cref="MigrationConstraintAttribute" />
    public class MigrationConstraintContext
    {
        /// <summary>
        /// Runner options under which current migration run is started
        /// </summary>
        /// <value>The runner options.</value>
        public RunnerOptions RunnerOptions { get; set; }
        /// <summary>
        /// Provides information about the current state of target database
        /// </summary>
        /// <value>The version information.</value>
        public IVersionInfo VersionInfo { get; set; }
    }
}
