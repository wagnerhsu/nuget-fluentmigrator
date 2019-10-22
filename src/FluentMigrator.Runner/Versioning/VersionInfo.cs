// ***********************************************************************
// Assembly         : FluentMigrator.Runner
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="VersionInfo.cs" company="FluentMigrator Project">
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

using System.Collections.Generic;
using System.Linq;

namespace FluentMigrator.Runner.Versioning
{
    /// <summary>
    /// Class VersionInfo.
    /// Implements the <see cref="FluentMigrator.Runner.Versioning.IVersionInfo" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Versioning.IVersionInfo" />
    public class VersionInfo : IVersionInfo
    {
        /// <summary>
        /// The versions applied
        /// </summary>
        private IList<long> _versionsApplied = new List<long>();

        /// <summary>
        /// Gets the version number of the latest migration that has been applied
        /// </summary>
        /// <returns>The version number</returns>
        public long Latest()
        {
            return _versionsApplied.OrderByDescending(x => x).FirstOrDefault();
        }

        /// <summary>
        /// Adds a migration version number as applied
        /// </summary>
        /// <param name="migration">The version number</param>
        public void AddAppliedMigration(long migration)
        {
            _versionsApplied.Add(migration);
        }

        /// <summary>
        /// Returns a value indicating whether a migration with the given version number has been applied
        /// </summary>
        /// <param name="migration">The migration version number to validate</param>
        /// <returns><c>true</c> when the migration with the given version number has been applied</returns>
        public bool HasAppliedMigration(long migration)
        {
            return _versionsApplied.Contains(migration);
        }

        /// <summary>
        /// Applieds the migrations.
        /// </summary>
        /// <returns>IEnumerable&lt;System.Int64&gt;.</returns>
        public IEnumerable<long> AppliedMigrations()
        {
            return _versionsApplied.OrderByDescending(x => x).AsEnumerable();
        }
    }
}
