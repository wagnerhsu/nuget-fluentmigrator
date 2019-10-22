// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SqlServer2000ClusteredTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
//
// Copyright (c) 2018, Fluent Migrator Project
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

using FluentMigrator.Runner.Generators.SqlServer;
using FluentMigrator.SqlServer;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Generators.SqlServer2000
{
    /// <summary>
    /// Defines test class SqlServer2000ClusteredTests.
    /// Implements the <see cref="FluentMigrator.Tests.Unit.Generators.BaseSqlServerClusteredTests" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Unit.Generators.BaseSqlServerClusteredTests" />
    [TestFixture]
    public class SqlServer2000ClusteredTests : BaseSqlServerClusteredTests
    {
        /// <summary>
        /// The generator
        /// </summary>
        protected SqlServer2000Generator Generator;

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            Generator = new SqlServer2000Generator();
        }

        /// <summary>
        /// Defines the test method CanCreateClusteredIndexWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanCreateClusteredIndexWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetCreateIndexExpression();
            expression.Index.IsClustered = true;
            expression.Index.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE CLUSTERED INDEX [TestIndex] ON [TestTable1] ([TestColumn1] ASC)");
        }

        /// <summary>
        /// Defines the test method CanCreateClusteredIndexWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateClusteredIndexWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateIndexExpression();
            expression.Index.IsClustered = true;

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE CLUSTERED INDEX [TestIndex] ON [TestTable1] ([TestColumn1] ASC)");
        }

        /// <summary>
        /// Defines the test method CanCreateMultiColumnClusteredIndexWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanCreateMultiColumnClusteredIndexWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetCreateMultiColumnCreateIndexExpression();
            expression.Index.IsClustered = true;
            expression.Index.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE CLUSTERED INDEX [TestIndex] ON [TestTable1] ([TestColumn1] ASC, [TestColumn2] DESC)");
        }

        /// <summary>
        /// Defines the test method CanCreateMultiColumnClusteredIndexWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateMultiColumnClusteredIndexWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateMultiColumnCreateIndexExpression();
            expression.Index.IsClustered = true;

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE CLUSTERED INDEX [TestIndex] ON [TestTable1] ([TestColumn1] ASC, [TestColumn2] DESC)");
        }

        /// <summary>
        /// Defines the test method CanCreateNamedClusteredPrimaryKeyConstraintWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanCreateNamedClusteredPrimaryKeyConstraintWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetCreateNamedPrimaryKeyExpression();
            expression.Constraint.AdditionalFeatures.Add(SqlServerExtensions.ConstraintType, SqlServerConstraintType.Clustered);
            expression.Constraint.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE [TestTable1] ADD CONSTRAINT [TESTPRIMARYKEY] PRIMARY KEY CLUSTERED ([TestColumn1])");
        }

        /// <summary>
        /// Defines the test method CanCreateNamedClusteredPrimaryKeyConstraintWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateNamedClusteredPrimaryKeyConstraintWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateNamedPrimaryKeyExpression();
            expression.Constraint.AdditionalFeatures.Add(SqlServerExtensions.ConstraintType, SqlServerConstraintType.Clustered);

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE [TestTable1] ADD CONSTRAINT [TESTPRIMARYKEY] PRIMARY KEY CLUSTERED ([TestColumn1])");
        }

        /// <summary>
        /// Defines the test method CanCreateNamedClusteredUniqueConstraintWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanCreateNamedClusteredUniqueConstraintWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetCreateNamedUniqueConstraintExpression();
            expression.Constraint.AdditionalFeatures.Add(SqlServerExtensions.ConstraintType, SqlServerConstraintType.Clustered);
            expression.Constraint.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE [TestTable1] ADD CONSTRAINT [TESTUNIQUECONSTRAINT] UNIQUE CLUSTERED ([TestColumn1])");
        }

        /// <summary>
        /// Defines the test method CanCreateNamedClusteredUniqueConstraintWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateNamedClusteredUniqueConstraintWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateNamedUniqueConstraintExpression();
            expression.Constraint.AdditionalFeatures.Add(SqlServerExtensions.ConstraintType, SqlServerConstraintType.Clustered);

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE [TestTable1] ADD CONSTRAINT [TESTUNIQUECONSTRAINT] UNIQUE CLUSTERED ([TestColumn1])");
        }

        /// <summary>
        /// Defines the test method CanCreateNamedMultiColumnClusteredPrimaryKeyConstraintWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanCreateNamedMultiColumnClusteredPrimaryKeyConstraintWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetCreateNamedMultiColumnPrimaryKeyExpression();
            expression.Constraint.AdditionalFeatures.Add(SqlServerExtensions.ConstraintType, SqlServerConstraintType.Clustered);
            expression.Constraint.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE [TestTable1] ADD CONSTRAINT [TESTPRIMARYKEY] PRIMARY KEY CLUSTERED ([TestColumn1], [TestColumn2])");
        }

        /// <summary>
        /// Defines the test method CanCreateNamedMultiColumnClusteredPrimaryKeyConstraintWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateNamedMultiColumnClusteredPrimaryKeyConstraintWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateNamedMultiColumnPrimaryKeyExpression();
            expression.Constraint.AdditionalFeatures.Add(SqlServerExtensions.ConstraintType, SqlServerConstraintType.Clustered);

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE [TestTable1] ADD CONSTRAINT [TESTPRIMARYKEY] PRIMARY KEY CLUSTERED ([TestColumn1], [TestColumn2])");
        }

        /// <summary>
        /// Defines the test method CanCreateNamedMultiColumnClusteredUniqueConstraintWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanCreateNamedMultiColumnClusteredUniqueConstraintWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetCreateNamedMultiColumnUniqueConstraintExpression();
            expression.Constraint.AdditionalFeatures.Add(SqlServerExtensions.ConstraintType, SqlServerConstraintType.Clustered);
            expression.Constraint.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE [TestTable1] ADD CONSTRAINT [TESTUNIQUECONSTRAINT] UNIQUE CLUSTERED ([TestColumn1], [TestColumn2])");
        }

        /// <summary>
        /// Defines the test method CanCreateNamedMultiColumnClusteredUniqueConstraintWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateNamedMultiColumnClusteredUniqueConstraintWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateNamedMultiColumnUniqueConstraintExpression();
            expression.Constraint.AdditionalFeatures.Add(SqlServerExtensions.ConstraintType, SqlServerConstraintType.Clustered);

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE [TestTable1] ADD CONSTRAINT [TESTUNIQUECONSTRAINT] UNIQUE CLUSTERED ([TestColumn1], [TestColumn2])");
        }

        /// <summary>
        /// Defines the test method CanCreateNamedMultiColumnNonClusteredPrimaryKeyConstraintWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanCreateNamedMultiColumnNonClusteredPrimaryKeyConstraintWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetCreateNamedMultiColumnPrimaryKeyExpression();
            expression.Constraint.AdditionalFeatures.Add(SqlServerExtensions.ConstraintType, SqlServerConstraintType.NonClustered);
            expression.Constraint.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE [TestTable1] ADD CONSTRAINT [TESTPRIMARYKEY] PRIMARY KEY NONCLUSTERED ([TestColumn1], [TestColumn2])");
        }

        /// <summary>
        /// Defines the test method CanCreateNamedMultiColumnNonClusteredPrimaryKeyConstraintWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateNamedMultiColumnNonClusteredPrimaryKeyConstraintWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateNamedMultiColumnPrimaryKeyExpression();
            expression.Constraint.AdditionalFeatures.Add(SqlServerExtensions.ConstraintType, SqlServerConstraintType.NonClustered);

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE [TestTable1] ADD CONSTRAINT [TESTPRIMARYKEY] PRIMARY KEY NONCLUSTERED ([TestColumn1], [TestColumn2])");
        }

        /// <summary>
        /// Defines the test method CanCreateNamedMultiColumnNonClusteredUniqueConstraintWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanCreateNamedMultiColumnNonClusteredUniqueConstraintWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetCreateNamedMultiColumnUniqueConstraintExpression();
            expression.Constraint.AdditionalFeatures.Add(SqlServerExtensions.ConstraintType, SqlServerConstraintType.NonClustered);
            expression.Constraint.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE [TestTable1] ADD CONSTRAINT [TESTUNIQUECONSTRAINT] UNIQUE NONCLUSTERED ([TestColumn1], [TestColumn2])");
        }

        /// <summary>
        /// Defines the test method CanCreateNamedMultiColumnNonClusteredUniqueConstraintWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateNamedMultiColumnNonClusteredUniqueConstraintWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateNamedMultiColumnUniqueConstraintExpression();
            expression.Constraint.AdditionalFeatures.Add(SqlServerExtensions.ConstraintType, SqlServerConstraintType.NonClustered);

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE [TestTable1] ADD CONSTRAINT [TESTUNIQUECONSTRAINT] UNIQUE NONCLUSTERED ([TestColumn1], [TestColumn2])");
        }

        /// <summary>
        /// Defines the test method CanCreateNamedNonClusteredPrimaryKeyConstraintWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanCreateNamedNonClusteredPrimaryKeyConstraintWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetCreateNamedPrimaryKeyExpression();
            expression.Constraint.AdditionalFeatures.Add(SqlServerExtensions.ConstraintType, SqlServerConstraintType.NonClustered);
            expression.Constraint.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE [TestTable1] ADD CONSTRAINT [TESTPRIMARYKEY] PRIMARY KEY NONCLUSTERED ([TestColumn1])");
        }

        /// <summary>
        /// Defines the test method CanCreateNamedNonClusteredPrimaryKeyConstraintWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateNamedNonClusteredPrimaryKeyConstraintWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateNamedPrimaryKeyExpression();
            expression.Constraint.AdditionalFeatures.Add(SqlServerExtensions.ConstraintType, SqlServerConstraintType.NonClustered);

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE [TestTable1] ADD CONSTRAINT [TESTPRIMARYKEY] PRIMARY KEY NONCLUSTERED ([TestColumn1])");
        }

        /// <summary>
        /// Defines the test method CanCreateNamedNonClusteredUniqueConstraintWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanCreateNamedNonClusteredUniqueConstraintWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetCreateNamedUniqueConstraintExpression();
            expression.Constraint.AdditionalFeatures.Add(SqlServerExtensions.ConstraintType, SqlServerConstraintType.NonClustered);
            expression.Constraint.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE [TestTable1] ADD CONSTRAINT [TESTUNIQUECONSTRAINT] UNIQUE NONCLUSTERED ([TestColumn1])");
        }

        /// <summary>
        /// Defines the test method CanCreateNamedNonClusteredUniqueConstraintWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateNamedNonClusteredUniqueConstraintWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateNamedUniqueConstraintExpression();
            expression.Constraint.AdditionalFeatures.Add(SqlServerExtensions.ConstraintType, SqlServerConstraintType.NonClustered);

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE [TestTable1] ADD CONSTRAINT [TESTUNIQUECONSTRAINT] UNIQUE NONCLUSTERED ([TestColumn1])");
        }

        /// <summary>
        /// Defines the test method CanCreateUniqueClusteredIndexWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanCreateUniqueClusteredIndexWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetCreateUniqueIndexExpression();
            expression.Index.IsClustered = true;
            expression.Index.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE UNIQUE CLUSTERED INDEX [TestIndex] ON [TestTable1] ([TestColumn1] ASC)");
        }

        /// <summary>
        /// Defines the test method CanCreateUniqueClusteredIndexWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateUniqueClusteredIndexWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateUniqueIndexExpression();
            expression.Index.IsClustered = true;

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE UNIQUE CLUSTERED INDEX [TestIndex] ON [TestTable1] ([TestColumn1] ASC)");
        }

        /// <summary>
        /// Defines the test method CanCreateUniqueClusteredMultiColumnIndexWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanCreateUniqueClusteredMultiColumnIndexWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetCreateUniqueMultiColumnIndexExpression();
            expression.Index.IsClustered = true;
            expression.Index.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE UNIQUE CLUSTERED INDEX [TestIndex] ON [TestTable1] ([TestColumn1] ASC, [TestColumn2] DESC)");
        }

        /// <summary>
        /// Defines the test method CanCreateUniqueClusteredMultiColumnIndexWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateUniqueClusteredMultiColumnIndexWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateUniqueMultiColumnIndexExpression();
            expression.Index.IsClustered = true;

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE UNIQUE CLUSTERED INDEX [TestIndex] ON [TestTable1] ([TestColumn1] ASC, [TestColumn2] DESC)");
        }
    }
}
