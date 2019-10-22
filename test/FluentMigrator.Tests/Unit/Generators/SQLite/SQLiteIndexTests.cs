// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SQLiteIndexTests.cs" company="FluentMigrator Project">
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

using FluentMigrator.Runner.Generators.SQLite;
using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Generators.SQLite
{
    /// <summary>
    /// Defines test class SQLiteIndexTests.
    /// Implements the <see cref="FluentMigrator.Tests.Unit.Generators.BaseIndexTests" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Unit.Generators.BaseIndexTests" />
    [TestFixture]
    // ReSharper disable once InconsistentNaming
    public class SQLiteIndexTests : BaseIndexTests
    {
        /// <summary>
        /// The generator
        /// </summary>
        protected SQLiteGenerator Generator;

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            Generator = new SQLiteGenerator();
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
            result.ShouldBe("CREATE INDEX \"TestIndex\" ON \"TestTable1\" (\"TestColumn1\" ASC)");
        }

        /// <summary>
        /// Defines the test method CanCreateIndexWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateIndexWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateIndexExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE INDEX \"TestIndex\" ON \"TestTable1\" (\"TestColumn1\" ASC)");
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
            result.ShouldBe("CREATE INDEX \"TestIndex\" ON \"TestTable1\" (\"TestColumn1\" ASC, \"TestColumn2\" DESC)");
        }

        /// <summary>
        /// Defines the test method CanCreateMultiColumnIndexWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateMultiColumnIndexWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateMultiColumnCreateIndexExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE INDEX \"TestIndex\" ON \"TestTable1\" (\"TestColumn1\" ASC, \"TestColumn2\" DESC)");
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
            result.ShouldBe("CREATE UNIQUE INDEX \"TestIndex\" ON \"TestTable1\" (\"TestColumn1\" ASC, \"TestColumn2\" DESC)");
        }

        /// <summary>
        /// Defines the test method CanCreateMultiColumnUniqueIndexWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateMultiColumnUniqueIndexWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateUniqueMultiColumnIndexExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE UNIQUE INDEX \"TestIndex\" ON \"TestTable1\" (\"TestColumn1\" ASC, \"TestColumn2\" DESC)");
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
            result.ShouldBe("CREATE UNIQUE INDEX \"TestIndex\" ON \"TestTable1\" (\"TestColumn1\" ASC)");
        }

        /// <summary>
        /// Defines the test method CanCreateUniqueIndexWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateUniqueIndexWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateUniqueIndexExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE UNIQUE INDEX \"TestIndex\" ON \"TestTable1\" (\"TestColumn1\" ASC)");
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
            result.ShouldBe("DROP INDEX \"TestIndex\"");
        }

        /// <summary>
        /// Defines the test method CanDropIndexWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanDropIndexWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetDeleteIndexExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("DROP INDEX \"TestIndex\"");
        }
    }
}
