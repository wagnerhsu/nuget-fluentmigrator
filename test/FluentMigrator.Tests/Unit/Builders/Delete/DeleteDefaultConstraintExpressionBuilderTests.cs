// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="DeleteDefaultConstraintExpressionBuilderTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using FluentMigrator.Builders.Delete.DefaultConstraint;
using FluentMigrator.Expressions;
using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Builders.Delete
{
    /// <summary>
    /// Defines test class DeleteDefaultConstraintExpressionBuilderTests.
    /// </summary>
    [TestFixture]
    public class DeleteDefaultConstraintExpressionBuilderTests
    {
        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _expression = new DeleteDefaultConstraintExpression();
            _builder = new DeleteDefaultConstraintExpressionBuilder(_expression);
        }

        /// <summary>
        /// The builder
        /// </summary>
        private DeleteDefaultConstraintExpressionBuilder _builder;
        /// <summary>
        /// The expression
        /// </summary>
        private DeleteDefaultConstraintExpression _expression;

        /// <summary>
        /// Defines the test method OnColumnShouldSetColumnNameOnExpression.
        /// </summary>
        [Test]
        public void OnColumnShouldSetColumnNameOnExpression()
        {
            _builder.OnColumn("column");
            _expression.ColumnName.ShouldBe("column");
        }

        /// <summary>
        /// Defines the test method OnSchemaShouldSetSchemaNameOnExpression.
        /// </summary>
        [Test]
        public void OnSchemaShouldSetSchemaNameOnExpression()
        {
            _builder.InSchema("Shema");
            _expression.SchemaName.ShouldBe("Shema");
        }

        /// <summary>
        /// Defines the test method OnTableShouldSetTableNameOnExpression.
        /// </summary>
        [Test]
        public void OnTableShouldSetTableNameOnExpression()
        {
            _builder.OnTable("ThaTable");
            _expression.TableName.ShouldBe("ThaTable");
        }
    }
}
