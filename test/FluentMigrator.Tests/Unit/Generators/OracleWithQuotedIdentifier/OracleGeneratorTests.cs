// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="OracleGeneratorTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using FluentMigrator.Exceptions;
using FluentMigrator.Expressions;
using FluentMigrator.Runner.Generators.Oracle;
using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Generators.OracleWithQuotedIdentifier
{
    /// <summary>
    /// Defines test class OracleGeneratorTests.
    /// </summary>
    [TestFixture]
    public class OracleGeneratorTests
    {
        /// <summary>
        /// The generator
        /// </summary>
        protected OracleGenerator Generator;

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            Generator = new OracleGenerator(new OracleQuoterQuotedIdentifier());
        }

        /// <summary>
        /// Defines the test method CanAlterColumnNoNullSettings.
        /// </summary>
        [Test]
        public void CanAlterColumnNoNullSettings()
        {
            var expression = GeneratorTestHelper.GetAlterColumnExpression();
            expression.Column.IsNullable = null;

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE \"TestTable1\" MODIFY \"TestColumn1\" NVARCHAR2(20)");
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
    }
}
