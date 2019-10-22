// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SqlServerCeGeneratorTests.cs" company="FluentMigrator Project">
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

using System.Data;

using FluentMigrator.Exceptions;
using FluentMigrator.Expressions;
using FluentMigrator.Model;
using FluentMigrator.Runner.Generators.SqlServer;
using FluentMigrator.SqlServer;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Generators.SqlServerCe
{
    /// <summary>
    /// Defines test class SqlServerCeGeneratorTests.
    /// </summary>
    [TestFixture]
    public class SqlServerCeGeneratorTests
    {
        /// <summary>
        /// The generator
        /// </summary>
        protected SqlServerCeGenerator Generator;

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            Generator = new SqlServerCeGenerator();
        }

        /// <summary>
        /// Defines the test method AlterDefaultConstraintThrowsNotSupportedException.
        /// </summary>
        [Test]
        public void AlterDefaultConstraintThrowsNotSupportedException()
        {
            var expression = GeneratorTestHelper.GetAlterDefaultConstraintExpression();

            Assert.Throws<DatabaseOperationNotSupportedException>(() => Generator.Generate(expression));
        }

        /// <summary>
        /// Defines the test method CanCreateClusteredIndexTreatedAsNonClustered.
        /// </summary>
        [Test]
        public void CanCreateClusteredIndexTreatedAsNonClustered()
        {
            var expression = GeneratorTestHelper.GetCreateIndexExpression();
            expression.Index.IsClustered = true;

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE INDEX [TestIndex] ON [TestTable1] ([TestColumn1] ASC)");
        }

        /// <summary>
        /// Defines the test method CanCreateMultiColumnClusteredIndexTreatedAsNonClustered.
        /// </summary>
        [Test]
        public void CanCreateMultiColumnClusteredIndexTreatedAsNonClustered()
        {
            var expression = GeneratorTestHelper.GetCreateMultiColumnCreateIndexExpression();
            expression.Index.IsClustered = true;

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE INDEX [TestIndex] ON [TestTable1] ([TestColumn1] ASC, [TestColumn2] DESC)");
        }

        /// <summary>
        /// Defines the test method CanCreatMultiColumnUniqueClusteredIndexTreatedAsNonClustered.
        /// </summary>
        [Test]
        public void CanCreatMultiColumnUniqueClusteredIndexTreatedAsNonClustered()
        {
            var expression = GeneratorTestHelper.GetCreateUniqueMultiColumnIndexExpression();
            expression.Index.IsClustered = true;

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE UNIQUE INDEX [TestIndex] ON [TestTable1] ([TestColumn1] ASC, [TestColumn2] DESC)");
        }

        /// <summary>
        /// Defines the test method CanCreateUniqueClusteredIndexTreatedAsNonClustered.
        /// </summary>
        [Test]
        public void CanCreateUniqueClusteredIndexTreatedAsNonClustered()
        {
            var expression = GeneratorTestHelper.GetCreateUniqueIndexExpression();
            expression.Index.IsClustered = true;

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE UNIQUE INDEX [TestIndex] ON [TestTable1] ([TestColumn1] ASC)");
        }

        /// <summary>
        /// Defines the test method CanCreateTableWithNtextSizeUpTo536870911.
        /// </summary>
        [Test]
        [Category("SqlServerCe"), Category("Generator"), Category("Table")]
        public void CanCreateTableWithNtextSizeUpTo536870911()
        {
            var expression = GeneratorTestHelper.GetCreateTableExpression();
            expression.Columns[0].Type = DbType.String;
            expression.Columns[0].Size = 536870911;

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE TABLE [TestTable1] ([TestColumn1] NTEXT NOT NULL, [TestColumn2] INT NOT NULL)");
        }

        /// <summary>
        /// Defines the test method CanCreateTableWithSeededIdentity.
        /// </summary>
        [Test]
        [Category("SqlServerCe"), Category("Generator"), Category("Table")]
        public void CanCreateTableWithSeededIdentity()
        {
            var expression = GeneratorTestHelper.GetCreateTableWithAutoIncrementExpression();
            expression.Columns[0].AdditionalFeatures.Add(SqlServerExtensions.IdentitySeed, 45);
            expression.Columns[0].AdditionalFeatures.Add(SqlServerExtensions.IdentityIncrement, 23);

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE TABLE [TestTable1] ([TestColumn1] INT NOT NULL IDENTITY(45,23), [TestColumn2] INT NOT NULL)");
        }

        /// <summary>
        /// Defines the test method CanCreateXmlColumn.
        /// </summary>
        [Test]
        public void CanCreateXmlColumn()
        {
            var expression = new CreateColumnExpression
                {
                    TableName = "TestTable1",
                    Column = new ColumnDefinition {Name = "TestColumn1", Type = DbType.Xml}
                };

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE [TestTable1] ADD [TestColumn1] NTEXT NOT NULL");
        }

        /// <summary>
        /// Defines the test method CanNotDropMultipleColumns.
        /// </summary>
        [Test]
        public void CanNotDropMultipleColumns()
        {
            //This does not work if column in used in constraint, index etc.
            var expression = GeneratorTestHelper.GetDeleteColumnExpression(new[] { "TestColumn1", "TestColumn2" });

            Assert.Throws<DatabaseOperationNotSupportedException>(() => Generator.Generate(expression));
        }

        /// <summary>
        /// Defines the test method GenerateNecessaryStatementsForADeleteDefaultExpressionIsThrowsException.
        /// </summary>
        [Test]
        public void GenerateNecessaryStatementsForADeleteDefaultExpressionIsThrowsException()
        {
            var expression = new DeleteDefaultConstraintExpression { ColumnName = "Name", SchemaName = "Personalia", TableName = "Person" };

            Assert.Throws<DatabaseOperationNotSupportedException>(() => Generator.Generate(expression));
        }
    }
}
