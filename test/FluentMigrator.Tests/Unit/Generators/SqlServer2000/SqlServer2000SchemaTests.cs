// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SqlServer2000SchemaTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using FluentMigrator.Runner.Generators.SqlServer;
using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Generators.SqlServer2000
{
    /// <summary>
    /// Defines test class SqlServer2000SchemaTests.
    /// Implements the <see cref="FluentMigrator.Tests.Unit.Generators.BaseSchemaTests" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Unit.Generators.BaseSchemaTests" />
    [TestFixture]
    public class SqlServer2000SchemaTests : BaseSchemaTests
    {
        /// <summary>
        /// The generator
        /// </summary>
        protected SqlServer2000Generator Generator;

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            Generator = new SqlServer2000Generator();
        }

        /// <summary>
        /// Defines the test method CanAlterSchema.
        /// </summary>
        [Test]
        public override void CanAlterSchema()
        {
            var expression = GeneratorTestHelper.GetAlterSchemaExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe(string.Empty);
        }

        /// <summary>
        /// Defines the test method CanCreateSchema.
        /// </summary>
        [Test]
        public override void CanCreateSchema()
        {
            var expression = GeneratorTestHelper.GetCreateSchemaExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe(string.Empty);
        }

        /// <summary>
        /// Defines the test method CanDropSchema.
        /// </summary>
        [Test]
        public override void CanDropSchema()
        {
            var expression = GeneratorTestHelper.GetDeleteSchemaExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe(string.Empty);
        }
    }
}
