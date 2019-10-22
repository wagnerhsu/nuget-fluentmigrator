// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="DefaultMigrationInformationLoaderTests.cs" company="FluentMigrator Project">
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
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

using FluentMigrator.Exceptions;
using FluentMigrator.Infrastructure;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Exceptions;
using FluentMigrator.Runner.Infrastructure;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Tests.Integration.Migrations;
using FluentMigrator.Tests.Unit.TaggingTestFakes;

using Microsoft.Extensions.DependencyInjection;

using Moq;

using NUnit.Framework;

using Shouldly;


namespace FluentMigrator.Tests.Unit
{
    /// <summary>
    /// Defines test class DefaultMigrationInformationLoaderTests.
    /// </summary>
    [TestFixture]
    public class DefaultMigrationInformationLoaderTests
    {
        /// <summary>
        /// Defines the test method CanFindMigrationsInAssembly.
        /// </summary>
        [Test]
        public void CanFindMigrationsInAssembly()
        {
            var loader = ServiceCollectionExtensions.CreateServices()
                .WithMigrationsIn("FluentMigrator.Tests.Integration.Migrations.Interleaved.Pass1")
                .BuildServiceProvider()
                .GetRequiredService<IMigrationInformationLoader>();

            var migrationList = loader.LoadMigrations();

            var count = migrationList.Count;

            count.ShouldBeGreaterThan(0);
        }

        /// <summary>
        /// Defines the test method CanFindMigrationsInNamespace.
        /// </summary>
        [Test]
        public void CanFindMigrationsInNamespace()
        {
            var loader = ServiceCollectionExtensions.CreateServices()
                .WithMigrationsIn("FluentMigrator.Tests.Integration.Migrations.Interleaved.Pass1")
                .BuildServiceProvider()
                .GetRequiredService<IMigrationInformationLoader>();

            var migrationList = loader.LoadMigrations();
            migrationList.Select(x => x.Value.Migration.GetType()).ShouldNotContain(typeof(VersionedMigration));
            migrationList.Count().ShouldBeGreaterThan(0);
        }

        /// <summary>
        /// Defines the test method DefaultBehaviorIsToNotLoadNestedNamespaces.
        /// </summary>
        [Test]
        public void DefaultBehaviorIsToNotLoadNestedNamespaces()
        {
            var loader = ServiceCollectionExtensions.CreateServices()
                .WithMigrationsIn("FluentMigrator.Tests.Integration.Migrations.Interleaved.Pass1")
                .BuildServiceProvider()
                .GetRequiredService<IMigrationInformationLoader>();

            Assert.IsInstanceOf<DefaultMigrationInformationLoader>(loader);

            var defaultLoader = (DefaultMigrationInformationLoader)loader;

            defaultLoader.LoadNestedNamespaces.ShouldBe(false);
        }

        /// <summary>
        /// Defines the test method FindsMigrationsInNestedNamespaceWhenLoadNestedNamespacesEnabled.
        /// </summary>
        [Test]
        public void FindsMigrationsInNestedNamespaceWhenLoadNestedNamespacesEnabled()
        {
            var loader = ServiceCollectionExtensions.CreateServices()
                .WithMigrationsIn("FluentMigrator.Tests.Integration.Migrations.Nested", true)
                .BuildServiceProvider()
                .GetRequiredService<IMigrationInformationLoader>();

            List<Type> expected = new List<Type>
            {
                typeof(Integration.Migrations.Nested.NotGrouped),
                typeof(Integration.Migrations.Nested.Group1.FromGroup1),
                typeof(Integration.Migrations.Nested.Group1.AnotherFromGroup1),
                typeof(Integration.Migrations.Nested.Group2.FromGroup2),
            };

            var migrationList = loader.LoadMigrations();
            List<Type> actual = migrationList.Select(m => m.Value.Migration.GetType()).ToList();

            CollectionAssert.AreEquivalent(expected, actual);
        }

