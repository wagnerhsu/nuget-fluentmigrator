// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="Db2ColumnTests.cs" company="FluentMigrator Project">
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

using FluentMigrator.Runner.Generators;
using FluentMigrator.Runner.Generators.DB2;
using FluentMigrator.Runner.Generators.DB2.iSeries;

using Microsoft.Extensions.Options;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Generators.Db2
{
    /// <summary>
    /// Defines test class Db2ColumnTests.
    /// Implements the <see cref="FluentMigrator.Tests.Unit.Generators.BaseColumnTests" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Unit.Generators.BaseColumnTests" />
    [TestFixture]
    public class Db2ColumnTests : BaseColumnTests
    {
        /// <summary>
        /// Gets or sets the generator.
        /// </summary>
        /// <value>The generator.</value>
        public Db2Generator Generator
        {
            get; set;
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
            result.ShouldBe("ALTER TABLE TestSchema.TestTable1 ADD COLUMN TestColumn1 MyDomainType DEFAULT");
        }

        /// <summary>
        /// Defines the test method CanCreateNullableColumnWithCustomDomainTypeAndDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateNullableColumnWithCustomDomainTypeAndDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateColumnExpressionWithNullableCustomType();

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE TestTable1 ADD COLUMN TestColumn1 MyDomainType DEFAULT");
        }

        /// <summary>
        /// Defines the test method CanAlterColumnWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanAlterColumnWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetAlterColumnExpression();
            expression.Column.IsNullable = null;
            expression.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE TestSchema.TestTable1 ALTER COLUMN TestColumn1 SET DATA TYPE VARGRAPHIC(20) CCSID 1200 NOT NULL");
        }

        /// <summary>
        /// Defines the test method CanAlterColumnWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanAlterColumnWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetAlterColumnExpression();
            expression.Column.IsNullable = null;

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE TestTable1 ALTER COLUMN TestColumn1 SET DATA TYPE VARGRAPHIC(20) CCSID 1200 NOT NULL");
        }

        /// <summary>
        /// Defines the test method CanCreateAutoIncrementColumnWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanCreateAutoIncrementColumnWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetAlterColumnAddAutoIncrementExpression();
            expression.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe(string.Empty);
        }

        /// <summary>
        /// Defines the test method CanCreateAutoIncrementColumnWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateAutoIncrementColumnWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetAlterColumnAddAutoIncrementExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe(string.Empty);
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
            result.ShouldBe("ALTER TABLE TestSchema.TestTable1 ADD COLUMN TestColumn1 VARGRAPHIC(5) CCSID 1200 NOT NULL DEFAULT");
        }

        /// <summary>
        /// Defines the test method CanCreateColumnWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateColumnWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateColumnExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE TestTable1 ADD COLUMN TestColumn1 VARGRAPHIC(5) CCSID 1200 NOT NULL DEFAULT");
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
                @"ALTER TABLE TestSchema.TestTable1 ADD COLUMN TestColumn1 TIMESTAMP DEFAULT" + Environment.NewLine +
                @"UPDATE TestSchema.TestTable1 SET TestColumn1 = CURRENT_TIMESTAMP");
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
                @"ALTER TABLE TestTable1 ADD COLUMN TestColumn1 TIMESTAMP DEFAULT" + Environment.NewLine +
                @"UPDATE TestTable1 SET TestColumn1 = CURRENT_TIMESTAMP");
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
            result.ShouldBe("ALTER TABLE TestSchema.TestTable1 ADD COLUMN TestColumn1 DECIMAL(19,2) NOT NULL DEFAULT");
        }

        /// <summary>
        /// Defines the test method CanCreateDecimalColumnWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateDecimalColumnWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateDecimalColumnExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE TestTable1 ADD COLUMN TestColumn1 DECIMAL(19,2) NOT NULL DEFAULT");
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
            result.ShouldBe("ALTER TABLE TestSchema.TestTable1 DROP COLUMN TestColumn1");
        }

        /// <summary>
        /// Defines the test method CanDropColumnWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanDropColumnWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetDeleteColumnExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE TestTable1 DROP COLUMN TestColumn1");
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
            result.ShouldBe("ALTER TABLE TestSchema.TestTable1 DROP COLUMN TestColumn1 DROP COLUMN TestColumn2");
        }

        /// <summary>
        /// Defines the test method CanDropMultipleColumnsWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanDropMultipleColumnsWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetDeleteColumnExpression(new[] { "TestColumn1", "TestColumn2" });

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE TestTable1 DROP COLUMN TestColumn1 DROP COLUMN TestColumn2");
        }

        /// <summary>
        /// Defines the test method CanRenameColumnWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanRenameColumnWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetRenameColumnExpression();

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

        /// <summary>
        /// Sets up.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            var generatorOptions = new OptionsWrapper<GeneratorOptions>(new GeneratorOptions());
            Generator = new Db2Generator(new Db2ISeriesQuoter(), generatorOptions);
        }
    }
}
