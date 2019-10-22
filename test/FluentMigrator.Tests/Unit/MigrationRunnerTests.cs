// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="MigrationRunnerTests.cs" company="FluentMigrator Project">
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
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Exceptions;
using FluentMigrator.Runner.Infrastructure;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Tests.Integration.Migrations;
using FluentMigrator.Tests.Integration.Migrations.Constrained.Constraints;
using FluentMigrator.Tests.Integration.Migrations.Constrained.ConstraintsMultiple;
using FluentMigrator.Tests.Integration.Migrations.Constrained.ConstraintsSuccess;
using FluentMigrator.Tests.Logging;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Moq;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit
{
    /// <summary>
    /// Defines test class MigrationRunnerTests.
    /// </summary>
    [TestFixture]
    public class MigrationRunnerTests
    {
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
        /// The assembly source mock
        /// </summary>
        private Mock<IAssemblySource> _assemblySourceMock;
        /// <summary>
        /// The migration scope handler mock
        /// </summary>
        private Mock<IMigrationScopeManager> _migrationScopeHandlerMock;

        /// <summary>
        /// The log messages
        /// </summary>
        private ICollection<string> _logMessages;
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
        /// The service collection
        /// </summary>
        private IServiceCollection _serviceCollection;

        /// <summary>
        /// Sets up.
        /// </summary>
        [SetUp]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void SetUp()
        {
            var asm = Assembly.GetExecutingAssembly();

            _applicationContext = new Random().Next();
            _migrationList = new SortedList<long, IMigrationInfo>();
            _processorMock = new Mock<IMigrationProcessor>(MockBehavior.Loose);
            _migrationLoaderMock = new Mock<IMigrationInformationLoader>(MockBehavior.Loose);
            _profileLoaderMock = new Mock<IProfileLoader>(MockBehavior.Loose);
            _migrationScopeHandlerMock = new Mock<IMigrationScopeManager>(MockBehavior.Loose);
            _migrationScopeHandlerMock.Setup(x => x.CreateOrWrapMigrationScope(It.IsAny<bool>())).Returns(new NoOpMigrationScope());

            _stopWatch = new Mock<IStopWatch>();
            _stopWatch.Setup(x => x.Time(It.IsAny<Action>())).Returns(new TimeSpan(1)).Callback((Action a) => a.Invoke());

            _assemblySourceMock = new Mock<IAssemblySource>();
            _assemblySourceMock.SetupGet(x => x.Assemblies).Returns(new[] { asm });

            _migrationLoaderMock.Setup(x => x.LoadMigrations()).Returns(()=> _migrationList);

            _logMessages = new List<string>();
            var connectionString = IntegrationTestOptions.SqlServer2008.ConnectionString;
            _serviceCollection = ServiceCollectionExtensions.CreateServices()
                .WithProcessor(_processorMock)
                .AddSingleton<ILoggerProvider>(new TextLineLoggerProvider(_logMessages, new FluentMigratorLoggerOptions() { ShowElapsedTime = true }))
                .AddSingleton(_stopWatch.Object)
                .AddSingleton(_assemblySourceMock.Object)
                .AddSingleton(_migrationLoaderMock.Object)
                .AddScoped<IConnectionStringReader>(_ => new PassThroughConnectionStringReader(connectionString))
                .AddScoped(_ => _profileLoaderMock.Object)
#pragma warning disable 612
                .Configure<RunnerOptions>(opt => opt.ApplicationContext = _applicationContext)
#pragma warning restore 612
                .Configure<ProcessorOptions>(
                    opt => opt.ConnectionString = connectionString)
                .Configure<AssemblySourceOptions>(opt => opt.AssemblyNames = new []{ asm.FullName })
                .Configure<TypeFilterOptions>(
                    opt => opt.Namespace = "FluentMigrator.Tests.Integration.Migrations")
                .ConfigureRunner(builder => builder.WithRunnerConventions(new CustomMigrationConventions()));
        }

        /// <summary>
        /// Creates the runner.
        /// </summary>
        /// <param name="initAction">The initialize action.</param>
        /// <returns>MigrationRunner.</returns>
        private MigrationRunner CreateRunner(Action<IServiceCollection> initAction = null)
        {
            initAction?.Invoke(_serviceCollection);
            var serviceProvider = _serviceCollection
                .BuildServiceProvider();

            var runner = (MigrationRunner) serviceProvider.GetRequiredService<IMigrationRunner>();
            _fakeVersionLoader = new TestVersionLoader(runner, runner.VersionLoader.VersionTableMetaData);
            runner.VersionLoader = _fakeVersionLoader;

            var readTableDataResult = new DataSet();
            readTableDataResult.Tables.Add(new DataTable());

            _processorMock.Setup(x => x.ReadTableData(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(readTableDataResult);
            _processorMock.Setup(x => x.SchemaExists(It.Is<string>(s => s == runner.VersionLoader.VersionTableMetaData.SchemaName)))
                .Returns(true);

            _processorMock.Setup(x => x.TableExists(It.Is<string>(s => s == runner.VersionLoader.VersionTableMetaData.SchemaName),
                    It.Is<string>(t => t == runner.VersionLoader.VersionTableMetaData.TableName)))
                .Returns(true);

            return runner;
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
            var runner = CreateRunner();
            runner.MigrateUp();
            _profileLoaderMock.Verify(x => x.ApplyProfiles(runner), Times.Once());
        }

        /// <summary>
        /// Defines the test method ProfilesAreAppliedWhenMigrateUpIsCalledWithVersionParameter.
        /// </summary>
        [Test]
        public void ProfilesAreAppliedWhenMigrateUpIsCalledWithVersionParameter()
        {
            var runner = CreateRunner();
            runner.MigrateUp(2009010101);
            _profileLoaderMock.Verify(x => x.ApplyProfiles(runner), Times.Once());
        }

        /// <summary>
        /// Defines the test method ProfilesAreAppliedWhenMigrateDownIsCalled.
        /// </summary>
        [Test]
        public void ProfilesAreAppliedWhenMigrateDownIsCalled()
        {
            var runner = CreateRunner();
            runner.MigrateDown(2009010101);
            _profileLoaderMock.Verify(x => x.ApplyProfiles(runner), Times.Once());
        }

        /// <summary>
        /// Unit test which ensures that the application context is correctly propagated down to each migration class.
        /// </summary>
        [Test(Description = "Ensure that the application context is correctly propagated down to each migration class.")]
        public void CanPassApplicationContext()
        {
            var runner = CreateRunner();

            IMigration migration = new TestEmptyMigration();
            runner.Up(migration);

            Assert.AreEqual(_applicationContext, migration.ApplicationContext, "The migration does not have the expected application context.");
        }

        /// <summary>
        /// Defines the test method CanPassConnectionString.
        /// </summary>
        [Test]
        public void CanPassConnectionString()
        {
            var runner = CreateRunner();

            IMigration migration = new TestEmptyMigration();
            runner.Up(migration);

            Assert.AreEqual(IntegrationTestOptions.SqlServer2008.ConnectionString, migration.ConnectionString, "The migration does not have the expected connection string.");
        }

        /// <summary>
        /// Defines the test method CanAnnounceUp.
        /// </summary>
        [Test]
        public void CanAnnounceUp()
        {
            var runner = CreateRunner();
            runner.Up(new TestMigration());
            _logMessages.ShouldContain(l => LineContainsAll(l, "Test", "migrating"));
        }

        /// <summary>
        /// Defines the test method CanAnnounceUpFinish.
        /// </summary>
        [Test]
        public void CanAnnounceUpFinish()
        {
            var runner = CreateRunner();
            runner.Up(new TestMigration());
            _logMessages.ShouldContain(l => LineContainsAll(l, "Test", "migrated"));
        }

        /// <summary>
        /// Defines the test method CanAnnounceDown.
        /// </summary>
        [Test]
        public void CanAnnounceDown()
        {
            var runner = CreateRunner();
            runner.Down(new TestMigration());
            _logMessages.ShouldContain(l => LineContainsAll(l, "Test", "reverting"));
        }

        /// <summary>
        /// Defines the test method CanAnnounceDownFinish.
        /// </summary>
        [Test]
        public void CanAnnounceDownFinish()
        {
            var runner = CreateRunner();
            runner.Down(new TestMigration());
            _logMessages.ShouldContain(l => LineContainsAll(l, "Test", "reverted"));
        }

        /// <summary>
        /// Defines the test method CanAnnounceUpElapsedTime.
        /// </summary>
        [Test]
        public void CanAnnounceUpElapsedTime()
        {
            var ts = new TimeSpan(0, 0, 0, 1, 3);

            _stopWatch.Setup(x => x.ElapsedTime()).Returns(ts);

            var runner = CreateRunner();
            runner.Up(new TestMigration());

            _logMessages.ShouldContain(l => l.Equals($"=> {ts.TotalSeconds}s"));
        }

        /// <summary>
        /// Defines the test method CanAnnounceDownElapsedTime.
        /// </summary>
        [Test]
        public void CanAnnounceDownElapsedTime()
        {
            var ts = new TimeSpan(0, 0, 0, 1, 3);

            _stopWatch.Setup(x => x.ElapsedTime()).Returns(ts);

            var runner = CreateRunner();
            runner.Down(new TestMigration());

            _logMessages.ShouldContain(l => l.Equals($"=> {ts.TotalSeconds}s"));
        }

        /// <summary>
        /// Defines the test method CanReportExceptions.
        /// </summary>
        [Test]
        public void CanReportExceptions()
        {
            var runner = CreateRunner();

            _processorMock.Setup(x => x.Process(It.IsAny<CreateTableExpression>())).Throws(new Exception("Oops"));

            var exception = Assert.Throws<Exception>(() => runner.Up(new TestMigration()));

            Assert.That(exception.Message, Does.Contain("Oops"));
        }

        /// <summary>
        /// Defines the test method CanSayExpression.
        /// </summary>
        [Test]
        public void CanSayExpression()
        {
            _stopWatch.Setup(x => x.ElapsedTime()).Returns(new TimeSpan(0, 0, 0, 1, 3));

            var runner = CreateRunner();
            runner.Up(new TestMigration());

            _logMessages.ShouldContain(l => LineContainsAll(l, "CreateTable"));
        }

        /// <summary>
        /// Defines the test method CanTimeExpression.
        /// </summary>
        [Test]
        public void CanTimeExpression()
        {
            var ts = new TimeSpan(0, 0, 0, 1, 3);

            _stopWatch.Setup(x => x.ElapsedTime()).Returns(ts);

            var runner = CreateRunner();
            runner.Up(new TestMigration());

            _logMessages.ShouldContain(l => l.Equals($"=> {ts.TotalSeconds}s"));
        }

        /// <summary>
        /// Defines the test method HasMigrationsToApplyUpWhenThereAreMigrations.
        /// </summary>
        [Test]
        public void HasMigrationsToApplyUpWhenThereAreMigrations()
        {
            var runner = CreateRunner();

            const long fakeMigrationVersion1 = 2009010101;
            const long fakeMigrationVersion2 = 2009010102;
            LoadVersionData(fakeMigrationVersion1, fakeMigrationVersion2);
            _fakeVersionLoader.Versions.Remove(fakeMigrationVersion2);
            _fakeVersionLoader.LoadVersionInfo();

            runner.HasMigrationsToApplyUp().ShouldBeTrue();
        }

        /// <summary>
        /// Defines the test method HasMigrationsToApplyUpWhenThereAreNoNewMigrations.
        /// </summary>
        [Test]
        public void HasMigrationsToApplyUpWhenThereAreNoNewMigrations()
        {
            var runner = CreateRunner();

            const long fakeMigrationVersion1 = 2009010101;
            const long fakeMigrationVersion2 = 2009010102;
            LoadVersionData(fakeMigrationVersion1, fakeMigrationVersion2);

            runner.HasMigrationsToApplyUp().ShouldBeFalse();
        }

        /// <summary>
        /// Defines the test method HasMigrationsToApplyUpToSpecificVersionWhenTheSpecificHasNotBeenApplied.
        /// </summary>
        [Test]
        public void HasMigrationsToApplyUpToSpecificVersionWhenTheSpecificHasNotBeenApplied()
        {
            var runner = CreateRunner();

            const long fakeMigrationVersion1 = 2009010101;
            const long fakeMigrationVersion2 = 2009010102;
            LoadVersionData(fakeMigrationVersion1, fakeMigrationVersion2);
            _fakeVersionLoader.Versions.Remove(fakeMigrationVersion2);
            _fakeVersionLoader.LoadVersionInfo();

            runner.HasMigrationsToApplyUp(fakeMigrationVersion2).ShouldBeTrue();
        }

        /// <summary>
        /// Defines the test method HasMigrationsToApplyUpToSpecificVersionWhenTheSpecificHasBeenApplied.
        /// </summary>
        [Test]
        public void HasMigrationsToApplyUpToSpecificVersionWhenTheSpecificHasBeenApplied()
        {
            var runner = CreateRunner();

            const long fakeMigrationVersion1 = 2009010101;
            const long fakeMigrationVersion2 = 2009010102;
            LoadVersionData(fakeMigrationVersion1, fakeMigrationVersion2);
            _fakeVersionLoader.Versions.Remove(fakeMigrationVersion2);
            _fakeVersionLoader.LoadVersionInfo();

            runner.HasMigrationsToApplyUp(fakeMigrationVersion1).ShouldBeFalse();
        }

        /// <summary>
        /// Defines the test method HasMigrationsToApplyRollbackWithOneMigrationApplied.
        /// </summary>
        [Test]
        public void HasMigrationsToApplyRollbackWithOneMigrationApplied()
        {
            var runner = CreateRunner();

            const long fakeMigrationVersion1 = 2009010101;
            LoadVersionData(fakeMigrationVersion1);

            runner.HasMigrationsToApplyRollback().ShouldBeTrue();
        }

        /// <summary>
        /// Defines the test method HasMigrationsToApplyRollbackWithNoMigrationsApplied.
        /// </summary>
        [Test]
        public void HasMigrationsToApplyRollbackWithNoMigrationsApplied()
        {
            var runner = CreateRunner();
            LoadVersionData();
            runner.HasMigrationsToApplyRollback().ShouldBeFalse();
        }

        /// <summary>
        /// Defines the test method HasMigrationsToApplyDownWhenTheVersionHasNotBeenApplied.
        /// </summary>
        [Test]
        public void HasMigrationsToApplyDownWhenTheVersionHasNotBeenApplied()
        {
            var runner = CreateRunner();

            const long fakeMigrationVersion1 = 2009010101;
            const long fakeMigrationVersion2 = 2009010102;
            LoadVersionData(fakeMigrationVersion1, fakeMigrationVersion2);
            _fakeVersionLoader.Versions.Remove(fakeMigrationVersion2);
            _fakeVersionLoader.LoadVersionInfo();

            runner.HasMigrationsToApplyDown(fakeMigrationVersion1).ShouldBeFalse();
        }

        /// <summary>
        /// Defines the test method HasMigrationsToApplyDownWhenTheVersionHasBeenApplied.
        /// </summary>
        [Test]
        public void HasMigrationsToApplyDownWhenTheVersionHasBeenApplied()
        {
            var runner = CreateRunner();

            const long fakeMigrationVersion1 = 2009010101;
            const long fakeMigrationVersion2 = 2009010102;
            LoadVersionData(fakeMigrationVersion1, fakeMigrationVersion2);

            runner.HasMigrationsToApplyDown(fakeMigrationVersion1).ShouldBeTrue();
        }

        /// <summary>
        /// Defines the test method RollbackOnlyOneStepsOfTwoShouldNotDeleteVersionInfoTable.
        /// </summary>
        [Test]
        public void RollbackOnlyOneStepsOfTwoShouldNotDeleteVersionInfoTable()
        {
            const long fakeMigrationVersion = 2009010101;
            const long fakeMigrationVersion2 = 2009010102;

            var runner = CreateRunner();
            Assert.NotNull(runner.VersionLoader.VersionTableMetaData.TableName);

            LoadVersionData(fakeMigrationVersion, fakeMigrationVersion2);

            runner.VersionLoader.LoadVersionInfo();
            runner.Rollback(1);

            _fakeVersionLoader.DidRemoveVersionTableGetCalled.ShouldBeFalse();
        }

        /// <summary>
        /// Defines the test method RollbackLastVersionShouldDeleteVersionInfoTable.
        /// </summary>
        [Test]
        public void RollbackLastVersionShouldDeleteVersionInfoTable()
        {
            var runner = CreateRunner();

            const long fakeMigrationVersion = 2009010101;

            LoadVersionData(fakeMigrationVersion);

            Assert.NotNull(runner.VersionLoader.VersionTableMetaData.TableName);

            runner.Rollback(1);

            _fakeVersionLoader.DidRemoveVersionTableGetCalled.ShouldBeTrue();
        }

        /// <summary>
        /// Defines the test method RollbackToVersionZeroShouldDeleteVersionInfoTable.
        /// </summary>
        [Test]
        public void RollbackToVersionZeroShouldDeleteVersionInfoTable()
        {
            var runner = CreateRunner();

            Assert.NotNull(runner.VersionLoader.VersionTableMetaData.TableName);

            runner.RollbackToVersion(0);

            _fakeVersionLoader.DidRemoveVersionTableGetCalled.ShouldBeTrue();
        }

        /// <summary>
        /// Defines the test method RollbackToVersionZeroShouldNotCreateVersionInfoTableAfterRemoval.
        /// </summary>
        [Test]
        public void RollbackToVersionZeroShouldNotCreateVersionInfoTableAfterRemoval()
        {
            var runner = CreateRunner();

            var versionInfoTableName = runner.VersionLoader.VersionTableMetaData.TableName;

            runner.RollbackToVersion(0);

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

            var runner = CreateRunner();
            LoadVersionData(fakeMigration1,fakeMigration3);

            _fakeVersionLoader.Versions.Add(fakeMigration2);
            _fakeVersionLoader.LoadVersionInfo();

            runner.RollbackToVersion(2011010101);

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

            var runner = CreateRunner();
            LoadVersionData(fakeMigration1, fakeMigration2, fakeMigration3);

            _migrationList.Remove(fakeMigration1);
            _migrationList.Remove(fakeMigration2);
            _fakeVersionLoader.LoadVersionInfo();

            runner.RollbackToVersion(0);

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
            var runner = CreateRunner();

            const long fakeMigration1 = 2011010101;
            const long fakeMigration2 = 2011010102;
            const long fakeMigration3 = 2011010103;

            LoadVersionData(fakeMigration1, fakeMigration3);

            _fakeVersionLoader.Versions.Add(fakeMigration2);
            _fakeVersionLoader.LoadVersionInfo();

            runner.Rollback(2);

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
            var runner = CreateRunner();

            var versionInfoTableName = runner.VersionLoader.VersionTableMetaData.TableName;

            runner.RollbackToVersion(1);

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

            var runner = CreateRunner();
            LoadVersionData(version1, version2);

            _migrationList.Clear();
            _migrationList.Add(version1,new MigrationInfo(version1, TransactionBehavior.Default, mockMigration1.Object));
            _migrationList.Add(version2, new MigrationInfo(version2, TransactionBehavior.Default, mockMigration2.Object));

            Assert.DoesNotThrow(() => runner.ValidateVersionOrder());

            _logMessages.ShouldContain(l => LineContainsAll(l, "Version ordering valid."));

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

            var runner = CreateRunner();
            LoadVersionData(version1);

            _migrationList.Clear();
            _migrationList.Add(version1, new MigrationInfo(version1, TransactionBehavior.Default, mockMigration1.Object));
            _migrationList.Add(version2, new MigrationInfo(version2, TransactionBehavior.Default, mockMigration2.Object));

            Assert.DoesNotThrow(() => runner.ValidateVersionOrder());

            _logMessages.ShouldContain(l => LineContainsAll(l, "Version ordering valid."));

            _fakeVersionLoader.DidRemoveVersionTableGetCalled.ShouldBeFalse();
        }

        /// <summary>
        /// Defines the test method ValidateVersionOrderingShouldThrowExceptionIfUnappliedMigrationVersionIsLessThanGreatestAppliedMigrationVersion.
        /// </summary>
        [Test]
        public void ValidateVersionOrderingShouldThrowExceptionIfUnappliedMigrationVersionIsLessThanGreatestAppliedMigrationVersion()
        {
            var runner = CreateRunner();

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

            var exception = Assert.Throws<VersionOrderInvalidException>(() => runner.ValidateVersionOrder());

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

            var runner = CreateRunner();

            LoadVersionData(version1, version3);

            _migrationList.Clear();
            _migrationList.Add(version1, new MigrationInfo(version1, TransactionBehavior.Default, mockMigration1.Object));
            _migrationList.Add(version2, new MigrationInfo(version2, TransactionBehavior.Default, mockMigration2.Object));
            _migrationList.Add(version3, new MigrationInfo(version3, TransactionBehavior.Default, mockMigration3.Object));
            _migrationList.Add(version4, new MigrationInfo(version4, TransactionBehavior.Default, true, mockMigration4.Object));

            runner.ListMigrations();

            _logMessages.ShouldContain(l => LineContainsAll(l, "2011010101: IMigrationProxy"));
            _logMessages.ShouldContain(l => LineContainsAll(l, "2011010102: IMigrationProxy (not applied)"));
            _logMessages.ShouldContain(l => LineContainsAll(l, "2011010103: IMigrationProxy (current)"));
            _logMessages.ShouldContain(l => LineContainsAll(l, "2011010104: IMigrationProxy (not applied, BREAKING)"));
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

            var runner = CreateRunner();
            Assert.Throws<InvalidMigrationException>(() => runner.Up(invalidMigration.Object));

            var expectedErrorMessage = ErrorMessages.UpdateDataExpressionMustSpecifyWhereClauseOrAllRows;
            _logMessages.ShouldContain(l => LineContainsAll(l, $"UpdateDataExpression: {expectedErrorMessage}"));
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

            var runner = CreateRunner();
            Assert.Throws<InvalidMigrationException>(() => runner.Down(invalidMigration.Object));

            var expectedErrorMessage = ErrorMessages.UpdateDataExpressionMustSpecifyWhereClauseOrAllRows;
            _logMessages.ShouldContain(l => LineContainsAll(l, $"UpdateDataExpression: {expectedErrorMessage}"));
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

            var runner = CreateRunner();
            Assert.Throws<InvalidMigrationException>(() => runner.Up(invalidMigration.Object));

            _logMessages.ShouldContain(l => LineContainsAll(l, $"UpdateDataExpression: {ErrorMessages.UpdateDataExpressionMustSpecifyWhereClauseOrAllRows}"));
            _logMessages.ShouldContain(l => LineContainsAll(l, $"CreateColumnExpression: {ErrorMessages.TableNameCannotBeNullOrEmpty}"));
        }

        /// <summary>
        /// Defines the test method CanLoadCustomMigrationConventions.
        /// </summary>
        [Test]
        public void CanLoadCustomMigrationConventions()
        {
            var runner = CreateRunner();
            Assert.That(runner.Conventions, Is.TypeOf<CustomMigrationConventions>());
        }

        /// <summary>
        /// Defines the test method CanLoadDefaultMigrationConventionsIfNoCustomConventionsAreSpecified.
        /// </summary>
        [Test]
        public void CanLoadDefaultMigrationConventionsIfNoCustomConventionsAreSpecified()
        {
            var processorMock = new Mock<IMigrationProcessor>(MockBehavior.Loose);
            var serviceProvider = ServiceCollectionExtensions.CreateServices(false)
                .WithProcessor(processorMock)
                .BuildServiceProvider();
            var runner = (MigrationRunner) serviceProvider.GetRequiredService<IMigrationRunner>();
            Assert.That(runner.Conventions, Is.TypeOf<DefaultMigrationRunnerConventions>());
        }

        /// <summary>
        /// Defines the test method CanBlockBreakingChangesByDefault.
        /// </summary>
        [Test]
        public void CanBlockBreakingChangesByDefault()
        {
            var runner = CreateRunner(sc => sc.Configure<RunnerOptions>(opt => opt.AllowBreakingChange = false));

            InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() =>
                runner.ApplyMigrationUp(
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
            _serviceCollection
                .Configure<RunnerOptions>(opt => opt.AllowBreakingChange = true);

            var runner = CreateRunner();

            Assert.DoesNotThrow(() =>
                runner.ApplyMigrationUp(
                    new MigrationInfo(7, TransactionBehavior.Default, true, new TestBreakingMigration()), true));
        }

        /// <summary>
        /// Defines the test method CanRunBreakingChangesInPreview.
        /// </summary>
        [Test]
        public void CanRunBreakingChangesInPreview()
        {
            _serviceCollection
                .Configure<RunnerOptions>(opt => opt.AllowBreakingChange = true)
                .Configure<ProcessorOptions>(opt => opt.PreviewOnly = true);

            var runner = CreateRunner();

            Assert.DoesNotThrow(() =>
                runner.ApplyMigrationUp(
                    new MigrationInfo(7, TransactionBehavior.Default, true, new TestBreakingMigration()), true));
        }

        /// <summary>
        /// Defines the test method CanLoadCustomMigrationScopeHandler.
        /// </summary>
        [Test]
        public void CanLoadCustomMigrationScopeHandler()
        {
            _serviceCollection.AddScoped<IMigrationScopeManager>(scoped => { return _migrationScopeHandlerMock.Object; });
            var runner = CreateRunner();
            runner.BeginScope();
            _migrationScopeHandlerMock.Verify(x => x.BeginScope(), Times.Once());
        }

        /// <summary>
        /// Defines the test method CanLoadDefaultMigrationScopeHandlerIfNoCustomHandlerIsSpecified.
        /// </summary>
        [Test]
        public void CanLoadDefaultMigrationScopeHandlerIfNoCustomHandlerIsSpecified()
        {
            var runner = CreateRunner();
            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.NonPublic;
            FieldInfo field = typeof(MigrationRunner).GetField("_migrationScopeManager", bindFlags);
            Assert.That(field.GetValue(runner), Is.TypeOf<MigrationScopeHandler>());
        }

        /// <summary>
        /// Defines the test method DoesRunMigrationsThatMeetConstraints.
        /// </summary>
        [Test]
        public void DoesRunMigrationsThatMeetConstraints()
        {
            _migrationList.Clear();
            _migrationList.Add(1, new MigrationInfo(1, TransactionBehavior.Default, new Step1Migration()));
            _migrationList.Add(2, new MigrationInfo(2, TransactionBehavior.Default, new Step2Migration()));
            _migrationList.Add(3, new MigrationInfo(3, TransactionBehavior.Default, new Step2Migration2()));
            var runner = CreateRunner();
            runner.MigrateUp();
            Assert.AreEqual(1, runner.VersionLoader.VersionInfo.Latest());
        }

        /// <summary>
        /// Defines the test method DoesRunMigrationsThatDoMeetConstraints.
        /// </summary>
        [Test]
        public void DoesRunMigrationsThatDoMeetConstraints()
        {
            _migrationList.Clear();
            _migrationList.Add(1, new MigrationInfo(1, TransactionBehavior.Default, new Step1Migration()));
            _migrationList.Add(2, new MigrationInfo(2, TransactionBehavior.Default, new Step2Migration()));
            _migrationList.Add(3, new MigrationInfo(3, TransactionBehavior.Default, new Step2Migration2()));
            var runner = CreateRunner();
            runner.MigrateUp();
            runner.MigrateUp(); // run migrations second time, this time satisfying constraints
            Assert.AreEqual(3, runner.VersionLoader.VersionInfo.Latest());
        }

        /// <summary>
        /// Defines the test method MultipleConstraintsEachNeedsToReturnTrue.
        /// </summary>
        [Test]
        public void MultipleConstraintsEachNeedsToReturnTrue()
        {
            _migrationList.Clear();
            _migrationList.Add(1, new MigrationInfo(1, TransactionBehavior.Default, new MultipleConstraintsMigration()));
            var runner = CreateRunner();
            runner.MigrateUp();
            Assert.AreEqual(0, runner.VersionLoader.VersionInfo.Latest());
        }

        /// <summary>
        /// Defines the test method DoesRunMigrationWithPositiveConstraint.
        /// </summary>
        [Test]
        public void DoesRunMigrationWithPositiveConstraint()
        {
            _migrationList.Clear();
            _migrationList.Add(1, new MigrationInfo(1, TransactionBehavior.Default, new ConstrainedMigrationSuccess()));
            var runner = CreateRunner();
            runner.MigrateUp();
            Assert.AreEqual(1, runner.VersionLoader.VersionInfo.Latest());
        }

        /// <summary>
        /// Lines the contains all.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="words">The words.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private static bool LineContainsAll(string line, params string[] words)
        {
            var pattern = string.Join(".*?", words.Select(Regex.Escape));
            return Regex.IsMatch(line, pattern);
        }

        /// <summary>
        /// Class CustomMigrationConventions.
        /// Implements the <see cref="FluentMigrator.Runner.MigrationRunnerConventions" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Runner.MigrationRunnerConventions" />
        public class CustomMigrationConventions : MigrationRunnerConventions
        {
        }
    }
}
