// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="RenameTableExpressionTests.cs" company="FluentMigrator Project">
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
    /// Defines test class RenameTableExpressionTests.
    /// </summary>
    [TestFixture]
    public class RenameTableExpressionTests
    {
        /// <summary>
        /// Defines the test method ErrorIsReturnedWhenOldNameIsNull.
        /// </summary>
        [Test]
        public void ErrorIsReturnedWhenOldNameIsNull()
        {
            var expression = new RenameTableExpression { OldName = null };
            var errors = ValidationHelper.CollectErrors(expression);
            errors.ShouldContain(ErrorMessages.OldTableNameCannotBeNullOrEmpty);
        }

        /// <summary>
        /// Defines the test method ErrorIsReturnedWhenOldNameIsEmptyString.
        /// </summary>
        [Test]
        public void ErrorIsReturnedWhenOldNameIsEmptyString()
        {
            var expression = new RenameTableExpression { OldName = string.Empty };
            var errors = ValidationHelper.CollectErrors(expression);
            errors.ShouldContain(ErrorMessages.OldTableNameCannotBeNullOrEmpty);
        }

        /// <summary>
        /// Defines the test method ErrorIsNotReturnedWhenOldNameIsNotNullEmptyString.
        /// </summary>
        [Test]
        public void ErrorIsNotReturnedWhenOldNameIsNotNullEmptyString()
        {
            var expression = new RenameTableExpression { OldName = "Bacon" };
            var errors = ValidationHelper.CollectErrors(expression);
            errors.ShouldNotContain(ErrorMessages.OldTableNameCannotBeNullOrEmpty);
        }

        /// <summary>
        /// Defines the test method ErrorIsReturnedWhenNewNameIsNull.
        /// </summary>
        [Test]
        public void ErrorIsReturnedWhenNewNameIsNull()
        {
            var expression = new RenameTableExpression { NewName = null };
            var errors = ValidationHelper.CollectErrors(expression);
            errors.ShouldContain(ErrorMessages.NewTableNameCannotBeNullOrEmpty);
        }

        /// <summary>
        /// Defines the test method ErrorIsReturnedWhenNewNameIsEmptyString.
        /// </summary>
        [Test]
        public void ErrorIsReturnedWhenNewNameIsEmptyString()
        {
            var expression = new RenameTableExpression { NewName = string.Empty };
            var errors = ValidationHelper.CollectErrors(expression);
            errors.ShouldContain(ErrorMessages.NewTableNameCannotBeNullOrEmpty);
        }

        /// <summary>
        /// Defines the test method ErrorIsNotReturnedWhenNewNameIsNotNullOrEmptyString.
        /// </summary>
        [Test]
        public void ErrorIsNotReturnedWhenNewNameIsNotNullOrEmptyString()
        {
            var expression = new RenameTableExpression { NewName = "Bacon" };
            var errors = ValidationHelper.CollectErrors(expression);
            errors.ShouldNotContain(ErrorMessages.NewTableNameCannotBeNullOrEmpty);
        }

        /// <summary>
        /// Defines the test method ReverseReturnsRenameTableExpression.
        /// </summary>
        [Test]
        public void ReverseReturnsRenameTableExpression()
        {
            var expression = new RenameTableExpression { OldName = "Bacon", NewName = "ChunkyBacon" };
            var reverse = expression.Reverse();
            reverse.ShouldBeOfType<RenameTableExpression>();
        }

        /// <summary>
        /// Defines the test method ReverseSetsOldNameAndNewNameOnGeneratedExpression.
        /// </summary>
        [Test]
        public void ReverseSetsOldNameAndNewNameOnGeneratedExpression()
        {
            var expression = new RenameTableExpression { OldName = "Bacon", NewName = "ChunkyBacon" };
            var reverse = (RenameTableExpression)expression.Reverse();
            reverse.OldName.ShouldBe("ChunkyBacon");
            reverse.NewName.ShouldBe("Bacon");
        }

        /// <summary>
        /// Defines the test method ToStringIsDescriptive.
        /// </summary>
        [Test]
        public void ToStringIsDescriptive()
        {
            var expression = new RenameTableExpression { OldName = "Bacon", NewName = "ChunkyBacon" };
            expression.ToString().ShouldBe("RenameTable Bacon ChunkyBacon");
        }

        /// <summary>
        /// Defines the test method WhenDefaultSchemaConventionIsAppliedAndSchemaIsNotSetThenSchemaShouldBeNull.
        /// </summary>
        [Test]
        public void WhenDefaultSchemaConventionIsAppliedAndSchemaIsNotSetThenSchemaShouldBeNull()
        {
            var expression = new RenameTableExpression { OldName = "Bacon", NewName = "ChunkyBacon" };

            var processed = expression.Apply(ConventionSets.NoSchemaName);

            Assert.That(processed.SchemaName, Is.Null);
        }

        /// <summary>
        /// Defines the test method WhenDefaultSchemaConventionIsAppliedAndSchemaIsSetThenSchemaShouldNotBeChanged.
        /// </summary>
        [Test]
        public void WhenDefaultSchemaConventionIsAppliedAndSchemaIsSetThenSchemaShouldNotBeChanged()
        {
            var expression = new RenameTableExpression { SchemaName = "testschema", OldName = "Bacon", NewName = "ChunkyBacon" };

            var processed = expression.Apply(ConventionSets.WithSchemaName);

            Assert.That(processed.SchemaName, Is.EqualTo("testschema"));
        }

        /// <summary>
        /// Defines the test method WhenDefaultSchemaConventionIsChangedAndSchemaIsNotSetThenSetSchema.
        /// </summary>
        [Test]
        public void WhenDefaultSchemaConventionIsChangedAndSchemaIsNotSetThenSetSchema()
        {
            var expression = new RenameTableExpression { OldName = "Bacon", NewName = "ChunkyBacon" };

            var processed = expression.Apply(ConventionSets.WithSchemaName);

            Assert.That(processed.SchemaName, Is.EqualTo("testdefault"));
        }
    }
}
