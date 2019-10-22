// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="UpdateDataExpressionTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using FluentMigrator.Runner;
using FluentMigrator.Tests.Helpers;
using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Expressions
{
    /// <summary>
    /// Defines test class UpdateDataExpressionTests.
    /// </summary>
    [TestFixture]
    public class UpdateDataExpressionTests {
        /// <summary>
        /// The expression
        /// </summary>
        private UpdateDataExpression _expression;

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        [SetUp]
        public void Initialize()
        {
            _expression =
                new UpdateDataExpression()
                {
                    TableName = "ExampleTable",
                    Set = new List<KeyValuePair<string, object>>
                    {
                        new KeyValuePair<string, object>("Column", "value")
                    },
                    IsAllRows = false
                };
        }

        /// <summary>
        /// Defines the test method NullUpdateTargetCausesErrorMessage.
        /// </summary>
        [Test]
        public void NullUpdateTargetCausesErrorMessage()
        {
            // null is the default value, but it might not always be, so I'm codifying it here anyway
            _expression.Where = null;

            var errors = ValidationHelper.CollectErrors(_expression);
            errors.ShouldContain(ErrorMessages.UpdateDataExpressionMustSpecifyWhereClauseOrAllRows);
        }

        /// <summary>
        /// Defines the test method EmptyUpdateTargetCausesErrorMessage.
        /// </summary>
        [Test]
        public void EmptyUpdateTargetCausesErrorMessage()
        {
            // The same should be true for an empty list
            _expression.Where = new List<KeyValuePair<string, object>>();

            var errors = ValidationHelper.CollectErrors(_expression);
            errors.ShouldContain(ErrorMessages.UpdateDataExpressionMustSpecifyWhereClauseOrAllRows);
        }

        /// <summary>
        /// Defines the test method DoesNotRequireWhereConditionWhenIsAllRowsIsSet.
        /// </summary>
        [Test]
        public void DoesNotRequireWhereConditionWhenIsAllRowsIsSet()
        {
            _expression.IsAllRows = true;

            var errors = ValidationHelper.CollectErrors(_expression);
            errors.ShouldNotContain(ErrorMessages.UpdateDataExpressionMustSpecifyWhereClauseOrAllRows);
        }

        /// <summary>
        /// Defines the test method DoesNotAllowWhereConditionWhenIsAllRowsIsSet.
        /// </summary>
        [Test]
        public void DoesNotAllowWhereConditionWhenIsAllRowsIsSet()
        {
            _expression.IsAllRows = true;
            _expression.Where = new List<KeyValuePair<string, object>> {new KeyValuePair<string, object>("key", "value")};

            var errors = ValidationHelper.CollectErrors(_expression);
            errors.ShouldContain(ErrorMessages.UpdateDataExpressionMustNotSpecifyBothWhereClauseAndAllRows);
        }

        /// <summary>
        /// Defines the test method WhenDefaultSchemaConventionIsAppliedAndSchemaIsNotSetThenSchemaShouldBeNull.
        /// </summary>
        [Test]
        public void WhenDefaultSchemaConventionIsAppliedAndSchemaIsNotSetThenSchemaShouldBeNull()
        {
            var processed = _expression.Apply(ConventionSets.NoSchemaName);

            Assert.That(processed.SchemaName, Is.Null);
        }

        /// <summary>
        /// Defines the test method WhenDefaultSchemaConventionIsAppliedAndSchemaIsSetThenSchemaShouldNotBeChanged.
        /// </summary>
        [Test]
        public void WhenDefaultSchemaConventionIsAppliedAndSchemaIsSetThenSchemaShouldNotBeChanged()
        {
            _expression.SchemaName = "testschema";

            var processed = _expression.Apply(ConventionSets.WithSchemaName);

            Assert.That(processed.SchemaName, Is.EqualTo("testschema"));
        }

        /// <summary>
        /// Defines the test method WhenDefaultSchemaConventionIsChangedAndSchemaIsNotSetThenSetSchema.
        /// </summary>
        [Test]
        public void WhenDefaultSchemaConventionIsChangedAndSchemaIsNotSetThenSetSchema()
        {
            var processed = _expression.Apply(ConventionSets.WithSchemaName);

            Assert.That(processed.SchemaName, Is.EqualTo("testdefault"));
        }
    }
}
