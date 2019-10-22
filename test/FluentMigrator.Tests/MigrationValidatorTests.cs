// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="MigrationValidatorTests.cs" company="FluentMigrator Project">
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
using FluentMigrator.Runner;
using FluentMigrator.Runner.Exceptions;

using Microsoft.Extensions.Logging;

using Moq;
using NUnit.Framework;

namespace FluentMigrator.Tests
{
    /// <summary>
    /// Defines test class MigrationValidatorTests.
    /// </summary>
    [TestFixture]
    public class MigrationValidatorTests
    {
        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _migration = Mock.Of<IMigration>();
            _migrationValidator = new MigrationValidator(Mock.Of<ILogger>(), new DefaultConventionSet());
        }

        /// <summary>
        /// The migration validator
        /// </summary>
        private MigrationValidator _migrationValidator;

        /// <summary>
        /// The migration
        /// </summary>
        private IMigration _migration;

        /// <summary>
        /// Builds the invalid expression.
        /// </summary>
        /// <returns>IMigrationExpression.</returns>
        private IMigrationExpression BuildInvalidExpression()
        {
            return new CreateTableExpression();
        }

        /// <summary>
        /// Builds the valid expression.
        /// </summary>
        /// <returns>IMigrationExpression.</returns>
        private IMigrationExpression BuildValidExpression()
        {
            var expression = new CreateTableExpression { TableName = "Foo" };
            return expression;
        }

        /// <summary>
        /// Defines the test method ItDoesNotThrowIfExpressionsAreValid.
        /// </summary>
        [Test]
        public void ItDoesNotThrowIfExpressionsAreValid()
        {
            Assert.DoesNotThrow(
                () => _migrationValidator.ApplyConventionsToAndValidateExpressions(
                    _migration,
                    new[] { BuildValidExpression() }));
        }

        /// <summary>
        /// Defines the test method ItThrowsInvalidMigrationExceptionIfExpressionsAreInvalid.
        /// </summary>
        [Test]
        public void ItThrowsInvalidMigrationExceptionIfExpressionsAreInvalid()
        {
            Assert.Throws<InvalidMigrationException>(
                () => _migrationValidator.ApplyConventionsToAndValidateExpressions(
                    _migration,
                    new[] { BuildInvalidExpression() }));
        }

        /// <summary>
        /// Defines the test method ItThrowsInvalidMigrationExceptionIfExpressionsContainsMultipleInvalidOfSameType.
        /// </summary>
        [Test]
        public void ItThrowsInvalidMigrationExceptionIfExpressionsContainsMultipleInvalidOfSameType()
        {
            Assert.Throws<InvalidMigrationException>(
                () => _migrationValidator.ApplyConventionsToAndValidateExpressions(
                    _migration,
                    new[] { BuildInvalidExpression(), BuildInvalidExpression() }));
        }
    }
}
