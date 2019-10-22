// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SqlServerCeSchemaTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using FluentMigrator.Exceptions;
using FluentMigrator.Runner.Generators.SqlServer;
using NUnit.Framework;

namespace FluentMigrator.Tests.Unit.Generators.SqlServerCe
{
    /// <summary>
    /// Defines test class SqlServerCeSchemaTests.
    /// Implements the <see cref="FluentMigrator.Tests.Unit.Generators.BaseSchemaTests" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Unit.Generators.BaseSchemaTests" />
    [TestFixture]
    public class SqlServerCeSchemaTests : BaseSchemaTests
    {
        /// <summary>
        /// The generator
        /// </summary>
        protected SqlServerCeGenerator Generator;

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            Generator = new SqlServerCeGenerator();
        }

        /// <summary>
        /// Defines the test method CanAlterSchema.
        /// </summary>
        [Test]
        public override void CanAlterSchema()
        {
            Assert.Throws<DatabaseOperationNotSupportedException>(() => Generator.Generate(GeneratorTestHelper.GetAlterSchemaExpression()));
        }

        /// <summary>
        /// Defines the test method CanCreateSchema.
        /// </summary>
        [Test]
        public override void CanCreateSchema()
        {
            Assert.Throws<DatabaseOperationNotSupportedException>(() => Generator.Generate(GeneratorTestHelper.GetCreateSchemaExpression()));
        }

        /// <summary>
        /// Defines the test method CanDropSchema.
        /// </summary>
        [Test]
        public override void CanDropSchema()
        {
            Assert.Throws<DatabaseOperationNotSupportedException>(() => Generator.Generate(GeneratorTestHelper.GetDeleteSchemaExpression()));
        }
    }
}
