// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="IfDatabaseExpressionRootTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
//
// Copyright (c) 2007-2018, Sean Chambers <schambers80@gmail.com>
// Copyright (c) 2011, Grant Archibald
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

using FluentMigrator.Builders.IfDatabase;
using FluentMigrator.Infrastructure;
using FluentMigrator.Runner;

using Microsoft.Extensions.DependencyInjection;

using Moq;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Builders.IfDatabase
{
    /// <summary>
    /// Defines test class IfDatabaseExpressionRootTests.
    /// </summary>
    [TestFixture]
    public class IfDatabaseExpressionRootTests
    {
        /// <summary>
        /// Defines the test method CallsDelegateIfDatabaseTypeApplies.
        /// </summary>
        [Test]
        public void CallsDelegateIfDatabaseTypeApplies()
        {
            var delegateCalled = false;
            var context = ExecuteTestMigration(new[] { "SQLite" }, expr =>
            {
                expr.Delegate(() => delegateCalled = true);
            });

            context.Expressions.Count.ShouldBe(0);
            delegateCalled.ShouldBeTrue();
        }

        /// <summary>
        /// Defines the test method DoesntCallsDelegateIfDatabaseTypeDoesntMatch.
        /// </summary>
        [Test]
        public void DoesntCallsDelegateIfDatabaseTypeDoesntMatch()
        {
            var delegateCalled = false;
            var context = ExecuteTestMigration(new[] { "Blurb" }, expr =>
            {
                expr.Delegate(() => delegateCalled = true);
            });

            context.Expressions.Count.ShouldBe(0);
            delegateCalled.ShouldBeFalse();
        }

        /// <summary>
        /// Defines the test method WillAddExpressionIfDatabaseTypeApplies.
        /// </summary>
        [Test]
        public void WillAddExpressionIfDatabaseTypeApplies()
        {
            var context = ExecuteTestMigration("SQLite");

            context.Expressions.Count.ShouldBe(1);
        }

        /// <summary>
        /// Defines the test method WillAddExpressionIfProcessorInMigrationProcessorPredicate.
        /// </summary>
        [Test]
        public void WillAddExpressionIfProcessorInMigrationProcessorPredicate()
        {
            var context = ExecuteTestMigration(x => x == "SQLite");

            context.Expressions.Count.ShouldBe(1);
        }

        /// <summary>
        /// Defines the test method WillNotAddExpressionIfProcessorNotInMigrationProcessorPredicate.
        /// </summary>
        [Test]
        public void WillNotAddExpressionIfProcessorNotInMigrationProcessorPredicate()
        {
            var context = ExecuteTestMigration(x => x == "Db2" || x == "Hana");

            context.Expressions.Count.ShouldBe(0);
        }

        /// <summary>
        /// Defines the test method WillNotAddExpressionIfDatabaseTypeApplies.
        /// </summary>
        [Test]
        public void WillNotAddExpressionIfDatabaseTypeApplies()
        {
            var context = ExecuteTestMigration("Unknown");

            context.Expressions.Count.ShouldBe(0);
        }

        /// <summary>
        /// Defines the test method WillNotAddExpressionIfProcessorNotMigrationProcessor.
        /// </summary>
        [Test]
        public void WillNotAddExpressionIfProcessorNotMigrationProcessor()
        {
            var mock = new Mock<IMigrationProcessor>();
            mock.SetupGet(x => x.DatabaseType).Returns("Unknown");
            mock.SetupGet(x => x.DatabaseTypeAliases).Returns(new List<string>());
            var context = ExecuteTestMigration(new List<string>() { "SQLite" }, mock);

            context.Expressions.Count.ShouldBe(0);
        }

        /// <summary>
        /// Defines the test method WillAddExpressionIfOneDatabaseTypeApplies.
        /// </summary>
        [Test]
        public void WillAddExpressionIfOneDatabaseTypeApplies()
        {
            var context = ExecuteTestMigration("SQLite", "Unknown");

            context.Expressions.Count.ShouldBe(1);
        }

        /// <summary>
        /// Defines the test method WillAddAlterExpression.
        /// </summary>
        [Test]
        public void WillAddAlterExpression()
        {
            var context = ExecuteTestMigration(new List<string>() { "SQLite" }, m => m.Alter.Table("Foo").AddColumn("Blah").AsString());

            context.Expressions.Count.ShouldBeGreaterThan(0);
        }

        /// <summary>
        /// Defines the test method WillAddCreateExpression.
        /// </summary>
        [Test]
        public void WillAddCreateExpression()
        {
            var context = ExecuteTestMigration(new List<string>() { "SQLite" }, m => m.Create.Table("Foo").WithColumn("Blah").AsString());

            context.Expressions.Count.ShouldBeGreaterThan(0);
        }

        /// <summary>
        /// Defines the test method WillAddDeleteExpression.
        /// </summary>
        [Test]
        public void WillAddDeleteExpression()
        {
            var context = ExecuteTestMigration(new List<string>() { "SQLite" }, m => m.Delete.Table("Foo"));

            context.Expressions.Count.ShouldBeGreaterThan(0);
        }

        /// <summary>
        /// Defines the test method WillAddExecuteExpression.
        /// </summary>
        [Test]
        public void WillAddExecuteExpression()
        {
            var context = ExecuteTestMigration(new List<string>() { "SQLite" }, m => m.Execute.Sql("DROP TABLE Foo"));

            context.Expressions.Count.ShouldBeGreaterThan(0);
        }

        /// <summary>
        /// Defines the test method WillAddInsertExpression.
        /// </summary>
        [Test]
        public void WillAddInsertExpression()
        {
            var context = ExecuteTestMigration(new List<string>() { "SQLite" }, m => m.Insert.IntoTable("Foo").Row(new { Id = 1 }));

            context.Expressions.Count.ShouldBeGreaterThan(0);
        }

        /// <summary>
        /// Defines the test method WillAddRenameExpression.
        /// </summary>
        [Test]
        public void WillAddRenameExpression()
        {
            var context = ExecuteTestMigration(new List<string>() { "SQLite" }, m => m.Rename.Table("Foo").To("Foo2"));

            context.Expressions.Count.ShouldBeGreaterThan(0);
        }

        /// <summary>
        /// Defines the test method WillAddSchemaExpression.
        /// </summary>
        [Test]
        public void WillAddSchemaExpression()
        {
            var databaseTypes = new List<string>() { "Unknown" };
            // Arrange
            var unknownProcessorMock = new Mock<IMigrationProcessor>(MockBehavior.Loose);

            unknownProcessorMock.SetupGet(x => x.DatabaseType).Returns(databaseTypes.First());
            unknownProcessorMock.SetupGet(x => x.DatabaseTypeAliases).Returns(new List<string>());

            var context = ExecuteTestMigration(databaseTypes, unknownProcessorMock, m => m.Schema.Table("Foo").Exists());

            context.Expressions.Count.ShouldBe(0);

            unknownProcessorMock.Verify(x => x.TableExists(null, "Foo"));
        }

        /// <summary>
        /// Defines the test method WillAddUpdateExpression.
        /// </summary>
        [Test]
        public void WillAddUpdateExpression()
        {
            var context = ExecuteTestMigration(new List<string>() { "SQLite" }, m => m.Update.Table("Foo").Set(new { Id = 1 }));

            context.Expressions.Count.ShouldBeGreaterThan(0);
        }

        /// <summary>
        /// Executes the test migration.
        /// </summary>
        /// <param name="databaseType">Type of the database.</param>
        /// <returns>IMigrationContext.</returns>
        private IMigrationContext ExecuteTestMigration(params string[] databaseType)
        {
            return ExecuteTestMigration(databaseType, (IMock<IMigrationProcessor>)null);
        }

        /// <summary>
        /// Executes the test migration.
        /// </summary>
        /// <param name="databaseType">Type of the database.</param>
        /// <param name="fluentEpression">The fluent epression.</param>
        /// <returns>IMigrationContext.</returns>
        private IMigrationContext ExecuteTestMigration(IEnumerable<string> databaseType, params Action<IIfDatabaseExpressionRoot>[] fluentEpression)
        {
            return ExecuteTestMigration(databaseType, null, fluentEpression);
        }

        /// <summary>
        /// Executes the test migration.
        /// </summary>
        /// <param name="databaseType">Type of the database.</param>
        /// <param name="processor">The processor.</param>
        /// <param name="fluentExpression">The fluent expression.</param>
        /// <returns>IMigrationContext.</returns>
        private IMigrationContext ExecuteTestMigration(IEnumerable<string> databaseType, IMock<IMigrationProcessor> processor, params Action<IIfDatabaseExpressionRoot>[] fluentExpression)
        {
            // Initialize
            var services = ServiceCollectionExtensions.CreateServices()
                .ConfigureRunner(r => r.WithGlobalConnectionString("No connection"));
            if (processor != null)
            {
                services.WithProcessor(processor);
            }
            else
            {
                services = services.ConfigureRunner(r => r.AddSQLite());
            }

            var serviceProvider = services
                .BuildServiceProvider();
            var context = serviceProvider.GetRequiredService<IMigrationContext>();

            // Arrange
            var expression = new IfDatabaseExpressionRoot(context, databaseType.ToArray());

            // Act
            if (fluentExpression == null || fluentExpression.Length == 0)
            {
                expression.Create.Table("Foo").WithColumn("Id").AsInt16();
            }
            else
            {
                foreach (var action in fluentExpression)
                {
                    action(expression);
                }

            }

            return context;
        }

        /// <summary>
        /// Executes the test migration.
        /// </summary>
        /// <param name="databaseTypePredicate">The database type predicate.</param>
        /// <param name="fluentExpression">The fluent expression.</param>
        /// <returns>IMigrationContext.</returns>
        private IMigrationContext ExecuteTestMigration(Predicate<string> databaseTypePredicate, params Action<IIfDatabaseExpressionRoot>[] fluentExpression)
        {
            // Initialize
            var serviceProvider = ServiceCollectionExtensions.CreateServices()
                .ConfigureRunner(r => r.AddSQLite().WithGlobalConnectionString("No connection"))
                .BuildServiceProvider();
            var context = serviceProvider.GetRequiredService<IMigrationContext>();

            // Arrange
            var expression = new IfDatabaseExpressionRoot(context, databaseTypePredicate);

            // Act
            if (fluentExpression == null || fluentExpression.Length == 0)
                expression.Create.Table("Foo").WithColumn("Id").AsInt16();
            else
            {
                foreach (var action in fluentExpression)
                {
                    action(expression);
                }
            }

            return context;
        }
    }
}
