// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="MaintenanceLoaderTests.cs" company="FluentMigrator Project">
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

using FluentMigrator.Infrastructure.Extensions;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Infrastructure;
using FluentMigrator.Runner.Initialization;

using Microsoft.Extensions.DependencyInjection;

using Moq;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Runners
{
    /// <summary>
    /// Defines test class MaintenanceLoaderTests.
    /// </summary>
    [TestFixture]
    public class MaintenanceLoaderTests
    {
        /// <summary>
        /// The tag1
        /// </summary>
        public const string Tag1 = "MaintenanceTestTag1";
        /// <summary>
        /// The tag2
        /// </summary>
        public const string Tag2 = "MaintenanceTestTag2";
        /// <summary>
        /// The tags
        /// </summary>
        private readonly string[] _tags = {Tag1, Tag2};

        /// <summary>
        /// The migration conventions
        /// </summary>
        private Mock<IMigrationRunnerConventions> _migrationConventions;
        /// <summary>
        /// The maintenance loader
        /// </summary>
        private IMaintenanceLoader _maintenanceLoader;
        /// <summary>
        /// The maintenance loader no tags
        /// </summary>
        private IMaintenanceLoader _maintenanceLoaderNoTags;

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _migrationConventions = new Mock<IMigrationRunnerConventions>();
            _migrationConventions.Setup(x => x.GetMaintenanceStage).Returns(DefaultMigrationRunnerConventions.Instance.GetMaintenanceStage);
            _migrationConventions.Setup(x => x.TypeIsMigration).Returns(DefaultMigrationRunnerConventions.Instance.TypeIsMigration);
            _migrationConventions.Setup(x => x.TypeHasTags).Returns(DefaultMigrationRunnerConventions.Instance.TypeHasTags);
            _migrationConventions.Setup(x => x.TypeHasMatchingTags).Returns(DefaultMigrationRunnerConventions.Instance.TypeHasMatchingTags);

            _maintenanceLoader = ServiceCollectionExtensions.CreateServices()
                .Configure<RunnerOptions>(opt => opt.Tags = _tags)
                .AddSingleton<IMigrationRunnerConventionsAccessor>(new PassThroughMigrationRunnerConventionsAccessor(_migrationConventions.Object))
                .BuildServiceProvider()
                .GetRequiredService<IMaintenanceLoader>();

            _maintenanceLoaderNoTags = ServiceCollectionExtensions.CreateServices()
                .AddSingleton<IMigrationRunnerConventionsAccessor>(new PassThroughMigrationRunnerConventionsAccessor(_migrationConventions.Object))
                .BuildServiceProvider()
                .GetRequiredService<IMaintenanceLoader>();
        }

        /// <summary>
        /// Defines the test method LoadsMigrationsForCorrectStage.
        /// </summary>
        [Test]
        public void LoadsMigrationsForCorrectStage()
        {
            var migrationInfos = _maintenanceLoader.LoadMaintenance(MigrationStage.BeforeEach);
            _migrationConventions.Verify(x => x.GetMaintenanceStage, Times.AtLeastOnce());
            Assert.IsNotEmpty(migrationInfos);

            foreach (var migrationInfo in migrationInfos)
            {
                migrationInfo.Migration.ShouldNotBeNull();

                // The NoTag maintenance should not be found in the tagged maintenanceLoader because it wants tagged classes
                Assert.AreNotSame(typeof(MaintenanceBeforeEachNoTag), migrationInfo.Migration.GetType());

                var maintenanceAttribute = migrationInfo.Migration.GetType().GetOneAttribute<MaintenanceAttribute>();
                maintenanceAttribute.ShouldNotBeNull();
                maintenanceAttribute.Stage.ShouldBe(MigrationStage.BeforeEach);
            }
        }

        /// <summary>
        /// Defines the test method LoadsMigrationsFilteredByTag.
        /// </summary>
        [Test]
        public void LoadsMigrationsFilteredByTag()
        {
            var migrationInfos = _maintenanceLoader.LoadMaintenance(MigrationStage.BeforeEach);
            _migrationConventions.Verify(x => x.TypeHasMatchingTags, Times.AtLeastOnce());
            Assert.IsNotEmpty(migrationInfos);

            foreach (var migrationInfo in migrationInfos)
            {
                migrationInfo.Migration.ShouldNotBeNull();

                // The NoTag maintenance should not be found in the tagged maintenanceLoader because it wants tagged classes
                Assert.AreNotSame(typeof(MaintenanceBeforeEachNoTag), migrationInfo.Migration.GetType());

                DefaultMigrationRunnerConventions.Instance.TypeHasMatchingTags(migrationInfo.Migration.GetType(), _tags)
                    .ShouldBeTrue();
            }
        }

        /// <summary>
        /// Defines the test method MigrationInfoIsAttributedIsFalse.
        /// </summary>
        [Test]
        public void MigrationInfoIsAttributedIsFalse()
        {
            var migrationInfos = _maintenanceLoader.LoadMaintenance(MigrationStage.BeforeEach);
            Assert.IsNotEmpty(migrationInfos);

            foreach (var migrationInfo in migrationInfos)
            {
                migrationInfo.IsAttributed().ShouldBeFalse();
            }
        }

        /// <summary>
        /// Defines the test method SetsTransactionBehaviorToSameAsMaintenanceAttribute.
        /// </summary>
        [Test]
        public void SetsTransactionBehaviorToSameAsMaintenanceAttribute()
        {
            var migrationInfos = _maintenanceLoader.LoadMaintenance(MigrationStage.BeforeEach);
            Assert.IsNotEmpty(migrationInfos);

            foreach (var migrationInfo in migrationInfos)
            {
                migrationInfo.Migration.ShouldNotBeNull();

                var maintenanceAttribute = migrationInfo.Migration.GetType().GetOneAttribute<MaintenanceAttribute>();
                maintenanceAttribute.ShouldNotBeNull();
                migrationInfo.TransactionBehavior.ShouldBe(maintenanceAttribute.TransactionBehavior);
            }
        }

        /// <summary>
        /// Defines the test method LoadsMigrationsNoTag.
        /// </summary>
        [Test]
        public void LoadsMigrationsNoTag()
        {
            var migrationInfos = _maintenanceLoaderNoTags.LoadMaintenance(MigrationStage.BeforeEach);
            _migrationConventions.Verify(x => x.TypeHasMatchingTags, Times.AtLeastOnce());
            Assert.IsNotEmpty(migrationInfos);

            bool foundNoTag = false;
            foreach (var migrationInfo in migrationInfos)
            {
                migrationInfo.Migration.ShouldNotBeNull();

                // Both notag maintenance and tagged maintenance should be found in the notag maintenanceLoader because he doesn't care about tags
                if (migrationInfo.Migration.GetType() == typeof(MaintenanceBeforeEachNoTag))
                {
                    foundNoTag = true;
                }
                else
                {
                    DefaultMigrationRunnerConventions.Instance.TypeHasMatchingTags(migrationInfo.Migration.GetType(), _tags)
                        .ShouldBeTrue();
                }
            }

            Assert.IsTrue(foundNoTag);
        }
    }

    /// <summary>
    /// Class MaintenanceBeforeEach.
    /// Implements the <see cref="FluentMigrator.Migration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Migration" />
    [Tags(MaintenanceLoaderTests.Tag1, MaintenanceLoaderTests.Tag2)]
    [Maintenance(MigrationStage.BeforeEach)]
    public class MaintenanceBeforeEach : Migration
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
    /// Class MaintenanceBeforeEachWithNonTransactionBehavior.
    /// Implements the <see cref="FluentMigrator.Migration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Migration" />
    [Tags(MaintenanceLoaderTests.Tag1)]
    [Tags(MaintenanceLoaderTests.Tag2)]
    [Maintenance(MigrationStage.BeforeEach, TransactionBehavior.None)]
    public class MaintenanceBeforeEachWithNonTransactionBehavior : Migration
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
    /// Class MaintenanceBeforeEachWithoutTestTag.
    /// Implements the <see cref="FluentMigrator.Migration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Migration" />
    [Tags("NonSpecifiedMaintenanceTestTag1")]
    [Maintenance(MigrationStage.BeforeEach)]
    public class MaintenanceBeforeEachWithoutTestTag : Migration
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
    /// Class MaintenanceAfterAllWithNoneTransactionBehavior.
    /// Implements the <see cref="FluentMigrator.Migration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Migration" />
    [Tags(MaintenanceLoaderTests.Tag1)]
    [Maintenance(MigrationStage.AfterAll, TransactionBehavior.None)]
    public class MaintenanceAfterAllWithNoneTransactionBehavior : Migration
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
    /// Class MaintenanceBeforeEachNoTag.
    /// Implements the <see cref="FluentMigrator.Migration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Migration" />
    [Maintenance(MigrationStage.BeforeEach)]
    public class MaintenanceBeforeEachNoTag : Migration
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
