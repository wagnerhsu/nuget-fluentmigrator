// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="AlterSchemaExpressionTests.cs" company="FluentMigrator Project">
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
using FluentMigrator.Tests.Helpers;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Expressions
{
    /// <summary>
    /// Defines test class AlterSchemaExpressionTests.
    /// </summary>
    [TestFixture]
    public class AlterSchemaExpressionTests
    {
        /// <summary>
        /// Defines the test method ErrorIsReturnedWhenTableNameIsNull.
        /// </summary>
        [Test]
        public void ErrorIsReturnedWhenTableNameIsNull()
        {
            var expression = new AlterSchemaExpression { TableName = null };
            var errors = ValidationHelper.CollectErrors(expression);
            errors.ShouldContain(ErrorMessages.TableNameCannotBeNullOrEmpty);
        }

        /// <summary>
        /// Defines the test method ErrorIsReturnedWhenTableNameIsEmptyString.
        /// </summary>
        [Test]
        public void ErrorIsReturnedWhenTableNameIsEmptyString()
        {
            var expression = new AlterSchemaExpression { TableName = string.Empty };
            var errors = ValidationHelper.CollectErrors(expression);
            errors.ShouldContain(ErrorMessages.TableNameCannotBeNullOrEmpty);
        }

        /// <summary>
        /// Defines the test method ErrorIsNotReturnedWhenTableNameIsNotNullEmptyString.
        /// </summary>
        [Test]
        public void ErrorIsNotReturnedWhenTableNameIsNotNullEmptyString()
        {
            var expression = new AlterSchemaExpression { TableName = "Bacon" };
            var errors = ValidationHelper.CollectErrors(expression);
            errors.ShouldNotContain(ErrorMessages.TableNameCannotBeNullOrEmpty);
        }

        /// <summary>
        /// Defines the test method ErrorIsReturnedWhenDestinationSchemaNameIsNull.
        /// </summary>
        [Test]
        public void ErrorIsReturnedWhenDestinationSchemaNameIsNull()
        {
            var expression = new AlterSchemaExpression { DestinationSchemaName = null };
            var errors = ValidationHelper.CollectErrors(expression);
            errors.ShouldContain(ErrorMessages.DestinationSchemaCannotBeNull);
        }

        /// <summary>
        /// Defines the test method ErrorIsReturnedWhenDestinationSchemaNameIsEmptyString.
        /// </summary>
        [Test]
        public void ErrorIsReturnedWhenDestinationSchemaNameIsEmptyString()
        {
            var expression = new AlterSchemaExpression { DestinationSchemaName = string.Empty };
            var errors = ValidationHelper.CollectErrors(expression);
            errors.ShouldContain(ErrorMessages.DestinationSchemaCannotBeNull);
        }

        /// <summary>
        /// Defines the test method ErrorIsNotReturnedWhenDestinationSchemaNameIsNotNullEmptyString.
        /// </summary>
        [Test]
        public void ErrorIsNotReturnedWhenDestinationSchemaNameIsNotNullEmptyString()
        {
            var expression = new AlterSchemaExpression { DestinationSchemaName = "Bacon" };
            var errors = ValidationHelper.CollectErrors(expression);
            errors.ShouldNotContain(ErrorMessages.DestinationSchemaCannotBeNull);
        }

        /// <summary>
        /// Defines the test method ReverseThrowsException.
        /// </summary>
        [Test]
        public void ReverseThrowsException()
        {
            Assert.Throws<NotSupportedException>(() => new AlterSchemaExpression().Reverse());
        }

        /// <summary>
        /// Defines the test method ToStringIsDescriptive.
        /// </summary>
        [Test]
        public void ToStringIsDescriptive()
        {
            var expression = new AlterSchemaExpression { TableName = "Test", DestinationSchemaName = "Bacon" };
            expression.ToString().ShouldBe("AlterSchema Bacon Table Test");
        }
    }
}
