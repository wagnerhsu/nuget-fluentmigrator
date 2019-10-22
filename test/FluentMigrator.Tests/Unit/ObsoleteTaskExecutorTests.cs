// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="ObsoleteTaskExecutorTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Data;
using System.Linq.Expressions;
using FluentMigrator.Exceptions;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Tests.Integration;
using Moq;
using NUnit.Framework;

namespace FluentMigrator.Tests.Unit
{
    /// <summary>
    /// Defines test class ObsoleteTaskExecutorTests.
    /// Implements the <see cref="FluentMigrator.Tests.Integration.IntegrationTestBase" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Integration.IntegrationTestBase" />
    [TestFixture]
    [Obsolete]
    public class ObsoleteTaskExecutorTests : IntegrationTestBase
    {
        #region Setup/Teardown

        /// <summary>
        /// Sets up.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            _migrationRunner = new Mock<IMigrationRunner>();
        }

        #endregion

        /// <summary>
        /// The migration runner
        /// </summary>
        private Mock<IMigrationRunner> _migrationRunner;

        /// <summary>
        /// Verifies the specified function.
        /// </summary>
        /// <param name="func">The function.</param>
        /// <param name="task">The task.</param>
        /// <param name="version">The version.</param>
        /// <param name="steps">The steps.</param>
        private void Verify(Expression<Action<IMigrationRunner>> func, string task, long version, int steps)
        {
            _migrationRunner.Setup(func).Verifiable();

            var processor = new Mock<IMigrationProcessor>();
            const string profile = "Debug";
            var dataSet = new DataSet();
            dataSet.Tables.Add(new DataTable());
            processor.Setup(x => x.ReadTableData(null, It.IsAny<string>())).Returns(dataSet);

            _migrationRunner.SetupGet(x => x.Processor).Returns(processor.Object);

            var stepsStore = steps;
            var announcer = new Mock<IAnnouncer>();
            var stopWatch = new Mock<IStopWatch>();
            var runnerContext = new Mock<IRunnerContext>();
            runnerContext.SetupGet(x => x.Announcer).Returns(announcer.Object);
            runnerContext.SetupGet(x => x.StopWatch).Returns(stopWatch.Object);
            runnerContext.SetupGet(x => x.Database).Returns("sqlserver2008");
            runnerContext.SetupGet(x => x.Connection).Returns(IntegrationTestOptions.SqlServer2008.ConnectionString);
            runnerContext.SetupGet(x => x.Task).Returns(task);
            runnerContext.SetupGet(x => x.Version).Returns(version);
            runnerContext.SetupGet(x => x.Steps).Returns(() => stepsStore);
            runnerContext.SetupSet(x => x.Steps = It.IsAny<int>()).Callback<int>(v => stepsStore = v);
            runnerContext.SetupGet(x => x.Targets).Returns(new[] { GetType().Assembly.Location });
            runnerContext.SetupGet(x => x.Profile).Returns(profile);
            runnerContext.SetupGet(x => x.Namespace).Returns("FluentMigrator.Tests.Integration.Migrations.Interleaved.Pass3");

            var taskExecutor = new FakeTaskExecutor(runnerContext.Object, _migrationRunner.Object);
            taskExecutor.Execute();

            _migrationRunner.VerifyAll();
        }

        /// <summary>
        /// Defines the test method InvalidProviderNameShouldThrowArgumentException.
        /// </summary>
        [Test]
        public void InvalidProviderNameShouldThrowArgumentException()
        {
            var runnerContext = new Mock<IRunnerContext>();
            runnerContext.SetupGet(x => x.Database).Returns("sqlWRONG");
            runnerContext.SetupGet(x => x.Connection).Returns(IntegrationTestOptions.SqlServer2008.ConnectionString);
            runnerContext.SetupGet(x => x.Targets).Returns(new[] { GetType().Assembly.Location });
            runnerContext.SetupGet(x => x.Announcer).Returns(new Mock<IAnnouncer>().Object);

            Assert.Throws<ProcessorFactoryNotFoundException>(() => new TaskExecutor(runnerContext.Object).Execute());
        }

