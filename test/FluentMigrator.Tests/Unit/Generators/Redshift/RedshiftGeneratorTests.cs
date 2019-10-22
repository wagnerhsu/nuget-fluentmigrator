// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="RedshiftGeneratorTests.cs" company="FluentMigrator Project">
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

using System;
using System.Collections.Generic;
using System.Data;

using FluentMigrator.Expressions;
using FluentMigrator.Model;
using FluentMigrator.Runner.Generators.Redshift;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Generators.Redshift
{
    /// <summary>
    /// Class RedshiftGeneratorTests. This class cannot be inherited.
    /// </summary>
    [TestFixture]
    public sealed class RedshiftGeneratorTests
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
        /// Defines the test method CanCreateTableWithBoolDefaultValue.
        /// </summary>
        [Test]
        public void CanCreateTableWithBoolDefaultValue()
        {
            var expression = GeneratorTestHelper.GetCreateTableExpression();
            expression.Columns[0].DefaultValue = true;

            var result = _generator.Generate(expression);
            result.ShouldBe("CREATE TABLE \"public\".\"TestTable1\" (\"TestColumn1\" text NOT NULL DEFAULT true, \"TestColumn2\" integer NOT NULL);");
        }

        /// <summary>
        /// Defines the test method CanUseSystemMethodCurrentUserAsADefaultValueForAColumn.
        /// </summary>
        [Test]
        public void CanUseSystemMethodCurrentUserAsADefaultValueForAColumn()
        {
            const string tableName = "NewTable";
            var columnDefinition = new ColumnDefinition { Name = "NewColumn", Size = 15, Type = DbType.String, DefaultValue = SystemMethods.CurrentUser };
            var expression = new CreateColumnExpression { Column = columnDefinition, TableName = tableName };

            var result = _generator.Generate(expression);
            result.ShouldBe("ALTER TABLE \"public\".\"NewTable\" ADD \"NewColumn\" varchar(15) NOT NULL DEFAULT current_user;");
        }

        /// <summary>
        /// Defines the test method CanUseSystemMethodCurrentUTCDateTimeAsADefaultValueForAColumn.
        /// </summary>
        [Test]
        public void CanUseSystemMethodCurrentUTCDateTimeAsADefaultValueForAColumn()
        {
            const string tableName = "NewTable";
            var columnDefinition = new ColumnDefinition { Name = "NewColumn", Size = 5, Type = DbType.String, DefaultValue = SystemMethods.CurrentUTCDateTime };
            var expression = new CreateColumnExpression { Column = columnDefinition, TableName = tableName };

            var result = _generator.Generate(expression);
            result.ShouldBe("ALTER TABLE \"public\".\"NewTable\" ADD \"NewColumn\" varchar(5) NOT NULL DEFAULT (SYSDATE at time zone 'UTC');");
        }

        /// <summary>
        /// Defines the test method NonUnicodeQuotesCorrectly.
        /// </summary>
        [Test]
        public void NonUnicodeQuotesCorrectly()
        {
            var expression = new InsertDataExpression { TableName = "TestTable" };
            expression.Rows.Add(new InsertionDataDefinition
            {
                new KeyValuePair<string, object>("NormalString", "Just'in"),
                new KeyValuePair<string, object>("UnicodeString", new NonUnicodeString("codethinked'.com"))
            });

            var result = _generator.Generate(expression);
            result.ShouldBe("INSERT INTO \"public\".\"TestTable\" (\"NormalString\",\"UnicodeString\") VALUES ('Just''in','codethinked''.com');");
        }

        /// <summary>
        /// Defines the test method ExplicitUnicodeStringIgnoredForNonSqlServer.
        /// </summary>
        [Test]
        [Obsolete]
        public void ExplicitUnicodeStringIgnoredForNonSqlServer()
        {
            var expression = new InsertDataExpression {TableName = "TestTable"};
            expression.Rows.Add(new InsertionDataDefinition
                                    {
                                        new KeyValuePair<string, object>("NormalString", "Just'in"),
                                        new KeyValuePair<string, object>("UnicodeString", new ExplicitUnicodeString("codethinked'.com"))
                                    });

            var result = _generator.Generate(expression);
            result.ShouldBe("INSERT INTO \"public\".\"TestTable\" (\"NormalString\",\"UnicodeString\") VALUES ('Just''in','codethinked''.com');");
        }

        /// <summary>
        /// Defines the test method CanAlterColumnAndSetAsNullable.
        /// </summary>
        [Test]
        public void CanAlterColumnAndSetAsNullable()
        {
            var expression = new AlterColumnExpression
            {
                Column = new ColumnDefinition { Type = DbType.String, Name = "TestColumn1", IsNullable = true },
                SchemaName = "TestSchema",
                TableName = "TestTable1"
            };

            var result = _generator.Generate(expression);
            result.ShouldBe("ALTER TABLE \"TestSchema\".\"TestTable1\" ALTER \"TestColumn1\" TYPE text, ALTER \"TestColumn1\" DROP NOT NULL;");
        }

        /// <summary>
        /// Defines the test method CanAlterColumnAndSetAsNotNullable.
        /// </summary>
        [Test]
        public void CanAlterColumnAndSetAsNotNullable()
        {
            var expression = new AlterColumnExpression
            {
                Column = new ColumnDefinition { Type = DbType.String, Name = "TestColumn1", IsNullable = false },
                SchemaName = "TestSchema",
                TableName = "TestTable1"
            };

            var result = _generator.Generate(expression);
            result.ShouldBe("ALTER TABLE \"TestSchema\".\"TestTable1\" ALTER \"TestColumn1\" TYPE text, ALTER \"TestColumn1\" SET NOT NULL;");
        }

        /// <summary>
        /// Defines the test method CanDeleteDefaultConstraint.
        /// </summary>
        [Test]
        public void CanDeleteDefaultConstraint()
        {
            var expression = new DeleteDefaultConstraintExpression
            {
                ColumnName = "TestColumn1",
                SchemaName = "TestSchema",
                TableName = "TestTable1"
            };

            var result = _generator.Generate(expression);
            result.ShouldBe("ALTER TABLE \"TestSchema\".\"TestTable1\" ALTER \"TestColumn1\" DROP DEFAULT;");
        }

        /// <summary>
        /// Defines the test method CanAlterDefaultConstraintToCurrentUser.
        /// </summary>
        [Test]
        public void CanAlterDefaultConstraintToCurrentUser()
        {
            var expression = GeneratorTestHelper.GetAlterDefaultConstraintExpression();
            expression.DefaultValue = SystemMethods.CurrentUser;
            expression.SchemaName = "TestSchema";

            var result = _generator.Generate(expression);
            result.ShouldBe("ALTER TABLE \"TestSchema\".\"TestTable1\" ALTER \"TestColumn1\" DROP DEFAULT, ALTER \"TestColumn1\" SET DEFAULT current_user;");
        }

        /// <summary>
        /// Defines the test method CanAlterDefaultConstraintToCurrentDate.
        /// </summary>
        [Test]
        public void CanAlterDefaultConstraintToCurrentDate()
        {
            var expression = GeneratorTestHelper.GetAlterDefaultConstraintExpression();
            expression.DefaultValue = SystemMethods.CurrentDateTime;
            expression.SchemaName = "TestSchema";

            var result = _generator.Generate(expression);
            result.ShouldBe("ALTER TABLE \"TestSchema\".\"TestTable1\" ALTER \"TestColumn1\" DROP DEFAULT, ALTER \"TestColumn1\" SET DEFAULT SYSDATE;");
        }

        /// <summary>
        /// Defines the test method CanAlterDefaultConstraintToCurrentUtcDateTime.
        /// </summary>
        [Test]
        public void CanAlterDefaultConstraintToCurrentUtcDateTime()
        {
            var expression = GeneratorTestHelper.GetAlterDefaultConstraintExpression();
            expression.DefaultValue = SystemMethods.CurrentUTCDateTime;
            expression.SchemaName = "TestSchema";

            var result = _generator.Generate(expression);
            result.ShouldBe("ALTER TABLE \"TestSchema\".\"TestTable1\" ALTER \"TestColumn1\" DROP DEFAULT, ALTER \"TestColumn1\" SET DEFAULT (SYSDATE at time zone 'UTC');");
        }

        /// <summary>
        /// Defines the test method CanAlterColumnAndOnlySetTypeIfIsNullableNotSet.
        /// </summary>
        [Test]
        public void CanAlterColumnAndOnlySetTypeIfIsNullableNotSet()
        {
            var expression = new AlterColumnExpression
            {
                Column = new ColumnDefinition { Type = DbType.String, Name = "TestColumn1", IsNullable = null },
                SchemaName = "TestSchema",
                TableName = "TestTable1"
            };

            var result = _generator.Generate(expression);
            result.ShouldBe("ALTER TABLE \"TestSchema\".\"TestTable1\" ALTER \"TestColumn1\" TYPE text;");
        }
    }
}