        /// <summary>
        /// Defines the test method DoesNotFindsMigrationsInNestedNamespaceWhenLoadNestedNamespacesDisabled.
        /// </summary>
        [Test]
        public void DoesNotFindsMigrationsInNestedNamespaceWhenLoadNestedNamespacesDisabled()
        {
            var loader = ServiceCollectionExtensions.CreateServices()
                .WithMigrationsIn("FluentMigrator.Tests.Integration.Migrations.Nested")
                .BuildServiceProvider()
                .GetRequiredService<IMigrationInformationLoader>();

            List<Type> expected = new List<Type>
                                      {
                typeof(Integration.Migrations.Nested.NotGrouped),
            };

            var migrationList = loader.LoadMigrations();
            List<Type> actual = migrationList.Select(m => m.Value.Migration.GetType()).ToList();

            CollectionAssert.AreEquivalent(expected, actual);
        }

        /// <summary>
        /// Defines the test method DoesFindMigrationsThatHaveMatchingTags.
        /// </summary>
        [Test]
        public void DoesFindMigrationsThatHaveMatchingTags()
        {
            var migrationType = typeof(TaggedMigration);
            var tagsToMatch = new[] { "UK", "Production" };

            var conventionsMock = new Mock<IMigrationRunnerConventions>();
            conventionsMock.SetupGet(m => m.GetMigrationInfoForMigration).Returns(DefaultMigrationRunnerConventions.Instance.GetMigrationInfoForMigration);
            conventionsMock.SetupGet(m => m.TypeIsMigration).Returns(t => true);
            conventionsMock.SetupGet(m => m.TypeHasTags).Returns(t => migrationType == t);
            conventionsMock.SetupGet(m => m.TypeHasMatchingTags).Returns((type, tags) => (migrationType == type && tagsToMatch == tags));

            var loader = ServiceCollectionExtensions.CreateServices()
                .WithMigrationsIn(migrationType.Namespace)
                .Configure<RunnerOptions>(opt => opt.Tags = tagsToMatch)
                .ConfigureRunner(builder => builder.WithRunnerConventions(conventionsMock.Object))
                .BuildServiceProvider()
                .GetRequiredService<IMigrationInformationLoader>();

            var expected = new List<Type> { typeof(UntaggedMigration), migrationType };

            var actual = loader.LoadMigrations().Select(m => m.Value.Migration.GetType()).ToList();

            CollectionAssert.AreEquivalent(expected, actual);
        }

        /// <summary>
        /// Defines the test method DoesNotFindMigrationsThatDoNotHaveMatchingTags.
        /// </summary>
        [Test]
        public void DoesNotFindMigrationsThatDoNotHaveMatchingTags()
        {
            var migrationType = typeof(TaggedMigration);
            var tagsToMatch = new[] { "UK", "Production" };

            var conventionsMock = new Mock<IMigrationRunnerConventions>();
            conventionsMock.SetupGet(m => m.GetMigrationInfoForMigration).Returns(DefaultMigrationRunnerConventions.Instance.GetMigrationInfoForMigration);
            conventionsMock.SetupGet(m => m.TypeIsMigration).Returns(t => true);
            conventionsMock.SetupGet(m => m.TypeHasTags).Returns(t => migrationType == t);
            conventionsMock.SetupGet(m => m.TypeHasMatchingTags).Returns((type, tags) => false);

            var loader = ServiceCollectionExtensions.CreateServices()
                .WithMigrationsIn(migrationType.Namespace)
                .Configure<RunnerOptions>(opt => opt.Tags = tagsToMatch)
                .ConfigureRunner(builder => builder.WithRunnerConventions(conventionsMock.Object))
                .BuildServiceProvider()
                .GetRequiredService<IMigrationInformationLoader>();

            var expected = new List<Type> { typeof(UntaggedMigration) };

            var actual = loader.LoadMigrations().Select(m => m.Value.Migration.GetType()).ToList();

            CollectionAssert.AreEquivalent(expected, actual);
        }

        /// <summary>
        /// Defines the test method HandlesNotFindingMigrations.
        /// </summary>
        [Test]
        public void HandlesNotFindingMigrations()
        {
            var loader = ServiceCollectionExtensions.CreateServices()
                .WithMigrationsIn("FluentMigrator.Tests.Unit.EmptyNamespace")
                .BuildServiceProvider()
                .GetRequiredService<IMigrationInformationLoader>();
            Assert.Throws<MissingMigrationsException>(() => loader.LoadMigrations());
        }

