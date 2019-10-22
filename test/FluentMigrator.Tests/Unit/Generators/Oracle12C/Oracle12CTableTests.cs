// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="Oracle12CTableTests.cs" company="FluentMigrator Project">
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

using FluentMigrator.Runner.Generators.Oracle;
using FluentMigrator.Tests.Unit.Generators.Oracle;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Generators.Oracle12C
{
    /// <summary>
    /// Defines test class Oracle12CTableTests.
    /// Implements the <see cref="FluentMigrator.Tests.Unit.Generators.Oracle.OracleBaseTableTests{FluentMigrator.Runner.Generators.Oracle.Oracle12CGenerator}" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Unit.Generators.Oracle.OracleBaseTableTests{FluentMigrator.Runner.Generators.Oracle.Oracle12CGenerator}" />
    [TestFixture]
    public class Oracle12CTableTests : OracleBaseTableTests<Oracle12CGenerator>
    {
        /// <summary>
        /// Defines the test method CanCreateTableWithIdentityWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanCreateTableWithIdentityWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetCreateTableWithAutoIncrementExpression();
            expression.SchemaName = "TestSchema";

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE TABLE TestSchema.TestTable1 (TestColumn1 NUMBER(10,0) GENERATED ALWAYS AS IDENTITY , TestColumn2 NUMBER(10,0) NOT NULL)");
        }

        /// <summary>
        /// Defines the test method CanCreateTableWithIdentityWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateTableWithIdentityWithDefaultSchema()
        {
            var expression = GeneratorTestHelper.GetCreateTableWithAutoIncrementExpression();

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE TABLE TestTable1 (TestColumn1 NUMBER(10,0) GENERATED ALWAYS AS IDENTITY , TestColumn2 NUMBER(10,0) NOT NULL)");
        }
    }
}
