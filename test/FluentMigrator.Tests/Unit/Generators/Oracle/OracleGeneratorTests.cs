// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="OracleGeneratorTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using FluentMigrator.Exceptions;
using FluentMigrator.Expressions;
using FluentMigrator.Runner.Generators.Oracle;
using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Generators.Oracle
{
    /// <summary>
    /// Defines test class OracleGeneratorTests.
    /// </summary>
    [TestFixture]
    public class OracleGeneratorTests
    {
        /// <summary>
        /// The generator
        /// </summary>
        protected OracleGenerator Generator;

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            Generator = new OracleGenerator();
        }

        /// <summary>
        /// Defines the test method CanAlterColumnNoNullSettings.
        /// </summary>
        [Test]
        public void CanAlterColumnNoNullSettings()
        {
            var expression = GeneratorTestHelper.GetAlterColumnExpression();
            expression.Column.IsNullable = null;

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE TestTable1 MODIFY TestColumn1 NVARCHAR2(20)");
        }

        /// <summary>
        /// Defines the test method CanAlterColumnNull.
        /// </summary>
        [Test]
        public void CanAlterColumnNull()
        {
            var expression = GeneratorTestHelper.GetAlterColumnExpression();
            expression.Column.IsNullable = true;

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE TestTable1 MODIFY TestColumn1 NVARCHAR2(20) NULL");
        }

        /// <summary>
        /// Defines the test method CanAlterColumnNotNull.
        /// </summary>
        [Test]
        public void CanAlterColumnNotNull()
        {
            var expression = GeneratorTestHelper.GetAlterColumnExpression();
            expression.Column.IsNullable = false;

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE TestTable1 MODIFY TestColumn1 NVARCHAR2(20) NOT NULL");
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
        /// Defines the test method CanCreateSchemaInStrictMode.
        /// </summary>
        [Test]
        public void CanCreateSchemaInStrictMode()
        {
            Generator.CompatibilityMode = Runner.CompatibilityMode.STRICT;

            Assert.Throws<DatabaseOperationNotSupportedException>(() => Generator.Generate(new CreateSchemaExpression()));
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
        /// Defines the test method CanCreateTableWithoutAnyDescriptions.
        /// </summary>
        [Test]
        public void CanCreateTableWithoutAnyDescriptions()
        {
            var expression = GeneratorTestHelper.GetCreateTableExpression();

            var result = Generator.Generate(expression);

            result.ShouldBe("CREATE TABLE TestTable1 (TestColumn1 NVARCHAR2(255) NOT NULL, TestColumn2 NUMBER(10,0) NOT NULL)");
        }

        /// <summary>
        /// Defines the test method CanCreateTableWithDescriptionAndColumnDescription.
        /// </summary>
        [Test]
        public void CanCreateTableWithDescriptionAndColumnDescription()
        {
            var expression = GeneratorTestHelper.GetCreateTableWithTableDescriptionAndColumnDescriptions();

            var result = Generator.Generate(expression);

            result.ShouldBe("BEGIN EXECUTE IMMEDIATE 'CREATE TABLE TestTable1 (TestColumn1 NVARCHAR2(255), TestColumn2 NUMBER(10,0) NOT NULL)';EXECUTE IMMEDIATE 'COMMENT ON TABLE TestTable1 IS ''TestDescription''';EXECUTE IMMEDIATE 'COMMENT ON COLUMN TestTable1.TestColumn1 IS ''TestColumn1Description''';EXECUTE IMMEDIATE 'COMMENT ON COLUMN TestTable1.TestColumn2 IS ''TestColumn2Description'''; END;");
        }

        /// <summary>
        /// Defines the test method CanAlterTableWithDescription.
        /// </summary>
        [Test]
        public void CanAlterTableWithDescription()
        {
            var expression = GeneratorTestHelper.GetAlterTableWithDescriptionExpression();

            var result = Generator.Generate(expression);

            result.ShouldBe("COMMENT ON TABLE TestTable1 IS 'TestDescription'");
        }

        /// <summary>
        /// Defines the test method CanAlterTableWithoutAnyDescripion.
        /// </summary>
        [Test]
        public void CanAlterTableWithoutAnyDescripion()
        {
            var expression = GeneratorTestHelper.GetAlterTable();

            var result = Generator.Generate(expression);

            result.ShouldBe(string.Empty);
        }

        /// <summary>
        /// Defines the test method CanCreateColumnWithDescription.
        /// </summary>
        [Test]
        public void CanCreateColumnWithDescription()
        {
            var expression = GeneratorTestHelper.GetCreateColumnExpressionWithDescription();

            var result = Generator.Generate(expression);

            result.ShouldBe("BEGIN EXECUTE IMMEDIATE 'ALTER TABLE TestTable1 ADD TestColumn1 NVARCHAR2(5) NOT NULL';EXECUTE IMMEDIATE 'COMMENT ON COLUMN TestTable1.TestColumn1 IS ''TestColumn1Description'''; END;");
        }

        /// <summary>
        /// Defines the test method CanCreateColumnWithoutDescription.
        /// </summary>
        [Test]
        public void CanCreateColumnWithoutDescription()
        {
            var expression = GeneratorTestHelper.GetCreateColumnExpression();

            var result = Generator.Generate(expression);

            result.ShouldBe("ALTER TABLE TestTable1 ADD TestColumn1 NVARCHAR2(5) NOT NULL");
        }

        /// <summary>
        /// Defines the test method CanAlterColumnWithDescription.
        /// </summary>
        [Test]
        public void CanAlterColumnWithDescription()
        {
            var expression = GeneratorTestHelper.GetAlterColumnExpressionWithDescription();

            var result = Generator.Generate(expression);

            result.ShouldBe("BEGIN EXECUTE IMMEDIATE 'ALTER TABLE TestTable1 MODIFY TestColumn1 NVARCHAR2(20) NOT NULL';EXECUTE IMMEDIATE 'COMMENT ON COLUMN TestTable1.TestColumn1 IS ''TestColumn1Description'''; END;");
        }

        /// <summary>
        /// Defines the test method CanAlterColumnWithoutDescription.
        /// </summary>
        [Test]
        public void CanAlterColumnWithoutDescription()
        {
            var expression = GeneratorTestHelper.GetAlterColumnExpression();

            var result = Generator.Generate(expression);

            result.ShouldBe("ALTER TABLE TestTable1 MODIFY TestColumn1 NVARCHAR2(20) NOT NULL");
        }
    }
}
