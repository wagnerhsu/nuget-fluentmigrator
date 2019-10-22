// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="DeleteDefaultConstraintExpressionTests.cs" company="FluentMigrator Project">
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

using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using FluentMigrator.Runner;
using FluentMigrator.Tests.Helpers;

using Moq;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Expressions
{
    /// <summary>
    /// Defines test class DeleteDefaultConstraintExpressionTests.
    /// </summary>
    [TestFixture]
    public class DeleteDefaultConstraintExpressionTests
    {
        /// <summary>
        /// Defines the test method CollectValidationErrorsShouldReturnErrorIfColumnNameIsEmpty.
        /// </summary>
        [Test]
        public void CollectValidationErrorsShouldReturnErrorIfColumnNameIsEmpty()
        {
            var expression = new DeleteDefaultConstraintExpression {ColumnName = string.Empty};
            var errors = ValidationHelper.CollectErrors(expression);
            errors.ShouldContain(ErrorMessages.ColumnNameCannotBeNullOrEmpty);
        }

        /// <summary>
        /// Defines the test method CollectValidationErrorsShouldReturnErrorIfColumnNameIsNull.
        /// </summary>
        [Test]
        public void CollectValidationErrorsShouldReturnErrorIfColumnNameIsNull()
        {
            var expression = new DeleteDefaultConstraintExpression {ColumnName = null};
            var errors = ValidationHelper.CollectErrors(expression);
            errors.ShouldContain(ErrorMessages.ColumnNameCannotBeNullOrEmpty);
        }

        /// <summary>
        /// Defines the test method CollectValidationErrorsShouldReturnErrorIfTableNameIsEmpty.
        /// </summary>
        [Test]
        public void CollectValidationErrorsShouldReturnErrorIfTableNameIsEmpty()
        {
            var expression = new DeleteDefaultConstraintExpression {TableName = string.Empty};
            var errors = ValidationHelper.CollectErrors(expression);
            errors.ShouldContain(ErrorMessages.TableNameCannotBeNullOrEmpty);
        }

        /// <summary>
        /// Defines the test method CollectValidationErrorsShouldReturnErrorIfTableNameIsNull.
        /// </summary>
        [Test]
        public void CollectValidationErrorsShouldReturnErrorIfTableNameIsNull()
        {
            var expression = new DeleteDefaultConstraintExpression {TableName = null};
            var errors = ValidationHelper.CollectErrors(expression);
            errors.ShouldContain(ErrorMessages.TableNameCannotBeNullOrEmpty);
        }

        /// <summary>
        /// Defines the test method ExecuteWithShouldDelegateProcessOnMigrationProcessor.
        /// </summary>
        [Test]
        public void ExecuteWithShouldDelegateProcessOnMigrationProcessor()
        {
            var expression = new DeleteDefaultConstraintExpression();
            var processorMock = new Mock<IMigrationProcessor>(MockBehavior.Strict);
            processorMock.Setup(p => p.Process(expression)).Verifiable();

            expression.ExecuteWith(processorMock.Object);

            processorMock.VerifyAll();
        }

        /// <summary>
        /// Defines the test method ToStringIsDescriptive.
        /// </summary>
        [Test]
        public void ToStringIsDescriptive()
        {
            var expression = new DeleteDefaultConstraintExpression {SchemaName = "ThaSchema", TableName = "ThaTable", ColumnName = "ThaColumn"};

            expression.ToString().ShouldBe("DeleteDefaultConstraint ThaSchema.ThaTable ThaColumn");
        }

        /// <summary>
        /// Defines the test method WhenDefaultSchemaConventionIsAppliedAndSchemaIsNotSetThenSchemaShouldBeNull.
        /// </summary>
        [Test]
        public void WhenDefaultSchemaConventionIsAppliedAndSchemaIsNotSetThenSchemaShouldBeNull()
        {
            var expression = new DeleteDefaultConstraintExpression { TableName = "ThaTable", ColumnName = "ThaColumn" };

            var processed = expression.Apply(ConventionSets.NoSchemaName);

            Assert.That(processed.SchemaName, Is.Null);
        }

        /// <summary>
        /// Defines the test method WhenDefaultSchemaConventionIsAppliedAndSchemaIsSetThenSchemaShouldNotBeChanged.
        /// </summary>
        [Test]
        public void WhenDefaultSchemaConventionIsAppliedAndSchemaIsSetThenSchemaShouldNotBeChanged()
        {
            var expression = new DeleteDefaultConstraintExpression { SchemaName = "testschema", TableName = "ThaTable", ColumnName = "ThaColumn" };

            var processed = expression.Apply(ConventionSets.WithSchemaName);

            Assert.That(processed.SchemaName, Is.EqualTo("testschema"));
        }

        /// <summary>
        /// Defines the test method WhenDefaultSchemaConventionIsChangedAndSchemaIsNotSetThenSetSchema.
        /// </summary>
        [Test]
        public void WhenDefaultSchemaConventionIsChangedAndSchemaIsNotSetThenSetSchema()
        {
            var expression = new DeleteDefaultConstraintExpression { TableName = "ThaTable", ColumnName = "ThaColumn" };

            var processed = expression.Apply(ConventionSets.WithSchemaName);

            Assert.That(processed.SchemaName, Is.EqualTo("testdefault"));
        }
    }
}
