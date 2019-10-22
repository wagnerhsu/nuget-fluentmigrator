// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="ObsoleteProfileLoaderTests.cs" company="FluentMigrator Project">
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

using System;
using System.Linq;

using FluentMigrator.Infrastructure;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;

using Moq;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit
{
    /// <summary>
    /// Defines test class ObsoleteProfileLoaderTests.
    /// </summary>
    [TestFixture]
    [Obsolete]
    public class ObsoleteProfileLoaderTests
    {
        /// <summary>
        /// Defines the test method BlankProfileDoesntLoadProfiles.
        /// </summary>
        [Test]
        public void BlankProfileDoesntLoadProfiles()
        {
            var runnerContextMock = new Mock<IRunnerContext>();
            var runnerMock = new Mock<IMigrationRunner>();
            var conventionsMock = new Mock<IMigrationRunnerConventions>();

            runnerContextMock.Setup(x => x.Profile).Returns(string.Empty);
            //_runnerContextMock.VerifyGet(x => x.Profile).Returns(string.Empty);
            runnerMock.SetupGet(x => x.MigrationAssemblies).Returns(new SingleAssembly(typeof(MigrationRunnerTests).Assembly));

            var profileLoader = new ProfileLoader(runnerContextMock.Object, runnerMock.Object, conventionsMock.Object);

            profileLoader.ApplyProfiles();

            profileLoader.Profiles.Count().ShouldBe(0);
        }
    }
}
