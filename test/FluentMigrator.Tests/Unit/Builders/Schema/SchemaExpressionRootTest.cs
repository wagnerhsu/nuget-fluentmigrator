// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SchemaExpressionRootTest.cs" company="FluentMigrator Project">
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

using FluentMigrator.Builders.Schema;
using FluentMigrator.Infrastructure;
using Moq;
using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Builders.Schema
{
    /// <summary>
    /// Defines test class SchemaExpressionRootTest.
    /// </summary>
    [TestFixture]
    public class SchemaExpressionRootTest
    {
        /// <summary>
        /// The query schema mock
        /// </summary>
        private Mock<IQuerySchema> _querySchemaMock;
        /// <summary>
        /// The migration context mock
        /// </summary>
        private Mock<IMigrationContext> _migrationContextMock;
        /// <summary>
        /// The test column
        /// </summary>
        private string _testColumn;
        /// <summary>
        /// The test constraint
        /// </summary>
        private string _testConstraint;
        /// <summary>
        /// The test index
        /// </summary>
        private string _testIndex;
        /// <summary>
        /// The test table
        /// </summary>
        private string _testTable;
        /// <summary>
        /// The test schema
        /// </summary>
        private string _testSchema;
        /// <summary>
        /// The builder
        /// </summary>
        private SchemaExpressionRoot _builder;

        /// <summary>
        /// Sets up.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            _migrationContextMock = new Mock<IMigrationContext>();
            _querySchemaMock = new Mock<IQuerySchema>();
            _testSchema = "testSchema";
            _testTable = "testTable";
            _testIndex = "testIndex";
            _testColumn = "testColumn";
            _testConstraint = "testConstraint";

            _migrationContextMock.Setup(x => x.QuerySchema).Returns(_querySchemaMock.Object);
            _builder = new SchemaExpressionRoot(_migrationContextMock.Object);
        }

        /// <summary>
        /// Defines the test method TestTableExists.
        /// </summary>
        [Test]
        public void TestTableExists()
        {
            _querySchemaMock.Setup(x => x.TableExists(null, _testTable)).Returns(true);

            _builder.Table(_testTable).Exists().ShouldBeTrue();
            _querySchemaMock.Verify(x => x.TableExists(null, _testTable));
        }

        /// <summary>
        /// Defines the test method TestConstraintExists.
        /// </summary>
        [Test]
        public void TestConstraintExists()
        {
            _querySchemaMock.Setup(x => x.ConstraintExists(null, _testTable, _testConstraint)).Returns(true);

            _builder.Table(_testTable).Constraint(_testConstraint).Exists().ShouldBeTrue();
            _querySchemaMock.Verify(x => x.ConstraintExists(null, _testTable, _testConstraint));
        }

        /// <summary>
        /// Defines the test method TestColumnExists.
        /// </summary>
        [Test]
        public void TestColumnExists()
        {
            _querySchemaMock.Setup(x => x.ColumnExists(null, _testTable, _testColumn)).Returns(true);

            _builder.Table(_testTable).Column(_testColumn).Exists().ShouldBeTrue();
            _querySchemaMock.Verify(x => x.ColumnExists(null, _testTable, _testColumn));
        }

        /// <summary>
        /// Defines the test method TestIndexExists.
        /// </summary>
        [Test]
        public void TestIndexExists()
        {
            _querySchemaMock.Setup(x => x.IndexExists(null, _testTable, _testIndex)).Returns(true);


            _builder.Table(_testTable).Index(_testIndex).Exists().ShouldBeTrue();
            _querySchemaMock.Verify(x => x.IndexExists(null, _testTable, _testIndex));
        }

        /// <summary>
        /// Defines the test method TestTableExistsWithSchema.
        /// </summary>
        [Test]
        public void TestTableExistsWithSchema()
        {
            _querySchemaMock.Setup(x => x.TableExists(_testSchema, _testTable)).Returns(true);

            _builder.Schema(_testSchema).Table(_testTable).Exists().ShouldBeTrue();
            _querySchemaMock.Verify(x => x.TableExists(_testSchema, _testTable));
        }

        /// <summary>
        /// Defines the test method TestColumnExistsWithSchema.
        /// </summary>
        [Test]
        public void TestColumnExistsWithSchema()
        {
            _querySchemaMock.Setup(x => x.ColumnExists(_testSchema, _testTable, _testColumn)).Returns(true);

            _builder.Schema(_testSchema).Table(_testTable).Column(_testColumn).Exists().ShouldBeTrue();
            _querySchemaMock.Verify(x => x.ColumnExists(_testSchema, _testTable, _testColumn));
        }

        /// <summary>
        /// Defines the test method TestConstraintExistsWithSchema.
        /// </summary>
        [Test]
        public void TestConstraintExistsWithSchema()
        {
            _querySchemaMock.Setup(x => x.ConstraintExists(_testSchema, _testTable, _testConstraint)).Returns(true);

            _builder.Schema(_testSchema).Table(_testTable).Constraint(_testConstraint).Exists().ShouldBeTrue();
            _querySchemaMock.Verify(x => x.ConstraintExists(_testSchema, _testTable, _testConstraint));
        }

        /// <summary>
        /// Defines the test method TestIndexExistsWithSchema.
        /// </summary>
        [Test]
        public void TestIndexExistsWithSchema()
        {
            _querySchemaMock.Setup(x => x.IndexExists(_testSchema, _testTable, _testIndex)).Returns(true);

            _builder.Schema(_testSchema).Table(_testTable).Index(_testIndex).Exists().ShouldBeTrue();
            _querySchemaMock.Verify(x => x.IndexExists(_testSchema, _testTable, _testIndex));
        }
    }
}
