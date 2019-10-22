// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="MySql5ColumnTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using FluentMigrator.Runner.Generators.MySql;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Generators.MySql5
{
    /// <summary>
    /// Defines test class MySql5ColumnTest.
    /// </summary>
    [TestFixture]
    public class MySql5ColumnTest
    {
        /// <summary>
        /// The generator
        /// </summary>
        protected MySql4Generator Generator;

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            Generator = new MySql5Generator();
        }

        /// <summary>
        /// Defines the test method CanAlterColumnWithCustomSchema.
        /// </summary>
        [Test]
        public void CanAlterColumnWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetAlterColumnExpression();
            expression.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE `TestTable1` MODIFY COLUMN `TestColumn1` NVARCHAR(20) NOT NULL");
        }

        /// <summary>
        /// Defines the test method CanAlterColumnWithDefaultSchema.
        /// </summary>
        [Test]
        public void CanAlterColumnWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetAlterColumnExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE `TestTable1` MODIFY COLUMN `TestColumn1` NVARCHAR(20) NOT NULL");
        }

        /// <summary>
        /// Defines the test method CanCreateColumnWithCustomSchema.
        /// </summary>
        [Test]
        public void CanCreateColumnWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetCreateColumnExpression();
            expression.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE `TestTable1` ADD COLUMN `TestColumn1` NVARCHAR(5) NOT NULL");
        }

        /// <summary>
        /// Defines the test method CanCreateColumnWithDefaultSchema.
        /// </summary>
        [Test]
        public void CanCreateColumnWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateColumnExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE `TestTable1` ADD COLUMN `TestColumn1` NVARCHAR(5) NOT NULL");
        }

        /// <summary>
        /// Defines the test method CanAlterColumnWithDescription.
        /// </summary>
        [Test]
        public void CanAlterColumnWithDescription()
        {
            var expression = GeneratorTestHelper.GetAlterColumnExpressionWithDescription();

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE `TestTable1` MODIFY COLUMN `TestColumn1` NVARCHAR(20) NOT NULL COMMENT 'TestColumn1Description'");
        }

        /// <summary>
        /// Defines the test method CanCreateColumnWithDescription.
        /// </summary>
        [Test]
        public void CanCreateColumnWithDescription()
        {
            var expression = GeneratorTestHelper.GetCreateColumnExpressionWithDescription();

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE `TestTable1` ADD COLUMN `TestColumn1` NVARCHAR(5) NOT NULL COMMENT 'TestColumn1Description'");
        }
    }
}
