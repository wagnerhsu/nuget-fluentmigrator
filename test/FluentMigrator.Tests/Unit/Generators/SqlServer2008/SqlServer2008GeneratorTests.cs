// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SqlServer2008GeneratorTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Data;
using FluentMigrator.Expressions;
using FluentMigrator.Model;
using FluentMigrator.Runner.Generators.SqlServer;
using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Generators.SqlServer2008
{
    /// <summary>
    /// Defines test class SqlServer2008GeneratorTests.
    /// </summary>
    [TestFixture]
    public class SqlServer2008GeneratorTests
    {
        /// <summary>
        /// The generator
        /// </summary>
        protected SqlServer2008Generator Generator;

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            Generator = new SqlServer2008Generator();
        }

        /// <summary>
        /// Defines the test method CanCreateTableWithDateTimeOffsetColumn.
        /// </summary>
        [Test]
        public void CanCreateTableWithDateTimeOffsetColumn()
        {
            var expression = new CreateTableExpression {TableName = "TestTable1"};
            expression.Columns.Add(new ColumnDefinition {TableName = "TestTable1", Name = "TestColumn1", Type = DbType.DateTimeOffset});
            expression.Columns.Add(new ColumnDefinition {TableName = "TestTable1", Name = "TestColumn2", Type = DbType.DateTime2});
            expression.Columns.Add(new ColumnDefinition {TableName = "TestTable1", Name = "TestColumn3", Type = DbType.Date});
            expression.Columns.Add(new ColumnDefinition { TableName = "TestTable1", Name = "TestColumn4", Type = DbType.Time });

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE TABLE [dbo].[TestTable1] ([TestColumn1] DATETIMEOFFSET NOT NULL, [TestColumn2] DATETIME2 NOT NULL, [TestColumn3] DATE NOT NULL, [TestColumn4] TIME NOT NULL)");
        }

        /// <summary>
        /// Defines the test method CanUseSystemMethodCurrentDateTimeOffsetAsADefaultValueForAColumn.
        /// </summary>
        [Test]
        public void CanUseSystemMethodCurrentDateTimeOffsetAsADefaultValueForAColumn()
        {
            var expression = new CreateColumnExpression
            {
                Column = new ColumnDefinition
                {
                    Name = "NewColumn",
                    Type = DbType.DateTime,
                    DefaultValue = SystemMethods.CurrentDateTimeOffset
                },
                TableName = "NewTable"
            };

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE [dbo].[NewTable] ADD [NewColumn] DATETIME NOT NULL CONSTRAINT [DF__NewColumn] DEFAULT SYSDATETIMEOFFSET()");
        }

        /// <summary>
        /// Defines the test method CanInsertScopeIdentity.
        /// </summary>
        [Test]
        public void CanInsertScopeIdentity()
        {
            var expression = new InsertDataExpression {TableName = "TestTable"};
            expression.Rows.Add(new InsertionDataDefinition
                                    {
                                        new KeyValuePair<string, object>("Id", 1),
                                        new KeyValuePair<string, object>("Name", RawSql.Insert("SCOPE_IDENTITY()")),
                                        new KeyValuePair<string, object>("Website", "codethinked.com")
                                    });

            var result = Generator.Generate(expression);
            result.ShouldBe("INSERT INTO [dbo].[TestTable] ([Id], [Name], [Website]) VALUES (1, SCOPE_IDENTITY(), N'codethinked.com')");
        }

        /// <summary>
        /// Defines the test method CanInsertAtAtIdentity.
        /// </summary>
        [Test]
        public void CanInsertAtAtIdentity()
        {
            var expression = new InsertDataExpression {TableName = "TestTable"};
            expression.Rows.Add(new InsertionDataDefinition
                                    {
                                        new KeyValuePair<string, object>("Id", 1),
                                        new KeyValuePair<string, object>("Name", RawSql.Insert("@@IDENTITY")),
                                        new KeyValuePair<string, object>("Website", "codethinked.com")
                                    });

            var result = Generator.Generate(expression);
            result.ShouldBe("INSERT INTO [dbo].[TestTable] ([Id], [Name], [Website]) VALUES (1, @@IDENTITY, N'codethinked.com')");
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
                new KeyValuePair<string, object>("NonUnicodeStringValue", new NonUnicodeString("NonUnicodeString")),
            });

            var result = Generator.Generate(expression);
            result.ShouldBe("INSERT INTO [dbo].[TestTable] ([NonUnicodeStringValue]) VALUES ('NonUnicodeString')");
        }

        /// <summary>
        /// Defines the test method ExplicitUnicodeQuotesCorrectly.
        /// </summary>
        [Test]
        [Obsolete]
        public void ExplicitUnicodeQuotesCorrectly()
        {
            var expression = new InsertDataExpression {TableName = "TestTable"};
            expression.Rows.Add(new InsertionDataDefinition
                                    {
                                        new KeyValuePair<string, object>("UnicodeStringValue", new ExplicitUnicodeString("UnicodeString")),
                                        new KeyValuePair<string, object>("StringValue", "String")
                                    });

            var result = Generator.Generate(expression);
            result.ShouldBe("INSERT INTO [dbo].[TestTable] ([UnicodeStringValue], [StringValue]) VALUES (N'UnicodeString', N'String')");
        }
    }
}
