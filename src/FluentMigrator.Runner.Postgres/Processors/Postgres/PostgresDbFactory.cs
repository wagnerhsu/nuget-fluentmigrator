// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Postgres
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="PostgresDbFactory.cs" company="FluentMigrator Project">
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

namespace FluentMigrator.Runner.Processors.Postgres
{
    /// <summary>
    /// Class PostgresDbFactory.
    /// Implements the <see cref="FluentMigrator.Runner.Processors.ReflectionBasedDbFactory" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Processors.ReflectionBasedDbFactory" />
    public class PostgresDbFactory : ReflectionBasedDbFactory
    {
        /// <summary>
        /// The entries
        /// </summary>
        private static readonly TestEntry[] _entries =
        {
            new TestEntry("Npgsql", "Npgsql.NpgsqlFactory"),
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgresDbFactory"/> class.
        /// </summary>
        [Obsolete]
        public PostgresDbFactory()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgresDbFactory"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public PostgresDbFactory(IServiceProvider serviceProvider)
            : base(serviceProvider, _entries)
        {
        }
    }
}
