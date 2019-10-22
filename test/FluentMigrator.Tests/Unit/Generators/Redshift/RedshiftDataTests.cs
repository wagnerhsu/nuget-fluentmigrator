// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="RedshiftDataTests.cs" company="FluentMigrator Project">
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

using FluentMigrator.Runner.Generators.Redshift;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Generators.Redshift
{
    /// <summary>
    /// Class RedshiftDataTests. This class cannot be inherited.
    /// Implements the <see cref="FluentMigrator.Tests.Unit.Generators.BaseDataTests" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Unit.Generators.BaseDataTests" />
    [TestFixture]
    public sealed class RedshiftDataTests : BaseDataTests
    {
        /// <summary>
        /// The generator
        /// </summary>
        private RedshiftGenerator _generator;

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _generator = new RedshiftGenerator();
        }

        /// <summary>
        /// Defines the test method CanDeleteDataForAllRowsWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanDeleteDataForAllRowsWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetDeleteDataAllRowsExpression();
            expression.SchemaName = "TestSchema";

            var result = _generator.Generate(expression);
            result.ShouldBe("DELETE FROM \"TestSchema\".\"TestTable1\";");
        }

        /// <summary>
        /// Defines the test method CanDeleteDataForAllRowsWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanDeleteDataForAllRowsWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetDeleteDataAllRowsExpression();

            var result = _generator.Generate(expression);
            result.ShouldBe("DELETE FROM \"public\".\"TestTable1\";");
        }

        /// <summary>
        /// Defines the test method CanDeleteDataForMultipleRowsWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanDeleteDataForMultipleRowsWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetDeleteDataMultipleRowsExpression();
            expression.SchemaName = "TestSchema";

            var result = _generator.Generate(expression);
            result.ShouldBe(
                "DELETE FROM \"TestSchema\".\"TestTable1\" WHERE \"Name\" = 'Just''in' AND \"Website\" IS NULL;DELETE FROM \"TestSchema\".\"TestTable1\" WHERE \"Website\" = 'github.com';");
        }

        /// <summary>
        /// Defines the test method CanDeleteDataForMultipleRowsWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanDeleteDataForMultipleRowsWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetDeleteDataMultipleRowsExpression();

            var result = _generator.Generate(expression);
            result.ShouldBe(
                "DELETE FROM \"public\".\"TestTable1\" WHERE \"Name\" = 'Just''in' AND \"Website\" IS NULL;DELETE FROM \"public\".\"TestTable1\" WHERE \"Website\" = 'github.com';");
        }

        /// <summary>
        /// Defines the test method CanDeleteDataWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanDeleteDataWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetDeleteDataExpression();
            expression.SchemaName = "TestSchema";

            var result = _generator.Generate(expression);
            result.ShouldBe(
                "DELETE FROM \"TestSchema\".\"TestTable1\" WHERE \"Name\" = 'Just''in' AND \"Website\" IS NULL;");
        }

        /// <summary>
        /// Defines the test method CanDeleteDataWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanDeleteDataWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetDeleteDataExpression();

            var result = _generator.Generate(expression);
            result.ShouldBe(
                "DELETE FROM \"public\".\"TestTable1\" WHERE \"Name\" = 'Just''in' AND \"Website\" IS NULL;");
        }

        /// <summary>
        /// Determines whether this instance [can delete data with database null criteria].
        /// </summary>
        public override void CanDeleteDataWithDbNullCriteria()
        {
            var expression = GeneratorTestHelper.GetDeleteDataExpressionWithDbNullValue();
            var result = _generator.Generate(expression);
            result.ShouldBe(
                "DELETE FROM \"public\".\"TestTable1\" WHERE \"Name\" = 'Just''in' AND \"Website\" IS NULL;");
        }

        /// <summary>
        /// Defines the test method CanInsertDataWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanInsertDataWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetInsertDataExpression();
            expression.SchemaName = "TestSchema";

            var expected =
                "INSERT INTO \"TestSchema\".\"TestTable1\" (\"Id\",\"Name\",\"Website\") VALUES (1,'Just''in','codethinked.com');";
            expected +=
                "INSERT INTO \"TestSchema\".\"TestTable1\" (\"Id\",\"Name\",\"Website\") VALUES (2,'Na\\te','kohari.org');";

            var result = _generator.Generate(expression);
            result.ShouldBe(expected);
        }

        /// <summary>
        /// Defines the test method CanInsertDataWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanInsertDataWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetInsertDataExpression();

            var expected =
                "INSERT INTO \"public\".\"TestTable1\" (\"Id\",\"Name\",\"Website\") VALUES (1,'Just''in','codethinked.com');";
            expected +=
                "INSERT INTO \"public\".\"TestTable1\" (\"Id\",\"Name\",\"Website\") VALUES (2,'Na\\te','kohari.org');";

            var result = _generator.Generate(expression);
            result.ShouldBe(expected);
        }

        /// <summary>
        /// Defines the test method CanInsertGuidDataWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanInsertGuidDataWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetInsertGUIDExpression();
            expression.SchemaName = "TestSchema";

            var result = _generator.Generate(expression);
            result.ShouldBe(string.Format("INSERT INTO \"TestSchema\".\"TestTable1\" (\"guid\") VALUES ('{0}');",
                                                 GeneratorTestHelper.TestGuid));
        }

        /// <summary>
        /// Defines the test method CanInsertGuidDataWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanInsertGuidDataWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetInsertGUIDExpression();

            var result = _generator.Generate(expression);
            result.ShouldBe(string.Format("INSERT INTO \"public\".\"TestTable1\" (\"guid\") VALUES ('{0}');",
                                                 GeneratorTestHelper.TestGuid));
        }

        /// <summary>
        /// Defines the test method CanUpdateDataForAllDataWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanUpdateDataForAllDataWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetUpdateDataExpressionWithAllRows();
            expression.SchemaName = "TestSchema";

            var result = _generator.Generate(expression);
            result.ShouldBe("UPDATE \"TestSchema\".\"TestTable1\" SET \"Name\" = 'Just''in', \"Age\" = 25 WHERE 1 = 1;");
        }

        /// <summary>
        /// Defines the test method CanUpdateDataForAllDataWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanUpdateDataForAllDataWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetUpdateDataExpressionWithAllRows();

            var result = _generator.Generate(expression);
            result.ShouldBe("UPDATE \"public\".\"TestTable1\" SET \"Name\" = 'Just''in', \"Age\" = 25 WHERE 1 = 1;");
        }

        /// <summary>
        /// Defines the test method CanUpdateDataWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanUpdateDataWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetUpdateDataExpression();
            expression.SchemaName = "TestSchema";

            var result = _generator.Generate(expression);
            result.ShouldBe(
                "UPDATE \"TestSchema\".\"TestTable1\" SET \"Name\" = 'Just''in', \"Age\" = 25 WHERE \"Id\" = 9 AND \"Homepage\" IS NULL;");
        }

        /// <summary>
        /// Defines the test method CanUpdateDataWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanUpdateDataWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetUpdateDataExpression();

            var result = _generator.Generate(expression);
            result.ShouldBe(
                "UPDATE \"public\".\"TestTable1\" SET \"Name\" = 'Just''in', \"Age\" = 25 WHERE \"Id\" = 9 AND \"Homepage\" IS NULL;");
        }

        /// <summary>
        /// Determines whether this instance [can update data with database null criteria].
        /// </summary>
        public override void CanUpdateDataWithDbNullCriteria()
        {
            var expression = GeneratorTestHelper.GetUpdateDataExpressionWithDbNullValue();

            var result = _generator.Generate(expression);
            result.ShouldBe(
                "UPDATE \"public\".\"TestTable1\" SET \"Name\" = 'Just''in', \"Age\" = 25 WHERE \"Id\" = 9 AND \"Homepage\" IS NULL;");
        }
    }
}
