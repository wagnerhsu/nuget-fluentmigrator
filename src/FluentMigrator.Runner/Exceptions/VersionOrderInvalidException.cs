// ***********************************************************************
// Assembly         : FluentMigrator.Runner
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="VersionOrderInvalidException.cs" company="FluentMigrator Project">
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

using System;
using System.Collections.Generic;
using System.Linq;

using FluentMigrator.Infrastructure;

namespace FluentMigrator.Runner.Exceptions
{
    /// <summary>
    /// Class VersionOrderInvalidException.
    /// Implements the <see cref="FluentMigrator.Runner.Exceptions.RunnerException" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Exceptions.RunnerException" />
    public class VersionOrderInvalidException : RunnerException
    {
        /// <summary>
        /// The invalid migrations
        /// </summary>
        private IReadOnlyCollection<KeyValuePair<long, IMigrationInfo>> _invalidMigrations;

        /// <summary>
        /// Gets or sets the invalid migrations.
        /// </summary>
        /// <value>The invalid migrations.</value>
        public IEnumerable<KeyValuePair<long, IMigrationInfo>> InvalidMigrations
        {
            get => _invalidMigrations;
            set => _invalidMigrations = value.ToList();
        }

        /// <summary>
        /// Gets the invalid versions.
        /// </summary>
        /// <value>The invalid versions.</value>
        public IEnumerable<long> InvalidVersions => _invalidMigrations.Select(x => x.Key);

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionOrderInvalidException"/> class.
        /// </summary>
        /// <param name="invalidMigrations">The invalid migrations.</param>
        public VersionOrderInvalidException(IEnumerable<KeyValuePair<long, IMigrationInfo>> invalidMigrations)
        {
            _invalidMigrations = invalidMigrations.ToList();
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>The message.</value>
        public override string Message
        {
            get
            {
                var result = "Unapplied migrations have version numbers that are less than the greatest version number of applied migrations:";

                foreach (var pair in InvalidMigrations)
                {
                    result = result + string.Format("{0}{1} - {2}", Environment.NewLine, pair.Key, pair.Value.Migration.GetType().Name);
                }

                return result;
            }
        }
    }
}
