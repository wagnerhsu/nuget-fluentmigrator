// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Core
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="IMigrationRunnerConventions.cs" company="FluentMigrator Project">
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

using FluentMigrator.Infrastructure;

namespace FluentMigrator.Runner
{
    /// <summary>
    /// Interface IMigrationRunnerConventions
    /// </summary>
    public interface IMigrationRunnerConventions
    {
        /// <summary>
        /// Gets the type is migration.
        /// </summary>
        /// <value>The type is migration.</value>
        Func<Type, bool> TypeIsMigration { get; }

        /// <summary>
        /// Gets the type is profile.
        /// </summary>
        /// <value>The type is profile.</value>
        Func<Type, bool> TypeIsProfile { get; }

        /// <summary>
        /// Gets the get maintenance stage.
        /// </summary>
        /// <value>The get maintenance stage.</value>
        Func<Type, MigrationStage?> GetMaintenanceStage { get; }

        /// <summary>
        /// Gets the type is version table meta data.
        /// </summary>
        /// <value>The type is version table meta data.</value>
        Func<Type, bool> TypeIsVersionTableMetaData { get; }

        /// <summary>
        /// Gets the get migration information.
        /// </summary>
        /// <value>The get migration information.</value>
        [Obsolete]
        Func<Type, IMigrationInfo> GetMigrationInfo { get; }

        /// <summary>
        /// Create an <see cref="IMigrationInfo" /> instance for a given <see cref="IMigration" />
        /// </summary>
        /// <value>The get migration information for migration.</value>
        Func<IMigration, IMigrationInfo> GetMigrationInfoForMigration { get; }

        /// <summary>
        /// Gets the type has tags.
        /// </summary>
        /// <value>The type has tags.</value>
        Func<Type, bool> TypeHasTags { get; }

        /// <summary>
        /// Gets the type has matching tags.
        /// </summary>
        /// <value>The type has matching tags.</value>
        Func<Type, IEnumerable<string>, bool> TypeHasMatchingTags { get; }
    }
}
