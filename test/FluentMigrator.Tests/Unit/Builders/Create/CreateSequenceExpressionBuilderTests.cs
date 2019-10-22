// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="CreateSequenceExpressionBuilderTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
// 
// Copyright (c) 2007-2018, Sean Chambers <schambers80@gmail.com>
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

using System;
using FluentMigrator.Expressions;
using FluentMigrator.Model;
using Moq;
using NUnit.Framework;

namespace FluentMigrator.Tests.Unit.Builders.Create
{
    using FluentMigrator.Builders.Create.Sequence;

    /// <summary>
    /// Defines test class CreateSequenceExpressionBuilderTests.
    /// </summary>
    [TestFixture]
    public class CreateSequenceExpressionBuilderTests
    {
        /// <summary>
        /// Defines the test method CallingInSchemaSetsSchemaName.
        /// </summary>
        [Test]
        public void CallingInSchemaSetsSchemaName()
        {
            VerifySequenceProperty(c => c.SchemaName = "Schema", b => b.InSchema("Schema"));
        }

        /// <summary>
        /// Defines the test method CallingIncrementBySetsIncrement.
        /// </summary>
        [Test]
        public void CallingIncrementBySetsIncrement()
        {
            VerifySequenceProperty(c => c.Increment = 10, b => b.IncrementBy(10));
        }

        /// <summary>
        /// Defines the test method CallingMinValueSetsMinValue.
        /// </summary>
        [Test]
        public void CallingMinValueSetsMinValue()
        {
            VerifySequenceProperty(c => c.MinValue = 10, b => b.MinValue(10));
        }

        /// <summary>
        /// Defines the test method CallingMaxValueSetsMaxValue.
        /// </summary>
        [Test]
        public void CallingMaxValueSetsMaxValue()
        {
            VerifySequenceProperty(c => c.MaxValue = 10, b => b.MaxValue(10));
        }

        /// <summary>
        /// Defines the test method CallingStartWithSetsStartWith.
        /// </summary>
        [Test]
        public void CallingStartWithSetsStartWith()
        {
            VerifySequenceProperty(c => c.StartWith = 10, b => b.StartWith(10));
        }

        /// <summary>
        /// Defines the test method CallingCacheSetsCache.
        /// </summary>
        [Test]
        public void CallingCacheSetsCache()
        {
            VerifySequenceProperty(c => c.Cache = 10, b => b.Cache(10));
        }

        /// <summary>
        /// Defines the test method CallingCycleSetsCycleToTrue.
        /// </summary>
        [Test]
        public void CallingCycleSetsCycleToTrue()
        {
            VerifySequenceProperty(c => c.Cycle = true, b => b.Cycle());
        }

        /// <summary>
        /// Verifies the sequence property.
        /// </summary>
        /// <param name="sequenceExpression">The sequence expression.</param>
        /// <param name="callToTest">The call to test.</param>
        private void VerifySequenceProperty(Action<SequenceDefinition> sequenceExpression, Action<CreateSequenceExpressionBuilder> callToTest)
        {
            var sequenceMock = new Mock<SequenceDefinition>();

            var expressionMock = new Mock<CreateSequenceExpression>();
            expressionMock.SetupProperty(e => e.Sequence);

            var expression = expressionMock.Object;
            expression.Sequence = sequenceMock.Object;

            callToTest(new CreateSequenceExpressionBuilder(expression));

            sequenceMock.VerifySet(sequenceExpression);
        }
    }
}