        /// <summary>
        /// Defines the test method ShouldThrowExceptionIfDuplicateVersionNumbersAreLoaded.
        /// </summary>
        [Test]
        public void ShouldThrowExceptionIfDuplicateVersionNumbersAreLoaded()
        {
            var loader = ServiceCollectionExtensions.CreateServices()
                .WithMigrationsIn("FluentMigrator.Tests.Unit.DuplicateVersionNumbers")
                .BuildServiceProvider()
                .GetRequiredService<IMigrationInformationLoader>();
            Assert.Throws<DuplicateMigrationException>(() => loader.LoadMigrations());
        }

        /// <summary>
        /// Defines the test method HandlesMigrationThatDoesNotInheritFromMigrationBaseClass.
        /// </summary>
        [Test]
        public void HandlesMigrationThatDoesNotInheritFromMigrationBaseClass()
        {
            var loader = ServiceCollectionExtensions.CreateServices()
                .WithMigrationsIn("FluentMigrator.Tests.Unit.DoesNotInheritFromBaseClass")
                .BuildServiceProvider()
                .GetRequiredService<IMigrationInformationLoader>();
            Assert.That(loader.LoadMigrations().Count(), Is.EqualTo(1));
        }

        /// <summary>
        /// Defines the test method ShouldHandleTransactionlessMigrations.
        /// </summary>
        [Test]
        public void ShouldHandleTransactionlessMigrations()
        {
            var loader = ServiceCollectionExtensions.CreateServices()
                .WithMigrationsIn("FluentMigrator.Tests.Unit.DoesHandleTransactionLessMigrations")
                .BuildServiceProvider()
                .GetRequiredService<IMigrationInformationLoader>();

            var list = loader.LoadMigrations().ToList();

            list.Count().ShouldBe(2);

            list[0].Value.Migration.GetType().ShouldBe(typeof(DoesHandleTransactionLessMigrations.MigrationThatIsTransactionLess));
            list[0].Value.TransactionBehavior.ShouldBe(TransactionBehavior.None);
            list[0].Value.Version.ShouldBe(1);

            list[1].Value.Migration.GetType().ShouldBe(typeof(DoesHandleTransactionLessMigrations.MigrationThatIsNotTransactionLess));
            list[1].Value.TransactionBehavior.ShouldBe(TransactionBehavior.Default);
            list[1].Value.Version.ShouldBe(2);
        }

        /// <summary>
        /// Defines the test method ObsoleteCanFindMigrationsInAssembly.
        /// </summary>
        [Test]
        [Obsolete]
        public void ObsoleteCanFindMigrationsInAssembly()
        {
            var conventions = new MigrationRunnerConventions();
            var asm = Assembly.GetExecutingAssembly();
            var loader = new DefaultMigrationInformationLoader(conventions, asm, "FluentMigrator.Tests.Integration.Migrations.Interleaved.Pass1", null);

            SortedList<long, IMigrationInfo> migrationList = loader.LoadMigrations();

            //if this works, there will be at least one migration class because i've included on in this code file
            int count = migrationList.Count();
            count.ShouldBeGreaterThan(0);
        }

        /// <summary>
        /// Defines the test method ObsoleteCanFindMigrationsInNamespace.
        /// </summary>
        [Test]
        [Obsolete]
        public void ObsoleteCanFindMigrationsInNamespace()
        {
            var conventions = new MigrationRunnerConventions();
            var asm = Assembly.GetExecutingAssembly();
            var loader = new DefaultMigrationInformationLoader(conventions, asm, "FluentMigrator.Tests.Integration.Migrations.Interleaved.Pass1", null);

            var migrationList = loader.LoadMigrations();
            migrationList.Select(x => x.Value.Migration.GetType()).ShouldNotContain(typeof(VersionedMigration));
            migrationList.Count().ShouldBeGreaterThan(0);
        }

