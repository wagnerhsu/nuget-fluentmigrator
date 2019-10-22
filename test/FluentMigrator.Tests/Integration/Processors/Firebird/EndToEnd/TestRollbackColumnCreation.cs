// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="TestRollbackColumnCreation.cs" company="FluentMigrator Project">
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

using FluentMigrator.Tests.Integration.Processors.Firebird.EndToEnd.ExplicitlyCreatedFk;
using FluentMigrator.Tests.Integration.Processors.Firebird.EndToEnd.ImplicitlyCreatedFk;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Integration.Processors.Firebird.EndToEnd
{
    /// <summary>
    /// Defines test class TestRollbackColumnCreation.
    /// Implements the <see cref="FluentMigrator.Tests.Integration.Processors.Firebird.EndToEnd.FbEndToEndFixture" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Integration.Processors.Firebird.EndToEnd.FbEndToEndFixture" />
    [TestFixture]
    [Category("Integration")]
    [Category("Firebird")]
    public class TestRollbackColumnCreation : FbEndToEndFixture
    {
        /// <summary>
        /// Defines the test method Rollback_ColumnCreatedOnTableWithImplicitlyCreatedFk_CreatedColumnShouldBeDropped.
        /// </summary>
        [Test]
        public void Rollback_ColumnCreatedOnTableWithImplicitlyCreatedFk_CreatedColumnShouldBeDropped()
        {
            var namespaceFilter = typeof(CreateImplicitFk).Namespace;
            Migrate(namespaceFilter);

            Rollback(namespaceFilter);

            ColumnExists("table2", "silly").ShouldBe(false);
        }

        /// <summary>
        /// Defines the test method Rollback_ColumnCreatedOnTableWithExplictlyCreatedFk_CreatedColumnShouldBeDropped.
        /// </summary>
        [Test]
        public void Rollback_ColumnCreatedOnTableWithExplictlyCreatedFk_CreatedColumnShouldBeDropped()
        {
            var namespaceFilter = typeof(CreateExplicitFk).Namespace;
            Migrate(namespaceFilter);

            Rollback(namespaceFilter);

            ColumnExists("table2", "silly").ShouldBe(false);
        }

        /// <summary>
        /// Defines the test method Delete_ColumnCreateOnTableWithExplicitPk_ColumnShouldBeDropped.
        /// </summary>
        [Test]
        public void Delete_ColumnCreateOnTableWithExplicitPk_ColumnShouldBeDropped()
        {
            Migrate(typeof(DeleteColumnOnTableWithFk.CreateExplicitFk).Namespace);

            ColumnExists("table2", "silly").ShouldBe(false);
        }

        /// <summary>
        /// Defines the test method Rollback_DeletedColumnOnTableWithExplicitFk_ColumnShouldBeRecreated.
        /// </summary>
        [Test]
        public void Rollback_DeletedColumnOnTableWithExplicitFk_ColumnShouldBeRecreated()
        {
            var namespaceFilter = typeof(DeleteColumnOnTableWithFk.CreateExplicitFk).Namespace;
            Migrate(namespaceFilter);

            Rollback(namespaceFilter);

            ColumnExists("table2", "silly").ShouldBe(true);
        }
    }

    namespace ImplicitlyCreatedFk
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
                Execute.Sql("create table table1(id bigint primary key)");

                // the foreign key "table1_fk" doesn't explictly reference "id" of table1!
                Execute.Sql("create table table2(id bigint primary key, table1_fk bigint references table1)");
            }

            /// <summary>
            /// Collects the DOWN migration expressions
            /// </summary>
            public override void Down()
            {
                Delete.Table("table2");
                Delete.Table("table1");
            }
        }

        /// <summary>
        /// Class CreateSillyColumnOnTable2.
        /// Implements the <see cref="FluentMigrator.Migration" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Migration" />
        [Migration(2)]
        public class CreateSillyColumnOnTable2 : Migration
        {
            /// <summary>
            /// Collect the UP migration expressions
            /// </summary>
            public override void Up()
            {
                Create.Column("silly").OnTable("table2").AsString(30);
            }

            /// <summary>
            /// Collects the DOWN migration expressions
            /// </summary>
            public override void Down()
            {
                Delete.Column("silly").FromTable("table2");
            }
        }
    }

    namespace ExplicitlyCreatedFk
    {
        /// <summary>
        /// Class CreateExplicitFk.
        /// Implements the <see cref="FluentMigrator.Migration" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Migration" />
        [Migration(1)]
        public class CreateExplicitFk : Migration
        {
            /// <summary>
            /// Collect the UP migration expressions
            /// </summary>
            public override void Up()
            {
                Create.Table("table1")
                    .WithColumn("id").AsInt64().PrimaryKey();

                Create.Table("table2")
                    .WithColumn("id").AsInt64().PrimaryKey()
                    .WithColumn("table1_fk").AsInt64().ForeignKey("table1", "id");
            }

            /// <summary>
            /// Collects the DOWN migration expressions
            /// </summary>
            public override void Down()
            {
                Delete.Table("table2");
                Delete.Table("table1");
            }
        }

        /// <summary>
        /// Class CreateSillyColumnOnTable2.
        /// Implements the <see cref="FluentMigrator.Migration" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Migration" />
        [Migration(2)]
        public class CreateSillyColumnOnTable2 : Migration
        {
            /// <summary>
            /// Collect the UP migration expressions
            /// </summary>
            public override void Up()
            {
                Create.Column("silly").OnTable("table2").AsString(30);
            }

            /// <summary>
            /// Collects the DOWN migration expressions
            /// </summary>
            public override void Down()
            {
                Delete.Column("silly").FromTable("table2");
            }
        }
    }

    namespace DeleteColumnOnTableWithFk
    {
        /// <summary>
        /// Class CreateExplicitFk.
        /// Implements the <see cref="FluentMigrator.Migration" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Migration" />
        [Migration(1)]
        public class CreateExplicitFk : Migration
        {
            /// <summary>
            /// Collect the UP migration expressions
            /// </summary>
            public override void Up()
            {
                Create.Table("table1")
                    .WithColumn("id").AsInt64().PrimaryKey();

                Create.Table("table2")
                    .WithColumn("id").AsInt64().PrimaryKey()
                    .WithColumn("table1_fk").AsInt64().ForeignKey("table1", "id")
                    .WithColumn("silly").AsString(30);
            }

            /// <summary>
            /// Collects the DOWN migration expressions
            /// </summary>
            public override void Down()
            {
                Delete.Table("table2");
                Delete.Table("table1");
            }
        }

        /// <summary>
        /// Class DeleteSillyColumnOnTable2.
        /// Implements the <see cref="FluentMigrator.Migration" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Migration" />
        [Migration(2)]
        public class DeleteSillyColumnOnTable2 : Migration
        {
            /// <summary>
            /// Collect the UP migration expressions
            /// </summary>
            public override void Up()
            {
                Delete.Column("silly").FromTable("table2");
            }

            /// <summary>
            /// Downs this instance.
            /// </summary>
            public override void Down()
            {
                Create.Column("silly").OnTable("table2").AsString(30);
            }
        }
    }
}
