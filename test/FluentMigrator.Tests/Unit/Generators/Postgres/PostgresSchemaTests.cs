// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="PostgresSchemaTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using FluentMigrator.Runner.Generators.Postgres;
using FluentMigrator.Runner.Processors.Postgres;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Generators.Postgres
{
    /// <summary>
    /// Defines test class PostgresSchemaTests.
    /// Implements the <see cref="FluentMigrator.Tests.Unit.Generators.BaseSchemaTests" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Unit.Generators.BaseSchemaTests" />
    [TestFixture]
    public class PostgresSchemaTests : BaseSchemaTests
    {
        /// <summary>
        /// The generator
        /// </summary>
        protected PostgresGenerator Generator;

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            var quoter = new PostgresQuoter(new PostgresOptions());
            Generator = new PostgresGenerator(quoter);
        }

        /// <summary>
        /// Defines the test method CanAlterSchema.
        /// </summary>
        [Test]
        public override void CanAlterSchema()
        {
            var expression = GeneratorTestHelper.GetAlterSchemaExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE \"TestSchema1\".\"TestTable\" SET SCHEMA \"TestSchema2\";");
        }

        /// <summary>
        /// Defines the test method CanCreateSchema.
        /// </summary>
        [Test]
        public override void CanCreateSchema()
        {
            var expression = GeneratorTestHelper.GetCreateSchemaExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE SCHEMA \"TestSchema\";");
        }

        /// <summary>
        /// Defines the test method CanDropSchema.
        /// </summary>
        [Test]
        public override void CanDropSchema()
        {
            var expression = GeneratorTestHelper.GetDeleteSchemaExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("DROP SCHEMA \"TestSchema\";");
        }
    }
}
