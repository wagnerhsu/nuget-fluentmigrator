// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SqlServer2005IndexTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Linq;

using FluentMigrator.Builders.Create.Index;
using FluentMigrator.Builders.Delete.Index;
using FluentMigrator.Infrastructure.Extensions;
using FluentMigrator.Runner.Generators.SqlServer;
using FluentMigrator.SqlServer;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Generators.SqlServer2005
{
    /// <summary>
    /// Defines test class SqlServer2005IndexTests.
    /// Implements the <see cref="FluentMigrator.Tests.Unit.Generators.BaseIndexTests" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Unit.Generators.BaseIndexTests" />
    [TestFixture]
    public class SqlServer2005IndexTests : BaseIndexTests
    {
        /// <summary>
        /// The generator
        /// </summary>
        protected SqlServer2005Generator Generator;

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            Generator = new SqlServer2005Generator();
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
            result.ShouldBe("CREATE INDEX [TestIndex] ON [TestSchema].[TestTable1] ([TestColumn1] ASC)");
        }

        /// <summary>
        /// Defines the test method CanCreateIndexWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateIndexWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateIndexExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE INDEX [TestIndex] ON [dbo].[TestTable1] ([TestColumn1] ASC)");
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
            result.ShouldBe("CREATE INDEX [TestIndex] ON [TestSchema].[TestTable1] ([TestColumn1] ASC, [TestColumn2] DESC)");
        }

        /// <summary>
        /// Defines the test method CanCreateMultiColumnIndexWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateMultiColumnIndexWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateMultiColumnCreateIndexExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE INDEX [TestIndex] ON [dbo].[TestTable1] ([TestColumn1] ASC, [TestColumn2] DESC)");
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
            result.ShouldBe("CREATE UNIQUE INDEX [TestIndex] ON [TestSchema].[TestTable1] ([TestColumn1] ASC, [TestColumn2] DESC)");
        }

        /// <summary>
        /// Defines the test method CanCreateMultiColumnUniqueIndexWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateMultiColumnUniqueIndexWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateUniqueMultiColumnIndexExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE UNIQUE INDEX [TestIndex] ON [dbo].[TestTable1] ([TestColumn1] ASC, [TestColumn2] DESC)");
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
            result.ShouldBe("CREATE UNIQUE INDEX [TestIndex] ON [TestSchema].[TestTable1] ([TestColumn1] ASC)");
        }

        /// <summary>
        /// Defines the test method CanCreateUniqueIndexWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateUniqueIndexWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateUniqueIndexExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE UNIQUE INDEX [TestIndex] ON [dbo].[TestTable1] ([TestColumn1] ASC)");
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
            result.ShouldBe("DROP INDEX [TestIndex] ON [TestSchema].[TestTable1]");
        }

        /// <summary>
        /// Defines the test method CanDropIndexWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanDropIndexWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetDeleteIndexExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("DROP INDEX [TestIndex] ON [dbo].[TestTable1]");
        }

        /// <summary>
        /// Defines the test method CanCreateUniqueIndexWithDistinctNulls.
        /// </summary>
        [Test]
        public void CanCreateUniqueIndexWithDistinctNulls()
        {
            var expression = GeneratorTestHelper.GetCreateUniqueIndexExpression();
            expression.Index.Columns.First().SetAdditionalFeature(SqlServerExtensions.IndexColumnNullsDistinct, true);

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE UNIQUE INDEX [TestIndex] ON [dbo].[TestTable1] ([TestColumn1] ASC)");
        }

        /// <summary>
        /// Defines the test method CanCreateUniqueIndexIgnoringNonDistinctNulls.
        /// </summary>
        [Test]
        public void CanCreateUniqueIndexIgnoringNonDistinctNulls()
        {
            var expression = GeneratorTestHelper.GetCreateUniqueIndexExpression();
            expression.Index.Columns.First().SetAdditionalFeature(SqlServerExtensions.IndexColumnNullsDistinct, false);

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE UNIQUE INDEX [TestIndex] ON [dbo].[TestTable1] ([TestColumn1] ASC)");
        }

        /// <summary>
        /// Defines the test method CanCreateIndexWithOnlineOnOption.
        /// </summary>
        [Test]
        public void CanCreateIndexWithOnlineOnOption()
        {
            var expression = GeneratorTestHelper.GetCreateIndexExpression();
            new CreateIndexExpressionBuilder(expression).Online();
            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE INDEX [TestIndex] ON [dbo].[TestTable1] ([TestColumn1] ASC) WITH (ONLINE=ON)");
        }

        /// <summary>
        /// Defines the test method CanCreateIndexWithOnlineOffOption.
        /// </summary>
        [Test]
        public void CanCreateIndexWithOnlineOffOption()
        {
            var expression = GeneratorTestHelper.GetCreateIndexExpression();
            new CreateIndexExpressionBuilder(expression).Online(false);
            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE INDEX [TestIndex] ON [dbo].[TestTable1] ([TestColumn1] ASC) WITH (ONLINE=OFF)");
        }

        /// <summary>
        /// Defines the test method CanDropIndexWithOnlineOn.
        /// </summary>
        [Test]
        public void CanDropIndexWithOnlineOn()
        {
            var expression = GeneratorTestHelper.GetDeleteIndexExpression();
            new DeleteIndexExpressionBuilder(expression).Online();
            var result = Generator.Generate(expression);
            result.ShouldBe("DROP INDEX [TestIndex] ON [dbo].[TestTable1] WITH (ONLINE=ON)");
        }

        /// <summary>
        /// Defines the test method CanDropIndexWithOnlineOff.
        /// </summary>
        [Test]
        public void CanDropIndexWithOnlineOff()
        {
            var expression = GeneratorTestHelper.GetDeleteIndexExpression();
            new DeleteIndexExpressionBuilder(expression).Online(false);
            var result = Generator.Generate(expression);
            result.ShouldBe("DROP INDEX [TestIndex] ON [dbo].[TestTable1] WITH (ONLINE=OFF)");
        }
    }
}
