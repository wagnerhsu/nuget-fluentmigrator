// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SqlServer2005DescriptionGeneratorTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Linq;
using FluentMigrator.Runner.Generators.SqlServer;
using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Generators.SqlServer2005
{
    /// <summary>
    /// Defines test class SqlServer2005DescriptionGeneratorTests.
    /// Implements the <see cref="FluentMigrator.Tests.Unit.Generators.BaseDescriptionGeneratorTests" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Unit.Generators.BaseDescriptionGeneratorTests" />
    [TestFixture]
    public class SqlServer2005DescriptionGeneratorTests : BaseDescriptionGeneratorTests
    {
        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            DescriptionGenerator = new SqlServer2005DescriptionGenerator();
        }

        /// <summary>
        /// Defines the test method GenerateDescriptionStatementsForCreateTableReturnTableDescriptionStatement.
        /// </summary>
        [Test]
        public override void GenerateDescriptionStatementsForCreateTableReturnTableDescriptionStatement()
        {
            var createTableExpression = GeneratorTestHelper.GetCreateTableWithTableDescription();
            var statements = DescriptionGenerator.GenerateDescriptionStatements(createTableExpression);

            var result = statements.First();
            result.ShouldBe("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'TestDescription', @level0type=N'SCHEMA', @level0name='dbo', @level1type=N'TABLE', @level1name='TestTable1'");
        }

        /// <summary>
        /// Defines the test method GenerateDescriptionStatementsForCreateTableReturnTableDescriptionAndColumnDescriptionsStatements.
        /// </summary>
        [Test]
        public override void GenerateDescriptionStatementsForCreateTableReturnTableDescriptionAndColumnDescriptionsStatements()
        {
            var createTableExpression = GeneratorTestHelper.GetCreateTableWithTableDescriptionAndColumnDescriptions();
            var statements = DescriptionGenerator.GenerateDescriptionStatements(createTableExpression).ToArray();

            var result = string.Join(";", statements);
            result.ShouldBe("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'TestDescription', @level0type=N'SCHEMA', @level0name='dbo', @level1type=N'TABLE', @level1name='TestTable1';EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'TestColumn1Description', @level0type = N'SCHEMA', @level0name = 'dbo', @level1type = N'Table', @level1name = 'TestTable1', @level2type = N'Column',  @level2name = 'TestColumn1';EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'TestColumn2Description', @level0type = N'SCHEMA', @level0name = 'dbo', @level1type = N'Table', @level1name = 'TestTable1', @level2type = N'Column',  @level2name = 'TestColumn2'");
        }

        /// <summary>
        /// Defines the test method GenerateDescriptionStatementForAlterTableReturnTableDescriptionStatement.
        /// </summary>
        [Test]
        public override void GenerateDescriptionStatementForAlterTableReturnTableDescriptionStatement()
        {
            var alterTableExpression = GeneratorTestHelper.GetAlterTableWithDescriptionExpression();
            var statement = DescriptionGenerator.GenerateDescriptionStatement(alterTableExpression);

            statement.ShouldBe("IF EXISTS ( SELECT * FROM fn_listextendedproperty(N'MS_Description', N'SCHEMA', N'dbo', N'TABLE', N'TestTable1', NULL, NULL)) EXEC sys.sp_dropextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name='dbo', @level1type=N'TABLE', @level1name='TestTable1';EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'TestDescription', @level0type=N'SCHEMA', @level0name='dbo', @level1type=N'TABLE', @level1name='TestTable1'");
        }

        /// <summary>
        /// Defines the test method GenerateDescriptionStatementForCreateColumnReturnColumnDescriptionStatement.
        /// </summary>
        [Test]
        public override void GenerateDescriptionStatementForCreateColumnReturnColumnDescriptionStatement()
        {
            var createColumnExpression = GeneratorTestHelper.GetCreateColumnExpressionWithDescription();
            var statement = DescriptionGenerator.GenerateDescriptionStatement(createColumnExpression);

            statement.ShouldBe("EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'TestColumn1Description', @level0type = N'SCHEMA', @level0name = 'dbo', @level1type = N'Table', @level1name = 'TestTable1', @level2type = N'Column',  @level2name = 'TestColumn1'");
        }

        /// <summary>
        /// Defines the test method GenerateDescriptionStatementForAlterColumnReturnColumnDescriptionStatement.
        /// </summary>
        [Test]
        public override void GenerateDescriptionStatementForAlterColumnReturnColumnDescriptionStatement()
        {
            var alterColumnExpression = GeneratorTestHelper.GetAlterColumnExpressionWithDescription();
            var statement = DescriptionGenerator.GenerateDescriptionStatement(alterColumnExpression);

            statement.ShouldBe("IF EXISTS (SELECT * FROM fn_listextendedproperty(N'MS_Description', N'SCHEMA', N'dbo', N'TABLE', N'TestTable1', N'Column', N'TestColumn1' )) EXEC sys.sp_dropextendedproperty @name=N'MS_Description', @level0type = N'SCHEMA', @level0name = 'dbo', @level1type = N'Table', @level1name = 'TestTable1', @level2type = N'Column',  @level2name = 'TestColumn1';EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'TestColumn1Description', @level0type = N'SCHEMA', @level0name = 'dbo', @level1type = N'Table', @level1name = 'TestTable1', @level2type = N'Column',  @level2name = 'TestColumn1'");
        }
    }
}
