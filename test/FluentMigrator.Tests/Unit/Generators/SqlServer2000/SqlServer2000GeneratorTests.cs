// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SqlServer2000GeneratorTests.cs" company="FluentMigrator Project">
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

using System;
using System.Data;

using FluentMigrator.Exceptions;
using FluentMigrator.Expressions;
using FluentMigrator.Model;
using FluentMigrator.Runner.Generators.SqlServer;
using FluentMigrator.SqlServer;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Generators.SqlServer2000
{
    /// <summary>
    /// Defines test class SqlServer2000GeneratorTests.
    /// </summary>
    [TestFixture]
    public class SqlServer2000GeneratorTests
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
        /// Defines the test method CanAlterColumnWithDefaultValue.
        /// </summary>
        [Test]
        public void CanAlterColumnWithDefaultValue()
        {
            //TODO: This will fail if there are any keys attached
            var expression = GeneratorTestHelper.GetAlterColumnExpression();
            expression.Column.DefaultValue = "Foo";

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE [TestTable1] ALTER COLUMN [TestColumn1] NVARCHAR(20) NOT NULL");
        }

        /// <summary>
        /// Defines the test method CanAlterDefaultConstraint.
        /// </summary>
        [Test]
        public void CanAlterDefaultConstraint()
        {
            var expression = GeneratorTestHelper.GetAlterDefaultConstraintExpression();

            string expected = "DECLARE @default sysname, @sql nvarchar(4000);" + Environment.NewLine + Environment.NewLine +
            "-- get name of default constraint" + Environment.NewLine +
            "SELECT @default = name" + Environment.NewLine +
            "FROM sys.default_constraints" + Environment.NewLine +
            "WHERE parent_object_id = object_id('[TestTable1]')" + Environment.NewLine + "" +
            "AND type = 'D'" + Environment.NewLine + "" +
            "AND parent_column_id = (" + Environment.NewLine +
            "SELECT column_id" + Environment.NewLine +
            "FROM sys.columns" + Environment.NewLine +
            "WHERE object_id = object_id('[TestTable1]')" + Environment.NewLine +
            "AND name = 'TestColumn1'" + Environment.NewLine +
            ");" + Environment.NewLine + Environment.NewLine +
            "-- create alter table command to drop constraint as string and run it" + Environment.NewLine +
            "SET @sql = N'ALTER TABLE [TestTable1] DROP CONSTRAINT ' + QUOTENAME(@default);" + Environment.NewLine +
            "EXEC sp_executesql @sql;" + Environment.NewLine + Environment.NewLine +
            "-- create alter table command to create new default constraint as string and run it" + Environment.NewLine +
            "ALTER TABLE [TestTable1] WITH NOCHECK ADD CONSTRAINT [DF_TestTable1_TestColumn1] DEFAULT(1) FOR [TestColumn1];";

            var result = Generator.Generate(expression);
            result.ShouldBe(expected);
        }

        /// <summary>
        /// Defines the test method CanAlterSchemaInStrictMode.
        /// </summary>
        [Test]
        public void CanAlterSchemaInStrictMode()
        {
            Generator.CompatibilityMode = Runner.CompatibilityMode.STRICT;

            Assert.Throws<DatabaseOperationNotSupportedException>(() => Generator.Generate(new CreateSchemaExpression()));
        }

        /// <summary>
        /// Defines the test method CanAddColumnWithGetDateDefault.
        /// </summary>
        [Test]
        public void CanAddColumnWithGetDateDefault()
        {
            var column = new ColumnDefinition
            {
                Name = "TestColumn1",
                Type = DbType.String,
                Size = 5,
                DefaultValue = "GetDate()"
            };
            var expression = new CreateColumnExpression { TableName = "TestTable1", Column = column };

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE [TestTable1] ADD [TestColumn1] NVARCHAR(5) NOT NULL DEFAULT GetDate()");
        }

        /// <summary>
        /// Defines the test method CanCreateSchemaInStrictMode.
        /// </summary>
        [Test]
        public void CanCreateSchemaInStrictMode()
        {
            Generator.CompatibilityMode = Runner.CompatibilityMode.STRICT;

            Assert.Throws<DatabaseOperationNotSupportedException>(() => Generator.Generate(new CreateSchemaExpression()));
        }

        /// <summary>
        /// Defines the test method CanCreateTableWithGetDateDefault.
        /// </summary>
        [Test]
        public void CanCreateTableWithGetDateDefault()
        {
            var column = new ColumnDefinition
            {
                Name = "TestColumn1",
                Type = DbType.String,
                Size = 5,
                DefaultValue = "GetDate()"
            };
            var expression = new CreateTableExpression { TableName = "TestTable1" };
            expression.Columns.Add(column);

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE TABLE [TestTable1] ([TestColumn1] NVARCHAR(5) NOT NULL DEFAULT GetDate())");
        }

        /// <summary>
        /// Defines the test method CanCreateTableWithSeededIdentity.
        /// </summary>
        [Test]
        public void CanCreateTableWithSeededIdentity()
        {
            var expression = GeneratorTestHelper.GetCreateTableWithAutoIncrementExpression();
            expression.Columns[0].AdditionalFeatures.Add(SqlServerExtensions.IdentitySeed, 45);
            expression.Columns[0].AdditionalFeatures.Add(SqlServerExtensions.IdentityIncrement, 23);

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE TABLE [TestTable1] ([TestColumn1] INT NOT NULL IDENTITY(45,23), [TestColumn2] INT NOT NULL)");
        }

        /// <summary>
        /// Defines the test method CanDropSchemaInStrictMode.
        /// </summary>
        [Test]
        public void CanDropSchemaInStrictMode()
        {
            Generator.CompatibilityMode = Runner.CompatibilityMode.STRICT;

            Assert.Throws<DatabaseOperationNotSupportedException>(() => Generator.Generate(new DeleteSchemaExpression()));
        }

        /// <summary>
        /// Defines the test method CanGenerateNecessaryStatementsForADeleteDefaultExpression.
        /// </summary>
        [Test]
        public void CanGenerateNecessaryStatementsForADeleteDefaultExpression()
        {
            var expression = new DeleteDefaultConstraintExpression {ColumnName = "Name", SchemaName = "Personalia", TableName = "Person"};

            var expected = "DECLARE @default sysname, @sql nvarchar(4000);" + Environment.NewLine + Environment.NewLine +
                                    "-- get name of default constraint" + Environment.NewLine +
                                    "SELECT @default = name" + Environment.NewLine +
                                    "FROM sys.default_constraints" + Environment.NewLine +
                                    "WHERE parent_object_id = object_id('[Person]')" + Environment.NewLine +
                                    "AND type = 'D'" + Environment.NewLine +
                                    "AND parent_column_id = (" + Environment.NewLine +
                                    "SELECT column_id" + Environment.NewLine +
                                    "FROM sys.columns" + Environment.NewLine +
                                    "WHERE object_id = object_id('[Person]')" + Environment.NewLine +
                                    "AND name = 'Name'" + Environment.NewLine +
                                    ");" + Environment.NewLine + Environment.NewLine +
                                    "-- create alter table command to drop constraint as string and run it" + Environment.NewLine +
                                    "SET @sql = N'ALTER TABLE [Person] DROP CONSTRAINT ' + QUOTENAME(@default);" + Environment.NewLine +
                                    "EXEC sp_executesql @sql;";

            var result = Generator.Generate(expression);
            result.ShouldBe(expected);
        }
    }
}
