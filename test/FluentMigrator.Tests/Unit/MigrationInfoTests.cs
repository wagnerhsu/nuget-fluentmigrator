// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="MigrationInfoTests.cs" company="FluentMigrator Project">
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
using FluentMigrator.Infrastructure;
using Moq;
using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit
{
    /// <summary>
    /// Defines test class MigrationInfoTests.
    /// </summary>
    [TestFixture]
    public class MigrationInfoTests
    {
        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _expectedVersion = new Random().Next();
            _migration = Mock.Of<IMigration>();
        }

        /// <summary>
        /// The migration
        /// </summary>
        private IMigration _migration;
        /// <summary>
        /// The expected version
        /// </summary>
        private long _expectedVersion;

        /// <summary>
        /// Creates the specified behavior.
        /// </summary>
        /// <param name="behavior">The behavior.</param>
        /// <returns>MigrationInfo.</returns>
        private MigrationInfo Create(TransactionBehavior behavior = TransactionBehavior.Default)
        {
            return new MigrationInfo(_expectedVersion, behavior, _migration);
        }

        /// <summary>
        /// Defines the test method ConstructingShouldRetainMigration.
        /// </summary>
        [Test]
        public void ConstructingShouldRetainMigration()
        {
            MigrationInfo migrationinfo = Create();
            migrationinfo.Migration.ShouldBeSameAs(_migration);
        }

        /// <summary>
        /// Defines the test method ConstructingShouldRetainTransactionBehaviorDefault.
        /// </summary>
        [Test]
        public void ConstructingShouldRetainTransactionBehaviorDefault()
        {
            MigrationInfo migrationinfo = Create();
            migrationinfo.TransactionBehavior.ShouldBe(TransactionBehavior.Default);
        }

        /// <summary>
        /// Defines the test method ConstructingShouldRetainTransactionBehaviorNone.
        /// </summary>
        [Test]
        public void ConstructingShouldRetainTransactionBehaviorNone()
        {
            MigrationInfo migrationinfo = Create(TransactionBehavior.None);
            migrationinfo.TransactionBehavior.ShouldBe(TransactionBehavior.None);
        }

        /// <summary>
        /// Defines the test method ConstructingShouldRetainValueOfVersion.
        /// </summary>
        [Test]
        public void ConstructingShouldRetainValueOfVersion()
        {
            MigrationInfo migrationinfo = Create();
            migrationinfo.Version.ShouldBe(_expectedVersion);
        }

        /// <summary>
        /// Defines the test method HasTraitReturnsFalseWhenTraitIsNotDefined.
        /// </summary>
        [Test]
        public void HasTraitReturnsFalseWhenTraitIsNotDefined()
        {
            MigrationInfo migrationinfo = Create();
            migrationinfo.HasTrait("foo").ShouldBeFalse();
        }

        /// <summary>
        /// Defines the test method HasTraitReturnsTrueWhenTraitIsDefined.
        /// </summary>
        [Test]
        public void HasTraitReturnsTrueWhenTraitIsDefined()
        {
            MigrationInfo migrationinfo = Create();
            migrationinfo.AddTrait("foo", 42);
            migrationinfo.HasTrait("foo").ShouldBeTrue();
        }

        /// <summary>
        /// Defines the test method TraitMethodReturnsNullForNonExistentTrait.
        /// </summary>
        [Test]
        public void TraitMethodReturnsNullForNonExistentTrait()
        {
            MigrationInfo migrationinfo = Create();
            migrationinfo.Trait("foo").ShouldBeNull();
        }

        /// <summary>
        /// Defines the test method TraitMethodReturnsTraitValue.
        /// </summary>
        [Test]
        public void TraitMethodReturnsTraitValue()
        {
            MigrationInfo migrationinfo = Create();
            const string value = "bar";
            migrationinfo.AddTrait("foo", value);
            migrationinfo.Trait("foo").ShouldBeSameAs(value);
        }
    }
}
