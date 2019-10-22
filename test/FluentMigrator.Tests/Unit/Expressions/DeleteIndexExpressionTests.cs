// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="DeleteIndexExpressionTests.cs" company="FluentMigrator Project">
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

using System.Collections.ObjectModel;

using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using FluentMigrator.Model;
using FluentMigrator.Runner;
using FluentMigrator.Tests.Helpers;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Expressions
{
    /// <summary>
    /// Defines test class DeleteIndexExpressionTests.
    /// </summary>
    [TestFixture]
    public class DeleteIndexExpressionTests
    {
        /// <summary>
        /// Defines the test method ToStringIsDescriptive.
        /// </summary>
        [Test]
        public void ToStringIsDescriptive()
        {
            new DeleteIndexExpression
                {
                    Index = new IndexDefinition
                                {
                                    Columns = new Collection<IndexColumnDefinition>
                                                  {
                                                      new IndexColumnDefinition {Name = "Name"},
                                                      new IndexColumnDefinition {Name = "Slug"}
                                                  },
                                    TableName = "Table",
                                    Name = "NameIndex"
                                }
                }.ToString().ShouldBe("DeleteIndex Table (Name, Slug)");
        }

        /// <summary>
        /// Defines the test method ErrorIsReturnedWhenNameIsNull.
        /// </summary>
        [Test]
        public void ErrorIsReturnedWhenNameIsNull()
        {
            var expression = new DeleteIndexExpression { Index = { Name = null, TableName = "test" } };

            var errors = ValidationHelper.CollectErrors(expression);
            errors.ShouldContain(ErrorMessages.IndexNameCannotBeNullOrEmpty);
        }

        /// <summary>
        /// Defines the test method ErrorIsReturnedWhenTableNameIsNull.
        /// </summary>
        [Test]
        public void ErrorIsReturnedWhenTableNameIsNull()
        {
            var expression = new DeleteIndexExpression { Index = { Name = "IX", TableName = null } };

            var errors = ValidationHelper.CollectErrors(expression);
            errors.ShouldContain(ErrorMessages.TableNameCannotBeNullOrEmpty);
        }

        /// <summary>
        /// Defines the test method ErrorIsNotReturnedWhenIndexNameAndTableNameAreSet.
        /// </summary>
        [Test]
        public void ErrorIsNotReturnedWhenIndexNameAndTableNameAreSet()
        {
            var expression = new DeleteIndexExpression { Index = { Name = "IX", TableName = "test" } };

            var errors = ValidationHelper.CollectErrors(expression);
            Assert.That(errors.Count, Is.EqualTo(0));
        }

        /// <summary>
        /// Defines the test method WhenDefaultSchemaConventionIsAppliedAndSchemaIsNotSetThenSchemaShouldBeNull.
        /// </summary>
        [Test]
        public void WhenDefaultSchemaConventionIsAppliedAndSchemaIsNotSetThenSchemaShouldBeNull()
        {
            var expression = new DeleteIndexExpression();

            var processed = expression.Apply(ConventionSets.NoSchemaName);

            Assert.That(processed.Index.SchemaName, Is.Null);
        }

        /// <summary>
        /// Defines the test method WhenDefaultSchemaConventionIsAppliedAndSchemaIsSetThenSchemaShouldNotBeChanged.
        /// </summary>
        [Test]
        public void WhenDefaultSchemaConventionIsAppliedAndSchemaIsSetThenSchemaShouldNotBeChanged()
        {
            var expression = new DeleteIndexExpression()
            {
                Index =
                {
                    SchemaName = "testschema",
                },
            };

            var processed = expression.Apply(ConventionSets.WithSchemaName);

            Assert.That(processed.Index.SchemaName, Is.EqualTo("testschema"));
        }

        /// <summary>
        /// Defines the test method WhenDefaultSchemaConventionIsChangedAndSchemaIsNotSetThenSetSchema.
        /// </summary>
        [Test]
        public void WhenDefaultSchemaConventionIsChangedAndSchemaIsNotSetThenSetSchema()
        {
            var expression = new DeleteIndexExpression();

            var processed = expression.Apply(ConventionSets.WithSchemaName);

            Assert.That(processed.Index.SchemaName, Is.EqualTo("testdefault"));
        }
    }
}
