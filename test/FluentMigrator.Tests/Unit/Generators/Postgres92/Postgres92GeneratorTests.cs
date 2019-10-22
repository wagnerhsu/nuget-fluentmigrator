// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="Postgres92GeneratorTests.cs" company="FluentMigrator Project">
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

using FluentMigrator.Runner.Generators;
using FluentMigrator.Runner.Generators.Postgres;
using FluentMigrator.Runner.Generators.Postgres92;
using FluentMigrator.Runner.Processors.Postgres;

using Microsoft.Extensions.Options;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Generators.Postgres92
{
    /// <summary>
    /// Defines test class Postgres92GeneratorTests.
    /// </summary>
    [TestFixture]
    public class Postgres92GeneratorTests
    {
        /// <summary>
        /// The generator
        /// </summary>
        protected PostgresGenerator Generator;

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            var quoter = new PostgresQuoter(new PostgresOptions());
            Generator = new Postgres92Generator(quoter, new OptionsWrapper<GeneratorOptions>(new GeneratorOptions()));
        }

        /// <summary>
        /// Defines the test method CanCreateTableWithBlobColumnWithObjectType.
        /// </summary>
        [Test]
        public void CanCreateTableWithBlobColumnWithObjectType()
        {
            var expression = GeneratorTestHelper.GetCreateTableExpression();
            expression.Columns[0].Type = DbType.Object;

            var result = Generator.Generate(expression);
            result.ShouldBe("CREATE TABLE \"public\".\"TestTable1\" (\"TestColumn1\" json NOT NULL, \"TestColumn2\" integer NOT NULL);");
        }
    }
}
