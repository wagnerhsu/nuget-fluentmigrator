// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="DeleteConstraintExpressionTests.cs" company="FluentMigrator Project">
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

using FluentMigrator.Builders.Delete.Constraint;
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
    /// Defines test class DeleteConstraintExpressionTests.
    /// </summary>
    [TestFixture]
    public class DeleteConstraintExpressionTests
    {
        /// <summary>
        /// Defines the test method ErrorIsReturnedWhenTableNameIsNull.
        /// </summary>
        [Test]
        public void ErrorIsReturnedWhenTableNameIsNull()
        {
            var expression = new DeleteConstraintExpression(ConstraintType.Unique) { Constraint = { TableName = null } };
            var errors = ValidationHelper.CollectErrors(expression);
            errors.ShouldContain(ErrorMessages.TableNameCannotBeNullOrEmpty);
        }

        /// <summary>
        /// Defines the test method ErrorIsReturnedWhenTableNameIsEmptyString.
        /// </summary>
        [Test]
        public void ErrorIsReturnedWhenTableNameIsEmptyString()
        {
            var expression = new DeleteConstraintExpression(ConstraintType.Unique) { Constraint = { TableName = string.Empty } };
            var errors = ValidationHelper.CollectErrors(expression);
            errors.ShouldContain(ErrorMessages.TableNameCannotBeNullOrEmpty);
        }

        /// <summary>
        /// Defines the test method ErrorIsNotReturnedWhenTableNameIsNotNullEmptyString.
        /// </summary>
        [Test]
        public void ErrorIsNotReturnedWhenTableNameIsNotNullEmptyString()
        {
            var expression = new DeleteConstraintExpression(ConstraintType.Unique) { Constraint = { TableName = "aTable" } };
            var errors = ValidationHelper.CollectErrors(expression);
            errors.ShouldNotContain(ErrorMessages.TableNameCannotBeNullOrEmpty);
        }

        /// <summary>
        /// Defines the test method ApplyDefaultContraintName.
        /// </summary>
        [Test]
        public void ApplyDefaultContraintName()
        {
            var expression = new DeleteConstraintExpression(ConstraintType.Unique);
            var builder = new DeleteConstraintExpressionBuilder(expression);
            builder.FromTable("Users").Column("AccountId");

            var processed = expression.Apply(ConventionSets.NoSchemaName);

            Assert.That(processed.Constraint.ConstraintName, Is.EqualTo("UC_Users_AccountId"));
        }

        /// <summary>
        /// Defines the test method WhenDefaultSchemaConventionIsAppliedAndSchemaIsNotSetThenSchemaShouldBeNull.
        /// </summary>
        [Test]
        public void WhenDefaultSchemaConventionIsAppliedAndSchemaIsNotSetThenSchemaShouldBeNull()
        {
            var expression = new DeleteConstraintExpression(ConstraintType.Unique);

            var processed = expression.Apply(ConventionSets.NoSchemaName);

            Assert.That(processed.Constraint.SchemaName, Is.Null);
        }

        /// <summary>
        /// Defines the test method WhenDefaultSchemaConventionIsAppliedAndSchemaIsSetThenSchemaShouldNotBeChanged.
        /// </summary>
        [Test]
        public void WhenDefaultSchemaConventionIsAppliedAndSchemaIsSetThenSchemaShouldNotBeChanged()
        {
            var expression = new DeleteConstraintExpression(ConstraintType.Unique)
            {
                Constraint =
                {
                    SchemaName = "testschema",
                },
            };

            var processed = expression.Apply(ConventionSets.WithSchemaName);

            Assert.That(processed.Constraint.SchemaName, Is.EqualTo("testschema"));
        }

        /// <summary>
        /// Defines the test method WhenDefaultSchemaConventionIsChangedAndSchemaIsNotSetThenSetSchema.
        /// </summary>
        [Test]
        public void WhenDefaultSchemaConventionIsChangedAndSchemaIsNotSetThenSetSchema()
        {
            var expression = new DeleteConstraintExpression(ConstraintType.Unique);

            var processed = expression.Apply(ConventionSets.WithSchemaName);

            Assert.That(processed.Constraint.SchemaName, Is.EqualTo("testdefault"));
        }
    }
}