        /// <summary>
        /// Defines the test method ShouldCallMigrateDownIfSpecified.
        /// </summary>
        [Test]
        public void ShouldCallMigrateDownIfSpecified()
        {
            Verify(x => x.MigrateDown(It.Is<long>(c => c == 20)), "migrate:down", 20, 0);
        }

        /// <summary>
        /// Defines the test method ShouldCallMigrateUpByDefault.
        /// </summary>
        [Test]
        public void ShouldCallMigrateUpByDefault()
        {
            Verify(x => x.MigrateUp(), null, 0, 0);
            Verify(x => x.MigrateUp(), "", 0, 0);
        }

        /// <summary>
        /// Defines the test method ShouldCallMigrateUpIfSpecified.
        /// </summary>
        [Test]
        public void ShouldCallMigrateUpIfSpecified()
        {
            Verify(x => x.MigrateUp(), "migrate", 0, 0);
            Verify(x => x.MigrateUp(), "migrate:up", 0, 0);
        }

        /// <summary>
        /// Defines the test method ShouldCallMigrateUpWithVersionIfSpecified.
        /// </summary>
        [Test]
        public void ShouldCallMigrateUpWithVersionIfSpecified()
        {
            Verify(x => x.MigrateUp(It.Is<long>(c => c == 1)), "migrate", 1, 0);
            Verify(x => x.MigrateUp(It.Is<long>(c => c == 1)), "migrate:up", 1, 0);
        }

        /// <summary>
        /// Defines the test method ShouldCallRollbackIfSpecified.
        /// </summary>
        [Test]
        public void ShouldCallRollbackIfSpecified()
        {
            Verify(x => x.Rollback(It.Is<int>(c => c == 2)), "rollback", 0, 2);
        }

        /// <summary>
        /// Defines the test method ShouldCallRollbackIfSpecifiedAndDefaultTo1Step.
        /// </summary>
        [Test]
        public void ShouldCallRollbackIfSpecifiedAndDefaultTo1Step()
        {
            Verify(x => x.Rollback(It.Is<int>(c => c == 1)), "rollback", 0, 0);
        }

        /// <summary>
        /// Defines the test method ShouldCallValidateVersionOrder.
        /// </summary>
        [Test]
        public void ShouldCallValidateVersionOrder()
        {
            Verify(x => x.ValidateVersionOrder(), "validateversionorder", 0, 0);
        }

        /// <summary>
        /// Defines the test method ShouldCallHasMigrationsToApplyUpWithNullVersionOnNoTask.
        /// </summary>
        [Test]
        public void ShouldCallHasMigrationsToApplyUpWithNullVersionOnNoTask()
        {
            VerifyHasMigrationsToApply(x => x.HasMigrationsToApplyUp(It.Is<long?>(version => !version.HasValue)), "", 0);
        }

        /// <summary>
        /// Defines the test method ShouldCallHasMigrationsToApplyUpWithVersionOnNoTask.
        /// </summary>
        [Test]
        public void ShouldCallHasMigrationsToApplyUpWithVersionOnNoTask()
        {
            VerifyHasMigrationsToApply(x => x.HasMigrationsToApplyUp(It.Is<long?>(version => version.GetValueOrDefault() == 1)), "", 1);
        }

        /// <summary>
        /// Defines the test method ShouldCallHasMigrationsToApplyUpWithNullVersionOnMigrate.
        /// </summary>
        [Test]
        public void ShouldCallHasMigrationsToApplyUpWithNullVersionOnMigrate()
        {
            VerifyHasMigrationsToApply(x => x.HasMigrationsToApplyUp(It.Is<long?>(version => !version.HasValue)), "migrate", 0);
        }

        /// <summary>
        /// Defines the test method ShouldCallHasMigrationsToApplyUpWithVersionOnMigrate.
        /// </summary>
        [Test]
        public void ShouldCallHasMigrationsToApplyUpWithVersionOnMigrate()
        {
            VerifyHasMigrationsToApply(x => x.HasMigrationsToApplyUp(It.Is<long?>(version => version.GetValueOrDefault() == 1)), "migrate", 1);
        }

        /// <summary>
        /// Defines the test method ShouldCallHasMigrationsToApplyUpWithNullVersionOnMigrateUp.
        /// </summary>
        [Test]
        public void ShouldCallHasMigrationsToApplyUpWithNullVersionOnMigrateUp()
        {
            VerifyHasMigrationsToApply(x => x.HasMigrationsToApplyUp(It.Is<long?>(version => !version.HasValue)), "migrate:up", 0);
        }

