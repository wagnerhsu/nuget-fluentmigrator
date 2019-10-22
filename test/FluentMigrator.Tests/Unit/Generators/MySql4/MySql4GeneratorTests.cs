// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="MySql4GeneratorTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using FluentMigrator.Exceptions;
using FluentMigrator.Expressions;
using FluentMigrator.Model;
using FluentMigrator.Runner.Generators.MySql;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Generators.MySql4
{
    /// <summary>
    /// Defines test class MySql4GeneratorTests.
    /// </summary>
    [TestFixture]
    public class MySql4GeneratorTests
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
            Generator = new MySql4Generator();
        }

        /// <summary>
        /// Defines the test method CanAlterSchemaInStrictMode.
        /// </summary>
        [Test]
        public void CanAlterSchemaInStrictMode()
        {
            Generator.CompatibilityMode = Runner.CompatibilityMode.STRICT;

            Assert.Throws<DatabaseOperationNotSupportedException>(() => Generator.Generate(new CreateSchemaExpression()));
        }

        /// <summary>
        /// Defines the test method CanCreateSchemaInStrictMode.
        /// </summary>
        [Test]
        public void CanCreateSchemaInStrictMode()
        {
            Generator.CompatibilityMode = Runner.CompatibilityMode.STRICT;

            Assert.Throws<DatabaseOperationNotSupportedException>(() => Generator.Generate(new CreateSchemaExpression()));
        }

        /// <summary>
        /// Defines the test method CanDropSchemaInStrictMode.
        /// </summary>
        [Test]
        public void CanDropSchemaInStrictMode()
        {
            Generator.CompatibilityMode = Runner.CompatibilityMode.STRICT;

            Assert.Throws<DatabaseOperationNotSupportedException>(() => Generator.Generate(new DeleteSchemaExpression()));
        }

        /// <summary>
        /// Defines the test method CanUseSystemMethodCurrentDateTimeAsADefaultValueForAColumn.
        /// </summary>
        [Test]
        public void CanUseSystemMethodCurrentDateTimeAsADefaultValueForAColumn()
        {
            var columnDefinition = new ColumnDefinition { Name = "NewColumn", Size = 15, Type = null, CustomType = "TIMESTAMP", DefaultValue = SystemMethods.CurrentDateTime };
            var expression = new CreateColumnExpression { Column = columnDefinition, TableName = "NewTable" };

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE `NewTable` ADD COLUMN `NewColumn` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP");
        }

        /// <summary>
        /// Defines the test method CanAlterDefaultConstraintToCurrentTimestamp.
        /// </summary>
        [Test]
        public void CanAlterDefaultConstraintToCurrentTimestamp()
        {
            var expression = GeneratorTestHelper.GetAlterDefaultConstraintExpression();
            expression.DefaultValue = SystemMethods.CurrentDateTime;

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE `TestTable1` ALTER `TestColumn1` SET DEFAULT CURRENT_TIMESTAMP");
        }

        /// <summary>
        /// Defines the test method CanDeleteDefaultConstraint.
        /// </summary>
        [Test]
        public void CanDeleteDefaultConstraint()
        {
            var expression = new DeleteDefaultConstraintExpression
            {
                ColumnName = "TestColumn1",
                TableName = "TestTable1"
            };

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE `TestTable1` ALTER `TestColumn1` DROP DEFAULT");
        }
    }
}
