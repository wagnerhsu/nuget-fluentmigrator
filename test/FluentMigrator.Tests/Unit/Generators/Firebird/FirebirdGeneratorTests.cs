// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="FirebirdGeneratorTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using NUnit.Framework;

using FluentMigrator.Runner.Processors.Firebird;
using FluentMigrator.Runner.Generators.Firebird;
using FluentMigrator.Expressions;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Generators.Firebird
{
    /// <summary>
    /// Defines test class FirebirdGeneratorTests.
    /// </summary>
    [TestFixture]
    public class FirebirdGeneratorTests
    {
        /// <summary>
        /// Gets or sets the generator.
        /// </summary>
        /// <value>The generator.</value>
        protected FirebirdGenerator Generator { get; set; }

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            Generator = new FirebirdGenerator(FirebirdOptions.StandardBehaviour());
        }

        /// <summary>
        /// Defines the test method CanAlterSequence.
        /// </summary>
        [Test]
        public void CanAlterSequence()
        {
            var expression = new CreateSequenceExpression
            {
                Sequence =
                {
                    Cache = 10,
                    Cycle = true,
                    Increment = 2,
                    MaxValue = 100,
                    MinValue = 0,
                    Name = "Sequence",
                    SchemaName = "Schema",
                    StartWith = 2
                }
            };

            var result = Generator.GenerateAlterSequence(expression.Sequence);
            result.ShouldBe("ALTER SEQUENCE \"Sequence\" RESTART WITH 2");
        }
    }
}
