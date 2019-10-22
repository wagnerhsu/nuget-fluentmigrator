// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="BaseDescriptionGeneratorTests.cs" company="FluentMigrator Project">
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

using System.Linq;

using FluentMigrator.Runner.Generators;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Generators
{
    /// <summary>
    /// Class BaseDescriptionGeneratorTests.
    /// </summary>
    public abstract class BaseDescriptionGeneratorTests
    {
        /// <summary>
        /// Gets or sets the description generator.
        /// </summary>
        /// <value>The description generator.</value>
        protected IDescriptionGenerator DescriptionGenerator { get; set; }

        /// <summary>
        /// Generates the description statements for create table return table description statement.
        /// </summary>
        public abstract void GenerateDescriptionStatementsForCreateTableReturnTableDescriptionStatement();
        /// <summary>
        /// Generates the description statements for create table return table description and column descriptions statements.
        /// </summary>
        public abstract void GenerateDescriptionStatementsForCreateTableReturnTableDescriptionAndColumnDescriptionsStatements();
        /// <summary>
        /// Generates the description statement for alter table return table description statement.
        /// </summary>
        public abstract void GenerateDescriptionStatementForAlterTableReturnTableDescriptionStatement();
        /// <summary>
        /// Generates the description statement for create column return column description statement.
        /// </summary>
        public abstract void GenerateDescriptionStatementForCreateColumnReturnColumnDescriptionStatement();
        /// <summary>
        /// Generates the description statement for alter column return column description statement.
        /// </summary>
        public abstract void GenerateDescriptionStatementForAlterColumnReturnColumnDescriptionStatement();

        /// <summary>
        /// Defines the test method GenerateDescriptionStatementsReturnEmptyForNoDescriptionsOnCreateTable.
        /// </summary>
        [Test]
        public void GenerateDescriptionStatementsReturnEmptyForNoDescriptionsOnCreateTable()
        {
            var createTableExpression = GeneratorTestHelper.GetCreateTableExpression();
            var result = DescriptionGenerator.GenerateDescriptionStatements(createTableExpression);

            result.ShouldBe(Enumerable.Empty<string>());
        }

        /// <summary>
        /// Defines the test method GenerateDescriptionStatementReturnEmptyForNoDescriptionOnAlterTable.
        /// </summary>
        [Test]
        public void GenerateDescriptionStatementReturnEmptyForNoDescriptionOnAlterTable()
        {
            var alterTableExpression = GeneratorTestHelper.GetAlterTableAutoIncrementColumnExpression();
            var result = DescriptionGenerator.GenerateDescriptionStatement(alterTableExpression);

            result.ShouldBe(string.Empty);
        }

        /// <summary>
        /// Defines the test method GenerateDescriptionStatementReturnEmptyForNoDescriptionOnCreateColumn.
        /// </summary>
        [Test]
        public void GenerateDescriptionStatementReturnEmptyForNoDescriptionOnCreateColumn()
        {
            var createColumnExpression = GeneratorTestHelper.GetCreateColumnExpression();
            var result = DescriptionGenerator.GenerateDescriptionStatement(createColumnExpression);

            result.ShouldBe(string.Empty);
        }

        /// <summary>
        /// Defines the test method GenerateDescriptionStatementReturnEmptyForNoDescriptionOnAlterColumn.
        /// </summary>
        [Test]
        public void GenerateDescriptionStatementReturnEmptyForNoDescriptionOnAlterColumn()
        {
            var alterColumnExpression = GeneratorTestHelper.GetAlterColumnExpression();
            var result = DescriptionGenerator.GenerateDescriptionStatement(alterColumnExpression);

            result.ShouldBe(string.Empty);
        }

        /// <summary>
        /// Defines the test method GenerateDescriptionStatementsHaveSingleStatementForDescriptionOnCreate.
        /// </summary>
        [Test]
        public void GenerateDescriptionStatementsHaveSingleStatementForDescriptionOnCreate()
        {
            var createTableExpression = GeneratorTestHelper.GetCreateTableWithTableDescription();
            var result = DescriptionGenerator.GenerateDescriptionStatements(createTableExpression);

            result.Count().ShouldBe(1);
        }
    }
}