        /// <summary>
        /// Defines the test method ObsoleteDefaultBehaviorIsToNotLoadNestedNamespaces.
        /// </summary>
        [Test]
        [Obsolete]
        public void ObsoleteDefaultBehaviorIsToNotLoadNestedNamespaces()
        {
            var conventions = new MigrationRunnerConventions();
            var asm = Assembly.GetExecutingAssembly();
            var loader = new DefaultMigrationInformationLoader(conventions, asm, "FluentMigrator.Tests.Integration.Migrations.Nested", null);

            loader.LoadNestedNamespaces.ShouldBe(false);
        }

        /// <summary>
        /// Defines the test method ObsoleteFindsMigrationsInNestedNamespaceWhenLoadNestedNamespacesEnabled.
        /// </summary>
        [Test]
        [Obsolete]
        public void ObsoleteFindsMigrationsInNestedNamespaceWhenLoadNestedNamespacesEnabled()
        {
            var conventions = new MigrationRunnerConventions();
            var asm = Assembly.GetExecutingAssembly();
            var loader = new DefaultMigrationInformationLoader(conventions, asm, "FluentMigrator.Tests.Integration.Migrations.Nested", true, null);

            List<Type> expected = new List<Type>
                                      {
                typeof(Integration.Migrations.Nested.NotGrouped),
                typeof(Integration.Migrations.Nested.Group1.FromGroup1),
                typeof(Integration.Migrations.Nested.Group1.AnotherFromGroup1),
                typeof(Integration.Migrations.Nested.Group2.FromGroup2),
            };

            var migrationList = loader.LoadMigrations();
            List<Type> actual = migrationList.Select(m => m.Value.Migration.GetType()).ToList();

            CollectionAssert.AreEquivalent(expected, actual);
        }

        /// <summary>
        /// Defines the test method ObsoleteDoesNotFindsMigrationsInNestedNamespaceWhenLoadNestedNamespacesDisabled.
        /// </summary>
        [Test]
        [Obsolete]
        public void ObsoleteDoesNotFindsMigrationsInNestedNamespaceWhenLoadNestedNamespacesDisabled()
        {
            var conventions = new MigrationRunnerConventions();
            var asm = Assembly.GetExecutingAssembly();
            var loader = new DefaultMigrationInformationLoader(conventions, asm, "FluentMigrator.Tests.Integration.Migrations.Nested", false, null);

            List<Type> expected = new List<Type>
                                      {
                typeof(Integration.Migrations.Nested.NotGrouped),
            };

            var migrationList = loader.LoadMigrations();
            List<Type> actual = migrationList.Select(m => m.Value.Migration.GetType()).ToList();

            CollectionAssert.AreEquivalent(expected, actual);
        }

        /// <summary>
        /// Defines the test method ObsoleteDoesFindMigrationsThatHaveMatchingTags.
        /// </summary>
        [Test]
        [Obsolete]
        public void ObsoleteDoesFindMigrationsThatHaveMatchingTags()
        {
            var asm = Assembly.GetExecutingAssembly();
            var migrationType = typeof(TaggedMigration);
            var tagsToMatch = new[] { "UK", "Production" };

            var conventionsMock = new Mock<IMigrationRunnerConventions>();
            conventionsMock.SetupGet(m => m.GetMigrationInfoForMigration).Returns(DefaultMigrationRunnerConventions.Instance.GetMigrationInfoForMigration);
            conventionsMock.SetupGet(m => m.TypeIsMigration).Returns(t => true);
            conventionsMock.SetupGet(m => m.TypeHasTags).Returns(t => migrationType == t);
            conventionsMock.SetupGet(m => m.TypeHasMatchingTags).Returns((type, tags) => (migrationType == type && tagsToMatch == tags));

            var loader = new DefaultMigrationInformationLoader(conventionsMock.Object, asm, migrationType.Namespace, tagsToMatch);

            var expected = new List<Type> { typeof(UntaggedMigration), migrationType };

            var actual = loader.LoadMigrations().Select(m => m.Value.Migration.GetType()).ToList();

            CollectionAssert.AreEquivalent(expected, actual);
        }

