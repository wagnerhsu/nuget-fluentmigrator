// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="OracleConstraintsTests.cs" company="FluentMigrator Project">
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

using FluentMigrator.Runner.Generators.Oracle;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Generators.Oracle
{
    /// <summary>
    /// Defines test class OracleConstraintsTests.
    /// Implements the <see cref="FluentMigrator.Tests.Unit.Generators.BaseConstraintsTests" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Unit.Generators.BaseConstraintsTests" />
    [TestFixture]
    public class OracleConstraintsTests : BaseConstraintsTests
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
        /// Defines the test method CanCreateForeignKeyWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanCreateForeignKeyWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetCreateForeignKeyExpression();
            expression.ForeignKey.ForeignTableSchema = "TestSchema";
            expression.ForeignKey.PrimaryTableSchema = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE TestSchema.TestTable1 ADD CONSTRAINT FK_TestTable1_TestColumn1_TestTable2_TestColumn2 FOREIGN KEY (TestColumn1) REFERENCES TestSchema.TestTable2 (TestColumn2)");
        }

        /// <summary>
        /// Defines the test method CanCreateForeignKeyWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateForeignKeyWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateForeignKeyExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE TestTable1 ADD CONSTRAINT FK_TestTable1_TestColumn1_TestTable2_TestColumn2 FOREIGN KEY (TestColumn1) REFERENCES TestTable2 (TestColumn2)");
        }

        /// <summary>
        /// Defines the test method CanCreateForeignKeyWithDifferentSchemas.
        /// </summary>
        [Test]
        public override void CanCreateForeignKeyWithDifferentSchemas()
        {
            var expression = GeneratorTestHelper.GetCreateForeignKeyExpression();
            expression.ForeignKey.ForeignTableSchema = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE TestSchema.TestTable1 ADD CONSTRAINT FK_TestTable1_TestColumn1_TestTable2_TestColumn2 FOREIGN KEY (TestColumn1) REFERENCES TestTable2 (TestColumn2)");
        }

        /// <summary>
        /// Defines the test method CanCreateMultiColumnForeignKeyWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanCreateMultiColumnForeignKeyWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetCreateMultiColumnForeignKeyExpression();
            expression.ForeignKey.ForeignTableSchema = "TestSchema";
            expression.ForeignKey.PrimaryTableSchema = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE TestSchema.TestTable1 ADD CONSTRAINT FK_TestTable1_TestColumn1_TestColumn3_TestTable2_TestColumn2_TestColumn4 FOREIGN KEY (TestColumn1, TestColumn3) REFERENCES TestSchema.TestTable2 (TestColumn2, TestColumn4)");
        }

        /// <summary>
        /// Defines the test method CanCreateMultiColumnForeignKeyWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateMultiColumnForeignKeyWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateMultiColumnForeignKeyExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE TestTable1 ADD CONSTRAINT FK_TestTable1_TestColumn1_TestColumn3_TestTable2_TestColumn2_TestColumn4 FOREIGN KEY (TestColumn1, TestColumn3) REFERENCES TestTable2 (TestColumn2, TestColumn4)");
        }

        /// <summary>
        /// Defines the test method CanCreateMultiColumnForeignKeyWithDifferentSchemas.
        /// </summary>
        [Test]
        public override void CanCreateMultiColumnForeignKeyWithDifferentSchemas()
        {
            var expression = GeneratorTestHelper.GetCreateMultiColumnForeignKeyExpression();
            expression.ForeignKey.ForeignTableSchema = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE TestSchema.TestTable1 ADD CONSTRAINT FK_TestTable1_TestColumn1_TestColumn3_TestTable2_TestColumn2_TestColumn4 FOREIGN KEY (TestColumn1, TestColumn3) REFERENCES TestTable2 (TestColumn2, TestColumn4)");
        }

        /// <summary>
        /// Defines the test method CanCreateMultiColumnPrimaryKeyConstraintWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanCreateMultiColumnPrimaryKeyConstraintWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetCreateMultiColumnPrimaryKeyExpression();
            expression.Constraint.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE TestSchema.TestTable1 ADD CONSTRAINT PK_TestTable1_TestColumn1_TestColumn2 PRIMARY KEY (TestColumn1, TestColumn2)");
        }

        /// <summary>
        /// Defines the test method CanCreateMultiColumnPrimaryKeyConstraintWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateMultiColumnPrimaryKeyConstraintWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateMultiColumnPrimaryKeyExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE TestTable1 ADD CONSTRAINT PK_TestTable1_TestColumn1_TestColumn2 PRIMARY KEY (TestColumn1, TestColumn2)");
        }

        /// <summary>
        /// Defines the test method CanCreateMultiColumnUniqueConstraintWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanCreateMultiColumnUniqueConstraintWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetCreateMultiColumnUniqueConstraintExpression();
            expression.Constraint.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE TestSchema.TestTable1 ADD CONSTRAINT UC_TestTable1_TestColumn1_TestColumn2 UNIQUE (TestColumn1, TestColumn2)");
        }

        /// <summary>
        /// Defines the test method CanCreateMultiColumnUniqueConstraintWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateMultiColumnUniqueConstraintWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateMultiColumnUniqueConstraintExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE TestTable1 ADD CONSTRAINT UC_TestTable1_TestColumn1_TestColumn2 UNIQUE (TestColumn1, TestColumn2)");
        }

        /// <summary>
        /// Defines the test method CanCreateNamedForeignKeyWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanCreateNamedForeignKeyWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetCreateNamedForeignKeyExpression();
            expression.ForeignKey.ForeignTableSchema = "TestSchema";
            expression.ForeignKey.PrimaryTableSchema = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE TestSchema.TestTable1 ADD CONSTRAINT FK_Test FOREIGN KEY (TestColumn1) REFERENCES TestSchema.TestTable2 (TestColumn2)");
        }

        /// <summary>
        /// Defines the test method CanCreateNamedForeignKeyWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateNamedForeignKeyWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateNamedForeignKeyExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE TestTable1 ADD CONSTRAINT FK_Test FOREIGN KEY (TestColumn1) REFERENCES TestTable2 (TestColumn2)");
        }

        /// <summary>
        /// Defines the test method CanCreateNamedForeignKeyWithDifferentSchemas.
        /// </summary>
        [Test]
        public override void CanCreateNamedForeignKeyWithDifferentSchemas()
        {
            var expression = GeneratorTestHelper.GetCreateNamedForeignKeyExpression();
            expression.ForeignKey.ForeignTableSchema = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE TestSchema.TestTable1 ADD CONSTRAINT FK_Test FOREIGN KEY (TestColumn1) REFERENCES TestTable2 (TestColumn2)");
        }

        /// <summary>
        /// Defines the test method CanCreateNamedForeignKeyWithOnDeleteAndOnUpdateOptions.
        /// </summary>
        [Test]
        public override void CanCreateNamedForeignKeyWithOnDeleteAndOnUpdateOptions()
        {
            var expression = GeneratorTestHelper.GetCreateNamedForeignKeyExpression();
            expression.ForeignKey.OnDelete = Rule.Cascade;
            expression.ForeignKey.OnUpdate = Rule.SetDefault;

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE TestTable1 ADD CONSTRAINT FK_Test FOREIGN KEY (TestColumn1) REFERENCES TestTable2 (TestColumn2) ON DELETE CASCADE ON UPDATE SET DEFAULT");
        }

        /// <summary>
        /// Defines the test method CanCreateNamedForeignKeyWithOnDeleteOptions.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <param name="output">The output.</param>
        [TestCase(Rule.SetDefault, "SET DEFAULT"), TestCase(Rule.SetNull, "SET NULL"), TestCase(Rule.Cascade, "CASCADE")]
        public override void CanCreateNamedForeignKeyWithOnDeleteOptions(Rule rule, string output)
        {
            var expression = GeneratorTestHelper.GetCreateNamedForeignKeyExpression();
            expression.ForeignKey.OnDelete = rule;

            var result = Generator.Generate(expression);
            result.ShouldBe(string.Format("ALTER TABLE TestTable1 ADD CONSTRAINT FK_Test FOREIGN KEY (TestColumn1) REFERENCES TestTable2 (TestColumn2) ON DELETE {0}", output));
        }

        /// <summary>
        /// Defines the test method CanCreateNamedForeignKeyWithOnUpdateOptions.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <param name="output">The output.</param>
        [TestCase(Rule.SetDefault, "SET DEFAULT"), TestCase(Rule.SetNull, "SET NULL"), TestCase(Rule.Cascade, "CASCADE")]
        public override void CanCreateNamedForeignKeyWithOnUpdateOptions(Rule rule, string output)
        {
            var expression = GeneratorTestHelper.GetCreateNamedForeignKeyExpression();
            expression.ForeignKey.OnUpdate = rule;

            var result = Generator.Generate(expression);
            result.ShouldBe(string.Format("ALTER TABLE TestTable1 ADD CONSTRAINT FK_Test FOREIGN KEY (TestColumn1) REFERENCES TestTable2 (TestColumn2) ON UPDATE {0}", output));
        }

        /// <summary>
        /// Defines the test method CanCreateNamedMultiColumnForeignKeyWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanCreateNamedMultiColumnForeignKeyWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetCreateNamedMultiColumnForeignKeyExpression();
            expression.ForeignKey.ForeignTableSchema = "TestSchema";
            expression.ForeignKey.PrimaryTableSchema = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE TestSchema.TestTable1 ADD CONSTRAINT FK_Test FOREIGN KEY (TestColumn1, TestColumn3) REFERENCES TestSchema.TestTable2 (TestColumn2, TestColumn4)");
        }

        /// <summary>
        /// Defines the test method CanCreateNamedMultiColumnForeignKeyWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateNamedMultiColumnForeignKeyWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateNamedMultiColumnForeignKeyExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE TestTable1 ADD CONSTRAINT FK_Test FOREIGN KEY (TestColumn1, TestColumn3) REFERENCES TestTable2 (TestColumn2, TestColumn4)");
        }

        /// <summary>
        /// Defines the test method CanCreateNamedMultiColumnForeignKeyWithDifferentSchemas.
        /// </summary>
        [Test]
        public override void CanCreateNamedMultiColumnForeignKeyWithDifferentSchemas()
        {
            var expression = GeneratorTestHelper.GetCreateNamedMultiColumnForeignKeyExpression();
            expression.ForeignKey.ForeignTableSchema = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE TestSchema.TestTable1 ADD CONSTRAINT FK_Test FOREIGN KEY (TestColumn1, TestColumn3) REFERENCES TestTable2 (TestColumn2, TestColumn4)");
        }

        /// <summary>
        /// Defines the test method CanCreateNamedMultiColumnPrimaryKeyConstraintWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanCreateNamedMultiColumnPrimaryKeyConstraintWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetCreateNamedMultiColumnPrimaryKeyExpression();
            expression.Constraint.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE TestSchema.TestTable1 ADD CONSTRAINT TESTPRIMARYKEY PRIMARY KEY (TestColumn1, TestColumn2)");
        }

        /// <summary>
        /// Defines the test method CanCreateNamedMultiColumnPrimaryKeyConstraintWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateNamedMultiColumnPrimaryKeyConstraintWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateNamedMultiColumnPrimaryKeyExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE TestTable1 ADD CONSTRAINT TESTPRIMARYKEY PRIMARY KEY (TestColumn1, TestColumn2)");
        }

        /// <summary>
        /// Defines the test method CanCreateNamedMultiColumnUniqueConstraintWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanCreateNamedMultiColumnUniqueConstraintWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetCreateNamedMultiColumnUniqueConstraintExpression();
            expression.Constraint.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE TestSchema.TestTable1 ADD CONSTRAINT TESTUNIQUECONSTRAINT UNIQUE (TestColumn1, TestColumn2)");
        }

        /// <summary>
        /// Defines the test method CanCreateNamedMultiColumnUniqueConstraintWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateNamedMultiColumnUniqueConstraintWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateNamedMultiColumnUniqueConstraintExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE TestTable1 ADD CONSTRAINT TESTUNIQUECONSTRAINT UNIQUE (TestColumn1, TestColumn2)");
        }

        /// <summary>
        /// Defines the test method CanCreateNamedPrimaryKeyConstraintWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanCreateNamedPrimaryKeyConstraintWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetCreateNamedPrimaryKeyExpression();
            expression.Constraint.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE TestSchema.TestTable1 ADD CONSTRAINT TESTPRIMARYKEY PRIMARY KEY (TestColumn1)");
        }

        /// <summary>
        /// Defines the test method CanCreateNamedPrimaryKeyConstraintWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateNamedPrimaryKeyConstraintWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateNamedPrimaryKeyExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE TestTable1 ADD CONSTRAINT TESTPRIMARYKEY PRIMARY KEY (TestColumn1)");
        }

        /// <summary>
        /// Defines the test method CanCreateNamedUniqueConstraintWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanCreateNamedUniqueConstraintWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetCreateNamedUniqueConstraintExpression();
            expression.Constraint.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE TestSchema.TestTable1 ADD CONSTRAINT TESTUNIQUECONSTRAINT UNIQUE (TestColumn1)");
        }

        /// <summary>
        /// Defines the test method CanCreateNamedUniqueConstraintWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateNamedUniqueConstraintWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateNamedUniqueConstraintExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE TestTable1 ADD CONSTRAINT TESTUNIQUECONSTRAINT UNIQUE (TestColumn1)");
        }

        /// <summary>
        /// Defines the test method CanCreatePrimaryKeyConstraintWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanCreatePrimaryKeyConstraintWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetCreatePrimaryKeyExpression();
            expression.Constraint.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE TestSchema.TestTable1 ADD CONSTRAINT PK_TestTable1_TestColumn1 PRIMARY KEY (TestColumn1)");
        }

        /// <summary>
        /// Defines the test method CanCreatePrimaryKeyConstraintWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreatePrimaryKeyConstraintWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreatePrimaryKeyExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE TestTable1 ADD CONSTRAINT PK_TestTable1_TestColumn1 PRIMARY KEY (TestColumn1)");
        }

        /// <summary>
        /// Defines the test method CanCreateUniqueConstraintWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanCreateUniqueConstraintWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetCreateUniqueConstraintExpression();
            expression.Constraint.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE TestSchema.TestTable1 ADD CONSTRAINT UC_TestTable1_TestColumn1 UNIQUE (TestColumn1)");
        }

        /// <summary>
        /// Defines the test method CanCreateUniqueConstraintWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateUniqueConstraintWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateUniqueConstraintExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE TestTable1 ADD CONSTRAINT UC_TestTable1_TestColumn1 UNIQUE (TestColumn1)");
        }

        /// <summary>
        /// Defines the test method CanDropForeignKeyWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanDropForeignKeyWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetDeleteForeignKeyExpression();
            expression.ForeignKey.ForeignTableSchema = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE TestSchema.TestTable1 DROP CONSTRAINT FK_Test");
        }

        /// <summary>
        /// Defines the test method CanDropForeignKeyWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanDropForeignKeyWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetDeleteForeignKeyExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE TestTable1 DROP CONSTRAINT FK_Test");
        }

        /// <summary>
        /// Defines the test method CanDropPrimaryKeyConstraintWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanDropPrimaryKeyConstraintWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetDeletePrimaryKeyExpression();
            expression.Constraint.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE TestSchema.TestTable1 DROP CONSTRAINT TESTPRIMARYKEY");
        }

        /// <summary>
        /// Defines the test method CanDropPrimaryKeyConstraintWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanDropPrimaryKeyConstraintWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetDeletePrimaryKeyExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE TestTable1 DROP CONSTRAINT TESTPRIMARYKEY");
        }

        /// <summary>
        /// Defines the test method CanDropUniqueConstraintWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanDropUniqueConstraintWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetDeleteUniqueConstraintExpression();
            expression.Constraint.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE TestSchema.TestTable1 DROP CONSTRAINT TESTUNIQUECONSTRAINT");
        }

        /// <summary>
        /// Defines the test method CanDropUniqueConstraintWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanDropUniqueConstraintWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetDeleteUniqueConstraintExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("ALTER TABLE TestTable1 DROP CONSTRAINT TESTUNIQUECONSTRAINT");
        }

        /// <summary>
        /// Defines the test method CanAlterDefaultConstraintWithValueAsDefault.
        /// </summary>
        [Test]
        public void CanAlterDefaultConstraintWithValueAsDefault()
        {
            var expression = GeneratorTestHelper.GetAlterDefaultConstraintExpression();

            var result = Generator.Generate(expression);

            result.ShouldBe("ALTER TABLE TestTable1 MODIFY TestColumn1 DEFAULT 1");
        }

        /// <summary>
        /// Defines the test method CanAlterDefaultConstraintWithStringValueAsDefault.
        /// </summary>
        [Test]
        public void CanAlterDefaultConstraintWithStringValueAsDefault()
        {
            var expression = GeneratorTestHelper.GetAlterDefaultConstraintExpression();
            expression.DefaultValue = "1";

            var result = Generator.Generate(expression);

            result.ShouldBe("ALTER TABLE TestTable1 MODIFY TestColumn1 DEFAULT '1'");
        }

        /// <summary>
        /// Defines the test method CanAlterDefaultConstraintWithDefaultSystemMethodNewGuid.
        /// </summary>
        [Test]
        public void CanAlterDefaultConstraintWithDefaultSystemMethodNewGuid()
        {
            var expression = GeneratorTestHelper.GetAlterDefaultConstraintExpression();
            expression.DefaultValue = SystemMethods.NewGuid;

            var result = Generator.Generate(expression);

            result.ShouldBe("ALTER TABLE TestTable1 MODIFY TestColumn1 DEFAULT sys_guid()");
        }

        /// <summary>
        /// Defines the test method CanAlterDefaultConstraintWithDefaultSystemMethodCurrentDateTime.
        /// </summary>
        [Test]
        public void CanAlterDefaultConstraintWithDefaultSystemMethodCurrentDateTime()
        {
            var expression = GeneratorTestHelper.GetAlterDefaultConstraintExpression();
            expression.DefaultValue = SystemMethods.CurrentDateTime;

            var result = Generator.Generate(expression);

            result.ShouldBe("ALTER TABLE TestTable1 MODIFY TestColumn1 DEFAULT LOCALTIMESTAMP");
        }

        /// <summary>
        /// Defines the test method CanAlterDefaultConstraintWithDefaultSystemMethodCurrentUser.
        /// </summary>
        [Test]
        public void CanAlterDefaultConstraintWithDefaultSystemMethodCurrentUser()
        {
            var expression = GeneratorTestHelper.GetAlterDefaultConstraintExpression();
            expression.DefaultValue = SystemMethods.CurrentUser;

            var result = Generator.Generate(expression);

            result.ShouldBe("ALTER TABLE TestTable1 MODIFY TestColumn1 DEFAULT USER");
        }

        /// <summary>
        /// Defines the test method CanRemoveDefaultConstraint.
        /// </summary>
        [Test]
        public void CanRemoveDefaultConstraint()
        {
            var expression = GeneratorTestHelper.GetDeleteDefaultConstraintExpression();

            var result = Generator.Generate(expression);

            result.ShouldBe("ALTER TABLE TestTable1 MODIFY TestColumn1 DEFAULT NULL");
        }
    }
}
