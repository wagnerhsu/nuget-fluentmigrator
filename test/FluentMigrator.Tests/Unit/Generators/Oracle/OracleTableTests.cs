// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="OracleTableTests.cs" company="FluentMigrator Project">
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

using FluentMigrator.Exceptions;
using FluentMigrator.Runner.Generators.Oracle;
using NUnit.Framework;

namespace FluentMigrator.Tests.Unit.Generators.Oracle
{
    /// <summary>
    /// Defines test class OracleTableTests.
    /// Implements the <see cref="FluentMigrator.Tests.Unit.Generators.Oracle.OracleBaseTableTests{FluentMigrator.Runner.Generators.Oracle.OracleGenerator}" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Unit.Generators.Oracle.OracleBaseTableTests{FluentMigrator.Runner.Generators.Oracle.OracleGenerator}" />
    [TestFixture]
    public class OracleTableTests : OracleBaseTableTests<OracleGenerator>
    {
        /// <summary>
        /// Defines the test method CanCreateTableWithIdentityWithCustomSchema.
        /// </summary>
        [Test]
        public override void CanCreateTableWithIdentityWithCustomSchema()
        {
            var expression = GeneratorTestHelper.GetCreateTableWithAutoIncrementExpression();
            expression.SchemaName = "TestSchema";

            Assert.Throws<DatabaseOperationNotSupportedException>(() => Generator.Generate(expression));
        }

        /// <summary>
        /// Defines the test method CanCreateTableWithIdentityWithDefaultSchema.
        /// </summary>
        [Test]
        public override void CanCreateTableWithIdentityWithDefaultSchema()
        {
            Assert.Throws<DatabaseOperationNotSupportedException>(() => Generator.Generate(GeneratorTestHelper.GetCreateTableWithAutoIncrementExpression()));
        }
    }
}