        /// <summary>
        /// Defines the test method ObsoleteDoesNotFindMigrationsThatDoNotHaveMatchingTags.
        /// </summary>
        [Test]
        [Obsolete]
        public void ObsoleteDoesNotFindMigrationsThatDoNotHaveMatchingTags()
        {
            var asm = Assembly.GetExecutingAssembly();
            var migrationType = typeof(TaggedMigration);
            var tagsToMatch = new[] { "UK", "Production" };

            var conventionsMock = new Mock<IMigrationRunnerConventions>();
            conventionsMock.SetupGet(m => m.GetMigrationInfoForMigration).Returns(DefaultMigrationRunnerConventions.Instance.GetMigrationInfoForMigration);
            conventionsMock.SetupGet(m => m.TypeIsMigration).Returns(t => true);
            conventionsMock.SetupGet(m => m.TypeHasTags).Returns(t => migrationType == t);
            conventionsMock.SetupGet(m => m.TypeHasMatchingTags).Returns((type, tags) => false);

            var loader = new DefaultMigrationInformationLoader(conventionsMock.Object, asm, migrationType.Namespace, tagsToMatch);

            var expected = new List<Type> { typeof(UntaggedMigration) };

            var actual = loader.LoadMigrations().Select(m => m.Value.Migration.GetType()).ToList();

            CollectionAssert.AreEquivalent(expected, actual);
        }

        /// <summary>
        /// Defines the test method ObsoleteHandlesNotFindingMigrations.
        /// </summary>
        [Test]
        [Obsolete]
        public void ObsoleteHandlesNotFindingMigrations()
        {
            var conventions = new MigrationRunnerConventions();
            var asm = Assembly.GetExecutingAssembly();
            var loader = new DefaultMigrationInformationLoader(conventions, asm, "FluentMigrator.Tests.Unit.EmptyNamespace", null);
            Assert.Throws<MissingMigrationsException>(() => loader.LoadMigrations());
        }

        /// <summary>
        /// Defines the test method ObsoleteShouldThrowExceptionIfDuplicateVersionNumbersAreLoaded.
        /// </summary>
        [Test]
        [Obsolete]
        public void ObsoleteShouldThrowExceptionIfDuplicateVersionNumbersAreLoaded()
        {
            var conventions = new MigrationRunnerConventions();
            var asm = Assembly.GetExecutingAssembly();
            var migrationLoader = new DefaultMigrationInformationLoader(conventions, asm, "FluentMigrator.Tests.Unit.DuplicateVersionNumbers", null);
            Assert.Throws<DuplicateMigrationException>(() => migrationLoader.LoadMigrations());
        }

        /// <summary>
        /// Defines the test method ObsoleteHandlesMigrationThatDoesNotInheritFromMigrationBaseClass.
        /// </summary>
        [Test]
        [Obsolete]
        public void ObsoleteHandlesMigrationThatDoesNotInheritFromMigrationBaseClass()
        {
            var conventions = new MigrationRunnerConventions();
            var asm = Assembly.GetExecutingAssembly();
            var loader = new DefaultMigrationInformationLoader(conventions, asm, "FluentMigrator.Tests.Unit.DoesNotInheritFromBaseClass", null);

            Assert.That(loader.LoadMigrations().Count(), Is.EqualTo(1));
        }

        /// <summary>
        /// Defines the test method ObsoleteShouldHandleTransactionlessMigrations.
        /// </summary>
        [Test]
        [Obsolete]
        public void ObsoleteShouldHandleTransactionlessMigrations()
        {
            var conventions = new MigrationRunnerConventions();
            var asm = Assembly.GetExecutingAssembly();
            var loader = new DefaultMigrationInformationLoader(conventions, asm, "FluentMigrator.Tests.Unit.DoesHandleTransactionLessMigrations", null);

            var list = loader.LoadMigrations().ToList();

            list.Count().ShouldBe(2);

            list[0].Value.Migration.GetType().ShouldBe(typeof(DoesHandleTransactionLessMigrations.MigrationThatIsTransactionLess));
            list[0].Value.TransactionBehavior.ShouldBe(TransactionBehavior.None);
            list[0].Value.Version.ShouldBe(1);

            list[1].Value.Migration.GetType().ShouldBe(typeof(DoesHandleTransactionLessMigrations.MigrationThatIsNotTransactionLess));
            list[1].Value.TransactionBehavior.ShouldBe(TransactionBehavior.Default);
            list[1].Value.Version.ShouldBe(2);
        }
    }

