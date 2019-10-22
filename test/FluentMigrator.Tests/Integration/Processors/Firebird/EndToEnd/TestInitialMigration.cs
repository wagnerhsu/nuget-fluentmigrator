// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="TestInitialMigration.cs" company="FluentMigrator Project">
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

using FluentMigrator.Tests.Integration.Processors.Firebird.EndToEnd.SimpleMigration;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Integration.Processors.Firebird.EndToEnd
{
    namespace SimpleMigration
    {
        /// <summary>
        /// Class VersionOneSimpleTableMigration.
        /// Implements the <see cref="FluentMigrator.Migration" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Migration" />
        [Migration(1)]
        public class VersionOneSimpleTableMigration : Migration
        {
            /// <summary>
            /// Collect the UP migration expressions
            /// </summary>
            public override void Up()
            {
                Create.Table("SIMPLE")
                    .WithColumn("ID").AsInt32().PrimaryKey()
                    .WithColumn("COL_STR").AsString(10);
            }

            /// <summary>
            /// Collects the DOWN migration expressions
            /// </summary>
            public override void Down()
            {
                Delete.Table("SIMPLE");
            }
        }
    }

    /// <summary>
    /// Defines test class TestInitialMigration.
    /// Implements the <see cref="FluentMigrator.Tests.Integration.Processors.Firebird.EndToEnd.FbEndToEndFixture" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Integration.Processors.Firebird.EndToEnd.FbEndToEndFixture" />
    [TestFixture]
    [Category("Integration")]
    [Category("Firebird")]
    public class TestInitialMigration : FbEndToEndFixture
    {
        /// <summary>
        /// Defines the test method Migrate_FirstVersion_ShouldCreateTable.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        [TestCase("SIMPLE")]
        [TestCase("VersionInfo")]
        public void Migrate_FirstVersion_ShouldCreateTable(string tableName)
        {
            Migrate(typeof(VersionOneSimpleTableMigration).Namespace);

            TableExists(tableName).ShouldBe(true, string.Format("Table {0} should have been created but it wasn't", tableName));
        }

        /// <summary>
        /// Defines the test method Migrate_FirstVersion_ShouldCreateColumn.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        [TestCase("ID")]
        [TestCase("COL_STR")]
        public void Migrate_FirstVersion_ShouldCreateColumn(string columnName)
        {
            Migrate(typeof(VersionOneSimpleTableMigration).Namespace);

            ColumnExists("SIMPLE", columnName).ShouldBe(true, string.Format("Column {0} should have been created but it wasn't", columnName));
        }

        /// <summary>
        /// Defines the test method Rollback_FirstVersion_ShouldDropTable.
        /// </summary>
        /// <param name="table">The table.</param>
        [TestCase("SIMPLE")]
        [TestCase("VersionInfo")]
        public void Rollback_FirstVersion_ShouldDropTable(string table)
        {
            var migrationsNamespace = typeof(VersionOneSimpleTableMigration).Namespace;
            Migrate(migrationsNamespace);

            Rollback(migrationsNamespace);

            TableExists(table).ShouldBe(false, string.Format("Table {0} should have been dropped but it wasn't", table));
        }
    }
}
