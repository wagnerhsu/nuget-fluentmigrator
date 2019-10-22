// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="RenameColumnExpressionTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
//
// Copyright (c) 2007-2018, Sean Chambers <schambers80@gmail.com>
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
using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Expressions
{
    /// <summary>
    /// Defines test class RenameColumnExpressionTests.
    /// </summary>
    [TestFixture]
    public class RenameColumnExpressionTests
    {
        /// <summary>
        /// Defines the test method ErrorIsReturnedWhenOldNameIsNull.
        /// </summary>
        [Test]
        public void ErrorIsReturnedWhenOldNameIsNull()
        {
            var expression = new RenameColumnExpression { OldName = null };
            var errors = ValidationHelper.CollectErrors(expression);
            errors.ShouldContain(ErrorMessages.OldColumnNameCannotBeNullOrEmpty);
        }

        /// <summary>
        /// Defines the test method ErrorIsReturnedWhenOldNameIsEmptyString.
        /// </summary>
        [Test]
        public void ErrorIsReturnedWhenOldNameIsEmptyString()
        {
            var expression = new RenameColumnExpression { OldName = string.Empty };
            var errors = ValidationHelper.CollectErrors(expression);
            errors.ShouldContain(ErrorMessages.OldColumnNameCannotBeNullOrEmpty);
        }

        /// <summary>
        /// Defines the test method ErrorIsNotReturnedWhenOldNameIsNotNullEmptyString.
        /// </summary>
        [Test]
        public void ErrorIsNotReturnedWhenOldNameIsNotNullEmptyString()
        {
            var expression = new RenameColumnExpression { OldName = "Bacon" };
            var errors = ValidationHelper.CollectErrors(expression);
            errors.ShouldNotContain(ErrorMessages.OldColumnNameCannotBeNullOrEmpty);
        }

        /// <summary>
        /// Defines the test method ErrorIsReturnedWhenNewNameIsNull.
        /// </summary>
        [Test]
        public void ErrorIsReturnedWhenNewNameIsNull()
        {
            var expression = new RenameColumnExpression { NewName = null };
            var errors = ValidationHelper.CollectErrors(expression);
            errors.ShouldContain(ErrorMessages.NewColumnNameCannotBeNullOrEmpty);
        }

        /// <summary>
        /// Defines the test method ErrorIsReturnedWhenNewNameIsEmptyString.
        /// </summary>
        [Test]
        public void ErrorIsReturnedWhenNewNameIsEmptyString()
        {
            var expression = new RenameColumnExpression { NewName = string.Empty };
            var errors = ValidationHelper.CollectErrors(expression);
            errors.ShouldContain(ErrorMessages.NewColumnNameCannotBeNullOrEmpty);
        }

        /// <summary>
        /// Defines the test method ErrorIsNotReturnedWhenNewNameIsNotNullOrEmptyString.
        /// </summary>
        [Test]
        public void ErrorIsNotReturnedWhenNewNameIsNotNullOrEmptyString()
        {
            var expression = new RenameColumnExpression { NewName = "Bacon" };
            var errors = ValidationHelper.CollectErrors(expression);
            errors.ShouldNotContain(ErrorMessages.NewColumnNameCannotBeNullOrEmpty);
        }

        /// <summary>
        /// Defines the test method ReverseReturnsRenameColumnExpression.
        /// </summary>
        [Test]
        public void ReverseReturnsRenameColumnExpression()
        {
            var expression = new RenameColumnExpression { TableName = "Bacon", OldName = "BaconId", NewName = "ChunkyBaconId" };
            var reverse = expression.Reverse();
            reverse.ShouldBeOfType<RenameColumnExpression>();
        }

        /// <summary>
        /// Defines the test method ReverseSetsTableNameOldNameAndNewNameOnGeneratedExpression.
        /// </summary>
        [Test]
        public void ReverseSetsTableNameOldNameAndNewNameOnGeneratedExpression()
        {
            var expression = new RenameColumnExpression { TableName = "Bacon", OldName = "BaconId", NewName = "ChunkyBaconId" };
            var reverse = (RenameColumnExpression)expression.Reverse();

            reverse.TableName.ShouldBe("Bacon");
            reverse.OldName.ShouldBe("ChunkyBaconId");
            reverse.NewName.ShouldBe("BaconId");
        }

        /// <summary>
        /// Defines the test method ToStringIsDescriptive.
        /// </summary>
        [Test]
        public void ToStringIsDescriptive()
        {
            var expression = new RenameColumnExpression { TableName = "Bacon", OldName = "BaconId", NewName = "ChunkyBaconId" };
            expression.ToString().ShouldBe("RenameColumn Bacon BaconId to ChunkyBaconId");
        }

        /// <summary>
        /// Defines the test method WhenDefaultSchemaConventionIsAppliedAndSchemaIsNotSetThenSchemaShouldBeNull.
        /// </summary>
        [Test]
        public void WhenDefaultSchemaConventionIsAppliedAndSchemaIsNotSetThenSchemaShouldBeNull()
        {
            var expression = new RenameColumnExpression { TableName = "Bacon", OldName = "BaconId", NewName = "ChunkyBaconId" };

            var processed = expression.Apply(ConventionSets.NoSchemaName);

            Assert.That(processed.SchemaName, Is.Null);
        }

        /// <summary>
        /// Defines the test method WhenDefaultSchemaConventionIsAppliedAndSchemaIsSetThenSchemaShouldNotBeChanged.
        /// </summary>
        [Test]
        public void WhenDefaultSchemaConventionIsAppliedAndSchemaIsSetThenSchemaShouldNotBeChanged()
        {
            var expression = new RenameColumnExpression { SchemaName = "testschema", TableName = "Bacon", OldName = "BaconId", NewName = "ChunkyBaconId" };

            var processed = expression.Apply(ConventionSets.WithSchemaName);

            Assert.That(processed.SchemaName, Is.EqualTo("testschema"));
        }

        /// <summary>
        /// Defines the test method WhenDefaultSchemaConventionIsChangedAndSchemaIsNotSetThenSetSchema.
        /// </summary>
        [Test]
        public void WhenDefaultSchemaConventionIsChangedAndSchemaIsNotSetThenSetSchema()
        {
            var expression = new RenameColumnExpression { TableName = "Bacon", OldName = "BaconId", NewName = "ChunkyBaconId" };

            var processed = expression.Apply(ConventionSets.WithSchemaName);

            Assert.That(processed.SchemaName, Is.EqualTo("testdefault"));
        }
    }
}
