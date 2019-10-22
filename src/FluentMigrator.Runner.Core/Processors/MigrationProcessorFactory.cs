// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Core
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="MigrationProcessorFactory.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
// Copyright (c) 2007-2018, Fluent Migrator Project
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

namespace FluentMigrator.Runner.Processors
{
    /// <summary>
    /// Class MigrationProcessorFactory.
    /// Implements the <see cref="FluentMigrator.Runner.Processors.IMigrationProcessorFactory" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Processors.IMigrationProcessorFactory" />
    [Obsolete]
    public abstract class MigrationProcessorFactory : IMigrationProcessorFactory
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public virtual string Name => GetType().Name.Replace("ProcessorFactory", string.Empty);

        /// <summary>
        /// Creates the specified connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="announcer">The announcer.</param>
        /// <param name="options">The options.</param>
        /// <returns>IMigrationProcessor.</returns>
        [Obsolete]
        public abstract IMigrationProcessor Create(string connectionString, IAnnouncer announcer, IMigrationProcessorOptions options);

        /// <summary>
        /// Determines whether [is for provider] [the specified provider].
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <returns><c>true</c> if [is for provider] [the specified provider]; otherwise, <c>false</c>.</returns>
        [Obsolete]
        public virtual bool IsForProvider(string provider)
        {
            return provider.IndexOf(Name, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
