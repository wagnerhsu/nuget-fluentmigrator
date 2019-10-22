// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="ConventionSets.cs" company="FluentMigrator Project">
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

using FluentMigrator.Runner.Conventions;

namespace FluentMigrator.Tests
{
    /// <summary>
    /// Class ConventionSets.
    /// </summary>
    public static class ConventionSets
    {
        /// <summary>
        /// The no schema name convention
        /// </summary>
        private static readonly DefaultSchemaConvention _noSchemaNameConvention = new DefaultSchemaConvention();
        /// <summary>
        /// The test schema name convention
        /// </summary>
        private static readonly DefaultSchemaConvention _testSchemaNameConvention = new DefaultSchemaConvention("testdefault");

        /// <summary>
        /// The no schema name
        /// </summary>
        public static readonly IConventionSet NoSchemaName = CreateNoSchemaName(null);

        /// <summary>
        /// The with schema name
        /// </summary>
        public static readonly IConventionSet WithSchemaName = CreateTestSchemaName(null);

        /// <summary>
        /// Creates the name of the no schema.
        /// </summary>
        /// <param name="rootPath">The root path.</param>
        /// <returns>IConventionSet.</returns>
        public static IConventionSet CreateNoSchemaName(string rootPath)
            => Create(_noSchemaNameConvention, rootPath);

        /// <summary>
        /// Creates the name of the test schema.
        /// </summary>
        /// <param name="rootPath">The root path.</param>
        /// <returns>IConventionSet.</returns>
        public static IConventionSet CreateTestSchemaName(string rootPath)
            => Create(_testSchemaNameConvention, rootPath);

        /// <summary>
        /// Creates the specified schema convention.
        /// </summary>
        /// <param name="schemaConvention">The schema convention.</param>
        /// <param name="rootPath">The root path.</param>
        /// <returns>IConventionSet.</returns>
        public static IConventionSet Create(DefaultSchemaConvention schemaConvention, string rootPath)
        {
            return new ConventionSet
            {
                SchemaConvention = schemaConvention,
                RootPathConvention = new DefaultRootPathConvention(rootPath),
                ConstraintConventions =
                {
                    new DefaultConstraintNameConvention(),
                    schemaConvention,
                },
                ColumnsConventions =
                {
                    new DefaultPrimaryKeyNameConvention(),
                },
                ForeignKeyConventions =
                {
                    new DefaultForeignKeyNameConvention(),
                    schemaConvention,
                },
                IndexConventions =
                {
                    new DefaultIndexNameConvention(),
                    schemaConvention,
                },
                SequenceConventions =
                {
                    schemaConvention,
                },
                AutoNameConventions =
                {
                    new DefaultAutoNameConvention(),
                }
            };
        }
    }
}