        /// <summary>
        /// Defines the test method ShouldCallHasMigrationsToApplyUpWithVersionOnMigrateUp.
        /// </summary>
        [Test]
        public void ShouldCallHasMigrationsToApplyUpWithVersionOnMigrateUp()
        {
            VerifyHasMigrationsToApply(x => x.HasMigrationsToApplyUp(It.Is<long?>(version => version.GetValueOrDefault() == 1)), "migrate:up", 1);
        }

        /// <summary>
        /// Defines the test method ShouldCallHasMigrationsToApplyRollbackOnRollback.
        /// </summary>
        [Test]
        public void ShouldCallHasMigrationsToApplyRollbackOnRollback()
        {
            VerifyHasMigrationsToApply(x => x.HasMigrationsToApplyRollback(), "rollback", 0);
        }

        /// <summary>
        /// Defines the test method ShouldCallHasMigrationsToApplyRollbackOnRollbackAll.
        /// </summary>
        [Test]
        public void ShouldCallHasMigrationsToApplyRollbackOnRollbackAll()
        {
            VerifyHasMigrationsToApply(x => x.HasMigrationsToApplyRollback(), "rollback:all", 0);
        }

        /// <summary>
        /// Defines the test method ShouldCallHasMigrationsToApplyDownOnRollbackToVersion.
        /// </summary>
        [Test]
        public void ShouldCallHasMigrationsToApplyDownOnRollbackToVersion()
        {
            VerifyHasMigrationsToApply(x => x.HasMigrationsToApplyDown(It.Is<long>(version => version == 2)), "rollback:toversion", 2);
        }

        /// <summary>
        /// Defines the test method ShouldCallHasMigrationsToApplyDownOnMigrateDown.
        /// </summary>
        [Test]
        public void ShouldCallHasMigrationsToApplyDownOnMigrateDown()
        {
            VerifyHasMigrationsToApply(x => x.HasMigrationsToApplyDown(It.Is<long>(version => version == 2)), "migrate:down", 2);
        }

        /// <summary>
        /// Verifies the has migrations to apply.
        /// </summary>
        /// <param name="func">The function.</param>
        /// <param name="task">The task.</param>
        /// <param name="version">The version.</param>
        private void VerifyHasMigrationsToApply(Expression<Func<IMigrationRunner, bool>> func, string task, long version)
        {
            _migrationRunner.Setup(func).Verifiable();

            var processor = new Mock<IMigrationProcessor>();
            var dataSet = new DataSet();
            dataSet.Tables.Add(new DataTable());
            processor.Setup(x => x.ReadTableData(null, It.IsAny<string>())).Returns(dataSet);
            _migrationRunner.SetupGet(x => x.Processor).Returns(processor.Object);

            var runnerContext = new Mock<IRunnerContext>();
            runnerContext.SetupGet(x => x.Task).Returns(task);
            runnerContext.SetupGet(rc => rc.Version).Returns(version);
            var taskExecutor = new FakeTaskExecutor(runnerContext.Object, _migrationRunner.Object);
            taskExecutor.HasMigrationsToApply();
            _migrationRunner.Verify(func, Times.Once());
        }

        /// <summary>
        /// Class FakeTaskExecutor.
        /// Implements the <see cref="FluentMigrator.Runner.Initialization.TaskExecutor" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Runner.Initialization.TaskExecutor" />
        internal class FakeTaskExecutor : TaskExecutor
        {
            /// <summary>
            /// The runner
            /// </summary>
            private readonly IMigrationRunner _runner;

            /// <summary>
            /// Initializes a new instance of the <see cref="FakeTaskExecutor"/> class.
            /// </summary>
            /// <param name="runnerContext">The runner context.</param>
            /// <param name="runner">The runner.</param>
            public FakeTaskExecutor(IRunnerContext runnerContext, IMigrationRunner runner) : base(runnerContext)
            {
                _runner = runner;
            }

            /// <summary>
            /// Initializes this instance.
            /// </summary>
            protected override void Initialize()
            {
                Runner = _runner;
            }
        }
    }
}
