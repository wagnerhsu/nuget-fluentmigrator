// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="OracleIndexTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using FluentMigrator.Runner.Generators.Oracle;
using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Generators.Oracle
{
    /// <summary>
    /// Defines test class OracleIndexTests.
    /// Implements the <see cref="FluentMigrator.Tests.Unit.Generators.BaseIndexTests" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Unit.Generators.BaseIndexTests" />
    [TestFixture]
    public class OracleIndexTests : BaseIndexTests
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
            Generator = new OracleGenerator();
        }

        /// <summary>
        /// Defines the test method CanCreateIndexWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanCreateIndexWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetCreateIndexExpression();
            expression.Index.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE INDEX TestIndex ON TestSchema.TestTable1 (TestColumn1 ASC)");
        }

        /// <summary>
        /// Defines the test method CanCreateIndexWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateIndexWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateIndexExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE INDEX TestIndex ON TestTable1 (TestColumn1 ASC)");
        }

        /// <summary>
        /// Defines the test method CanCreateMultiColumnIndexWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanCreateMultiColumnIndexWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetCreateMultiColumnCreateIndexExpression();
            expression.Index.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE INDEX TestIndex ON TestSchema.TestTable1 (TestColumn1 ASC, TestColumn2 DESC)");
        }

        /// <summary>
        /// Defines the test method CanCreateMultiColumnIndexWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateMultiColumnIndexWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateMultiColumnCreateIndexExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE INDEX TestIndex ON TestTable1 (TestColumn1 ASC, TestColumn2 DESC)");
        }

        /// <summary>
        /// Defines the test method CanCreateMultiColumnUniqueIndexWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanCreateMultiColumnUniqueIndexWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetCreateUniqueMultiColumnIndexExpression();
            expression.Index.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE UNIQUE INDEX TestIndex ON TestSchema.TestTable1 (TestColumn1 ASC, TestColumn2 DESC)");
        }

        /// <summary>
        /// Defines the test method CanCreateMultiColumnUniqueIndexWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateMultiColumnUniqueIndexWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateUniqueMultiColumnIndexExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE UNIQUE INDEX TestIndex ON TestTable1 (TestColumn1 ASC, TestColumn2 DESC)");
        }

        /// <summary>
        /// Defines the test method CanCreateUniqueIndexWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanCreateUniqueIndexWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetCreateUniqueIndexExpression();
            expression.Index.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE UNIQUE INDEX TestIndex ON TestSchema.TestTable1 (TestColumn1 ASC)");
        }

        /// <summary>
        /// Defines the test method CanCreateUniqueIndexWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateUniqueIndexWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateUniqueIndexExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE UNIQUE INDEX TestIndex ON TestTable1 (TestColumn1 ASC)");
        }

        /// <summary>
        /// Defines the test method CanDropIndexWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanDropIndexWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetDeleteIndexExpression();
            expression.Index.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("DROP INDEX TestSchema.TestIndex");
        }

        /// <summary>
        /// Defines the test method CanDropIndexWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanDropIndexWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetDeleteIndexExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("DROP INDEX TestIndex");
        }
    }
}
