// ***********************************************************************
// Assembly         : FluentMigrator.Runner.SqlAnywhere
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SqlAnywhere16ProcessorFactory.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
// Copyright (c) 2007-2018, FluentMigrator Project
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

using FluentMigrator.Runner.Generators.SqlAnywhere;

namespace FluentMigrator.Runner.Processors.SqlAnywhere
{
    /// <summary>
    /// Class SqlAnywhere16ProcessorFactory.
    /// Implements the <see cref="FluentMigrator.Runner.Processors.MigrationProcessorFactory" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Processors.MigrationProcessorFactory" />
    [Obsolete]
    public class SqlAnywhere16ProcessorFactory : MigrationProcessorFactory
    {
        /// <summary>
        /// Creates the specified connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="announcer">The announcer.</param>
        /// <param name="options">The options.</param>
        /// <returns>IMigrationProcessor.</returns>
        [Obsolete]
        public override IMigrationProcessor Create(string connectionString, IAnnouncer announcer, IMigrationProcessorOptions options)
        {
            var factory = new SqlAnywhereDbFactory();
            var connection = factory.CreateConnection(connectionString);
            return new SqlAnywhereProcessor("SqlAnywhere16", connection, new SqlAnywhere16Generator(new SqlAnywhereQuoter()), announcer, options, factory);
        }

        /// <summary>
        /// Determines whether [is for provider] [the specified provider].
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <returns><c>true</c> if [is for provider] [the specified provider]; otherwise, <c>false</c>.</returns>
        [Obsolete]
        public override bool IsForProvider(string provider)
        {
            return provider.ToLower().Contains("ianywhere");
        }
    }
}
