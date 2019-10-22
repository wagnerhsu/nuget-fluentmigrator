// ***********************************************************************
// Assembly         : FluentMigrator.Runner
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="MigrationConventions.cs" company="FluentMigrator Project">
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
using FluentMigrator.Runner.Infrastructure;

namespace FluentMigrator.Runner
{
    /// <summary>
    /// Class MigrationRunnerConventions.
    /// Implements the <see cref="FluentMigrator.Runner.IMigrationRunnerConventions" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.IMigrationRunnerConventions" />
    public class MigrationRunnerConventions : IMigrationRunnerConventions
    {
        /// <summary>
        /// The default
        /// </summary>
        private static readonly IMigrationRunnerConventions _default = DefaultMigrationRunnerConventions.Instance;

        /// <summary>
        /// Gets or sets the type is migration.
        /// </summary>
        /// <value>The type is migration.</value>
        public Func<Type, bool> TypeIsMigration { get; set; }
        /// <summary>
        /// Gets or sets the type is profile.
        /// </summary>
        /// <value>The type is profile.</value>
        public Func<Type, bool> TypeIsProfile { get; set; }
        /// <summary>
        /// Gets or sets the get maintenance stage.
        /// </summary>
        /// <value>The get maintenance stage.</value>
        public Func<Type, MigrationStage?> GetMaintenanceStage { get; set; }
        /// <summary>
        /// Gets or sets the type is version table meta data.
        /// </summary>
        /// <value>The type is version table meta data.</value>
        public Func<Type, bool> TypeIsVersionTableMetaData { get; set; }

        /// <summary>
        /// Gets or sets the get migration information.
        /// </summary>
        /// <value>The get migration information.</value>
        [Obsolete]
        public Func<Type, IMigrationInfo> GetMigrationInfo { get; set; }

        /// <inheritdoc />
        public Func<IMigration, IMigrationInfo> GetMigrationInfoForMigration { get; }
        /// <summary>
        /// Gets or sets the type has tags.
        /// </summary>
        /// <value>The type has tags.</value>
        public Func<Type, bool> TypeHasTags { get; set; }
        /// <summary>
        /// Gets or sets the type has matching tags.
        /// </summary>
        /// <value>The type has matching tags.</value>
        public Func<Type, IEnumerable<string>, bool> TypeHasMatchingTags { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationRunnerConventions"/> class.
        /// </summary>
        public MigrationRunnerConventions()
        {
            TypeIsMigration = _default.TypeIsMigration;
            TypeIsVersionTableMetaData = _default.TypeIsVersionTableMetaData;
#pragma warning disable 612
            GetMigrationInfo = _default.GetMigrationInfo;
#pragma warning restore 612
            TypeIsProfile = _default.TypeIsProfile;
            GetMaintenanceStage = _default.GetMaintenanceStage;
            GetMigrationInfoForMigration = _default.GetMigrationInfoForMigration;
            TypeHasTags = _default.TypeHasTags;
            TypeHasMatchingTags = _default.TypeHasMatchingTags;
        }
    }
}
