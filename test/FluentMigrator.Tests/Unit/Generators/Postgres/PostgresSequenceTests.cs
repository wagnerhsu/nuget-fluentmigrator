// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="PostgresSequenceTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using FluentMigrator.Exceptions;
using FluentMigrator.Runner.Generators.Postgres;
using FluentMigrator.Runner.Processors.Postgres;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Generators.Postgres
{
    /// <summary>
    /// Defines test class PostgresSequenceTests.
    /// Implements the <see cref="FluentMigrator.Tests.Unit.Generators.BaseSequenceTests" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Unit.Generators.BaseSequenceTests" />
    [TestFixture]
    public class PostgresSequenceTests : BaseSequenceTests
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
            Generator = new PostgresGenerator(quoter)
            {
                CompatibilityMode = Runner.CompatibilityMode.STRICT,
            };
        }

        /// <summary>
        /// Defines the test method CanCreateSequenceWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanCreateSequenceWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetCreateSequenceExpression();
            expression.Sequence.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE SEQUENCE \"TestSchema\".\"Sequence\" INCREMENT BY 2 MINVALUE 0 MAXVALUE 100 START WITH 2 CACHE 10 CYCLE;");
        }

        /// <summary>
        /// Defines the test method CanCreateSequenceWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateSequenceWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateSequenceExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE SEQUENCE \"Sequence\" INCREMENT BY 2 MINVALUE 0 MAXVALUE 100 START WITH 2 CACHE 10 CYCLE;");
        }

        /// <summary>
        /// Defines the test method CanCreateSequenceWithNocache.
        /// </summary>
        [Test]
        public void CanCreateSequenceWithNocache()
        {
            var expression = GeneratorTestHelper.GetCreateSequenceExpression();
            expression.Sequence.Cache = null;

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE SEQUENCE \"Sequence\" INCREMENT BY 2 MINVALUE 0 MAXVALUE 100 START WITH 2 CACHE 1 CYCLE;");
        }

        /// <summary>
        /// Defines the test method CanNotCreateSequenceWithCacheOne.
        /// </summary>
        [Test]
        public void CanNotCreateSequenceWithCacheOne()
        {
            var expression = GeneratorTestHelper.GetCreateSequenceExpression();
            expression.Sequence.Cache = 1;

            Should.Throw<DatabaseOperationNotSupportedException>(
                () => Generator.Generate(expression),
                "Cache size must be greater than 1; if you intended to disable caching, set Cache to null."
            );
        }

        /// <summary>
        /// Defines the test method CanDropSequenceWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanDropSequenceWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetDeleteSequenceExpression();
            expression.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("DROP SEQUENCE \"TestSchema\".\"Sequence\";");
        }

        /// <summary>
        /// Defines the test method CanDropSequenceWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanDropSequenceWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetDeleteSequenceExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("DROP SEQUENCE \"Sequence\";");
        }
    }
}
