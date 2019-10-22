// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SqlServer2012SequenceTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using FluentMigrator.Exceptions;
using FluentMigrator.Runner.Generators.SqlServer;
using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Generators.SqlServer2012
{
    /// <summary>
    /// Defines test class SqlServer2012SequenceTests.
    /// Implements the <see cref="FluentMigrator.Tests.Unit.Generators.BaseSequenceTests" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Unit.Generators.BaseSequenceTests" />
    [TestFixture]
    public class SqlServer2012SequenceTests : BaseSequenceTests
    {
        /// <summary>
        /// The generator
        /// </summary>
        protected SqlServer2012Generator Generator;

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            Generator = new SqlServer2012Generator()
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
            result.ShouldBe("CREATE SEQUENCE [TestSchema].[Sequence] INCREMENT BY 2 MINVALUE 0 MAXVALUE 100 START WITH 2 CACHE 10 CYCLE");
        }

        /// <summary>
        /// Defines the test method CanCreateSequenceWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateSequenceWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateSequenceExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE SEQUENCE [dbo].[Sequence] INCREMENT BY 2 MINVALUE 0 MAXVALUE 100 START WITH 2 CACHE 10 CYCLE");
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
            result.ShouldBe("CREATE SEQUENCE [dbo].[Sequence] INCREMENT BY 2 MINVALUE 0 MAXVALUE 100 START WITH 2 NO CACHE CYCLE");
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
            result.ShouldBe("DROP SEQUENCE [TestSchema].[Sequence]");
        }

        /// <summary>
        /// Defines the test method CanDropSequenceWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanDropSequenceWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetDeleteSequenceExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("DROP SEQUENCE [dbo].[Sequence]");
        }
    }
}
