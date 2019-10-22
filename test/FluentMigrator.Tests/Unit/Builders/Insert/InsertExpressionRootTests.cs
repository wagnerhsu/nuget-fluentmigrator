// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="InsertExpressionRootTests.cs" company="FluentMigrator Project">
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
using FluentMigrator.Builders.Insert;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using Moq;
using NUnit.Framework;

namespace FluentMigrator.Tests.Unit.Builders.Insert
{
    /// <summary>
    /// Defines test class InsertExpressionRootTests.
    /// </summary>
    [TestFixture]
    public class InsertExpressionRootTests
    {
        /// <summary>
        /// Defines the test method CallingIntoTableSetsTableName.
        /// </summary>
        [Test]
        public void CallingIntoTableSetsTableName()
        {
            var collectionMock = new Mock<ICollection<IMigrationExpression>>();

            var contextMock = new Mock<IMigrationContext>();
            contextMock.Setup(x => x.Expressions).Returns(collectionMock.Object);

            var root = new InsertExpressionRoot(contextMock.Object);
            root.IntoTable("Bacon");

            collectionMock.Verify(x => x.Add(It.Is<InsertDataExpression>(e => e.TableName.Equals("Bacon"))));
            contextMock.VerifyGet(x => x.Expressions);
        }
    }
}