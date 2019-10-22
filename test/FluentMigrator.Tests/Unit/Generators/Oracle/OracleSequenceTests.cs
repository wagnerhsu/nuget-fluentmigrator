// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="OracleSequenceTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using FluentMigrator.Exceptions;
using FluentMigrator.Runner.Generators.Oracle;
using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Generators.Oracle
{
    /// <summary>
    /// Defines test class OracleSequenceTests.
    /// Implements the <see cref="FluentMigrator.Tests.Unit.Generators.BaseSequenceTests" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Unit.Generators.BaseSequenceTests" />
    [TestFixture]
    public class OracleSequenceTests : BaseSequenceTests
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
            Generator = new OracleGenerator()
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
            result.ShouldBe("CREATE SEQUENCE TestSchema.Sequence INCREMENT BY 2 MINVALUE 0 MAXVALUE 100 START WITH 2 CACHE 10 CYCLE");
        }

        /// <summary>
        /// Defines the test method CanCreateSequenceWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateSequenceWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateSequenceExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE SEQUENCE Sequence INCREMENT BY 2 MINVALUE 0 MAXVALUE 100 START WITH 2 CACHE 10 CYCLE");
        }

        /// <summary>
        /// Defines the test method CanCreateSequenceWithNocacheSchema.
        /// </summary>
        [Test]
        public void CanCreateSequenceWithNocacheSchema()
        {
            var expression = GeneratorTestHelper.GetCreateSequenceExpression();
            expression.Sequence.Cache = null;

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE SEQUENCE Sequence INCREMENT BY 2 MINVALUE 0 MAXVALUE 100 START WITH 2 NOCACHE CYCLE");
        }

        /// <summary>
        /// Defines the test method CanNotCreateSequenceWithCacheOneSchema.
        /// </summary>
        [Test]
        public void CanNotCreateSequenceWithCacheOneSchema()
        {
            var expression = GeneratorTestHelper.GetCreateSequenceExpression();
            expression.Sequence.Cache = 1;

            Should.Throw<DatabaseOperationNotSupportedException>(
                () => Generator.Generate(expression),
                "Oracle does not support Cache value equal to 1; if you intended to disable caching, set Cache to null. For information on Oracle limitations, see: https://docs.oracle.com/en/database/oracle/oracle-database/18/sqlrf/CREATE-SEQUENCE.html#GUID-E9C78A8C-615A-4757-B2A8-5E6EFB130571__GUID-7E390BE1-2F6C-4E5A-9D5C-5A2567D636FB"
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
            result.ShouldBe("DROP SEQUENCE TestSchema.Sequence");
        }

        /// <summary>
        /// Defines the test method CanDropSequenceWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanDropSequenceWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetDeleteSequenceExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("DROP SEQUENCE Sequence");
        }
    }
}
