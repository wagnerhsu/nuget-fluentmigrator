// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="OracleDescriptionGeneratorTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Linq;
using FluentMigrator.Runner.Generators.Oracle;
using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Generators.Oracle
{
    /// <summary>
    /// Defines test class OracleDescriptionGeneratorTests.
    /// Implements the <see cref="FluentMigrator.Tests.Unit.Generators.BaseDescriptionGeneratorTests" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Unit.Generators.BaseDescriptionGeneratorTests" />
    [TestFixture]
    public class OracleDescriptionGeneratorTests : BaseDescriptionGeneratorTests
    {
        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            DescriptionGenerator = new OracleDescriptionGenerator();
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
            result.ShouldBe("COMMENT ON TABLE TestTable1 IS 'TestDescription'");
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
            result.ShouldBe(
                "COMMENT ON TABLE TestTable1 IS 'TestDescription';COMMENT ON COLUMN TestTable1.TestColumn1 IS 'TestColumn1Description';COMMENT ON COLUMN TestTable1.TestColumn2 IS 'TestColumn2Description'");
        }

        /// <summary>
        /// Defines the test method GenerateDescriptionStatementForAlterTableReturnTableDescriptionStatement.
        /// </summary>
        [Test]
        public override void GenerateDescriptionStatementForAlterTableReturnTableDescriptionStatement()
        {
            var alterTableExpression = GeneratorTestHelper.GetAlterTableWithDescriptionExpression();
            var statement = DescriptionGenerator.GenerateDescriptionStatement(alterTableExpression);

            statement.ShouldBe("COMMENT ON TABLE TestTable1 IS 'TestDescription'");
        }

        /// <summary>
        /// Defines the test method GenerateDescriptionStatementForCreateColumnReturnColumnDescriptionStatement.
        /// </summary>
        [Test]
        public override void GenerateDescriptionStatementForCreateColumnReturnColumnDescriptionStatement()
        {
            var createColumnExpression = GeneratorTestHelper.GetCreateColumnExpressionWithDescription();
            var statement = DescriptionGenerator.GenerateDescriptionStatement(createColumnExpression);

            statement.ShouldBe("COMMENT ON COLUMN TestTable1.TestColumn1 IS 'TestColumn1Description'");
        }

        /// <summary>
        /// Defines the test method GenerateDescriptionStatementForAlterColumnReturnColumnDescriptionStatement.
        /// </summary>
        [Test]
        public override void GenerateDescriptionStatementForAlterColumnReturnColumnDescriptionStatement()
        {
            var alterColumnExpression = GeneratorTestHelper.GetAlterColumnExpressionWithDescription();
            var statement = DescriptionGenerator.GenerateDescriptionStatement(alterColumnExpression);

            statement.ShouldBe("COMMENT ON COLUMN TestTable1.TestColumn1 IS 'TestColumn1Description'");
        }

        /// <summary>
        /// Defines the test method GenerateDescriptionStatementsWithSingleQuoteForCreateTableReturnTableDescriptionStatement.
        /// </summary>
        [Test]
        public void GenerateDescriptionStatementsWithSingleQuoteForCreateTableReturnTableDescriptionStatement()
        {
            var createTableExpression = GeneratorTestHelper.GetCreateTableWithTableDescription();
            createTableExpression.TableDescription = "Test Description with single quote (') character here >> '";
            var statements = DescriptionGenerator.GenerateDescriptionStatements(createTableExpression);

            var result = statements.First();
            result.ShouldBe("COMMENT ON TABLE TestTable1 IS 'Test Description with single quote ('') character here >> '''");
        }
    }
}
