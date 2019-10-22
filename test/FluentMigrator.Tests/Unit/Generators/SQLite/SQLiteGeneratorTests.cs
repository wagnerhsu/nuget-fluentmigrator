// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SQLiteGeneratorTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
//
// Copyright (c) 2018, Fluent Migrator Project
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

using System.Data;

using FluentMigrator.Exceptions;
using FluentMigrator.Expressions;
using FluentMigrator.Model;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Generators.SQLite;
using FluentMigrator.SqlServer;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Generators.SQLite
{
    /// <summary>
    /// Defines test class SQLiteGeneratorTests.
    /// </summary>
    [TestFixture]
    // ReSharper disable once InconsistentNaming
    public class SQLiteGeneratorTests
    {
        /// <summary>
        /// The generator
        /// </summary>
        protected SQLiteGenerator Generator;

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            Generator = new SQLiteGenerator();
        }

        /// <summary>
        /// Defines the test method CanAlterColumnInStrictMode.
        /// </summary>
        [Test]
        public void CanAlterColumnInStrictMode()
        {
            var expression = GeneratorTestHelper.GetRenameColumnExpression();
            Generator.CompatibilityMode = CompatibilityMode.STRICT;

            Assert.Throws<DatabaseOperationNotSupportedException>(() => Generator.Generate(expression));
        }

        /// <summary>
        /// Defines the test method CanAlterSchemaInStrictMode.
        /// </summary>
        [Test]
        public void CanAlterSchemaInStrictMode()
        {
            Generator.CompatibilityMode = CompatibilityMode.STRICT;

            Assert.Throws<DatabaseOperationNotSupportedException>(() => Generator.Generate(new CreateSchemaExpression()));
        }

        /// <summary>
        /// Defines the test method CanCreateForeignKeyInStrictMode.
        /// </summary>
        [Test]
        public void CanCreateForeignKeyInStrictMode()
        {
            Generator.CompatibilityMode = CompatibilityMode.STRICT;

            Assert.Throws<DatabaseOperationNotSupportedException>(() => Generator.Generate(GeneratorTestHelper.GetCreateNamedForeignKeyExpression()));
        }

        /// <summary>
        /// Defines the test method CanCreateMulitColumnForeignKeyInStrictMode.
        /// </summary>
        [Test]
        public void CanCreateMulitColumnForeignKeyInStrictMode()
        {
            Generator.CompatibilityMode = CompatibilityMode.STRICT;

            Assert.Throws<DatabaseOperationNotSupportedException>(() => Generator.Generate(GeneratorTestHelper.GetCreateNamedMultiColumnForeignKeyExpression()));
        }

        /// <summary>
        /// Defines the test method CanCreateSchemaInStrictMode.
        /// </summary>
        [Test]
        public void CanCreateSchemaInStrictMode()
        {
            Generator.CompatibilityMode = CompatibilityMode.STRICT;

            Assert.Throws<DatabaseOperationNotSupportedException>(() => Generator.Generate(new CreateSchemaExpression()));
        }

        /// <summary>
        /// Defines the test method CanCreateTableWithSeededIdentityAndLooseCompatibility.
        /// </summary>
        [Test]
        public void CanCreateTableWithSeededIdentityAndLooseCompatibility()
        {
            var expression = GeneratorTestHelper.GetCreateTableWithAutoIncrementExpression();
            expression.Columns[0].IsPrimaryKey = true;
            expression.Columns[0].AdditionalFeatures.Add(SqlServerExtensions.IdentitySeed, 3);
            expression.Columns[0].AdditionalFeatures.Add(SqlServerExtensions.IdentityIncrement, 3);
            Generator.CompatibilityMode = CompatibilityMode.LOOSE;

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE TABLE \"TestTable1\" (\"TestColumn1\" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, \"TestColumn2\" INTEGER NOT NULL)");
        }

        /// <summary>
        /// Defines the test method CanDropForeignKeyInStrictMode.
        /// </summary>
        [Test]
        public void CanDropForeignKeyInStrictMode()
        {
            var expression = GeneratorTestHelper.GetDeleteForeignKeyExpression();
            Generator.CompatibilityMode = CompatibilityMode.STRICT;

            Assert.Throws<DatabaseOperationNotSupportedException>(() => Generator.Generate(expression));
        }

        /// <summary>
        /// Defines the test method CanDropSchemaInStrictMode.
        /// </summary>
        [Test]
        public void CanDropSchemaInStrictMode()
        {
            Generator.CompatibilityMode = CompatibilityMode.STRICT;

            Assert.Throws<DatabaseOperationNotSupportedException>(() => Generator.Generate(new DeleteSchemaExpression()));
        }

        /// <summary>
        /// Defines the test method CanNotCreateTableWithSeededIdentityAndStrictCompatibility.
        /// </summary>
        [Test]
        public void CanNotCreateTableWithSeededIdentityAndStrictCompatibility()
        {
            var expression = GeneratorTestHelper.GetCreateTableWithAutoIncrementExpression();
            expression.Columns[0].IsPrimaryKey = true;
            expression.Columns[0].AdditionalFeatures.Add(SqlServerExtensions.IdentitySeed, 3);
            expression.Columns[0].AdditionalFeatures.Add(SqlServerExtensions.IdentityIncrement, 3);
            Generator.CompatibilityMode = CompatibilityMode.STRICT;

            Assert.Throws<DatabaseOperationNotSupportedException>(() => Generator.Generate(expression));
        }

        /// <summary>
        /// Defines the test method CanUseSystemMethodCurrentDateTimeAsADefaultValueForAColumn.
        /// </summary>
        [Test]
        public void CanUseSystemMethodCurrentDateTimeAsADefaultValueForAColumn()
        {
            var expression = new CreateTableExpression { TableName = "TestTable1" };
            expression.Columns.Add(new ColumnDefinition { Name = "DateTimeCol", Type = DbType.DateTime, DefaultValue = SystemMethods.CurrentDateTime});

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE TABLE \"TestTable1\" (\"DateTimeCol\" DATETIME NOT NULL DEFAULT (datetime('now','localtime')))");
        }

        /// <summary>
        /// Defines the test method CanUseSystemMethodCurrentUTCDateTimeAsDefaultValueForColumn.
        /// </summary>
        [Test]
        public void CanUseSystemMethodCurrentUTCDateTimeAsDefaultValueForColumn()
        {
            var expression = new CreateTableExpression { TableName = "TestTable1" };
            expression.Columns.Add(new ColumnDefinition { Name = "DateTimeCol", Type = DbType.DateTime, DefaultValue = SystemMethods.CurrentUTCDateTime });

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE TABLE \"TestTable1\" (\"DateTimeCol\" DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP)");
        }
        /// <summary>
        /// Defines the test method CanRenameColumnInStrictMode.
        /// </summary>
        [Test]
        public void CanRenameColumnInStrictMode()
        {
            Generator.CompatibilityMode = CompatibilityMode.STRICT;

            Assert.Throws<DatabaseOperationNotSupportedException>(() => Generator.Generate(GeneratorTestHelper.GetRenameColumnExpression()));
        }
    }
}
