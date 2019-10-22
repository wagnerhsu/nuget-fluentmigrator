// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="HanaTestDropPrimaryKey.cs" company="FluentMigrator Project">
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

using NUnit.Framework;

namespace FluentMigrator.Tests.Integration.Processors.Hana.EndToEnd
{
    /// <summary>
    /// Defines test class TestRollbackColumnCreation.
    /// Implements the <see cref="FluentMigrator.Tests.Integration.Processors.Hana.EndToEnd.HanaEndToEndFixture" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Integration.Processors.Hana.EndToEnd.HanaEndToEndFixture" />
    [TestFixture]
    [Category("Integration")]
    [Category("Hana")]
    public class TestRollbackColumnCreation : HanaEndToEndFixture
    {
        /// <summary>
        /// Sets up.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            if (!IntegrationTestOptions.Hana.IsEnabled)
                Assert.Ignore();
        }

        /// <summary>
        /// Defines the test method Delete_ColumnCreateOnTableWithExplicitPk_ColumnShouldBeDropped.
        /// </summary>
        [Test]
        public void Delete_ColumnCreateOnTableWithExplicitPk_ColumnShouldBeDropped()
        {
            DeleteTableIfExists("Teste", "Teste1");
            Migrate(typeof(ImplicitlyCreatedFkForHana.CreateImplicitFk).Namespace);
        }

        /// <summary>
        /// Deletes the table if exists.
        /// </summary>
        /// <param name="tableNames">The table names.</param>
        private void DeleteTableIfExists(params string[] tableNames)
        {
            using (var sc = new ScopedConnection())
            {
                foreach (var tableName in tableNames)
                {
                    if (sc.Processor.TableExists(null, tableName))
                        sc.Processor.Execute("DROP TABLE \"{0}\"", tableName);
                }
            }
        }
    }

    namespace ImplicitlyCreatedFkForHana
    {
        /// <summary>
        /// Class CreateImplicitFk.
        /// Implements the <see cref="FluentMigrator.Migration" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Migration" />
        [Migration(1)]
        public class CreateImplicitFk : Migration
        {
            /// <summary>
            /// Collect the UP migration expressions
            /// </summary>
            public override void Up()
            {

                Create.Table("Teste1")
                    .WithColumn("Id").AsInt32().PrimaryKey("PK_TST").Identity()
                    .WithColumn("Nome").AsString(100);

                Create.Table("Teste")
                    .WithColumn("Id").AsInt32().PrimaryKey()
                    .ForeignKey("Teste1", "Id")
                    .WithColumn("Nome").AsString();

                Delete.PrimaryKey("").FromTable("Teste");
            }

            /// <summary>
            /// Downs this instance.
            /// </summary>
            public override void Down()
            {
                Delete.Table("Teste");
                Delete.Table("Teste1");
            }
        }
    }
}
