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
// Copyright (c) 2010, Nathan Brown
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
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using FluentMigrator.Expressions;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Exceptions;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Logging;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Runner.Processors.Firebird;
using FluentMigrator.Runner.Processors.MySql;
using FluentMigrator.Runner.Processors.Postgres;
using FluentMigrator.Runner.Processors.SqlAnywhere;
using FluentMigrator.Runner.Processors.SQLite;
using FluentMigrator.Runner.Processors.SqlServer;
using FluentMigrator.SqlAnywhere;
using FluentMigrator.Tests.Integration.Migrations.Tagged;
using FluentMigrator.Tests.Unit;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Moq;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Integration
{
    /// <summary>
    /// Defines test class MigrationRunnerTests.
    /// Implements the <see cref="FluentMigrator.Tests.Integration.IntegrationTestBase" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Integration.IntegrationTestBase" />
    [TestFixture]
    [Category("Integration")]
    public class MigrationRunnerTests : IntegrationTestBase
    {
        /// <summary>
        /// The root namespace
        /// </summary>
        private const string RootNamespace = "FluentMigrator.Tests.Integration.Migrations";

        /// <summary>
        /// Defines the test method CanRunMigration.
        /// </summary>
        [Test]
        [Category("Firebird")]
        [Category("MySql")]
        [Category("SQLite")]
        [Category("Postgres")]
        [Category("SqlServer2005")]
        [Category("SqlServer2008")]
        [Category("SqlServer2012")]
        [Category("SqlServer2014")]
        [Category("SqlServer2016")]
        [Category("SqlAnywhere16")]
        public void CanRunMigration()
        {
            ExecuteWithSupportedProcessors(
                init => init.WithMigrationsIn(RootNamespace),
                (serviceProvider, processor) =>
                {
                    var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

                    runner.Up(new TestCreateAndDropTableMigration());

                    processor.TableExists(null, "TestTable").ShouldBeTrue();

                    // This is a hack until MigrationVersionRunner and MigrationRunner are refactored and merged together
                    //processor.CommitTransaction();

                    runner.Down(new TestCreateAndDropTableMigration());
                    processor.TableExists(null, "TestTable").ShouldBeFalse();
                });
        }

        /// <summary>
        /// Defines the test method CanSilentlyFail.
        /// </summary>
        [Test]
        [Category("Firebird")]
        [Category("MySql")]
        [Category("SQLite")]
        [Category("Postgres")]
        [Category("SqlServer2005")]
        [Category("SqlServer2008")]
        [Category("SqlServer2012")]
        [Category("SqlServer2014")]
        [Category("SqlServer2016")]
        [Category("SqlAnywhere16")]
        public void CanSilentlyFail()
        {
            var processor = new Mock<IMigrationProcessor>();
            processor.Setup(x => x.Process(It.IsAny<CreateForeignKeyExpression>())).Throws(new Exception("Error"));
            processor.Setup(x => x.Process(It.IsAny<DeleteForeignKeyExpression>())).Throws(new Exception("Error"));

            var serviceProvider = ServiceCollectionExtensions.CreateServices()
                .Configure<ProcessorOptions>(opt => opt.PreviewOnly = false)
                .AddScoped<IConnectionStringReader>(_ => new PassThroughConnectionStringReader("No connection"))
                .WithProcessor(processor)
                .WithMigrationsIn(RootNamespace)
                .BuildServiceProvider();

            var runner = (MigrationRunner) serviceProvider.GetRequiredService<IMigrationRunner>();
            runner.SilentlyFail = true;

            runner.Up(new TestForeignKeySilentFailure());

            runner.CaughtExceptions.Count.ShouldBeGreaterThan(0);

            runner.Down(new TestForeignKeySilentFailure());
            runner.CaughtExceptions.Count.ShouldBeGreaterThan(0);
        }

        /// <summary>
        /// Defines the test method CanApplyForeignKeyConvention.
        /// </summary>
        [Test]
        [Category("Firebird")]
        [Category("MySql")]
        [Category("Postgres")]
        [Category("SqlServer2005")]
        [Category("SqlServer2008")]
        [Category("SqlServer2012")]
        [Category("SqlServer2014")]
        [Category("SqlServer2016")]
        [Category("SqlAnywhere16")]
        public void CanApplyForeignKeyConvention()
        {
            ExecuteWithSupportedProcessors(
                services => services.WithMigrationsIn(RootNamespace),
                (serviceProvider, processor) =>
                {
                    var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

                    runner.Up(new TestForeignKeyNamingConvention());
                    processor.ConstraintExists(null, "Users", "FK_Users_GroupId_Groups_GroupId").ShouldBeTrue();

                    runner.Down(new TestForeignKeyNamingConvention());
                    processor.ConstraintExists(null, "Users", "FK_Users_GroupId_Groups_GroupId").ShouldBeFalse();
                },
                true,
                typeof(SQLiteProcessor));
        }

        /// <summary>
        /// Defines the test method CanApplyForeignKeyConventionWithSchema.
        /// </summary>
        [Test]
        [Category("MySql")]
        [Category("Postgres")]
        [Category("SqlServer2005")]
        [Category("SqlServer2008")]
        [Category("SqlServer2012")]
        [Category("SqlServer2014")]
        [Category("SqlServer2016")]
        [Category("SqlAnywhere16")]
        public void CanApplyForeignKeyConventionWithSchema()
        {
            ExecuteWithSupportedProcessors(
                services => services.WithMigrationsIn(RootNamespace),
                (serviceProvider, processor) =>
                {
                    var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

                    runner.Up(new TestForeignKeyNamingConventionWithSchema());

                    processor.ConstraintExists("TestSchema", "Users", "FK_Users_GroupId_Groups_GroupId").ShouldBeTrue();
                    runner.Down(new TestForeignKeyNamingConventionWithSchema());
                },
                false,
                typeof(SQLiteProcessor),
                typeof(FirebirdProcessor));
        }

        /// <summary>
        /// Defines the test method CanApplyIndexConvention.
        /// </summary>
        [Test]
        [Category("Firebird")]
        [Category("MySql")]
        [Category("SQLite")]
        [Category("Postgres")]
        [Category("SqlServer2005")]
        [Category("SqlServer2008")]
        [Category("SqlServer2012")]
        [Category("SqlServer2014")]
        [Category("SqlServer2016")]
        [Category("SqlAnywhere16")]
        public void CanApplyIndexConvention()
        {
            ExecuteWithSupportedProcessors(
                services => services.WithMigrationsIn(RootNamespace),
                (serviceProvider, processor) =>
                {
                    var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

                    runner.Up(new TestIndexNamingConvention());
                    processor.IndexExists(null, "Users", "IX_Users_GroupId").ShouldBeTrue();
                    processor.TableExists(null, "Users").ShouldBeTrue();

                    runner.Down(new TestIndexNamingConvention());
                    processor.IndexExists(null, "Users", "IX_Users_GroupId").ShouldBeFalse();
                    processor.TableExists(null, "Users").ShouldBeFalse();
                });
        }

        /// <summary>
        /// Defines the test method CanApplyUniqueConvention.
        /// </summary>
        [Test]
        [Category("Firebird")]
        [Category("MySql")]
        [Category("Postgres")]
        [Category("SqlServer2005")]
        [Category("SqlServer2008")]
        [Category("SqlServer2012")]
        [Category("SqlServer2014")]
        [Category("SqlServer2016")]
        [Category("SqlAnywhere16")]
        public void CanApplyUniqueConvention()
        {
            ExecuteWithSupportedProcessors(
                services => services.WithMigrationsIn(RootNamespace),
                (serviceProvider, processor) =>
                {
                    var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

                    runner.Up(new TestUniqueConstraintNamingConvention());
                    processor.ConstraintExists(null, "Users", "UC_Users_GroupId").ShouldBeTrue();
                    processor.ConstraintExists(null, "Users", "UC_Users_AccountId").ShouldBeTrue();
                    processor.TableExists(null, "Users").ShouldBeTrue();

                    runner.Down(new TestUniqueConstraintNamingConvention());
                    processor.ConstraintExists(null, "Users", "UC_Users_GroupId").ShouldBeFalse();
                    processor.ConstraintExists(null, "Users", "UC_Users_AccountId").ShouldBeFalse();
                    processor.TableExists(null, "Users").ShouldBeFalse();
                },
                false,
                typeof(SQLiteProcessor));
        }

        /// <summary>
        /// Defines the test method CanApplyIndexConventionWithSchema.
        /// </summary>
        [Test]
        [Category("Firebird")]
        [Category("MySql")]
        [Category("SQLite")]
        [Category("Postgres")]
        [Category("SqlServer2005")]
        [Category("SqlServer2008")]
        [Category("SqlServer2012")]
        [Category("SqlServer2014")]
        [Category("SqlServer2016")]
        [Category("SqlAnywhere16")]
        public void CanApplyIndexConventionWithSchema()
        {
            ExecuteWithSupportedProcessors(
                services => services.WithMigrationsIn(RootNamespace),
                (serviceProvider, processor) =>
                {
                    var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

                    runner.Up(new TestIndexNamingConventionWithSchema());
                    processor.IndexExists("TestSchema", "Users", "IX_Users_GroupId").ShouldBeTrue();
                    processor.TableExists("TestSchema", "Users").ShouldBeTrue();

                    runner.Down(new TestIndexNamingConventionWithSchema());
                    processor.IndexExists("TestSchema", "Users", "IX_Users_GroupId").ShouldBeFalse();
                    processor.TableExists("TestSchema", "Users").ShouldBeFalse();
                });
        }

        /// <summary>
        /// Defines the test method CanCreateAndDropIndex.
        /// </summary>
        [Test]
        [Category("Firebird")]
        [Category("MySql")]
        [Category("SQLite")]
        [Category("Postgres")]
        [Category("SqlServer2005")]
        [Category("SqlServer2008")]
        [Category("SqlServer2012")]
        [Category("SqlServer2014")]
        [Category("SqlServer2016")]
        [Category("SqlAnywhere16")]
        public void CanCreateAndDropIndex()
        {
            ExecuteWithSupportedProcessors(
                services => services.WithMigrationsIn(RootNamespace),
                (serviceProvider, processor) =>
                {
                    var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

                    runner.Up(new TestCreateAndDropTableMigration());
                    processor.IndexExists(null, "TestTable", "IX_TestTable_Name").ShouldBeFalse();

                    runner.Up(new TestCreateAndDropIndexMigration());
                    processor.IndexExists(null, "TestTable", "IX_TestTable_Name").ShouldBeTrue();

                    runner.Down(new TestCreateAndDropIndexMigration());
                    processor.IndexExists(null, "TestTable", "IX_TestTable_Name").ShouldBeFalse();

                    runner.Down(new TestCreateAndDropTableMigration());
                    processor.IndexExists(null, "TestTable", "IX_TestTable_Name").ShouldBeFalse();

                    //processor.CommitTransaction();
                });
        }

        /// <summary>
        /// Defines the test method CanCreateAndDropIndexWithSchema.
        /// </summary>
        [Test]
        [Category("MySql")]
        [Category("Postgres")]
        [Category("SqlServer2005")]
        [Category("SqlServer2008")]
        [Category("SqlServer2012")]
        [Category("SqlServer2014")]
        [Category("SqlServer2016")]
        [Category("SqlAnywhere16")]
        public void CanCreateAndDropIndexWithSchema()
        {
            ExecuteWithSupportedProcessors(
                services => services.WithMigrationsIn(RootNamespace),
                (serviceProvider, processor) =>
                {
                    var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

                    runner.Up(new TestCreateSchema());

                    runner.Up(new TestCreateAndDropTableMigrationWithSchema());
                    processor.IndexExists("TestSchema", "TestTable", "IX_TestTable_Name").ShouldBeFalse();

                    runner.Up(new TestCreateAndDropIndexMigrationWithSchema());
                    processor.IndexExists("TestSchema", "TestTable", "IX_TestTable_Name").ShouldBeTrue();

                    runner.Down(new TestCreateAndDropIndexMigrationWithSchema());
                    processor.IndexExists("TestSchema", "TestTable", "IX_TestTable_Name").ShouldBeFalse();

                    runner.Down(new TestCreateAndDropTableMigrationWithSchema());
                    processor.IndexExists("TestSchema", "TestTable", "IX_TestTable_Name").ShouldBeFalse();

                    runner.Down(new TestCreateSchema());
                    //processor.CommitTransaction();
                },
                false,
                typeof(SQLiteProcessor),
                typeof(FirebirdProcessor));
        }

        /// <summary>
        /// Defines the test method CanRenameTable.
        /// </summary>
        [Test]
        [Category("Firebird")]
        [Category("MySql")]
        [Category("SQLite")]
        [Category("Postgres")]
        [Category("SqlServer2005")]
        [Category("SqlServer2008")]
        [Category("SqlServer2012")]
        [Category("SqlServer2014")]
        [Category("SqlServer2016")]
        [Category("SqlAnywhere16")]
        public void CanRenameTable()
        {
            ExecuteWithSupportedProcessors(
                services => services.WithMigrationsIn(RootNamespace),
                (serviceProvider, processor) =>
                {
                    var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

                    runner.Up(new TestCreateAndDropTableMigration());
                    processor.TableExists(null, "TestTable2").ShouldBeTrue();

                    runner.Up(new TestRenameTableMigration());
                    processor.TableExists(null, "TestTable2").ShouldBeFalse();
                    processor.TableExists(null, "TestTable'3").ShouldBeTrue();

                    runner.Down(new TestRenameTableMigration());
                    processor.TableExists(null, "TestTable'3").ShouldBeFalse();
                    processor.TableExists(null, "TestTable2").ShouldBeTrue();

                    runner.Down(new TestCreateAndDropTableMigration());
                    processor.TableExists(null, "TestTable2").ShouldBeFalse();

                    //processor.CommitTransaction();
                });
        }

        /// <summary>
        /// Defines the test method CanRenameTableWithSchema.
        /// </summary>
        [Test]
        [Category("Firebird")]
        [Category("MySql")]
        [Category("SQLite")]
        [Category("Postgres")]
        [Category("SqlServer2005")]
        [Category("SqlServer2008")]
        [Category("SqlServer2012")]
        [Category("SqlServer2014")]
        [Category("SqlServer2016")]
        [Category("SqlAnywhere16")]
        public void CanRenameTableWithSchema()
        {
            ExecuteWithSupportedProcessors(
                services => services.WithMigrationsIn(RootNamespace),
                (serviceProvider, processor) =>
                {
                    var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

                    runner.Up(new TestCreateSchema());

                    runner.Up(new TestCreateAndDropTableMigrationWithSchema());
                    processor.TableExists("TestSchema", "TestTable2").ShouldBeTrue();

                    runner.Up(new TestRenameTableMigrationWithSchema());
                    processor.TableExists("TestSchema", "TestTable2").ShouldBeFalse();
                    processor.TableExists("TestSchema", "TestTable'3").ShouldBeTrue();

                    runner.Down(new TestRenameTableMigrationWithSchema());
                    processor.TableExists("TestSchema", "TestTable'3").ShouldBeFalse();
                    processor.TableExists("TestSchema", "TestTable2").ShouldBeTrue();

                    runner.Down(new TestCreateAndDropTableMigrationWithSchema());
                    processor.TableExists("TestSchema", "TestTable2").ShouldBeFalse();

                    runner.Down(new TestCreateSchema());

                    //processor.CommitTransaction();
                });
        }

        /// <summary>
        /// Defines the test method CanRenameColumn.
        /// </summary>
        [Test]
        [Category("Firebird")]
        [Category("MySql")]
        [Category("Postgres")]
        [Category("SqlServer2005")]
        [Category("SqlServer2008")]
        [Category("SqlServer2012")]
        [Category("SqlServer2014")]
        [Category("SqlServer2016")]
        [Category("SqlAnywhere16")]
        public void CanRenameColumn()
        {
            ExecuteWithSupportedProcessors(
                services => services.WithMigrationsIn(RootNamespace),
                (serviceProvider, processor) =>
                {
                    var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

                    runner.Up(new TestCreateAndDropTableMigration());
                    processor.ColumnExists(null, "TestTable2", "Name").ShouldBeTrue();

                    runner.Up(new TestRenameColumnMigration());
                    processor.ColumnExists(null, "TestTable2", "Name").ShouldBeFalse();
                    processor.ColumnExists(null, "TestTable2", "Name'3").ShouldBeTrue();

                    runner.Down(new TestRenameColumnMigration());
                    processor.ColumnExists(null, "TestTable2", "Name'3").ShouldBeFalse();
                    processor.ColumnExists(null, "TestTable2", "Name").ShouldBeTrue();

                    runner.Down(new TestCreateAndDropTableMigration());
                    processor.ColumnExists(null, "TestTable2", "Name").ShouldBeFalse();
                },
                true,
                typeof(SQLiteProcessor));
        }

        /// <summary>
        /// Defines the test method CanRenameColumnWithSchema.
        /// </summary>
        [Test]
        [Category("MySql")]
        [Category("Postgres")]
        [Category("SqlServer2005")]
        [Category("SqlServer2008")]
        [Category("SqlServer2012")]
        [Category("SqlServer2014")]
        [Category("SqlServer2016")]
        [Category("SqlAnywhere16")]
        public void CanRenameColumnWithSchema()
        {
            ExecuteWithSupportedProcessors(
                services => services.WithMigrationsIn(RootNamespace),
                (serviceProvider, processor) =>
                {
                    var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

                    runner.Up(new TestCreateSchema());

                    runner.Up(new TestCreateAndDropTableMigrationWithSchema());
                    processor.ColumnExists("TestSchema", "TestTable2", "Name").ShouldBeTrue();

                    runner.Up(new TestRenameColumnMigrationWithSchema());
                    processor.ColumnExists("TestSchema", "TestTable2", "Name").ShouldBeFalse();
                    processor.ColumnExists("TestSchema", "TestTable2", "Name'3").ShouldBeTrue();

                    runner.Down(new TestRenameColumnMigrationWithSchema());
                    processor.ColumnExists("TestSchema", "TestTable2", "Name'3").ShouldBeFalse();
                    processor.ColumnExists("TestSchema", "TestTable2", "Name").ShouldBeTrue();

                    runner.Down(new TestCreateAndDropTableMigrationWithSchema());
                    processor.ColumnExists("TestSchema", "TestTable2", "Name").ShouldBeFalse();

                    runner.Down(new TestCreateSchema());
                },
                true,
                typeof(SQLiteProcessor),
                typeof(FirebirdProcessor));
        }

        /// <summary>
        /// Defines the test method CanLoadMigrations.
        /// </summary>
        [Test]
        [Category("Firebird")]
        [Category("MySql")]
        [Category("SQLite")]
        [Category("Postgres")]
        [Category("SqlServer2005")]
        [Category("SqlServer2008")]
        [Category("SqlServer2012")]
        [Category("SqlServer2014")]
        [Category("SqlServer2016")]
        [Category("SqlAnywhere16")]
        public void CanLoadMigrations()
        {
            ExecuteWithSupportedProcessors(
                services => services.WithMigrationsIn(RootNamespace),
                (serviceProvider, _) =>
                {
                    var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

                    //runner.Processor.CommitTransaction();

                    runner.MigrationLoader.LoadMigrations().ShouldNotBeNull();
                });
        }

        /// <summary>
        /// Defines the test method CanLoadVersion.
        /// </summary>
        [Test]
        [Category("Firebird")]
        [Category("MySql")]
        [Category("SQLite")]
        [Category("Postgres")]
        [Category("SqlServer2005")]
        [Category("SqlServer2008")]
        [Category("SqlServer2012")]
        [Category("SqlServer2014")]
        [Category("SqlServer2016")]
        public void CanLoadVersion()
        {
            ExecuteWithSupportedProcessors(
                services => services.WithMigrationsIn(RootNamespace),
                (serviceProvider, _) =>
                {
                    var runner = (MigrationRunner) serviceProvider.GetRequiredService<IMigrationRunner>();

                    //runner.Processor.CommitTransaction();
                    runner.VersionLoader.VersionInfo.ShouldNotBeNull();
                },
                true,
                typeof(SqlAnywhere16Processor));
        }

        /// <summary>
        /// Defines the test method CanRunMigrations.
        /// </summary>
        [Test]
        [Category("Firebird")]
        [Category("MySql")]
        [Category("SQLite")]
        [Category("Postgres")]
        [Category("SqlServer2005")]
        [Category("SqlServer2008")]
        [Category("SqlServer2012")]
        [Category("SqlServer2014")]
        [Category("SqlServer2016")]
        public void CanRunMigrations()
        {
            ExecuteWithSupportedProcessors(
                services => services.WithMigrationsIn(RootNamespace),
                (serviceProvider, _) =>
                {
                    var runner = (MigrationRunner) serviceProvider.GetRequiredService<IMigrationRunner>();

                    runner.MigrateUp(false);

                    runner.VersionLoader.VersionInfo.HasAppliedMigration(1).ShouldBeTrue();
                    runner.VersionLoader.VersionInfo.HasAppliedMigration(2).ShouldBeTrue();
                    runner.VersionLoader.VersionInfo.HasAppliedMigration(3).ShouldBeTrue();
                    runner.VersionLoader.VersionInfo.HasAppliedMigration(4).ShouldBeTrue();
                    runner.VersionLoader.VersionInfo.HasAppliedMigration(5).ShouldBeTrue();
                    runner.VersionLoader.VersionInfo.HasAppliedMigration(6).ShouldBeTrue();
                    runner.VersionLoader.VersionInfo.Latest().ShouldBe(6);

                    runner.RollbackToVersion(0, false);
                },
                true,
                typeof(SqlAnywhere16Processor));
        }

        /// <summary>
        /// Defines the test method CanMigrateASpecificVersion.
        /// </summary>
        [Test]
        [Category("Firebird")]
        [Category("MySql")]
        [Category("SQLite")]
        [Category("Postgres")]
        [Category("SqlServer2005")]
        [Category("SqlServer2008")]
        [Category("SqlServer2012")]
        [Category("SqlServer2014")]
        [Category("SqlServer2016")]
        public void CanMigrateASpecificVersion()
        {
            ExecuteWithSupportedProcessors(
                services => services.WithMigrationsIn(RootNamespace),
                (serviceProvider, processor) =>
                {
                    var runner = (MigrationRunner) serviceProvider.GetRequiredService<IMigrationRunner>();
                    try
                    {
                        runner.MigrateUp(1, false);

                        runner.VersionLoader.VersionInfo.HasAppliedMigration(1).ShouldBeTrue();
                        processor.TableExists(null, "Users").ShouldBeTrue();
                    }
                    catch (Exception ex) when (LogException(ex))
                    {
                    }
                    finally
                    {
                        runner.RollbackToVersion(0, false);
                    }
                },
                true,
                typeof(SqlAnywhere16Processor));
        }

        /// <summary>
        /// Defines the test method CanMigrateASpecificVersionDown.
        /// </summary>
        [Test]
        [Category("Firebird")]
        [Category("MySql")]
        [Category("Postgres")]
        [Category("SqlServer2005")]
        [Category("SqlServer2008")]
        [Category("SqlServer2012")]
        [Category("SqlServer2014")]
        [Category("SqlServer2016")]
        public void CanMigrateASpecificVersionDown()
        {
            try
            {
                ExecuteWithSupportedProcessors(
                    services => services.WithMigrationsIn(RootNamespace),
                    (serviceProvider, processor) =>
                    {
                        using (var scope = serviceProvider.CreateScope())
                        {
                            var runner = (MigrationRunner) scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                            runner.MigrateUp(1, false);
                            runner.VersionLoader.VersionInfo.HasAppliedMigration(1).ShouldBeTrue();
                        }

                        processor.TableExists(null, "Users").ShouldBeTrue();

                        using (var scope = serviceProvider.CreateScope())
                        {
                            var runner = (MigrationRunner) scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                            runner.MigrateDown(0, false);
                            runner.VersionLoader.VersionInfo.HasAppliedMigration(1).ShouldBeFalse();
                        }

                        processor.TableExists(null, "Users").ShouldBeFalse();
                    },
                    false,
                    typeof(SQLiteProcessor), typeof(SqlAnywhere16Processor));
            }
            finally
            {
                ExecuteWithSupportedProcessors(
                    services => services.WithMigrationsIn(RootNamespace),
                    (serviceProvider, _) =>
                    {
                        var runner = (MigrationRunner) serviceProvider.GetRequiredService<IMigrationRunner>();
                        runner.RollbackToVersion(0, false);
                    },
                    false,
                    typeof(SQLiteProcessor), typeof(SqlAnywhere16Processor));
            }
        }

        /// <summary>
        /// Defines the test method RollbackAllShouldRemoveVersionInfoTable.
        /// </summary>
        [Test]
        [Category("Firebird")]
        [Category("MySql")]
        [Category("SQLite")]
        [Category("Postgres")]
        [Category("SqlServer2005")]
        [Category("SqlServer2008")]
        [Category("SqlServer2012")]
        [Category("SqlServer2014")]
        [Category("SqlServer2016")]
        public void RollbackAllShouldRemoveVersionInfoTable()
        {
            ExecuteWithSupportedProcessors(
                services => services.WithMigrationsIn(RootNamespace),
                (serviceProvider, processor) =>
                {
                    var runner = (MigrationRunner) serviceProvider.GetRequiredService<IMigrationRunner>();

                    runner.MigrateUp(2);

                    processor.TableExists(
                        runner.VersionLoader.VersionTableMetaData.SchemaName,
                        runner.VersionLoader.VersionTableMetaData.TableName).ShouldBeTrue();

                    runner.RollbackToVersion(0);

                    processor.TableExists(
                        runner.VersionLoader.VersionTableMetaData.SchemaName,
                        runner.VersionLoader.VersionTableMetaData.TableName).ShouldBeFalse();
                },
                true,
                typeof(SqlAnywhere16Processor));
        }

        /// <summary>
        /// Defines the test method MigrateUpWithSqlServerProcessorShouldCommitItsTransaction.
        /// </summary>
        [Test]
        [Category("SqlServer2008")]
        public void MigrateUpWithSqlServerProcessorShouldCommitItsTransaction()
        {
            if (!IntegrationTestOptions.SqlServer2008.IsEnabled)
            {
                return;
            }

            ExecuteWithProcessor<SqlServer2008Processor>(
                init => init.WithMigrationsIn(RootNamespace),
                (serviceProvider, processor) =>
                {
                    var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
                    runner.MigrateUp();

                    try
                    {
                        processor.WasCommitted.ShouldBeTrue();
                    }
                    finally
                    {
                        CleanupTestSqlServerDatabase(serviceProvider, processor);
                    }
                },
                false,
                IntegrationTestOptions.SqlServer2008);
        }

        /// <summary>
        /// Defines the test method MigrateUpSpecificVersionWithSqlServerProcessorShouldCommitItsTransaction.
        /// </summary>
        [Test]
        [Category("SqlServer2008")]
        public void MigrateUpSpecificVersionWithSqlServerProcessorShouldCommitItsTransaction()
        {
            if (!IntegrationTestOptions.SqlServer2008.IsEnabled)
            {
                return;
            }

            ExecuteWithProcessor<SqlServer2008Processor>(
                init => init.WithMigrationsIn(RootNamespace),
                (serviceProvider, processor) =>
                {
                    var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
                    runner.MigrateUp(1);

                    try
                    {
                        processor.WasCommitted.ShouldBeTrue();
                    }
                    finally
                    {
                        CleanupTestSqlServerDatabase(serviceProvider, processor);
                    }
                },
                false,
                IntegrationTestOptions.SqlServer2008);
        }

        /// <summary>
        /// Defines the test method MigrateUpWithTaggedMigrationsShouldOnlyApplyMatchedMigrations.
        /// </summary>
        [Test]
        [Category("Firebird")]
        [Category("MySql")]
        [Category("SQLite")]
        [Category("Postgres")]
        [Category("SqlServer2005")]
        [Category("SqlServer2008")]
        [Category("SqlServer2012")]
        [Category("SqlServer2014")]
        [Category("SqlServer2016")]
        public void MigrateUpWithTaggedMigrationsShouldOnlyApplyMatchedMigrations()
        {
            ExecuteWithSupportedProcessors(
                services => services.WithMigrationsIn(typeof(TenantATable).Namespace)
                    .Configure<RunnerOptions>(opt => opt.Tags = new[] { "TenantA" }),
                (serviceProvider, processor) =>
                {
                    var runner = (MigrationRunner) serviceProvider.GetRequiredService<IMigrationRunner>();

                    try
                    {
                        runner.MigrateUp(false);

                        processor.TableExists(null, "TenantATable").ShouldBeTrue();
                        processor.TableExists(null, "NormalTable").ShouldBeTrue();
                        processor.TableExists(null, "TenantBTable").ShouldBeFalse();
                        processor.TableExists(null, "TenantAandBTable").ShouldBeTrue();
                    }
                    finally
                    {
                        runner.RollbackToVersion(0);
                    }
                },
                true,
                typeof(SqlAnywhere16Processor));
        }

        /// <summary>
        /// Defines the test method MigrateUpWithTaggedMigrationsAndUsingMultipleTagsShouldOnlyApplyMatchedMigrations.
        /// </summary>
        [Test]
        [Category("Firebird")]
        [Category("MySql")]
        [Category("SQLite")]
        [Category("Postgres")]
        [Category("SqlServer2005")]
        [Category("SqlServer2008")]
        [Category("SqlServer2012")]
        [Category("SqlServer2014")]
        [Category("SqlServer2016")]
        public void MigrateUpWithTaggedMigrationsAndUsingMultipleTagsShouldOnlyApplyMatchedMigrations()
        {
            ExecuteWithSupportedProcessors(
                services => services.WithMigrationsIn(typeof(TenantATable).Namespace)
                    .Configure<RunnerOptions>(opt => opt.Tags = new[] { "TenantA", "TenantB" }),
                (serviceProvider, processor) =>
                {
                    var runner = (MigrationRunner) serviceProvider.GetRequiredService<IMigrationRunner>();

                    try
                    {
                        runner.MigrateUp(false);

                        processor.TableExists(null, "TenantATable").ShouldBeFalse();
                        processor.TableExists(null, "NormalTable").ShouldBeTrue();
                        processor.TableExists(null, "TenantBTable").ShouldBeFalse();
                        processor.TableExists(null, "TenantAandBTable").ShouldBeTrue();
                    }
                    finally
                    {
                        runner.RollbackToVersion(0);
                    }
                },
                true,
                typeof(SqlAnywhere16Processor));
        }

        /// <summary>
        /// Defines the test method MigrateUpWithDifferentTaggedShouldIgnoreConcreteOfTagged.
        /// </summary>
        [Test]
        [Category("Firebird")]
        [Category("MySql")]
        [Category("SQLite")]
        [Category("Postgres")]
        [Category("SqlServer2005")]
        [Category("SqlServer2008")]
        [Category("SqlServer2012")]
        [Category("SqlServer2014")]
        [Category("SqlServer2016")]
        public void MigrateUpWithDifferentTaggedShouldIgnoreConcreteOfTagged()
        {
            ExecuteWithSupportedProcessors(
                services => services.WithMigrationsIn(typeof(TenantATable).Namespace)
                    .Configure<RunnerOptions>(opt => opt.Tags = new[] { "TenantB" }),
                (serviceProvider, processor) =>
                {
                    var runner = (MigrationRunner) serviceProvider.GetRequiredService<IMigrationRunner>();

                    try
                    {
                        runner.MigrateUp(false);

                        processor.TableExists(null, "TenantATable").ShouldBeFalse();
                        processor.TableExists(null, "NormalTable").ShouldBeTrue();
                        processor.TableExists(null, "TenantBTable").ShouldBeTrue();
                    }
                    finally
                    {
                        runner.RollbackToVersion(0);
                    }
                },
                true,
                typeof(SqlAnywhere16Processor));
        }

        /// <summary>
        /// Defines the test method MigrateDownWithDifferentTagsToMigrateUpShouldApplyMatchedMigrations.
        /// </summary>
        [Test]
        [Category("Firebird")]
        [Category("MySql")]
        [Category("Postgres")]
        [Category("SqlServer2005")]
        [Category("SqlServer2008")]
        [Category("SqlServer2012")]
        [Category("SqlServer2014")]
        [Category("SqlServer2016")]
        public void MigrateDownWithDifferentTagsToMigrateUpShouldApplyMatchedMigrations()
        {
            var migrationsNamespace = typeof(TenantATable).Namespace;

            try
            {
                // Excluded SqliteProcessor as it errors on DB cleanup (RollbackToVersion).
                ExecuteWithSupportedProcessors(
                    services => services.WithMigrationsIn(migrationsNamespace).Configure<RunnerOptions>(opt => opt.Tags = new[] { "TenantA" }),
                    (serviceProvider, processor) =>
                    {
                        var runner = (MigrationRunner)serviceProvider.GetRequiredService<IMigrationRunner>();
                        runner.MigrateUp(false);

                        processor.TableExists(null, "TenantATable").ShouldBeTrue();
                        processor.TableExists(null, "NormalTable").ShouldBeTrue();
                        processor.TableExists(null, "TenantBTable").ShouldBeFalse();
                        processor.TableExists(null, "TenantAandBTable").ShouldBeTrue();
                    },
                    false,
                    typeof(SQLiteProcessor), typeof(SqlAnywhere16Processor));

                ExecuteWithSupportedProcessors(
                    services => services.WithMigrationsIn(migrationsNamespace).Configure<RunnerOptions>(opt => opt.Tags = new[] { "TenantB" }),
                    (serviceProvider, processor) =>
                    {
                        var runner = (MigrationRunner)serviceProvider.GetRequiredService<IMigrationRunner>();
                        runner.MigrateDown(0, false);

                        processor.TableExists(null, "TenantATable").ShouldBeTrue();
                        processor.TableExists(null, "NormalTable").ShouldBeFalse();
                        processor.TableExists(null, "TenantBTable").ShouldBeFalse();
                        processor.TableExists(null, "TenantAandBTable").ShouldBeFalse();
                    },
                    false,
                    typeof(SQLiteProcessor), typeof(SqlAnywhere16Processor));
            }
            finally
            {
                ExecuteWithSupportedProcessors(
                    services => services.WithMigrationsIn(migrationsNamespace).Configure<RunnerOptions>(opt => opt.Tags = new[] { "TenantA" }),
                    (serviceProvider, _) =>
                    {
                        var runner = (MigrationRunner)serviceProvider.GetRequiredService<IMigrationRunner>();
                        runner.RollbackToVersion(0, false);
                    },
                    false,
                    typeof(SQLiteProcessor), typeof(SqlAnywhere16Processor));
            }
        }

        /// <summary>
        /// Defines the test method VersionInfoCreationScriptsOnlyGeneratedOnceInPreviewMode.
        /// </summary>
        [Test]
        [Category("SqlServer2008")]
        public void VersionInfoCreationScriptsOnlyGeneratedOnceInPreviewMode()
        {
            if (!IntegrationTestOptions.SqlServer2008.IsEnabled)
            {
                return;
            }

            var outputSql = new StringWriter();
            var provider = new SqlScriptFluentMigratorLoggerProvider(
                outputSql,
                new SqlScriptFluentMigratorLoggerOptions()
                {
                    ShowSql = true,
                });

            ExecuteWithProcessor<SqlServer2008Processor>(
                services => services
                    .ConfigureRunner(rb => rb.WithVersionTable(new TestVersionTableMetaData()))
                    .WithMigrationsIn(RootNamespace)
                    .Configure<ProcessorOptions>(opt => opt.PreviewOnly = true)
                    .AddSingleton<ILoggerProvider>(provider),
                (serviceProvider, processor) =>
                {
                    try
                    {
                        var versionTableMetaData = new TestVersionTableMetaData();

                        var runner = (MigrationRunner)serviceProvider.GetRequiredService<IMigrationRunner>();
                        runner.MigrateUp(1, false);

                        processor.CommitTransaction();

                        var schemaName = versionTableMetaData.SchemaName;
                        var schemaAndTableName = $"[{schemaName}].[{TestVersionTableMetaData.TABLE_NAME}]";

                        var outputSqlString = outputSql.ToString();

                        var createSchemaMatches = new Regex(Regex.Escape($"CREATE SCHEMA [{schemaName}]"))
                            .Matches(outputSqlString).Count;
                        var createTableMatches = new Regex(Regex.Escape("CREATE TABLE " + schemaAndTableName))
                            .Matches(outputSqlString).Count;
                        var createIndexMatches = new Regex(Regex.Escape("CREATE UNIQUE CLUSTERED INDEX [" + TestVersionTableMetaData.UNIQUE_INDEX_NAME + "] ON " + schemaAndTableName))
                            .Matches(outputSqlString).Count;
                        var alterTableMatches = new Regex(Regex.Escape("ALTER TABLE " + schemaAndTableName))
                            .Matches(outputSqlString).Count;

                        System.Console.WriteLine(outputSqlString);

                        createSchemaMatches.ShouldBe(1);
                        createTableMatches.ShouldBe(1);
                        alterTableMatches.ShouldBe(2);
                        createIndexMatches.ShouldBe(1);
                    }
                    finally
                    {
                        CleanupTestSqlServerDatabase(serviceProvider, processor);
                    }
                },
                false,
                IntegrationTestOptions.SqlServer2008);
        }

        /// <summary>
        /// Defines the test method MigrateUpWithTaggedMigrationsShouldNotApplyAnyMigrationsIfNoTagsParameterIsPassedIntoTheRunner.
        /// </summary>
        [Test]
        [Category("Firebird")]
        [Category("MySql")]
        [Category("SQLite")]
        [Category("Postgres")]
        [Category("SqlServer2005")]
        [Category("SqlServer2008")]
        [Category("SqlServer2012")]
        [Category("SqlServer2014")]
        [Category("SqlServer2016")]
        public void MigrateUpWithTaggedMigrationsShouldNotApplyAnyMigrationsIfNoTagsParameterIsPassedIntoTheRunner()
        {
            ExecuteWithSupportedProcessors(
                services => services.WithMigrationsIn(typeof(TenantATable).Namespace),
                (serviceProvider, processor) =>
                {
                    var runner = (MigrationRunner)serviceProvider.GetRequiredService<IMigrationRunner>();

                    try
                    {
                        runner.MigrateUp(false);

                        processor.TableExists(null, "TenantATable").ShouldBeFalse();
                        processor.TableExists(null, "NormalTable").ShouldBeTrue();
                        processor.TableExists(null, "TenantBTable").ShouldBeFalse();
                        processor.TableExists(null, "TenantAandBTable").ShouldBeFalse();
                    }
                    finally
                    {
                        runner.RollbackToVersion(0);
                    }
                },
                true,
                typeof(SqlAnywhere16Processor));
        }

        /// <summary>
        /// Defines the test method ValidateVersionOrderShouldDoNothingIfUnappliedMigrationVersionIsGreaterThanLatestAppliedMigration.
        /// </summary>
        [Test]
        [Category("Firebird")]
        [Category("SqlServer2005")]
        [Category("SqlServer2008")]
        [Category("SqlServer2012")]
        [Category("SqlServer2014")]
        [Category("SqlServer2016")]
        public void ValidateVersionOrderShouldDoNothingIfUnappliedMigrationVersionIsGreaterThanLatestAppliedMigration()
        {
            // Using SqlServer instead of SQLite as versions not deleted from VersionInfo table when using Sqlite.
            var excludedProcessors = new[] { typeof(SQLiteProcessor), typeof(MySqlProcessor), typeof(PostgresProcessor), typeof(SqlAnywhere16Processor) };

            var namespacePass2 = typeof(Migrations.Interleaved.Pass2.User).Namespace;
            var namespacePass3 = typeof(Migrations.Interleaved.Pass3.User).Namespace;

            try
            {
                ExecuteWithSupportedProcessors(
                    services => services.WithMigrationsIn(namespacePass2),
                    (serviceProvider, _) =>
                    {
                        var runner = serviceProvider
                            .GetRequiredService<IMigrationRunner>();

                        runner.MigrateUp(3);
                    },
                    tryRollback: false,
                    excludedProcessors);

                ExecuteWithSupportedProcessors(
                    services => services.WithMigrationsIn(namespacePass3),
                    (serviceProvider, _) =>
                    {
                        var runner = serviceProvider
                            .GetRequiredService<IMigrationRunner>();

                        Assert.DoesNotThrow(runner.ValidateVersionOrder);
                    },
                    tryRollback: false,
                    excludedProcessors);
            }
            finally
            {
                ExecuteWithSupportedProcessors(
                    services => services.WithMigrationsIn(namespacePass3),
                    (serviceProvider, _) =>
                    {
                        var runner = serviceProvider
                            .GetRequiredService<IMigrationRunner>();
                        runner.RollbackToVersion(0);
                    },
                    tryRollback: true,
                    excludedProcessors);
            }
        }

        /// <summary>
        /// Defines the test method ValidateVersionOrderShouldThrowExceptionIfUnappliedMigrationVersionIsLessThanLatestAppliedMigration.
        /// </summary>
        [Test]
        [Category("Firebird")]
        [Category("Postgres")]
        [Category("SqlServer2005")]
        [Category("SqlServer2008")]
        [Category("SqlServer2012")]
        [Category("SqlServer2014")]
        [Category("SqlServer2016")]
        public void ValidateVersionOrderShouldThrowExceptionIfUnappliedMigrationVersionIsLessThanLatestAppliedMigration()
        {
            // Using SqlServer instead of SQLite as versions not deleted from VersionInfo table when using Sqlite.
            var excludedProcessors = new[] { typeof(MySqlProcessor), typeof(SQLiteProcessor), typeof(SqlAnywhere16Processor) };

            var namespacePass2 = typeof(Migrations.Interleaved.Pass2.User).Namespace;
            var namespacePass3 = typeof(Migrations.Interleaved.Pass3.User).Namespace;

            VersionOrderInvalidException caughtException = null;

            try
            {
                ExecuteWithSupportedProcessors(
                    services => services.WithMigrationsIn(namespacePass2),
                    (serviceProvider, _) =>
                    {
                        var migrationRunner = serviceProvider
                            .GetRequiredService<IMigrationRunner>();

                        migrationRunner.MigrateUp();
                    },
                    tryRollback: false,
                    excludedProcessors);

                ExecuteWithSupportedProcessors(
                    services => services.WithMigrationsIn(namespacePass3),
                    (serviceProvider, _) =>
                    {
                        var migrationRunner = serviceProvider
                            .GetRequiredService<IMigrationRunner>();

                        migrationRunner.ValidateVersionOrder();
                    },
                    tryRollback: false,
                    excludedProcessors);
            }
            catch (VersionOrderInvalidException ex)
            {
                caughtException = ex;
            }
            finally
            {
                ExecuteWithSupportedProcessors(
                    services => services.WithMigrationsIn(namespacePass3),
                    (serviceProvider, _) =>
                    {
                        var migrationRunner = serviceProvider
                            .GetRequiredService<IMigrationRunner>();

                        migrationRunner.RollbackToVersion(0);
                    },
                    tryRollback: true,
                    excludedProcessors);
            }

            caughtException.ShouldNotBeNull();

            caughtException.InvalidMigrations.Count().ShouldBe(1);
            var keyValuePair = caughtException.InvalidMigrations.First();
            keyValuePair.Key.ShouldBe(200909060935);
            keyValuePair.Value.Migration.ShouldBeOfType<Migrations.Interleaved.Pass3.UserEmail>();
        }

        /// <summary>
        /// Defines the test method CanCreateSequence.
        /// </summary>
        [Test]
        [Category("SqlServer2012")]
        public void CanCreateSequence()
        {
            ExecuteWithProcessor<SqlServer2012Processor>(
                services => services.WithMigrationsIn(RootNamespace),
                (serviceProvider, processor) =>
                {
                    var runner = (MigrationRunner)serviceProvider.GetRequiredService<IMigrationRunner>();

                    runner.Up(new TestCreateSequence());
                    processor.SequenceExists(null, "TestSequence");

                    runner.Down(new TestCreateSequence());
                    processor.SequenceExists(null, "TestSequence").ShouldBeFalse();
                },
                true,
                IntegrationTestOptions.SqlServer2012);
        }

        /// <summary>
        /// Defines the test method CanCreateSequenceWithSchema.
        /// </summary>
        [Test]
        [Category("Postgres")]
        [Category("SqlServer2012")]
        public void CanCreateSequenceWithSchema()
        {
            if (!IntegrationTestOptions.SqlServer2012.IsEnabled && !IntegrationTestOptions.Postgres.IsEnabled)
            {
                Assert.Ignore("No processor found for the given action.");
            }

            void CreateAndDropSequence(IServiceProvider serviceProvider, ProcessorBase processor)
            {
                var runner = (MigrationRunner)serviceProvider.GetRequiredService<IMigrationRunner>();

                runner.Up(new TestCreateSchema());
                runner.Up(new TestCreateSequenceWithSchema());
                processor.SequenceExists("TestSchema", "TestSequence").ShouldBeTrue();

                runner.Down(new TestCreateSequenceWithSchema());
                runner.Down(new TestCreateSchema());
                processor.SequenceExists("TestSchema", "TestSequence").ShouldBeFalse();
            }

            if (IntegrationTestOptions.SqlServer2012.IsEnabled)
            {
                ExecuteWithProcessor<SqlServer2012Processor>(
                    services => services.WithMigrationsIn(RootNamespace),
                    (Action<IServiceProvider, ProcessorBase>)CreateAndDropSequence,
                    true,
                    IntegrationTestOptions.SqlServer2012);
            }

            if (IntegrationTestOptions.Postgres.IsEnabled)
            {
                ExecuteWithProcessor<PostgresProcessor>(
                    services => services.WithMigrationsIn(RootNamespace),
                    (Action<IServiceProvider, ProcessorBase>)CreateAndDropSequence,
                    true,
                    IntegrationTestOptions.Postgres);
            }
        }

        /// <summary>
        /// Defines the test method CanAlterColumnWithSchema.
        /// </summary>
        [Test]
        [Category("MySql")]
        [Category("Postgres")]
        [Category("SqlServer2005")]
        [Category("SqlServer2008")]
        [Category("SqlServer2012")]
        [Category("SqlServer2014")]
        [Category("SqlServer2016")]
        public void CanAlterColumnWithSchema()
        {
            ExecuteWithSupportedProcessors(
                services => services.WithMigrationsIn(RootNamespace),
                (serviceProvider, processor) =>
                {
                    var runner = (MigrationRunner)serviceProvider.GetRequiredService<IMigrationRunner>();

                    runner.Up(new TestCreateSchema());

                    runner.Up(new TestCreateAndDropTableMigrationWithSchema());
                    processor.ColumnExists("TestSchema", "TestTable2", "Name2").ShouldBeTrue();
                    processor.DefaultValueExists("TestSchema", "TestTable", "Name", "Anonymous").ShouldBeTrue();

                    runner.Up(new TestAlterColumnWithSchema());
                    processor.ColumnExists("TestSchema", "TestTable2", "Name2").ShouldBeTrue();

                    runner.Down(new TestAlterColumnWithSchema());
                    processor.ColumnExists("TestSchema", "TestTable2", "Name2").ShouldBeTrue();

                    runner.Down(new TestCreateAndDropTableMigrationWithSchema());

                    runner.Down(new TestCreateSchema());
                },
                true,
                typeof(SQLiteProcessor),
                typeof(FirebirdProcessor),
                typeof(SqlAnywhereProcessor));
        }

        /// <summary>
        /// Defines the test method CanAlterTableWithSchema.
        /// </summary>
        [Test]
        [Category("MySql")]
        [Category("Postgres")]
        [Category("SqlServer2005")]
        [Category("SqlServer2008")]
        [Category("SqlServer2012")]
        [Category("SqlServer2014")]
        [Category("SqlServer2016")]
        [Category("SqlAnywhere16")]
        public void CanAlterTableWithSchema()
        {
            ExecuteWithSupportedProcessors(
                services => services.WithMigrationsIn(RootNamespace),
                (serviceProvider, processor) =>
                {
                    var runner = (MigrationRunner)serviceProvider.GetRequiredService<IMigrationRunner>();

                    runner.Up(new TestCreateSchema());

                    runner.Up(new TestCreateAndDropTableMigrationWithSchema());
                    processor.ColumnExists("TestSchema", "TestTable2", "NewColumn").ShouldBeFalse();

                    runner.Up(new TestAlterTableWithSchema());
                    processor.ColumnExists("TestSchema", "TestTable2", "NewColumn").ShouldBeTrue();

                    runner.Down(new TestAlterTableWithSchema());
                    processor.ColumnExists("TestSchema", "TestTable2", "NewColumn").ShouldBeFalse();

                    runner.Down(new TestCreateAndDropTableMigrationWithSchema());

                    runner.Down(new TestCreateSchema());
                },
                true,
                typeof(SQLiteProcessor),
                typeof(FirebirdProcessor));
        }

        /// <summary>
        /// Defines the test method CanAlterTablesSchema.
        /// </summary>
        [Test]
        [Category("MySql")]
        [Category("Postgres")]
        [Category("SqlServer2005")]
        [Category("SqlServer2008")]
        [Category("SqlServer2012")]
        [Category("SqlServer2014")]
        [Category("SqlServer2016")]
        public void CanAlterTablesSchema()
        {
            ExecuteWithSupportedProcessors(
                services => services.WithMigrationsIn(RootNamespace),
                (serviceProvider, processor) =>
                {
                    var runner = (MigrationRunner)serviceProvider.GetRequiredService<IMigrationRunner>();

                    runner.Up(new TestCreateSchema());

                    runner.Up(new TestCreateAndDropTableMigrationWithSchema());
                    processor.TableExists("TestSchema", "TestTable").ShouldBeTrue();

                    runner.Up(new TestAlterSchema());
                    processor.TableExists("NewSchema", "TestTable").ShouldBeTrue();

                    runner.Down(new TestAlterSchema());
                    processor.TableExists("TestSchema", "TestTable").ShouldBeTrue();

                    runner.Down(new TestCreateAndDropTableMigrationWithSchema());

                    runner.Down(new TestCreateSchema());
                },
                true,
                typeof(SQLiteProcessor),
                typeof(FirebirdProcessor),
                typeof(SqlAnywhereProcessor));
        }

        /// <summary>
        /// Defines the test method CanCreateUniqueConstraint.
        /// </summary>
        [Test]
        [Category("Firebird")]
        [Category("MySql")]
        [Category("Postgres")]
        [Category("SqlServer2005")]
        [Category("SqlServer2008")]
        [Category("SqlServer2012")]
        [Category("SqlServer2014")]
        [Category("SqlServer2016")]
        public void CanCreateUniqueConstraint()
        {
            ExecuteWithSupportedProcessors(
                services => services.WithMigrationsIn(RootNamespace),
                (serviceProvider, processor) =>
                {
                    var runner = (MigrationRunner)serviceProvider.GetRequiredService<IMigrationRunner>();

                    runner.Up(new TestCreateAndDropTableMigration());
                    processor.ConstraintExists(null, "TestTable2", "TestUnique").ShouldBeFalse();

                    runner.Up(new TestCreateUniqueConstraint());
                    processor.ConstraintExists(null, "TestTable2", "TestUnique").ShouldBeTrue();

                    runner.Down(new TestCreateUniqueConstraint());
                    processor.ConstraintExists(null, "TestTable2", "TestUnique").ShouldBeFalse();

                    runner.Down(new TestCreateAndDropTableMigration());
                }, true, typeof(SQLiteProcessor), typeof(SqlAnywhereProcessor));
        }

        /// <summary>
        /// Defines the test method CanCreateUniqueConstraintWithSchema.
        /// </summary>
        [Test]
        [Category("MySql")]
        [Category("Postgres")]
        [Category("SqlServer2005")]
        [Category("SqlServer2008")]
        [Category("SqlServer2012")]
        [Category("SqlServer2014")]
        [Category("SqlServer2016")]
        public void CanCreateUniqueConstraintWithSchema()
        {
            ExecuteWithSupportedProcessors(
                services => services.WithMigrationsIn(RootNamespace),
                (serviceProvider, processor) =>
                {
                    var runner = (MigrationRunner)serviceProvider.GetRequiredService<IMigrationRunner>();

                    runner.Up(new TestCreateSchema());

                    runner.Up(new TestCreateAndDropTableMigrationWithSchema());
                    processor.ConstraintExists("TestSchema", "TestTable2", "TestUnique").ShouldBeFalse();

                    runner.Up(new TestCreateUniqueConstraintWithSchema());
                    processor.ConstraintExists("TestSchema", "TestTable2", "TestUnique").ShouldBeTrue();

                    runner.Down(new TestCreateUniqueConstraintWithSchema());
                    processor.ConstraintExists("TestSchema", "TestTable2", "TestUnique").ShouldBeFalse();

                    runner.Down(new TestCreateAndDropTableMigrationWithSchema());

                    runner.Down(new TestCreateSchema());
                },
                true,
                typeof(SQLiteProcessor),
                typeof(FirebirdProcessor),
                typeof(SqlAnywhereProcessor));
        }

        /// <summary>
        /// Defines the test method CanInsertData.
        /// </summary>
        [Test]
        [Category("Firebird")]
        [Category("MySql")]
        [Category("Postgres")]
        [Category("SqlServer2005")]
        [Category("SqlServer2008")]
        [Category("SqlServer2012")]
        [Category("SqlServer2014")]
        [Category("SqlServer2016")]
        [Category("SqlAnywhere16")]
        public void CanInsertData()
        {
            ExecuteWithSupportedProcessors(
                services => services.WithMigrationsIn(RootNamespace),
                (serviceProvider, processor) =>
                {
                    var runner = (MigrationRunner)serviceProvider.GetRequiredService<IMigrationRunner>();

                    runner.Up(new TestCreateAndDropTableMigration());
                    DataSet ds = processor.ReadTableData(null, "TestTable");
                    ds.Tables[0].Rows.Count.ShouldBe(1);
                    ds.Tables[0].Rows[0][1].ShouldBe("Test");

                    runner.Down(new TestCreateAndDropTableMigration());
                },
                true,
                typeof(SQLiteProcessor));
        }

        /// <summary>
        /// Defines the test method CanInsertDataWithSchema.
        /// </summary>
        [Test]
        [Category("MySql")]
        [Category("Postgres")]
        [Category("SqlServer2005")]
        [Category("SqlServer2008")]
        [Category("SqlServer2012")]
        [Category("SqlServer2014")]
        [Category("SqlServer2016")]
        [Category("SqlAnywhere16")]
        public void CanInsertDataWithSchema()
        {
            ExecuteWithSupportedProcessors(
                services => services.WithMigrationsIn(RootNamespace),
                (serviceProvider, processor) =>
                {
                    var runner = (MigrationRunner)serviceProvider.GetRequiredService<IMigrationRunner>();

                    runner.Up(new TestCreateSchema());

                    runner.Up(new TestCreateAndDropTableMigrationWithSchema());
                    DataSet ds = processor.ReadTableData("TestSchema", "TestTable");
                    ds.Tables[0].Rows.Count.ShouldBe(1);
                    ds.Tables[0].Rows[0][1].ShouldBe("Test");

                    runner.Down(new TestCreateAndDropTableMigrationWithSchema());

                    runner.Down(new TestCreateSchema());
                },
                true,
                typeof(SQLiteProcessor),
                typeof(FirebirdProcessor));
        }

        /// <summary>
        /// Defines the test method CanUpdateData.
        /// </summary>
        [Test]
        [Category("MySql")]
        [Category("Postgres")]
        [Category("SqlServer2005")]
        [Category("SqlServer2008")]
        [Category("SqlServer2012")]
        [Category("SqlServer2014")]
        [Category("SqlServer2016")]
        [Category("SqlAnywhere16")]
        public void CanUpdateData()
        {
            ExecuteWithSupportedProcessors(
                services => services.WithMigrationsIn(RootNamespace),
                (serviceProvider, processor) =>
                {
                    var runner = (MigrationRunner)serviceProvider.GetRequiredService<IMigrationRunner>();

                    runner.Up(new TestCreateSchema());

                    runner.Up(new TestCreateAndDropTableMigrationWithSchema());

                    runner.Up(new TestUpdateData());
                    DataSet upDs = processor.ReadTableData("TestSchema", "TestTable");
                    upDs.Tables[0].Rows.Count.ShouldBe(1);
                    upDs.Tables[0].Rows[0][1].ShouldBe("Updated");

                    runner.Down(new TestUpdateData());
                    DataSet downDs = processor.ReadTableData("TestSchema", "TestTable");
                    downDs.Tables[0].Rows.Count.ShouldBe(1);
                    downDs.Tables[0].Rows[0][1].ShouldBe("Test");

                    runner.Down(new TestCreateAndDropTableMigrationWithSchema());

                    runner.Down(new TestCreateSchema());
                },
                true,
                typeof(SQLiteProcessor),
                typeof(FirebirdProcessor));
        }

        /// <summary>
        /// Defines the test method CanDeleteData.
        /// </summary>
        [Test]
        [Category("Firebird")]
        [Category("MySql")]
        [Category("Postgres")]
        [Category("SqlServer2005")]
        [Category("SqlServer2008")]
        [Category("SqlServer2012")]
        [Category("SqlServer2014")]
        [Category("SqlServer2016")]
        [Category("SqlAnywhere16")]
        public void CanDeleteData()
        {
            ExecuteWithSupportedProcessors(
                services => services.WithMigrationsIn(RootNamespace),
                (serviceProvider, processor) =>
                {
                    var runner = (MigrationRunner)serviceProvider.GetRequiredService<IMigrationRunner>();

                    runner.Up(new TestCreateAndDropTableMigration());

                    runner.Up(new TestDeleteData());
                    DataSet upDs = processor.ReadTableData(null, "TestTable");
                    upDs.Tables[0].Rows.Count.ShouldBe(0);

                    runner.Down(new TestDeleteData());
                    DataSet downDs = processor.ReadTableData(null, "TestTable");
                    downDs.Tables[0].Rows.Count.ShouldBe(1);
                    downDs.Tables[0].Rows[0][1].ShouldBe("Test");

                    runner.Down(new TestCreateAndDropTableMigration());
                },
                true,
                typeof(SQLiteProcessor));
        }

        /// <summary>
        /// Defines the test method CanDeleteDataWithSchema.
        /// </summary>
        [Test]
        [Category("MySql")]
        [Category("Postgres")]
        [Category("SqlServer2005")]
        [Category("SqlServer2008")]
        [Category("SqlServer2012")]
        [Category("SqlServer2014")]
        [Category("SqlServer2016")]
        [Category("SqlAnywhere16")]
        public void CanDeleteDataWithSchema()
        {
            ExecuteWithSupportedProcessors(
                services => services.WithMigrationsIn(RootNamespace),
                (serviceProvider, processor) =>
                {
                    var runner = (MigrationRunner)serviceProvider.GetRequiredService<IMigrationRunner>();

                    runner.Up(new TestCreateSchema());

                    runner.Up(new TestCreateAndDropTableMigrationWithSchema());

                    runner.Up(new TestDeleteDataWithSchema());
                    DataSet upDs = processor.ReadTableData("TestSchema", "TestTable");
                    upDs.Tables[0].Rows.Count.ShouldBe(0);

                    runner.Down(new TestDeleteDataWithSchema());
                    DataSet downDs = processor.ReadTableData("TestSchema", "TestTable");
                    downDs.Tables[0].Rows.Count.ShouldBe(1);
                    downDs.Tables[0].Rows[0][1].ShouldBe("Test");

                    runner.Down(new TestCreateAndDropTableMigrationWithSchema());

                    runner.Down(new TestCreateSchema());
                },
                true,
                typeof(SQLiteProcessor),
                typeof(FirebirdProcessor));
        }

        /// <summary>
        /// Defines the test method CanReverseCreateIndex.
        /// </summary>
        [Test]
        [Category("Firebird")]
        [Category("MySql")]
        [Category("Postgres")]
        [Category("SqlServer2005")]
        [Category("SqlServer2008")]
        [Category("SqlServer2012")]
        [Category("SqlServer2014")]
        [Category("SqlServer2016")]
        [Category("SqlAnywhere16")]
        public void CanReverseCreateIndex()
        {
            ExecuteWithSupportedProcessors(
                services => services.WithMigrationsIn(RootNamespace),
                (serviceProvider, processor) =>
                {
                    var runner = (MigrationRunner)serviceProvider.GetRequiredService<IMigrationRunner>();

                    runner.Up(new TestCreateSchema());

                    runner.Up(new TestCreateAndDropTableMigrationWithSchema());

                    runner.Up(new TestCreateIndexWithReversing());
                    processor.IndexExists("TestSchema", "TestTable2", "IX_TestTable2_Name2").ShouldBeTrue();

                    runner.Down(new TestCreateIndexWithReversing());
                    processor.IndexExists("TestSchema", "TestTable2", "IX_TestTable2_Name2").ShouldBeFalse();

                    runner.Down(new TestCreateAndDropTableMigrationWithSchema());

                    runner.Down(new TestCreateSchema());
                },
                true,
                typeof(SQLiteProcessor));
        }

        /// <summary>
        /// Defines the test method CanReverseCreateUniqueConstraint.
        /// </summary>
        [Test]
        [Category("Firebird")]
        [Category("MySql")]
        [Category("Postgres")]
        [Category("SqlServer2005")]
        [Category("SqlServer2008")]
        [Category("SqlServer2012")]
        [Category("SqlServer2014")]
        [Category("SqlServer2016")]
        public void CanReverseCreateUniqueConstraint()
        {
            ExecuteWithSupportedProcessors(
                services => services.WithMigrationsIn(RootNamespace),
                (serviceProvider, processor) =>
                {
                    var runner = (MigrationRunner)serviceProvider.GetRequiredService<IMigrationRunner>();

                    runner.Up(new TestCreateAndDropTableMigration());

                    runner.Up(new TestCreateUniqueConstraintWithReversing());
                    processor.ConstraintExists(null, "TestTable2", "TestUnique").ShouldBeTrue();

                    runner.Down(new TestCreateUniqueConstraintWithReversing());
                    processor.ConstraintExists(null, "TestTable2", "TestUnique").ShouldBeFalse();

                    runner.Down(new TestCreateAndDropTableMigration());
                },
                true,
                typeof(SQLiteProcessor),
                typeof(SqlAnywhereProcessor));
        }

        /// <summary>
        /// Defines the test method CanReverseCreateUniqueConstraintWithSchema.
        /// </summary>
        [Test]
        [Category("Firebird")]
        [Category("MySql")]
        [Category("Postgres")]
        [Category("SqlServer2005")]
        [Category("SqlServer2008")]
        [Category("SqlServer2012")]
        [Category("SqlServer2014")]
        [Category("SqlServer2016")]
        public void CanReverseCreateUniqueConstraintWithSchema()
        {
            ExecuteWithSupportedProcessors(
                services => services.WithMigrationsIn(RootNamespace),
                (serviceProvider, processor) =>
                {
                    var runner = (MigrationRunner)serviceProvider.GetRequiredService<IMigrationRunner>();

                    runner.Up(new TestCreateSchema());

                    runner.Up(new TestCreateAndDropTableMigrationWithSchema());

                    runner.Up(new TestCreateUniqueConstraintWithSchemaWithReversing());
                    processor.ConstraintExists("TestSchema", "TestTable2", "TestUnique").ShouldBeTrue();

                    runner.Down(new TestCreateUniqueConstraintWithSchemaWithReversing());
                    processor.ConstraintExists("TestSchema", "TestTable2", "TestUnique").ShouldBeFalse();

                    runner.Down(new TestCreateAndDropTableMigrationWithSchema());

                    runner.Down(new TestCreateSchema());
                },
                true,
                typeof(SQLiteProcessor),
                typeof(SqlAnywhereProcessor));
        }

        /// <summary>
        /// Defines the test method CanExecuteSql.
        /// </summary>
        [Test]
        [Category("MySql")]
        [Category("SQLite")]
        [Category("Postgres")]
        [Category("SqlServer2005")]
        [Category("SqlServer2008")]
        [Category("SqlServer2012")]
        [Category("SqlServer2014")]
        [Category("SqlServer2016")]
        [Category("SqlAnywhere16")]
        public void CanExecuteSql()
        {
            ExecuteWithSupportedProcessors(
                services => services.WithMigrationsIn(RootNamespace),
                (serviceProvider, _) =>
                {
                    var runner = (MigrationRunner)serviceProvider.GetRequiredService<IMigrationRunner>();

                    runner.Up(new TestExecuteSql());
                    runner.Down(new TestExecuteSql());
                }, true,
                typeof(FirebirdProcessor));
        }

        /// <summary>
        /// Cleanups the test SQL server database.
        /// </summary>
        /// <typeparam name="TProcessor">The type of the t processor.</typeparam>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="origProcessor">The original processor.</param>
        private void CleanupTestSqlServerDatabase<TProcessor>(IServiceProvider serviceProvider, TProcessor origProcessor)
            where TProcessor : SqlServerProcessor
        {
            if (origProcessor.WasCommitted)
            {
                origProcessor.Connection.Close();

                using (var scope = serviceProvider.CreateScope())
                {
                    var cleanupRunner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                    cleanupRunner.RollbackToVersion(0);
                }
            }
            else
            {
                origProcessor.RollbackTransaction();
            }
        }
    }

    /// <summary>
    /// Class TestForeignKeyNamingConvention.
    /// Implements the <see cref="FluentMigrator.Migration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Migration" />
    internal class TestForeignKeyNamingConvention : Migration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            Create.Table("Users")
                .WithColumn("UserId").AsInt32().Identity().PrimaryKey()
                .WithColumn("GroupId").AsInt32().NotNullable()
                .WithColumn("UserName").AsString(32).NotNullable()
                .WithColumn("Password").AsString(32).NotNullable();

            Create.Table("Groups")
                .WithColumn("GroupId").AsInt32().Identity().PrimaryKey()
                .WithColumn("Name").AsString(32).NotNullable();

            Create.ForeignKey().FromTable("Users").ForeignColumn("GroupId").ToTable("Groups").PrimaryColumn("GroupId");
        }

        /// <summary>
        /// Collects the DOWN migration expressions
        /// </summary>
        public override void Down()
        {
            Delete.Table("Users");
            Delete.Table("Groups");
        }
    }

    /// <summary>
    /// Class TestUniqueConstraintNamingConvention.
    /// Implements the <see cref="FluentMigrator.Migration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Migration" />
    internal class TestUniqueConstraintNamingConvention : Migration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            Create.Table("Users")
                .WithColumn("UserId").AsInt32().Identity().PrimaryKey()
                .WithColumn("GroupId").AsInt32().NotNullable()
                .WithColumn("AccountId").AsInt32().NotNullable()
                .WithColumn("UserName").AsString(32).NotNullable()
                .WithColumn("Password").AsString(32).NotNullable();

            Create.UniqueConstraint().OnTable("Users").Column("GroupId");
            Create.UniqueConstraint().OnTable("Users").Column("AccountId");
        }

        /// <summary>
        /// Collects the DOWN migration expressions
        /// </summary>
        public override void Down()
        {
            Delete.UniqueConstraint("UC_Users_GroupId").FromTable("Users");
            Delete.UniqueConstraint().FromTable("Users").Column("AccountId");
            Delete.Table("Users");
        }
    }

    /// <summary>
    /// Class TestIndexNamingConvention.
    /// Implements the <see cref="FluentMigrator.Migration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Migration" />
    internal class TestIndexNamingConvention : Migration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            Create.Table("Users")
                .WithColumn("UserId").AsInt32().Identity().PrimaryKey()
                .WithColumn("GroupId").AsInt32().NotNullable()
                .WithColumn("UserName").AsString(32).NotNullable()
                .WithColumn("Password").AsString(32).NotNullable();

            Create.Index().OnTable("Users").OnColumn("GroupId").Ascending();
        }

        /// <summary>
        /// Collects the DOWN migration expressions
        /// </summary>
        public override void Down()
        {
            Delete.Index("IX_Users_GroupId").OnTable("Users").OnColumn("GroupId");
            Delete.Table("Users");
        }
    }

    /// <summary>
    /// Class TestForeignKeySilentFailure.
    /// Implements the <see cref="FluentMigrator.Migration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Migration" />
    internal class TestForeignKeySilentFailure : Migration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            Create.Table("Users")
                .WithColumn("UserId").AsInt32().Identity().PrimaryKey()
                .WithColumn("GroupId").AsInt32().NotNullable()
                .WithColumn("UserName").AsString(32).NotNullable()
                .WithColumn("Password").AsString(32).NotNullable();

            Create.Table("Groups")
                .WithColumn("GroupId").AsInt32().Identity().PrimaryKey()
                .WithColumn("Name").AsString(32).NotNullable();

            Create.ForeignKey("FK_Foo").FromTable("Users").ForeignColumn("GroupId").ToTable("Groups").PrimaryColumn("GroupId");
        }

        /// <summary>
        /// Collects the DOWN migration expressions
        /// </summary>
        public override void Down()
        {
            Delete.ForeignKey("FK_Foo").OnTable("Users");
            Delete.Table("Users");
            Delete.Table("Groups");
        }
    }

    /// <summary>
    /// Class TestCreateAndDropTableMigration.
    /// Implements the <see cref="FluentMigrator.Migration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Migration" />
    internal class TestCreateAndDropTableMigration : Migration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            Create.Table("TestTable")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("Name").AsString(255).NotNullable().WithDefaultValue("Anonymous");

            Create.Table("TestTable2")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("Name").AsString(255).Nullable()
                .WithColumn("TestTableId").AsInt32().NotNullable();

            Create.Index("ix_Name").OnTable("TestTable2").OnColumn("Name").Ascending()
                .WithOptions().NonClustered();

            Create.Column("Name2").OnTable("TestTable2").AsBoolean().Nullable();

            Create.ForeignKey("fk_TestTable2_TestTableId_TestTable_Id")
                .FromTable("TestTable2").ForeignColumn("TestTableId")
                .ToTable("TestTable").PrimaryColumn("Id");

            Insert.IntoTable("TestTable").Row(new { Name = "Test" });
        }

        /// <summary>
        /// Collects the DOWN migration expressions
        /// </summary>
        public override void Down()
        {
            Delete.Table("TestTable2");
            Delete.Table("TestTable");
        }
    }

    /// <summary>
    /// Class TestRenameTableMigration.
    /// Implements the <see cref="FluentMigrator.AutoReversingMigration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.AutoReversingMigration" />
    internal class TestRenameTableMigration : AutoReversingMigration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            Rename.Table("TestTable2").To("TestTable'3");
        }
    }

    /// <summary>
    /// Class TestRenameColumnMigration.
    /// Implements the <see cref="FluentMigrator.AutoReversingMigration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.AutoReversingMigration" />
    internal class TestRenameColumnMigration : AutoReversingMigration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            Rename.Column("Name").OnTable("TestTable2").To("Name'3");
        }
    }

    /// <summary>
    /// Class TestCreateAndDropIndexMigration.
    /// Implements the <see cref="FluentMigrator.Migration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Migration" />
    internal class TestCreateAndDropIndexMigration : Migration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            Create.Index("IX_TestTable_Name").OnTable("TestTable").OnColumn("Name");
        }

        /// <summary>
        /// Collects the DOWN migration expressions
        /// </summary>
        public override void Down()
        {
            Delete.Index("IX_TestTable_Name").OnTable("TestTable");
        }
    }

    /// <summary>
    /// Class TestForeignKeyNamingConventionWithSchema.
    /// Implements the <see cref="FluentMigrator.Migration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Migration" />
    internal class TestForeignKeyNamingConventionWithSchema : Migration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            var createSchemaExpr = Create.Schema("TestSchema");
            IfDatabase(t => t.StartsWith("SqlAnywhere"))
                .Delegate(() => createSchemaExpr.Password("TestSchemaPassword"));
            Create.Table("Users")
                .InSchema("TestSchema")
                .WithColumn("UserId").AsInt32().Identity().PrimaryKey()
                .WithColumn("GroupId").AsInt32().NotNullable()
                .WithColumn("UserName").AsString(32).NotNullable()
                .WithColumn("Password").AsString(32).NotNullable();

            Create.Table("Groups")
                .InSchema("TestSchema")
                .WithColumn("GroupId").AsInt32().Identity().PrimaryKey()
                .WithColumn("Name").AsString(32).NotNullable();

            Create.ForeignKey().FromTable("Users").InSchema("TestSchema").ForeignColumn("GroupId").ToTable("Groups").InSchema("TestSchema").PrimaryColumn("GroupId");
        }

        /// <summary>
        /// Collects the DOWN migration expressions
        /// </summary>
        public override void Down()
        {
            Delete.Table("Users").InSchema("TestSchema");
            Delete.Table("Groups").InSchema("TestSchema");
            Delete.Schema("TestSchema");
        }
    }

    /// <summary>
    /// Class TestIndexNamingConventionWithSchema.
    /// Implements the <see cref="FluentMigrator.Migration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Migration" />
    internal class TestIndexNamingConventionWithSchema : Migration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            var createSchemaExpr = Create.Schema("TestSchema");
            IfDatabase(t => t.StartsWith("SqlAnywhere"))
                .Delegate(() => createSchemaExpr.Password("TestSchemaPassword"));

            Create.Table("Users")
                .InSchema("TestSchema")
                .WithColumn("UserId").AsInt32().Identity().PrimaryKey()
                .WithColumn("GroupId").AsInt32().NotNullable()
                .WithColumn("UserName").AsString(32).NotNullable()
                .WithColumn("Password").AsString(32).NotNullable();

            Create.Index().OnTable("Users").InSchema("TestSchema").OnColumn("GroupId").Ascending();
        }

        /// <summary>
        /// Collects the DOWN migration expressions
        /// </summary>
        public override void Down()
        {
            Delete.Index("IX_Users_GroupId").OnTable("Users").InSchema("TestSchema").OnColumn("GroupId");
            Delete.Table("Users").InSchema("TestSchema");
            Delete.Schema("TestSchema");
        }
    }

    /// <summary>
    /// Class TestCreateAndDropTableMigrationWithSchema.
    /// Implements the <see cref="FluentMigrator.Migration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Migration" />
    internal class TestCreateAndDropTableMigrationWithSchema : Migration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            Create.Table("TestTable")
                .InSchema("TestSchema")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("Name").AsString(255).NotNullable().WithDefaultValue("Anonymous");

            Create.Table("TestTable2")
                .InSchema("TestSchema")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("Name").AsString(255).Nullable()
                .WithColumn("TestTableId").AsInt32().NotNullable();

            Create.Index("ix_Name").OnTable("TestTable2").InSchema("TestSchema").OnColumn("Name").Ascending()
                .WithOptions().NonClustered();

            Create.Column("Name2").OnTable("TestTable2").InSchema("TestSchema").AsString(10).Nullable();

            Create.ForeignKey("fk_TestTable2_TestTableId_TestTable_Id")
                .FromTable("TestTable2").InSchema("TestSchema").ForeignColumn("TestTableId")
                .ToTable("TestTable").InSchema("TestSchema").PrimaryColumn("Id");

            Insert.IntoTable("TestTable").InSchema("TestSchema").Row(new { Name = "Test" });
        }

        /// <summary>
        /// Collects the DOWN migration expressions
        /// </summary>
        public override void Down()
        {
            Delete.Table("TestTable2").InSchema("TestSchema");
            Delete.Table("TestTable").InSchema("TestSchema");
        }
    }

    /// <summary>
    /// Class TestRenameTableMigrationWithSchema.
    /// Implements the <see cref="FluentMigrator.AutoReversingMigration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.AutoReversingMigration" />
    internal class TestRenameTableMigrationWithSchema : AutoReversingMigration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            Rename.Table("TestTable2").InSchema("TestSchema").To("TestTable'3");
        }
    }

    /// <summary>
    /// Class TestRenameColumnMigrationWithSchema.
    /// Implements the <see cref="FluentMigrator.AutoReversingMigration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.AutoReversingMigration" />
    internal class TestRenameColumnMigrationWithSchema : AutoReversingMigration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            Rename.Column("Name").OnTable("TestTable2").InSchema("TestSchema").To("Name'3");
        }
    }

    /// <summary>
    /// Class TestCreateAndDropIndexMigrationWithSchema.
    /// Implements the <see cref="FluentMigrator.Migration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Migration" />
    internal class TestCreateAndDropIndexMigrationWithSchema : Migration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            Create.Index("IX_TestTable_Name").OnTable("TestTable").InSchema("TestSchema").OnColumn("Name");
        }

        /// <summary>
        /// Collects the DOWN migration expressions
        /// </summary>
        public override void Down()
        {
            Delete.Index("IX_TestTable_Name").OnTable("TestTable").InSchema("TestSchema");
        }
    }

    /// <summary>
    /// Class TestCreateSchema.
    /// Implements the <see cref="FluentMigrator.Migration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Migration" />
    internal class TestCreateSchema : Migration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            var createSchemaExpr = Create.Schema("TestSchema");
            IfDatabase(t => t.StartsWith("SqlAnywhere"))
                .Delegate(() => createSchemaExpr.Password("TestSchemaPassword"));
        }

        /// <summary>
        /// Collects the DOWN migration expressions
        /// </summary>
        public override void Down()
        {
            Delete.Schema("TestSchema");
        }
    }

    /// <summary>
    /// Class TestCreateSequence.
    /// Implements the <see cref="FluentMigrator.Migration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Migration" />
    internal class TestCreateSequence : Migration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            Create.Sequence("TestSequence").StartWith(1).IncrementBy(1).MinValue(0).MaxValue(1000).Cycle().Cache(10);
        }

        /// <summary>
        /// Collects the DOWN migration expressions
        /// </summary>
        public override void Down()
        {
            Delete.Sequence("TestSequence");
        }
    }

    /// <summary>
    /// Class TestCreateSequenceWithSchema.
    /// Implements the <see cref="FluentMigrator.Migration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Migration" />
    internal class TestCreateSequenceWithSchema : Migration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            Create.Sequence("TestSequence").InSchema("TestSchema").StartWith(1).IncrementBy(1).MinValue(0).MaxValue(1000).Cycle().Cache(10);
        }

        /// <summary>
        /// Collects the DOWN migration expressions
        /// </summary>
        public override void Down()
        {
            Delete.Sequence("TestSequence").InSchema("TestSchema");
        }
    }

    /// <summary>
    /// Class TestAlterColumnWithSchema.
    /// Implements the <see cref="FluentMigrator.Migration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Migration" />
    internal class TestAlterColumnWithSchema: Migration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            Alter.Column("Name2").OnTable("TestTable2").InSchema("TestSchema").AsAnsiString(100).Nullable();
        }

        /// <summary>
        /// Collects the DOWN migration expressions
        /// </summary>
        public override void Down()
        {
            Alter.Column("Name2").OnTable("TestTable2").InSchema("TestSchema").AsString(10).Nullable();
        }
    }

    /// <summary>
    /// Class TestAlterTableWithSchema.
    /// Implements the <see cref="FluentMigrator.Migration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Migration" />
    internal class TestAlterTableWithSchema : Migration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            Alter.Table("TestTable2").InSchema("TestSchema").AddColumn("NewColumn").AsInt32().WithDefaultValue(1);
        }

        /// <summary>
        /// Collects the DOWN migration expressions
        /// </summary>
        public override void Down()
        {
            Delete.Column("NewColumn").FromTable("TestTable2").InSchema("TestSchema");
        }
    }

    /// <summary>
    /// Class TestAlterSchema.
    /// Implements the <see cref="FluentMigrator.Migration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Migration" />
    internal class TestAlterSchema : Migration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            var createSchemaExpr = Create.Schema("NewSchema");
            IfDatabase(t => t.StartsWith("SqlAnywhere"))
                .Delegate(() => createSchemaExpr.Password("NewSchemaPassword"));
            Alter.Table("TestTable").InSchema("TestSchema").ToSchema("NewSchema");
        }

        /// <summary>
        /// Collects the DOWN migration expressions
        /// </summary>
        public override void Down()
        {
            Alter.Table("TestTable").InSchema("NewSchema").ToSchema("TestSchema");
            Delete.Schema("NewSchema");
        }
    }

    /// <summary>
    /// Class TestCreateUniqueConstraint.
    /// Implements the <see cref="FluentMigrator.Migration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Migration" />
    internal class TestCreateUniqueConstraint : Migration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            Create.UniqueConstraint("TestUnique").OnTable("TestTable2").Column("Name");
        }

        /// <summary>
        /// Collects the DOWN migration expressions
        /// </summary>
        public override void Down()
        {
            Delete.UniqueConstraint("TestUnique").FromTable("TestTable2");
        }
    }

    /// <summary>
    /// Class TestCreateUniqueConstraintWithSchema.
    /// Implements the <see cref="FluentMigrator.Migration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Migration" />
    internal class TestCreateUniqueConstraintWithSchema : Migration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            Create.UniqueConstraint("TestUnique").OnTable("TestTable2").WithSchema("TestSchema").Column("Name");
        }

        /// <summary>
        /// Collects the DOWN migration expressions
        /// </summary>
        public override void Down()
        {
            Delete.UniqueConstraint("TestUnique").FromTable("TestTable2").InSchema("TestSchema");
        }
    }

    /// <summary>
    /// Class TestUpdateData.
    /// Implements the <see cref="FluentMigrator.Migration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Migration" />
    internal class TestUpdateData : Migration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            Update.Table("TestTable").InSchema("TestSchema").Set(new { Name = "Updated" }).AllRows();
        }

        /// <summary>
        /// Collects the DOWN migration expressions
        /// </summary>
        public override void Down()
        {
            Update.Table("TestTable").InSchema("TestSchema").Set(new { Name = "Test" }).AllRows();
        }
    }

    /// <summary>
    /// Class TestDeleteData.
    /// Implements the <see cref="FluentMigrator.Migration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Migration" />
    internal class TestDeleteData : Migration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            Delete.FromTable("TestTable").Row(new { Name = "Test" });
        }

        /// <summary>
        /// Collects the DOWN migration expressions
        /// </summary>
        public override void Down()
        {
            Insert.IntoTable("TestTable").Row(new { Name = "Test" });
        }
    }

    /// <summary>
    /// Class TestDeleteDataWithSchema.
    /// Implements the <see cref="FluentMigrator.Migration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Migration" />
    internal class TestDeleteDataWithSchema :Migration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            Delete.FromTable("TestTable").InSchema("TestSchema").Row(new { Name = "Test"});
        }

        /// <summary>
        /// Collects the DOWN migration expressions
        /// </summary>
        public override void Down()
        {
            Insert.IntoTable("TestTable").InSchema("TestSchema").Row(new { Name = "Test" });
        }
    }

    /// <summary>
    /// Class TestCreateIndexWithReversing.
    /// Implements the <see cref="FluentMigrator.AutoReversingMigration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.AutoReversingMigration" />
    internal class TestCreateIndexWithReversing : AutoReversingMigration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            Create.Index().OnTable("TestTable2").InSchema("TestSchema").OnColumn("Name2").Ascending();
        }
    }

    /// <summary>
    /// Class TestCreateUniqueConstraintWithReversing.
    /// Implements the <see cref="FluentMigrator.AutoReversingMigration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.AutoReversingMigration" />
    internal class TestCreateUniqueConstraintWithReversing : AutoReversingMigration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            Create.UniqueConstraint("TestUnique").OnTable("TestTable2").Column("Name");
        }
    }

    /// <summary>
    /// Class TestCreateUniqueConstraintWithSchemaWithReversing.
    /// Implements the <see cref="FluentMigrator.AutoReversingMigration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.AutoReversingMigration" />
    internal class TestCreateUniqueConstraintWithSchemaWithReversing : AutoReversingMigration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            Create.UniqueConstraint("TestUnique").OnTable("TestTable2").WithSchema("TestSchema").Column("Name");
        }
    }

    /// <summary>
    /// Class TestExecuteSql.
    /// Implements the <see cref="FluentMigrator.Migration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Migration" />
    internal class TestExecuteSql : Migration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            Execute.Sql("select 1");
        }

        /// <summary>
        /// Downs this instance.
        /// </summary>
        public override void Down()
        {
            Execute.Sql("select 2");
        }
    }
}
