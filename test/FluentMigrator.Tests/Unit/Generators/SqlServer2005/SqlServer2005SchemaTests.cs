// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SqlServer2005SchemaTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using FluentMigrator.Builders.Create.Schema;
using FluentMigrator.Runner.Generators.SqlServer;
using FluentMigrator.SqlServer;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Generators.SqlServer2005
{
    /// <summary>
    /// Defines test class SqlServer2005SchemaTests.
    /// Implements the <see cref="FluentMigrator.Tests.Unit.Generators.BaseSchemaTests" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Unit.Generators.BaseSchemaTests" />
    [TestFixture]
    public class SqlServer2005SchemaTests : BaseSchemaTests
    {
        /// <summary>
        /// The generator
        /// </summary>
        protected SqlServer2005Generator Generator;

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            Generator = new SqlServer2005Generator();
        }

        /// <summary>
        /// Defines the test method CanAlterSchema.
        /// </summary>
        [Test]
        public override void CanAlterSchema()
        {
            var expression = GeneratorTestHelper.GetAlterSchemaExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER SCHEMA [TestSchema2] TRANSFER [TestSchema1].[TestTable]");
        }

        /// <summary>
        /// Defines the test method CanCreateSchema.
        /// </summary>
        [Test]
        public override void CanCreateSchema()
        {
            var expression = GeneratorTestHelper.GetCreateSchemaExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE SCHEMA [TestSchema]");
        }

        /// <summary>
        /// Defines the test method CanDropSchema.
        /// </summary>
        [Test]
        public override void CanDropSchema()
        {
            var expression = GeneratorTestHelper.GetDeleteSchemaExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("DROP SCHEMA [TestSchema]");
        }

        /// <summary>
        /// Defines the test method CanCreateSchemaWithAuthorization.
        /// </summary>
        [Test]
        public void CanCreateSchemaWithAuthorization()
        {
            var expression = GeneratorTestHelper.GetCreateSchemaExpression();
            var builder = new CreateSchemaExpressionBuilder(expression);
            builder.Authorization("dbo");

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE SCHEMA [TestSchema] AUTHORIZATION [dbo]");
        }
    }
}
