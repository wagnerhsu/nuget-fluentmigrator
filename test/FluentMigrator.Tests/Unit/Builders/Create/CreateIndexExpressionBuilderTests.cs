// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="CreateIndexExpressionBuilderTests.cs" company="FluentMigrator Project">
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

using System.Collections.Generic;

using FluentMigrator.Builders.Create.Index;
using FluentMigrator.Expressions;
using FluentMigrator.Model;
using FluentMigrator.SqlServer;

using Moq;

using NUnit.Framework;

namespace FluentMigrator.Tests.Unit.Builders.Create
{
    /// <summary>
    /// Defines test class CreateIndexExpressionBuilderTests.
    /// </summary>
    [TestFixture]
    public class CreateIndexExpressionBuilderTests
    {
        /// <summary>
        /// Defines the test method CallingOnTableSetsTableNameToSpecifiedValue.
        /// </summary>
        [Test]
        public void CallingOnTableSetsTableNameToSpecifiedValue()
        {
            var indexMock = new Mock<IndexDefinition>();


            var expressionMock = new Mock<CreateIndexExpression>();
            expressionMock.SetupGet(e => e.Index).Returns(indexMock.Object);

            var builder = new CreateIndexExpressionBuilder(expressionMock.Object);
            builder.OnTable("Bacon");

            indexMock.VerifySet(x => x.TableName = "Bacon");
            expressionMock.VerifyGet(e => e.Index);
        }

        /// <summary>
        /// Defines the test method CallingOnColumnAddsNewColumnToExpression.
        /// </summary>
        [Test]
        public void CallingOnColumnAddsNewColumnToExpression()
        {
            var collectionMock = new Mock<IList<IndexColumnDefinition>>();

            var indexMock = new Mock<IndexDefinition>();
            indexMock.Setup(x => x.Columns).Returns(collectionMock.Object);

            var expressionMock = new Mock<CreateIndexExpression>();
            expressionMock.SetupGet(e => e.Index).Returns(indexMock.Object);

            var builder = new CreateIndexExpressionBuilder(expressionMock.Object);
            builder.OnColumn("BaconId");

            collectionMock.Verify(x => x.Add(It.Is<IndexColumnDefinition>(c => c.Name.Equals("BaconId"))));
            indexMock.VerifyGet(x => x.Columns);
            expressionMock.VerifyGet(e => e.Index);
        }

        /// <summary>
        /// Defines the test method CallingAscendingSetsDirectionToAscending.
        /// </summary>
        [Test]
        public void CallingAscendingSetsDirectionToAscending()
        {
            var columnMock = new Mock<IndexColumnDefinition>();
            var expressionMock = new Mock<CreateIndexExpression>();

            var builder = new CreateIndexExpressionBuilder(expressionMock.Object);
            builder.CurrentColumn = columnMock.Object;

            builder.Ascending();

            columnMock.VerifySet(c => c.Direction = Direction.Ascending);
        }

        /// <summary>
        /// Defines the test method CallingDescendingSetsDirectionToDescending.
        /// </summary>
        [Test]
        public void CallingDescendingSetsDirectionToDescending()
        {
            var columnMock = new Mock<IndexColumnDefinition>();
            var expressionMock = new Mock<CreateIndexExpression>();

            var builder = new CreateIndexExpressionBuilder(expressionMock.Object);
            builder.CurrentColumn = columnMock.Object;

            builder.Descending();

            columnMock.VerifySet(c => c.Direction = Direction.Descending);
        }

        /// <summary>
        /// Defines the test method CallingIncludeAddsNewIncludeToExpression.
        /// </summary>
        [Test]
        public void CallingIncludeAddsNewIncludeToExpression()
        {
            var collectionMock = new Mock<IList<IndexIncludeDefinition>>();

            var additionalFeatures = new Dictionary<string, object>()
            {
                [SqlServerExtensions.IncludesList] = collectionMock.Object
            };

            var indexMock = new Mock<IndexDefinition>();
            indexMock.Setup(x => x.AdditionalFeatures).Returns(additionalFeatures);

            var expressionMock = new Mock<CreateIndexExpression>();
            expressionMock.SetupGet(e => e.Index).Returns(indexMock.Object);

            ICreateIndexOnColumnOrInSchemaSyntax builder = new CreateIndexExpressionBuilder(expressionMock.Object);
            builder.Include("BaconId");

            collectionMock.Verify(x => x.Add(It.Is<IndexIncludeDefinition>(c => c.Name.Equals("BaconId"))));
            indexMock.VerifyGet(x => x.AdditionalFeatures);
            expressionMock.VerifyGet(e => e.Index);
        }
    }
}
