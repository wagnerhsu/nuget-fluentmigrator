// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="ObsoleteMigrationRunnerTests.cs" company="FluentMigrator Project">
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Exceptions;
using FluentMigrator.Runner.Infrastructure;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Tests.Integration.Migrations;

using Moq;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit
{
    /// <summary>
    /// Defines test class ObsoleteMigrationRunnerTests.
    /// </summary>
    [TestFixture]
    [Obsolete]
    public class ObsoleteMigrationRunnerTests
    {
        /// <summary>
        /// The runner
        /// </summary>
        private MigrationRunner _runner;
        /// <summary>
        /// The announcer
        /// </summary>
        private Mock<IAnnouncer> _announcer;
        /// <summary>
        /// The stop watch
        /// </summary>
        private Mock<IStopWatch> _stopWatch;

        /// <summary>
        /// The processor mock
        /// </summary>
        private Mock<IMigrationProcessor> _processorMock;
        /// <summary>
        /// The migration loader mock
        /// </summary>
        private Mock<IMigrationInformationLoader> _migrationLoaderMock;
        /// <summary>
        /// The profile loader mock
        /// </summary>
        private Mock<IProfileLoader> _profileLoaderMock;
        /// <summary>
        /// The runner context mock
        /// </summary>
        private Mock<IRunnerContext> _runnerContextMock;
        /// <summary>
        /// The migration list
        /// </summary>
        private SortedList<long, IMigrationInfo> _migrationList;
        /// <summary>
        /// The fake version loader
        /// </summary>
        private TestVersionLoader _fakeVersionLoader;
        /// <summary>
        /// The application context
        /// </summary>
        private int _applicationContext;

        /// <summary>
        /// Sets up.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            _applicationContext = new Random().Next();
            _migrationList = new SortedList<long, IMigrationInfo>();
            _runnerContextMock = new Mock<IRunnerContext>(MockBehavior.Loose);
            _processorMock = new Mock<IMigrationProcessor>(MockBehavior.Loose);
            _migrationLoaderMock = new Mock<IMigrationInformationLoader>(MockBehavior.Loose);
            _profileLoaderMock = new Mock<IProfileLoader>(MockBehavior.Loose);
            _profileLoaderMock.SetupGet(l => l.SupportsParameterlessApplyProfile).Returns(true);

            _announcer = new Mock<IAnnouncer>();
            _stopWatch = new Mock<IStopWatch>();
            _stopWatch.Setup(x => x.Time(It.IsAny<Action>())).Returns(new TimeSpan(1)).Callback((Action a) => a.Invoke());

            var options = new ProcessorOptions
                            {
                                PreviewOnly = false
                            };

            _processorMock.SetupGet(x => x.Options).Returns(options);
            _processorMock.SetupGet(x => x.ConnectionString).Returns(IntegrationTestOptions.SqlServer2008.ConnectionString);

            _runnerContextMock.SetupGet(x => x.Namespace).Returns("FluentMigrator.Tests.Integration.Migrations");
            _runnerContextMock.SetupGet(x => x.Announcer).Returns(_announcer.Object);
            _runnerContextMock.SetupGet(x => x.StopWatch).Returns(_stopWatch.Object);
            _runnerContextMock.SetupGet(x => x.Targets).Returns(new[] { Assembly.GetExecutingAssembly().ToString()});
            _runnerContextMock.SetupGet(x => x.Connection).Returns(IntegrationTestOptions.SqlServer2008.ConnectionString);
            _runnerContextMock.SetupGet(x => x.Database).Returns("sqlserver");
            _runnerContextMock.SetupGet(x => x.ApplicationContext).Returns(_applicationContext);

            _migrationLoaderMock.Setup(x => x.LoadMigrations()).Returns(()=> _migrationList);

            _runner = new MigrationRunner(Assembly.GetAssembly(typeof(MigrationRunnerTests)), _runnerContextMock.Object, _processorMock.Object)
                        {
                            MigrationLoader = _migrationLoaderMock.Object,
                            ProfileLoader = _profileLoaderMock.Object,
                        };

            _fakeVersionLoader = new TestVersionLoader(_runner, _runner.VersionLoader.VersionTableMetaData);

            _runner.VersionLoader = _fakeVersionLoader;

            _processorMock.Setup(x => x.SchemaExists(It.Is<string>(s => s == _runner.VersionLoader.VersionTableMetaData.SchemaName)))
                          .Returns(true);

            _processorMock.Setup(x => x.TableExists(It.Is<string>(s => s == _runner.VersionLoader.VersionTableMetaData.SchemaName),
                                                    It.Is<string>(t => t == _runner.VersionLoader.VersionTableMetaData.TableName)))
                          .Returns(true);
        }

        /// <summary>
        /// Loads the version data.
        /// </summary>
        /// <param name="fakeVersions">The fake versions.</param>
        private void LoadVersionData(params long[] fakeVersions)
        {
            _fakeVersionLoader.Versions.Clear();
            _migrationList.Clear();

            foreach (var version in fakeVersions)
            {
                _fakeVersionLoader.Versions.Add(version);
                _migrationList.Add(version,new MigrationInfo(version, TransactionBehavior.Default, new TestMigration()));
            }

            _fakeVersionLoader.LoadVersionInfo();
        }

        /// <summary>
        /// Defines the test method ProfilesAreAppliedWhenMigrateUpIsCalledWithNoVersion.
        /// </summary>
        [Test]
        public void ProfilesAreAppliedWhenMigrateUpIsCalledWithNoVersion()
        {
            _runner.MigrateUp();
            _profileLoaderMock.Verify(x => x.ApplyProfiles(), Times.Once());
        }

        /// <summary>
        /// Defines the test method ProfilesAreAppliedWhenMigrateUpIsCalledWithVersionParameter.
        /// </summary>
        [Test]
        public void ProfilesAreAppliedWhenMigrateUpIsCalledWithVersionParameter()
        {
            _runner.MigrateUp(2009010101);
            _profileLoaderMock.Verify(x => x.ApplyProfiles(), Times.Once());
        }

        /// <summary>
        /// Defines the test method ProfilesAreAppliedWhenMigrateDownIsCalled.
        /// </summary>
        [Test]
        public void ProfilesAreAppliedWhenMigrateDownIsCalled()
        {
            _runner.MigrateDown(2009010101);
            _profileLoaderMock.Verify(x => x.ApplyProfiles(), Times.Once());
        }

        /// <summary>
        /// Unit test which ensures that the application context is correctly propagated down to each migration class.
        /// </summary>
        [Test(Description = "Ensure that the application context is correctly propagated down to each migration class.")]
        public void CanPassApplicationContext()
        {
            IMigration migration = new TestEmptyMigration();
            _runner.Up(migration);

            Assert.AreEqual(_applicationContext, _runnerContextMock.Object.ApplicationContext, "The runner context does not have the expected application context.");
            Assert.AreEqual(_applicationContext, _runner.RunnerContext?.ApplicationContext, "The MigrationRunner does not have the expected application context.");
            Assert.AreEqual(_applicationContext, migration.ApplicationContext, "The migration does not have the expected application context.");
            _announcer.VerifyAll();
        }

        /// <summary>
        /// Defines the test method CanPassConnectionString.
        /// </summary>
        [Test]
        public void CanPassConnectionString()
        {
            IMigration migration = new TestEmptyMigration();
            _runner.Up(migration);

            Assert.AreEqual(IntegrationTestOptions.SqlServer2008.ConnectionString, migration.ConnectionString, "The migration does not have the expected connection string.");
            _announcer.VerifyAll();
        }

        /// <summary>
        /// Defines the test method CanAnnounceUp.
        /// </summary>
        [Test]
        public void CanAnnounceUp()
        {
            _announcer.Setup(x => x.Heading(It.IsRegex(ContainsAll("Test", "migrating"))));
            _runner.Up(new TestMigration());
            _announcer.VerifyAll();
        }

        /// <summary>
        /// Defines the test method CanAnnounceUpFinish.
        /// </summary>
        [Test]
        public void CanAnnounceUpFinish()
        {
            _announcer.Setup(x => x.Say(It.IsRegex(ContainsAll("Test", "migrated"))));
            _runner.Up(new TestMigration());
            _announcer.VerifyAll();
        }

        /// <summary>
        /// Defines the test method CanAnnounceDown.
        /// </summary>
        [Test]
        public void CanAnnounceDown()
        {
            _announcer.Setup(x => x.Heading(It.IsRegex(ContainsAll("Test", "reverting"))));
            _runner.Down(new TestMigration());
            _announcer.VerifyAll();
        }

        /// <summary>
        /// Defines the test method CanAnnounceDownFinish.
        /// </summary>
        [Test]
        public void CanAnnounceDownFinish()
        {
            _announcer.Setup(x => x.Say(It.IsRegex(ContainsAll("Test", "reverted"))));
            _runner.Down(new TestMigration());
            _announcer.VerifyAll();
        }

        /// <summary>
        /// Defines the test method CanAnnounceUpElapsedTime.
        /// </summary>
        [Test]
        public void CanAnnounceUpElapsedTime()
        {
            var ts = new TimeSpan(0, 0, 0, 1, 3);
            _announcer.Setup(x => x.ElapsedTime(It.Is<TimeSpan>(y => y == ts)));

            _stopWatch.Setup(x => x.ElapsedTime()).Returns(ts);

            _runner.Up(new TestMigration());

            _announcer.VerifyAll();
        }

        /// <summary>
        /// Defines the test method CanAnnounceDownElapsedTime.
        /// </summary>
        [Test]
        public void CanAnnounceDownElapsedTime()
        {
            var ts = new TimeSpan(0, 0, 0, 1, 3);
            _announcer.Setup(x => x.ElapsedTime(It.Is<TimeSpan>(y => y == ts)));

            _stopWatch.Setup(x => x.ElapsedTime()).Returns(ts);

            _runner.Down(new TestMigration());

            _announcer.VerifyAll();
        }

        /// <summary>
        /// Defines the test method CanReportExceptions.
        /// </summary>
        [Test]
        public void CanReportExceptions()
        {
            _processorMock.Setup(x => x.Process(It.IsAny<CreateTableExpression>())).Throws(new Exception("Oops"));

            var exception = Assert.Throws<Exception>(() => _runner.Up(new TestMigration()));

            Assert.That(exception.Message, Does.Contain("Oops"));
        }

        /// <summary>
        /// Defines the test method CanSayExpression.
        /// </summary>
        [Test]
        public void CanSayExpression()
        {
            _announcer.Setup(x => x.Say(It.IsRegex(ContainsAll("CreateTable"))));

            _stopWatch.Setup(x => x.ElapsedTime()).Returns(new TimeSpan(0, 0, 0, 1, 3));

            _runner.Up(new TestMigration());

            _announcer.VerifyAll();
        }

        /// <summary>
        /// Defines the test method CanTimeExpression.
        /// </summary>
        [Test]
        public void CanTimeExpression()
        {
            var ts = new TimeSpan(0, 0, 0, 1, 3);
            _announcer.Setup(x => x.ElapsedTime(It.Is<TimeSpan>(y => y == ts)));

            _stopWatch.Setup(x => x.ElapsedTime()).Returns(ts);

            _runner.Up(new TestMigration());

            _announcer.VerifyAll();
        }

        /// <summary>
        /// Determines whether the specified words contains all.
        /// </summary>
        /// <param name="words">The words.</param>
        /// <returns>System.String.</returns>
        private static string ContainsAll(params string[] words)
        {
            return ".*?" + string.Join(".*?", words) + ".*?";
        }

        /// <summary>
        /// Defines the test method LoadsCorrectCallingAssembly.
        /// </summary>
        /// <exception cref="InvalidOperationException">MigrationAssemblies aren't set</exception>
        [Test]
        public void LoadsCorrectCallingAssembly()
        {
            if (_runner.MigrationAssemblies == null)
            {
                throw new InvalidOperationException("MigrationAssemblies aren't set");
            }

            var asm = _runner.MigrationAssemblies.Assemblies.Single();
            asm.ShouldBe(Assembly.GetAssembly(typeof(MigrationRunnerTests)));
        }

        /// <summary>
        /// Defines the test method HasMigrationsToApplyUpWhenThereAreMigrations.
        /// </summary>
        [Test]
        public void HasMigrationsToApplyUpWhenThereAreMigrations()
        {
            const long fakeMigrationVersion1 = 2009010101;
            const long fakeMigrationVersion2 = 2009010102;
            LoadVersionData(fakeMigrationVersion1, fakeMigrationVersion2);
            _fakeVersionLoader.Versions.Remove(fakeMigrationVersion2);
            _fakeVersionLoader.LoadVersionInfo();

            _runner.HasMigrationsToApplyUp().ShouldBeTrue();
        }

        /// <summary>
        /// Defines the test method HasMigrationsToApplyUpWhenThereAreNoNewMigrations.
        /// </summary>
        [Test]
        public void HasMigrationsToApplyUpWhenThereAreNoNewMigrations()
        {
            const long fakeMigrationVersion1 = 2009010101;
            const long fakeMigrationVersion2 = 2009010102;
            LoadVersionData(fakeMigrationVersion1, fakeMigrationVersion2);

            _runner.HasMigrationsToApplyUp().ShouldBeFalse();
        }

        /// <summary>
        /// Defines the test method HasMigrationsToApplyUpToSpecificVersionWhenTheSpecificHasNotBeenApplied.
        /// </summary>
        [Test]
        public void HasMigrationsToApplyUpToSpecificVersionWhenTheSpecificHasNotBeenApplied()
        {
            const long fakeMigrationVersion1 = 2009010101;
            const long fakeMigrationVersion2 = 2009010102;
            LoadVersionData(fakeMigrationVersion1, fakeMigrationVersion2);
            _fakeVersionLoader.Versions.Remove(fakeMigrationVersion2);
            _fakeVersionLoader.LoadVersionInfo();

            _runner.HasMigrationsToApplyUp(fakeMigrationVersion2).ShouldBeTrue();
        }

        /// <summary>
        /// Defines the test method HasMigrationsToApplyUpToSpecificVersionWhenTheSpecificHasBeenApplied.
        /// </summary>
        [Test]
        public void HasMigrationsToApplyUpToSpecificVersionWhenTheSpecificHasBeenApplied()
        {
            const long fakeMigrationVersion1 = 2009010101;
            const long fakeMigrationVersion2 = 2009010102;
            LoadVersionData(fakeMigrationVersion1, fakeMigrationVersion2);
            _fakeVersionLoader.Versions.Remove(fakeMigrationVersion2);
            _fakeVersionLoader.LoadVersionInfo();

            _runner.HasMigrationsToApplyUp(fakeMigrationVersion1).ShouldBeFalse();
        }

        /// <summary>
        /// Defines the test method HasMigrationsToApplyRollbackWithOneMigrationApplied.
        /// </summary>
        [Test]
        public void HasMigrationsToApplyRollbackWithOneMigrationApplied()
        {
            const long fakeMigrationVersion1 = 2009010101;
            LoadVersionData(fakeMigrationVersion1);

            _runner.HasMigrationsToApplyRollback().ShouldBeTrue();
        }

        /// <summary>
        /// Defines the test method HasMigrationsToApplyRollbackWithNoMigrationsApplied.
        /// </summary>
        [Test]
        public void HasMigrationsToApplyRollbackWithNoMigrationsApplied()
        {
            LoadVersionData();

            _runner.HasMigrationsToApplyRollback().ShouldBeFalse();
        }

        /// <summary>
        /// Defines the test method HasMigrationsToApplyDownWhenTheVersionHasNotBeenApplied.
        /// </summary>
        [Test]
        public void HasMigrationsToApplyDownWhenTheVersionHasNotBeenApplied()
        {
            const long fakeMigrationVersion1 = 2009010101;
            const long fakeMigrationVersion2 = 2009010102;
            LoadVersionData(fakeMigrationVersion1, fakeMigrationVersion2);
            _fakeVersionLoader.Versions.Remove(fakeMigrationVersion2);
            _fakeVersionLoader.LoadVersionInfo();

            _runner.HasMigrationsToApplyDown(fakeMigrationVersion1).ShouldBeFalse();
        }

        /// <summary>
        /// Defines the test method HasMigrationsToApplyDownWhenTheVersionHasBeenApplied.
        /// </summary>
        [Test]
        public void HasMigrationsToApplyDownWhenTheVersionHasBeenApplied()
        {
            const long fakeMigrationVersion1 = 2009010101;
            const long fakeMigrationVersion2 = 2009010102;
            LoadVersionData(fakeMigrationVersion1, fakeMigrationVersion2);

            _runner.HasMigrationsToApplyDown(fakeMigrationVersion1).ShouldBeTrue();
        }

        /// <summary>
        /// Defines the test method RollbackOnlyOneStepsOfTwoShouldNotDeleteVersionInfoTable.
        /// </summary>
        [Test]
        public void RollbackOnlyOneStepsOfTwoShouldNotDeleteVersionInfoTable()
        {
            const long fakeMigrationVersion = 2009010101;
            const long fakeMigrationVersion2 = 2009010102;

            Assert.NotNull(_runner.VersionLoader.VersionTableMetaData.TableName);

            LoadVersionData(fakeMigrationVersion, fakeMigrationVersion2);

            _runner.VersionLoader.LoadVersionInfo();
            _runner.Rollback(1);

            _fakeVersionLoader.DidRemoveVersionTableGetCalled.ShouldBeFalse();
        }

        /// <summary>
        /// Defines the test method RollbackLastVersionShouldDeleteVersionInfoTable.
        /// </summary>
        [Test]
        public void RollbackLastVersionShouldDeleteVersionInfoTable()
        {
            const long fakeMigrationVersion = 2009010101;

            LoadVersionData(fakeMigrationVersion);

            Assert.NotNull(_runner.VersionLoader.VersionTableMetaData.TableName);

            _runner.Rollback(1);

            _fakeVersionLoader.DidRemoveVersionTableGetCalled.ShouldBeTrue();
        }

        /// <summary>
        /// Defines the test method RollbackToVersionZeroShouldDeleteVersionInfoTable.
        /// </summary>
        [Test]
        public void RollbackToVersionZeroShouldDeleteVersionInfoTable()
        {
            Assert.NotNull(_runner.VersionLoader.VersionTableMetaData.TableName);

            _runner.RollbackToVersion(0);

            _fakeVersionLoader.DidRemoveVersionTableGetCalled.ShouldBeTrue();
        }

        /// <summary>
        /// Defines the test method RollbackToVersionZeroShouldNotCreateVersionInfoTableAfterRemoval.
        /// </summary>
        [Test]
        public void RollbackToVersionZeroShouldNotCreateVersionInfoTableAfterRemoval()
        {
            var versionInfoTableName = _runner.VersionLoader.VersionTableMetaData.TableName;

            _runner.RollbackToVersion(0);

            //Should only be called once in setup
            _processorMock.Verify(
                pm => pm.Process(It.Is<CreateTableExpression>(
                    dte => dte.TableName == versionInfoTableName)
                    ),
                    Times.Once()
                );
        }

        /// <summary>
        /// Defines the test method RollbackToVersionShouldShouldLimitMigrationsToNamespace.
        /// </summary>
        [Test]
        public void RollbackToVersionShouldShouldLimitMigrationsToNamespace()
        {
            const long fakeMigration1 = 2011010101;
            const long fakeMigration2 = 2011010102;
            const long fakeMigration3 = 2011010103;

            LoadVersionData(fakeMigration1,fakeMigration3);

            _fakeVersionLoader.Versions.Add(fakeMigration2);
            _fakeVersionLoader.LoadVersionInfo();

            _runner.RollbackToVersion(2011010101);

            _fakeVersionLoader.Versions.ShouldContain(fakeMigration1);
            _fakeVersionLoader.Versions.ShouldContain(fakeMigration2);
            _fakeVersionLoader.Versions.ShouldNotContain(fakeMigration3);
        }

        /// <summary>
        /// Defines the test method RollbackToVersionZeroShouldShouldLimitMigrationsToNamespace.
        /// </summary>
        [Test]
        public void RollbackToVersionZeroShouldShouldLimitMigrationsToNamespace()
        {
            const long fakeMigration1 = 2011010101;
            const long fakeMigration2 = 2011010102;
            const long fakeMigration3 = 2011010103;

            LoadVersionData(fakeMigration1, fakeMigration2, fakeMigration3);

            _migrationList.Remove(fakeMigration1);
            _migrationList.Remove(fakeMigration2);
            _fakeVersionLoader.LoadVersionInfo();

            _runner.RollbackToVersion(0);

            _fakeVersionLoader.Versions.ShouldContain(fakeMigration1);
            _fakeVersionLoader.Versions.ShouldContain(fakeMigration2);
            _fakeVersionLoader.Versions.ShouldNotContain(fakeMigration3);
        }

        /// <summary>
        /// Defines the test method RollbackShouldLimitMigrationsToNamespace.
        /// </summary>
        [Test]
        public void RollbackShouldLimitMigrationsToNamespace()
        {
            const long fakeMigration1 = 2011010101;
            const long fakeMigration2 = 2011010102;
            const long fakeMigration3 = 2011010103;

            LoadVersionData(fakeMigration1, fakeMigration3);

            _fakeVersionLoader.Versions.Add(fakeMigration2);
            _fakeVersionLoader.LoadVersionInfo();

            _runner.Rollback(2);

            _fakeVersionLoader.Versions.ShouldNotContain(fakeMigration1);
            _fakeVersionLoader.Versions.ShouldContain(fakeMigration2);
            _fakeVersionLoader.Versions.ShouldNotContain(fakeMigration3);

            _fakeVersionLoader.DidRemoveVersionTableGetCalled.ShouldBeFalse();
        }

        /// <summary>
        /// Defines the test method RollbackToVersionShouldLoadVersionInfoIfVersionGreaterThanZero.
        /// </summary>
        [Test]
        public void RollbackToVersionShouldLoadVersionInfoIfVersionGreaterThanZero()
        {
            var versionInfoTableName = _runner.VersionLoader.VersionTableMetaData.TableName;

            _runner.RollbackToVersion(1);

            _fakeVersionLoader.DidRemoveVersionTableGetCalled.ShouldBeFalse();

            //Once in setup
            _processorMock.Verify(
                pm => pm.Process(It.Is<CreateTableExpression>(
                    dte => dte.TableName == versionInfoTableName)
                    ),
                    Times.Exactly(1)
                );

            //After setup is done, fake version loader owns the proccess
            _fakeVersionLoader.DidLoadVersionInfoGetCalled.ShouldBe(true);
        }

        /// <summary>
        /// Defines the test method ValidateVersionOrderingShouldReturnNothingIfNoUnappliedMigrations.
        /// </summary>
        [Test]
        public void ValidateVersionOrderingShouldReturnNothingIfNoUnappliedMigrations()
        {
            const long version1 = 2011010101;
            const long version2 = 2011010102;

            var mockMigration1 = new Mock<IMigration>();
            var mockMigration2 = new Mock<IMigration>();

            LoadVersionData(version1, version2);

            _migrationList.Clear();
            _migrationList.Add(version1,new MigrationInfo(version1, TransactionBehavior.Default, mockMigration1.Object));
            _migrationList.Add(version2, new MigrationInfo(version2, TransactionBehavior.Default, mockMigration2.Object));

            Assert.DoesNotThrow(() => _runner.ValidateVersionOrder());

            _announcer.Verify(a => a.Say("Version ordering valid."));

            _fakeVersionLoader.DidRemoveVersionTableGetCalled.ShouldBeFalse();
        }

        /// <summary>
        /// Defines the test method ValidateVersionOrderingShouldReturnNothingIfUnappliedMigrationVersionIsGreaterThanLatestAppliedMigration.
        /// </summary>
        [Test]
        public void ValidateVersionOrderingShouldReturnNothingIfUnappliedMigrationVersionIsGreaterThanLatestAppliedMigration()
        {
            const long version1 = 2011010101;
            const long version2 = 2011010102;

            var mockMigration1 = new Mock<IMigration>();
            var mockMigration2 = new Mock<IMigration>();

            LoadVersionData(version1);

            _migrationList.Clear();
            _migrationList.Add(version1, new MigrationInfo(version1, TransactionBehavior.Default, mockMigration1.Object));
            _migrationList.Add(version2, new MigrationInfo(version2, TransactionBehavior.Default, mockMigration2.Object));

            Assert.DoesNotThrow(() => _runner.ValidateVersionOrder());

            _announcer.Verify(a => a.Say("Version ordering valid."));

            _fakeVersionLoader.DidRemoveVersionTableGetCalled.ShouldBeFalse();
        }

        /// <summary>
        /// Defines the test method ValidateVersionOrderingShouldThrowExceptionIfUnappliedMigrationVersionIsLessThanGreatestAppliedMigrationVersion.
        /// </summary>
        [Test]
        public void ValidateVersionOrderingShouldThrowExceptionIfUnappliedMigrationVersionIsLessThanGreatestAppliedMigrationVersion()
        {
            const long version1 = 2011010101;
            const long version2 = 2011010102;
            const long version3 = 2011010103;
            const long version4 = 2011010104;

            var mockMigration1 = new Mock<IMigration>();
            var mockMigration2 = new Mock<IMigration>();
            var mockMigration3 = new Mock<IMigration>();
            var mockMigration4 = new Mock<IMigration>();

            LoadVersionData(version1, version4);

            _migrationList.Clear();
            _migrationList.Add(version1, new MigrationInfo(version1, TransactionBehavior.Default, mockMigration1.Object));
            _migrationList.Add(version2, new MigrationInfo(version2, TransactionBehavior.Default, mockMigration2.Object));
            _migrationList.Add(version3, new MigrationInfo(version3, TransactionBehavior.Default, mockMigration3.Object));
            _migrationList.Add(version4, new MigrationInfo(version4, TransactionBehavior.Default, mockMigration4.Object));

            var exception = Assert.Throws<VersionOrderInvalidException>(() => _runner.ValidateVersionOrder());

            var invalidMigrations = exception.InvalidMigrations.ToList();
            invalidMigrations.Count.ShouldBe(2);
            invalidMigrations.Select(x => x.Key).ShouldContain(version2);
            invalidMigrations.Select(x => x.Key).ShouldContain(version3);

            _fakeVersionLoader.DidRemoveVersionTableGetCalled.ShouldBeFalse();
        }

        /// <summary>
        /// Defines the test method CanListVersions.
        /// </summary>
        [Test]
        public void CanListVersions()
        {
            const long version1 = 2011010101;
            const long version2 = 2011010102;
            const long version3 = 2011010103;
            const long version4 = 2011010104;

            var mockMigration1 = new Mock<IMigration>();
            var mockMigration2 = new Mock<IMigration>();
            var mockMigration3 = new Mock<IMigration>();
            var mockMigration4 = new Mock<IMigration>();

            LoadVersionData(version1, version3);

            _migrationList.Clear();
            _migrationList.Add(version1, new MigrationInfo(version1, TransactionBehavior.Default, mockMigration1.Object));
            _migrationList.Add(version2, new MigrationInfo(version2, TransactionBehavior.Default, mockMigration2.Object));
            _migrationList.Add(version3, new MigrationInfo(version3, TransactionBehavior.Default, mockMigration3.Object));
            _migrationList.Add(version4, new MigrationInfo(version4, TransactionBehavior.Default, true, mockMigration4.Object));

            _runner.ListMigrations();

            _announcer.Verify(a => a.Say("2011010101: IMigrationProxy"));
            _announcer.Verify(a => a.Say("2011010102: IMigrationProxy (not applied)"));
            _announcer.Verify(a => a.Emphasize("2011010103: IMigrationProxy (current)"));
            _announcer.Verify(a => a.Emphasize("2011010104: IMigrationProxy (not applied, BREAKING)"));
        }

        /// <summary>
        /// Defines the test method IfMigrationHasAnInvalidExpressionDuringUpActionShouldThrowAnExceptionAndAnnounceTheError.
        /// </summary>
        [Test]
        public void IfMigrationHasAnInvalidExpressionDuringUpActionShouldThrowAnExceptionAndAnnounceTheError()
        {
            var invalidMigration = new Mock<IMigration>();
            var invalidExpression = new UpdateDataExpression {TableName = "Test"};
            invalidMigration.Setup(m => m.GetUpExpressions(It.IsAny<IMigrationContext>())).Callback((IMigrationContext mc) => mc.Expressions.Add(invalidExpression));

            Assert.Throws<InvalidMigrationException>(() => _runner.Up(invalidMigration.Object));

            var expectedErrorMessage = ErrorMessages.UpdateDataExpressionMustSpecifyWhereClauseOrAllRows;
            _announcer.Verify(a => a.Error(It.Is<string>(s => s.Contains($"UpdateDataExpression: {expectedErrorMessage}"))));
        }

        /// <summary>
        /// Defines the test method IfMigrationHasAnInvalidExpressionDuringDownActionShouldThrowAnExceptionAndAnnounceTheError.
        /// </summary>
        [Test]
        public void IfMigrationHasAnInvalidExpressionDuringDownActionShouldThrowAnExceptionAndAnnounceTheError()
        {
            var invalidMigration = new Mock<IMigration>();
            var invalidExpression = new UpdateDataExpression { TableName = "Test" };
            invalidMigration.Setup(m => m.GetDownExpressions(It.IsAny<IMigrationContext>())).Callback((IMigrationContext mc) => mc.Expressions.Add(invalidExpression));

            Assert.Throws<InvalidMigrationException>(() => _runner.Down(invalidMigration.Object));

            var expectedErrorMessage = ErrorMessages.UpdateDataExpressionMustSpecifyWhereClauseOrAllRows;
            _announcer.Verify(a => a.Error(It.Is<string>(s => s.Contains($"UpdateDataExpression: {expectedErrorMessage}"))));
        }

        /// <summary>
        /// Defines the test method IfMigrationHasTwoInvalidExpressionsShouldAnnounceBothErrors.
        /// </summary>
        [Test]
        public void IfMigrationHasTwoInvalidExpressionsShouldAnnounceBothErrors()
        {
            var invalidMigration = new Mock<IMigration>();
            var invalidExpression = new UpdateDataExpression { TableName = "Test" };
            var secondInvalidExpression = new CreateColumnExpression();
            invalidMigration.Setup(m => m.GetUpExpressions(It.IsAny<IMigrationContext>()))
                .Callback((IMigrationContext mc) => { mc.Expressions.Add(invalidExpression); mc.Expressions.Add(secondInvalidExpression); });

            Assert.Throws<InvalidMigrationException>(() => _runner.Up(invalidMigration.Object));

            _announcer.Verify(a => a.Error(It.Is<string>(s => s.Contains($"UpdateDataExpression: {ErrorMessages.UpdateDataExpressionMustSpecifyWhereClauseOrAllRows}"))));
            _announcer.Verify(a => a.Error(It.Is<string>(s => s.Contains($"CreateColumnExpression: {ErrorMessages.TableNameCannotBeNullOrEmpty}"))));
        }

        /// <summary>
        /// Defines the test method CanLoadCustomMigrationConventions.
        /// </summary>
        [Test]
        public void CanLoadCustomMigrationConventions()
        {
            Assert.That(_runner.Conventions, Is.TypeOf<MigrationRunnerTests.CustomMigrationConventions>());
        }

        /// <summary>
        /// Defines the test method CanLoadDefaultMigrationConventionsIfNoCustomConventionsAreSpecified.
        /// </summary>
        [Test]
        public void CanLoadDefaultMigrationConventionsIfNoCustomConventionsAreSpecified()
        {
            var processorMock = new Mock<IMigrationProcessor>(MockBehavior.Loose);

            var options = new ProcessorOptions
            {
                PreviewOnly = false
            };

            processorMock.SetupGet(x => x.Options).Returns(options);

            var asm = "s".GetType().Assembly;

            var runner = new MigrationRunner(asm, _runnerContextMock.Object, processorMock.Object);

            Assert.That(runner.Conventions, Is.TypeOf<DefaultMigrationRunnerConventions>());
        }

        /// <summary>
        /// Defines the test method CanBlockBreakingChangesByDefault.
        /// </summary>
        [Test]
        public void CanBlockBreakingChangesByDefault()
        {
            InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() =>
                _runner.ApplyMigrationUp(
                    new MigrationInfo(7, TransactionBehavior.Default, true, new TestBreakingMigration()), true));

            Assert.NotNull(ex);

            Assert.AreEqual(
                "The migration 7: TestBreakingMigration is identified as a breaking change, and will not be executed unless the necessary flag (allow-breaking-changes|abc) is passed to the runner.",
                ex.Message);
        }

        /// <summary>
        /// Defines the test method CanRunBreakingChangesIfSpecified.
        /// </summary>
        [Test]
        public void CanRunBreakingChangesIfSpecified()
        {
            _runnerContextMock.SetupGet(rcm => rcm.AllowBreakingChange).Returns(true);

            Assert.DoesNotThrow(() =>
                _runner.ApplyMigrationUp(
                    new MigrationInfo(7, TransactionBehavior.Default, true, new TestBreakingMigration()), true));
        }

        /// <summary>
        /// Defines the test method CanRunBreakingChangesInPreview.
        /// </summary>
        [Test]
        public void CanRunBreakingChangesInPreview()
        {
            _runnerContextMock.SetupGet(rcm => rcm.PreviewOnly).Returns(true);

            _runnerContextMock.SetupGet(rcm => rcm.AllowBreakingChange).Returns(true);

            Assert.DoesNotThrow(() =>
                _runner.ApplyMigrationUp(
                    new MigrationInfo(7, TransactionBehavior.Default, true, new TestBreakingMigration()), true));
        }
    }
}