    // ReSharper disable once EmptyNamespace
    namespace EmptyNamespace
    {

    }

    namespace DoesHandleTransactionLessMigrations
    {
        /// <summary>
        /// Class MigrationThatIsTransactionLess.
        /// Implements the <see cref="FluentMigrator.Migration" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Migration" />
        [Migration(1, TransactionBehavior.None)]
        public class MigrationThatIsTransactionLess : Migration
        {
            /// <summary>
            /// Collect the UP migration expressions
            /// </summary>
            /// <exception cref="NotImplementedException"></exception>
            public override void Up()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Collects the DOWN migration expressions
            /// </summary>
            /// <exception cref="NotImplementedException"></exception>
            public override void Down()
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Class MigrationThatIsNotTransactionLess.
        /// Implements the <see cref="FluentMigrator.Migration" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Migration" />
        [Migration(2)]
        public class MigrationThatIsNotTransactionLess : Migration
        {
            /// <summary>
            /// Collect the UP migration expressions
            /// </summary>
            /// <exception cref="NotImplementedException"></exception>
            public override void Up()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Collects the DOWN migration expressions
            /// </summary>
            /// <exception cref="NotImplementedException"></exception>
            public override void Down()
            {
                throw new NotImplementedException();
            }
        }
    }

    namespace DoesNotInheritFromBaseClass
    {
        /// <summary>
        /// Class MigrationThatDoesNotInheritFromMigrationBaseClass.
        /// Implements the <see cref="FluentMigrator.IMigration" />
        /// </summary>
        /// <seealso cref="FluentMigrator.IMigration" />
        [Migration(1)]
        public class MigrationThatDoesNotInheritFromMigrationBaseClass : IMigration
        {
            /// <summary>
            /// The arbitrary application context passed to the task runner.
            /// </summary>
            /// <value>The application context.</value>
            /// <exception cref="NotImplementedException"></exception>
            public object ApplicationContext
            {
                get { throw new NotImplementedException(); }
            }

            /// <summary>
            /// Gets the connection string passed to the task runner
            /// </summary>
            /// <value>The connection string.</value>
            public string ConnectionString { get; } = null;

            /// <summary>
            /// Collects all Up migration expressions in the <paramref name="context" />.
            /// </summary>
            /// <param name="context">The context to use while collecting the Up migration expressions</param>
            /// <exception cref="NotImplementedException"></exception>
            public void GetUpExpressions(IMigrationContext context)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Collects all Down migration expressions in the <paramref name="context" />.
            /// </summary>
            /// <param name="context">The context to use while collecting the Down migration expressions</param>
            /// <exception cref="NotImplementedException"></exception>
            public void GetDownExpressions(IMigrationContext context)
            {
                throw new NotImplementedException();
            }
        }
    }

    namespace DuplicateVersionNumbers
    {
        /// <summary>
        /// Class Duplicate1.
        /// Implements the <see cref="FluentMigrator.Migration" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Migration" />
        [Migration(1)]
        public class Duplicate1 : Migration
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
        /// Class Duplicate2.
        /// Implements the <see cref="FluentMigrator.Migration" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Migration" />
        [Migration(1)]
        public class Duplicate2 : Migration
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
    }

    namespace TaggingTestFakes
    {
        /// <summary>
        /// Class TaggedMigration.
        /// Implements the <see cref="FluentMigrator.Migration" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Migration" />
        [Tags("UK", "IE", "QA", "Production")]
        [Migration(123)]
        public class TaggedMigration : Migration
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
        /// Class UntaggedMigration.
        /// Implements the <see cref="FluentMigrator.Migration" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Migration" />
        [Migration(567)]
        public class UntaggedMigration : Migration
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
}
