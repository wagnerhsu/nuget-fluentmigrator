// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="VersionInfoTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
//
// Copyright (c) 2007-2018, Sean Chambers <schambers80@gmail.com>
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

using System.Linq;
using FluentMigrator.Runner.Versioning;
using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Versioning
{
    /// <summary>
    /// Defines test class VersionInfoTests.
    /// </summary>
    [TestFixture]
    public class VersionInfoTests
    {
        /// <summary>
        /// The version information
        /// </summary>
        private VersionInfo _versionInfo;

        /// <summary>
        /// Sets up.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            _versionInfo = new VersionInfo();
        }

        /// <summary>
        /// Defines the test method CanAddAppliedMigration.
        /// </summary>
        [Test]
        public void CanAddAppliedMigration()
        {
            _versionInfo.AddAppliedMigration(200909060953);
            _versionInfo.HasAppliedMigration(200909060953).ShouldBeTrue();
        }

        /// <summary>
        /// Defines the test method CanGetLatestMigration.
        /// </summary>
        [Test]
        public void CanGetLatestMigration()
        {
            _versionInfo.AddAppliedMigration(200909060953);
            _versionInfo.AddAppliedMigration(200909060935);
            _versionInfo.Latest().ShouldBe(200909060953);
        }

        /// <summary>
        /// Defines the test method CanGetAppliedMigrationsLatestFirst.
        /// </summary>
        [Test]
        public void CanGetAppliedMigrationsLatestFirst()
        {
            _versionInfo.AddAppliedMigration(200909060953);
            _versionInfo.AddAppliedMigration(200909060935);
            var applied = _versionInfo.AppliedMigrations().ToList();
            applied[0].ShouldBe(200909060953);
            applied[1].ShouldBe(200909060935);
        }
    }
}
