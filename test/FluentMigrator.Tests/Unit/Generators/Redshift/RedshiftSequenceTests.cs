// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="RedshiftSequenceTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
// Copyright (c) 2007-2018, FluentMigrator Project
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using FluentMigrator.Runner.Generators.Redshift;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Generators.Redshift
{
    /// <summary>
    /// Class RedshiftSequenceTests. This class cannot be inherited.
    /// Implements the <see cref="FluentMigrator.Tests.Unit.Generators.BaseSequenceTests" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Unit.Generators.BaseSequenceTests" />
    [TestFixture]
    public sealed class RedshiftSequenceTests : BaseSequenceTests
    {
        /// <summary>
        /// The generator
        /// </summary>
        private RedshiftGenerator _generator;

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _generator = new RedshiftGenerator();
        }

        /// <summary>
        /// Defines the test method CanCreateSequenceWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanCreateSequenceWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetCreateSequenceExpression();
            expression.Sequence.SchemaName = "TestSchema";

            var result = _generator.Generate(expression);
            result.ShouldBe(string.Empty);
        }

        /// <summary>
        /// Defines the test method CanCreateSequenceWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateSequenceWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateSequenceExpression();

            var result = _generator.Generate(expression);
            result.ShouldBe(string.Empty);
        }

        /// <summary>
        /// Defines the test method CanDropSequenceWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanDropSequenceWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetDeleteSequenceExpression();
            expression.SchemaName = "TestSchema";

            var result = _generator.Generate(expression);
            result.ShouldBe(string.Empty);
        }

        /// <summary>
        /// Defines the test method CanDropSequenceWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanDropSequenceWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetDeleteSequenceExpression();

            var result = _generator.Generate(expression);
            result.ShouldBe(string.Empty);
        }
    }
}
