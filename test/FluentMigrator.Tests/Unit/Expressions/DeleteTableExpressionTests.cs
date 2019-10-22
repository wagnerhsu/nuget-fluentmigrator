// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="DeleteTableExpressionTests.cs" company="FluentMigrator Project">
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

using System;

using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using FluentMigrator.Runner;
using FluentMigrator.Tests.Helpers;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Expressions
{
    /// <summary>
    /// Defines test class DeleteTableExpressionTests.
    /// </summary>
    [TestFixture]
    public class DeleteTableExpressionTests
    {
        /// <summary>
        /// Defines the test method ErrorIsReturnedWhenTableNameIsNull.
        /// </summary>
        [Test]
        public void ErrorIsReturnedWhenTableNameIsNull()
        {
            var expression = new DeleteTableExpression { TableName = null };
            var errors = ValidationHelper.CollectErrors(expression);
            errors.ShouldContain(ErrorMessages.TableNameCannotBeNullOrEmpty);
        }

        /// <summary>
        /// Defines the test method ErrorIsReturnedWhenTableNameIsEmptyString.
        /// </summary>
        [Test]
        public void ErrorIsReturnedWhenTableNameIsEmptyString()
        {
            var expression = new DeleteTableExpression { TableName = string.Empty };
            var errors = ValidationHelper.CollectErrors(expression);
            errors.ShouldContain(ErrorMessages.TableNameCannotBeNullOrEmpty);
        }

        /// <summary>
        /// Defines the test method ErrorIsNotReturnedWhenTableNameIsNotNullEmptyString.
        /// </summary>
        [Test]
        public void ErrorIsNotReturnedWhenTableNameIsNotNullEmptyString()
        {
            var expression = new DeleteTableExpression { TableName = "Bacon" };
            var errors = ValidationHelper.CollectErrors(expression);
            errors.ShouldNotContain(ErrorMessages.TableNameCannotBeNullOrEmpty);
        }

        /// <summary>
        /// Defines the test method ReverseThrowsException.
        /// </summary>
        [Test]
        public void ReverseThrowsException()
        {
            Assert.Throws<NotSupportedException>(() => new DeleteColumnExpression().Reverse());
        }

        /// <summary>
        /// Defines the test method ToStringIsDescriptive.
        /// </summary>
        [Test]
        public void ToStringIsDescriptive()
        {
            var expression = new DeleteTableExpression { TableName = "Bacon" };
            expression.ToString().ShouldBe("DeleteTable Bacon");
        }

        /// <summary>
        /// Defines the test method WhenDefaultSchemaConventionIsAppliedAndSchemaIsNotSetThenSchemaShouldBeNull.
        /// </summary>
        [Test]
        public void WhenDefaultSchemaConventionIsAppliedAndSchemaIsNotSetThenSchemaShouldBeNull()
        {
            var expression = new DeleteTableExpression { TableName = "table1" };

            var processed = expression.Apply(ConventionSets.NoSchemaName);

            Assert.That(processed.SchemaName, Is.Null);
        }

        /// <summary>
        /// Defines the test method WhenDefaultSchemaConventionIsAppliedAndSchemaIsSetThenSchemaShouldNotBeChanged.
        /// </summary>
        [Test]
        public void WhenDefaultSchemaConventionIsAppliedAndSchemaIsSetThenSchemaShouldNotBeChanged()
        {
            var expression = new DeleteTableExpression { SchemaName = "testschema", TableName = "table1" };

            var processed = expression.Apply(ConventionSets.WithSchemaName);

            Assert.That(processed.SchemaName, Is.EqualTo("testschema"));
        }

        /// <summary>
        /// Defines the test method WhenDefaultSchemaConventionIsChangedAndSchemaIsNotSetThenSetSchema.
        /// </summary>
        [Test]
        public void WhenDefaultSchemaConventionIsChangedAndSchemaIsNotSetThenSetSchema()
        {
            var expression = new DeleteTableExpression { TableName = "table1" };

            var processed = expression.Apply(ConventionSets.WithSchemaName);

            Assert.That(processed.SchemaName, Is.EqualTo("testdefault"));
        }
    }
}
