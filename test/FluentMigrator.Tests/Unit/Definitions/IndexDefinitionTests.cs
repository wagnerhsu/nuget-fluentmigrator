// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="IndexDefinitionTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using FluentMigrator.Expressions;
using FluentMigrator.Model;
using FluentMigrator.Runner;

using NUnit.Framework;

namespace FluentMigrator.Tests.Unit.Definitions
{
    /// <summary>
    /// Defines test class IndexDefinitionTests.
    /// </summary>
    [TestFixture]
    public class IndexDefinitionTests
    {
        /// <summary>
        /// Defines the test method ShouldApplyIndexNameConventionWhenIndexNameIsNull.
        /// </summary>
        [Test]
        public void ShouldApplyIndexNameConventionWhenIndexNameIsNull()
        {
            var expr = new CreateIndexExpression()
            {
                Index =
                {
                    TableName = "Table",
                    Columns =
                    {
                        new IndexColumnDefinition() {Name = "Name"}
                    }
                }
            };

            var processed = expr.Apply(ConventionSets.NoSchemaName);

            Assert.AreEqual("IX_Table_Name", processed.Index.Name);
        }
    }
}
