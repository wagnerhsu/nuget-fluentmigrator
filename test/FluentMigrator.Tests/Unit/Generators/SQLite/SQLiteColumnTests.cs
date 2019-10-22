// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SQLiteColumnTests.cs" company="FluentMigrator Project">
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
using System.Linq;

using FluentMigrator.Runner.Generators.SQLite;
using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Generators.SQLite
{
    /// <summary>
    /// Defines test class SQLiteColumnTests.
    /// Implements the <see cref="FluentMigrator.Tests.Unit.Generators.BaseColumnTests" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Unit.Generators.BaseColumnTests" />
    [TestFixture]
    // ReSharper disable once InconsistentNaming
    public class SQLiteColumnTests : BaseColumnTests
    {
        /// <summary>
        /// The generator
        /// </summary>
        protected SQLiteGenerator Generator;

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            Generator = new SQLiteGenerator();
        }

        /// <summary>
        /// Defines the test method CanCreateNullableColumnWithCustomDomainTypeAndCustomSchema.
        /// </summary>
        [Test]
        public override void CanCreateNullableColumnWithCustomDomainTypeAndCustomSchema()
        {
            var expression = GeneratorTestHelper.GetCreateColumnExpressionWithNullableCustomType();
            expression.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE \"TestTable1\" ADD COLUMN \"TestColumn1\" MyDomainType");
        }

        /// <summary>
        /// Defines the test method CanCreateNullableColumnWithCustomDomainTypeAndDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateNullableColumnWithCustomDomainTypeAndDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateColumnExpressionWithNullableCustomType();

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE \"TestTable1\" ADD COLUMN \"TestColumn1\" MyDomainType");
        }

        /// <summary>
        /// Defines the test method CanAlterColumnWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanAlterColumnWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetAlterColumnExpression();
            expression.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe(string.Empty);
        }

        /// <summary>
        /// Defines the test method CanAlterColumnWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanAlterColumnWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetAlterColumnExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe(string.Empty);
        }

        /// <summary>
        /// Defines the test method CanCreateAutoIncrementColumnWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanCreateAutoIncrementColumnWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetAlterTableAutoIncrementColumnExpression();
            expression.Column.IsPrimaryKey = true;
            expression.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE \"TestTable1\" ADD COLUMN \"TestColumn1\" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT");
        }

        /// <summary>
        /// Defines the test method CanCreateAutoIncrementColumnWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateAutoIncrementColumnWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetAlterTableAutoIncrementColumnExpression();
            expression.Column.IsPrimaryKey = true;

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE \"TestTable1\" ADD COLUMN \"TestColumn1\" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT");
        }

        /// <summary>
        /// Defines the test method CanCreateColumnWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanCreateColumnWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetCreateColumnExpression();
            expression.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE \"TestTable1\" ADD COLUMN \"TestColumn1\" TEXT NOT NULL");
        }

        /// <summary>
        /// Defines the test method CanCreateColumnWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateColumnWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateColumnExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE \"TestTable1\" ADD COLUMN \"TestColumn1\" TEXT NOT NULL");
        }

        /// <summary>
        /// Defines the test method CanCreateColumnWithSystemMethodAndCustomSchema.
        /// </summary>
        [Test]
        public override void CanCreateColumnWithSystemMethodAndCustomSchema()
        {
            var expressions = GeneratorTestHelper.GetCreateColumnWithSystemMethodExpression("TestSchema");
            var result = string.Join(Environment.NewLine, expressions.Select(x => (string)Generator.Generate((dynamic)x)));
            result.ShouldBe(
                @"ALTER TABLE ""TestTable1"" ADD COLUMN ""TestColumn1"" DATETIME" + Environment.NewLine +
                @"UPDATE ""TestTable1"" SET ""TestColumn1"" = (datetime('now','localtime')) WHERE 1 = 1");
        }

        /// <summary>
        /// Defines the test method CanCreateColumnWithSystemMethodAndDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateColumnWithSystemMethodAndDefaultSchema()
        {
            var expressions = GeneratorTestHelper.GetCreateColumnWithSystemMethodExpression();
            var result = string.Join(Environment.NewLine, expressions.Select(x => (string)Generator.Generate((dynamic)x)));
            result.ShouldBe(
                @"ALTER TABLE ""TestTable1"" ADD COLUMN ""TestColumn1"" DATETIME" + Environment.NewLine +
                @"UPDATE ""TestTable1"" SET ""TestColumn1"" = (datetime('now','localtime')) WHERE 1 = 1");
        }

        /// <summary>
        /// Defines the test method CanCreateDecimalColumnWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanCreateDecimalColumnWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetCreateDecimalColumnExpression();
            expression.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE \"TestTable1\" ADD COLUMN \"TestColumn1\" NUMERIC NOT NULL");
        }

        /// <summary>
        /// Defines the test method CanCreateDecimalColumnWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateDecimalColumnWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateDecimalColumnExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE \"TestTable1\" ADD COLUMN \"TestColumn1\" NUMERIC NOT NULL");
        }

        /// <summary>
        /// Defines the test method CanDropColumnWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanDropColumnWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetDeleteColumnExpression();
            expression.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe(string.Empty);
        }

        /// <summary>
        /// Defines the test method CanDropColumnWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanDropColumnWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetDeleteColumnExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe(string.Empty);
        }

        /// <summary>
        /// Defines the test method CanDropMultipleColumnsWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanDropMultipleColumnsWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetDeleteColumnExpression(new[] { "TestColumn1", "TestColumn2" });
            expression.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe(string.Empty);
        }

        /// <summary>
        /// Defines the test method CanDropMultipleColumnsWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanDropMultipleColumnsWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetDeleteColumnExpression(new [] { "TestColumn1", "TestColumn2" });

            var result = Generator.Generate(expression);
            result.ShouldBe(string.Empty);
        }

        /// <summary>
        /// Defines the test method CanRenameColumnWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanRenameColumnWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetRenameColumnExpression();
            expression.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe(string.Empty);
        }

        /// <summary>
        /// Defines the test method CanRenameColumnWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanRenameColumnWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetRenameColumnExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe(string.Empty);
        }
    }
}
