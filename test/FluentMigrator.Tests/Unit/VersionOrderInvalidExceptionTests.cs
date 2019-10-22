// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="VersionOrderInvalidExceptionTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
// Copyright (c) 2007-2018, Sean Chambers <schambers80@gmail.com>
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
using FluentMigrator.Infrastructure;
using FluentMigrator.Runner.Exceptions;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit
{
    /// <summary>
    /// Defines test class VersionOrderInvalidExceptionTests.
    /// </summary>
    [TestFixture]
    public class VersionOrderInvalidExceptionTests
    {
        /// <summary>
        /// Defines the test method InvalidMigrationsPopulated.
        /// </summary>
        [Test]
        public void InvalidMigrationsPopulated()
        {
            var migrations = new[]
                                 {
                                     new KeyValuePair<long,IMigrationInfo>(1, new MigrationInfo(1, TransactionBehavior.Default, new TestMigration1())),
                                     new KeyValuePair<long,IMigrationInfo>(2, new MigrationInfo(2, TransactionBehavior.Default, new TestMigration2()))
                                 };


            var exception = new VersionOrderInvalidException(migrations);

            exception.InvalidMigrations.ShouldBe(migrations);
        }

        /// <summary>
        /// Defines the test method ExceptionMessageListsInvalidMigrations.
        /// </summary>
        [Test]
        public void ExceptionMessageListsInvalidMigrations()
        {
            var migrations = new[]
                                 {
                                     new KeyValuePair<long,IMigrationInfo>(1, new MigrationInfo(1, TransactionBehavior.Default, new TestMigration1())),
                                     new KeyValuePair<long,IMigrationInfo>(2, new MigrationInfo(2, TransactionBehavior.Default, new TestMigration2()))
                                 };

            var exception = new VersionOrderInvalidException(migrations);

            var expectedMessage = "Unapplied migrations have version numbers that are less than the greatest version number of applied migrations:"
                + Environment.NewLine + "1 - TestMigration1"
                + Environment.NewLine + "2 - TestMigration2";

            System.Console.WriteLine(exception.Message);

            exception.Message.ShouldBe(expectedMessage);
        }
   }


    /// <summary>
    /// Class TestMigration1.
    /// Implements the <see cref="FluentMigrator.Migration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Migration" />
    class TestMigration1 : Migration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up() { }
        /// <summary>
        /// Collects the DOWN migration expressions
        /// </summary>
        public override void Down() { }
    }

    /// <summary>
    /// Class TestMigration2.
    /// Implements the <see cref="FluentMigrator.Migration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Migration" />
    class TestMigration2 : Migration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up() { }
        /// <summary>
        /// Downs this instance.
        /// </summary>
        public override void Down() { }
    }
}
