// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SqlServer2008IndexTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
// Copyright (c) 2007-2018, FluentMigrator Project
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System.Linq;

using FluentMigrator.Builders.Create.Index;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure.Extensions;
using FluentMigrator.Runner.Generators.SqlServer;
using FluentMigrator.SqlServer;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Generators.SqlServer2008
{
    /// <summary>
    /// Defines test class SqlServer2008IndexTests.
    /// Implements the <see cref="FluentMigrator.Tests.Unit.Generators.BaseIndexTests" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Unit.Generators.BaseIndexTests" />
    [TestFixture]
    public class SqlServer2008IndexTests : BaseIndexTests
    {
        /// <summary>
        /// The generator
        /// </summary>
        private SqlServer2008Generator _generator;

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _generator = new SqlServer2008Generator();
        }

        /// <summary>
        /// Defines the test method CanCreateIndexWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanCreateIndexWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetCreateIndexExpression();
            expression.Index.SchemaName = "TestSchema";

            var result = _generator.Generate(expression);
            result.ShouldBe("CREATE INDEX [TestIndex] ON [TestSchema].[TestTable1] ([TestColumn1] ASC)");
        }

        /// <summary>
        /// Defines the test method CanCreateIndexWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateIndexWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateIndexExpression();

            var result = _generator.Generate(expression);
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

            var result = _generator.Generate(expression);
            result.ShouldBe("CREATE INDEX [TestIndex] ON [TestSchema].[TestTable1] ([TestColumn1] ASC, [TestColumn2] DESC)");
        }

        /// <summary>
        /// Defines the test method CanCreateMultiColumnIndexWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateMultiColumnIndexWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateMultiColumnCreateIndexExpression();

            var result = _generator.Generate(expression);
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

            var result = _generator.Generate(expression);
            result.ShouldBe("CREATE UNIQUE INDEX [TestIndex] ON [TestSchema].[TestTable1] ([TestColumn1] ASC, [TestColumn2] DESC)");
        }

        /// <summary>
        /// Defines the test method CanCreateMultiColumnUniqueIndexWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateMultiColumnUniqueIndexWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateUniqueMultiColumnIndexExpression();

            var result = _generator.Generate(expression);
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

            var result = _generator.Generate(expression);
            result.ShouldBe("CREATE UNIQUE INDEX [TestIndex] ON [TestSchema].[TestTable1] ([TestColumn1] ASC)");
        }

        /// <summary>
        /// Defines the test method CanCreateUniqueIndexWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateUniqueIndexWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateUniqueIndexExpression();

            var result = _generator.Generate(expression);
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

            var result = _generator.Generate(expression);
            result.ShouldBe("DROP INDEX [TestIndex] ON [TestSchema].[TestTable1]");
        }

        /// <summary>
        /// Defines the test method CanDropIndexWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanDropIndexWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetDeleteIndexExpression();

            var result = _generator.Generate(expression);
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

            var result = _generator.Generate(expression);
            result.ShouldBe("CREATE UNIQUE INDEX [TestIndex] ON [dbo].[TestTable1] ([TestColumn1] ASC)");
        }

        /// <summary>
        /// Defines the test method CanCreateUniqueIndexWithNonDistinctNulls.
        /// </summary>
        [Test]
        public void CanCreateUniqueIndexWithNonDistinctNulls()
        {
            var expression = new CreateIndexExpression()
            {
                Index =
                {
                    Name = GeneratorTestHelper.TestIndexName,
                }
            };

            var builder = new CreateIndexExpressionBuilder(expression);
            builder
                .OnTable(GeneratorTestHelper.TestTableName1)
                .OnColumn(GeneratorTestHelper.TestColumnName1)
                .Ascending()
                .NullsNotDistinct()
                .WithOptions().Unique();

            var result = _generator.Generate(expression);
            result.ShouldBe("CREATE UNIQUE INDEX [TestIndex] ON [dbo].[TestTable1] ([TestColumn1] ASC) WHERE [TestColumn1] IS NOT NULL");
        }

        /// <summary>
        /// Defines the test method CanCreateUniqueIndexWithNonDistinctNullsAlternativeSyntax.
        /// </summary>
        [Test]
        public void CanCreateUniqueIndexWithNonDistinctNullsAlternativeSyntax()
        {
            var expression = new CreateIndexExpression()
            {
                Index =
                {
                    Name = GeneratorTestHelper.TestIndexName,
                }
            };

            var builder = new CreateIndexExpressionBuilder(expression);
            builder
                .OnTable(GeneratorTestHelper.TestTableName1)
                .OnColumn(GeneratorTestHelper.TestColumnName1)
                .Unique().NullsNotDistinct();

            var result = _generator.Generate(expression);
            result.ShouldBe("CREATE UNIQUE INDEX [TestIndex] ON [dbo].[TestTable1] ([TestColumn1] ASC) WHERE [TestColumn1] IS NOT NULL");
        }

        /// <summary>
        /// Defines the test method CanCreateMultiColumnUniqueIndexWithNonDistinctNulls.
        /// </summary>
        [Test]
        public void CanCreateMultiColumnUniqueIndexWithNonDistinctNulls()
        {
            var expression = new CreateIndexExpression()
            {
                Index =
                {
                    Name = GeneratorTestHelper.TestIndexName,
                }
            };

            var builder = new CreateIndexExpressionBuilder(expression);
            builder
                .OnTable(GeneratorTestHelper.TestTableName1)
                .OnColumn(GeneratorTestHelper.TestColumnName1).Ascending()
                .OnColumn(GeneratorTestHelper.TestColumnName2).Descending()
                .WithOptions().UniqueNullsNotDistinct();

            var result = _generator.Generate(expression);
            result.ShouldBe("CREATE UNIQUE INDEX [TestIndex] ON [dbo].[TestTable1] ([TestColumn1] ASC, [TestColumn2] DESC) WHERE [TestColumn1] IS NOT NULL AND [TestColumn2] IS NOT NULL");
        }

        /// <summary>
        /// Defines the test method CanCreateMultiColumnUniqueIndexWithNonDistinctNullsWithSingleColumnNullsDistinct.
        /// </summary>
        [Test]
        public void CanCreateMultiColumnUniqueIndexWithNonDistinctNullsWithSingleColumnNullsDistinct()
        {
            var expression = new CreateIndexExpression()
            {
                Index =
                {
                    Name = GeneratorTestHelper.TestIndexName,
                }
            };

            var builder = new CreateIndexExpressionBuilder(expression);
            builder
                .OnTable(GeneratorTestHelper.TestTableName1)
                .OnColumn(GeneratorTestHelper.TestColumnName1).Ascending().NullsDistinct()
                .OnColumn(GeneratorTestHelper.TestColumnName2).Descending()
                .WithOptions().UniqueNullsNotDistinct();

            var result = _generator.Generate(expression);
            result.ShouldBe("CREATE UNIQUE INDEX [TestIndex] ON [dbo].[TestTable1] ([TestColumn1] ASC, [TestColumn2] DESC) WHERE [TestColumn2] IS NOT NULL");
        }

        /// <summary>
        /// Defines the test method CanCreateMultiColumnUniqueIndexWithOneNonDistinctNulls.
        /// </summary>
        [Test]
        public void CanCreateMultiColumnUniqueIndexWithOneNonDistinctNulls()
        {
            var expression = GeneratorTestHelper.GetCreateUniqueMultiColumnIndexExpression();
            expression.Index.Columns.First().SetAdditionalFeature(SqlServerExtensions.IndexColumnNullsDistinct, false);

            var result = _generator.Generate(expression);
            result.ShouldBe("CREATE UNIQUE INDEX [TestIndex] ON [dbo].[TestTable1] ([TestColumn1] ASC, [TestColumn2] DESC) WHERE [TestColumn1] IS NOT NULL");
        }

        /// <summary>
        /// Defines the test method CanCreateMultiColumnUniqueIndexWithTwoNonDistinctNulls.
        /// </summary>
        [Test]
        public void CanCreateMultiColumnUniqueIndexWithTwoNonDistinctNulls()
        {
            var expression = GeneratorTestHelper.GetCreateUniqueMultiColumnIndexExpression();

            foreach (var c in expression.Index.Columns)
            {
                c.SetAdditionalFeature(SqlServerExtensions.IndexColumnNullsDistinct, false);
            }

            var result = _generator.Generate(expression);
            result.ShouldBe("CREATE UNIQUE INDEX [TestIndex] ON [dbo].[TestTable1] ([TestColumn1] ASC, [TestColumn2] DESC) WHERE [TestColumn1] IS NOT NULL AND [TestColumn2] IS NOT NULL");
        }
    }
}
