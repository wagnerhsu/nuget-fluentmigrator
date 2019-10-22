// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="PerformDBOperationExpressionTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using FluentMigrator.Tests.Helpers;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Expressions
{
    /// <summary>
    /// Defines test class PerformDBOperationExpressionTests.
    /// </summary>
    [TestFixture]
    public class PerformDBOperationExpressionTests
    {
        /// <summary>
        /// Defines the test method ErrorIsReturnedWhenOperationIsNull.
        /// </summary>
        [Test]
        public void ErrorIsReturnedWhenOperationIsNull()
        {
            var expression = new PerformDBOperationExpression() { Operation = null };
            var errors = ValidationHelper.CollectErrors(expression);
            errors.ShouldContain(ErrorMessages.OperationCannotBeNull);
        }
    }
}
