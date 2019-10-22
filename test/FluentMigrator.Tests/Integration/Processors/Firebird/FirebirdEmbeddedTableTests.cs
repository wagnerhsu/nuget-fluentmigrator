// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="FirebirdEmbeddedTableTests.cs" company="FluentMigrator Project">
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
using System.Reflection;

using FirebirdSql.Data.FirebirdClient;

using FluentMigrator.Runner;
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Generators.Firebird;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Runner.Processors.Firebird;

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

namespace FluentMigrator.Tests.Integration.Processors.Firebird
{
    /// <summary>
    /// Defines test class FirebirdEmbeddedTableTests.
    /// </summary>
    [TestFixture]
    [Category("Firebird")]
    [Category("Integration")]
    public class FirebirdEmbeddedTableTests
    {
        /// <summary>
        /// The firebird library prober
        /// </summary>
        private readonly FirebirdLibraryProber _firebirdLibraryProber = new FirebirdLibraryProber();

        /// <summary>
        /// Defines the test method RenameTable_WhenOriginalTableExistsAndContainsDataWithNulls_ShouldNotThrowException.
        /// </summary>
        [Test]
        public void RenameTable_WhenOriginalTableExistsAndContainsDataWithNulls_ShouldNotThrowException()
        {
            //---------------Set up test pack-------------------
            using (var tempDb = new TemporaryDatabase(IntegrationTestOptions.Firebird, _firebirdLibraryProber))
            {
                using (var serviceProvider = CreateServiceProvider(tempDb.ConnectionString, "FluentMigrator.Tests.Integration.Migrations"))
                {
                    var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
                    runner.Up(new CreateTableMigration());
                    runner.Up(new AddDataMigration(1));

                    //---------------Assert Precondition----------------
                    var processor = serviceProvider.GetRequiredService<IMigrationProcessor>();
                    var result = processor.Read("select * from TheTable");

                    Assert.That(result.Tables.Count, Is.GreaterThan(0));
                    var table = result.Tables[0];

                    Assert.That(table.Rows.Count, Is.GreaterThan(0));
                    var row = table.Rows[0];

                    Assert.IsTrue(table.Columns.Contains("Name"));
                    Assert.That(row["Name"], Is.InstanceOf<DBNull>());

                    //---------------Execute Test ----------------------
                    Exception thrown = null;
                    try
                    {
                        runner.Up(new RenameTableMigration());
                    }
                    catch (Exception ex)
                    {
                        thrown = ex;
                    }

                    //---------------Test Result -----------------------
                    Assert.IsNull(thrown);
                }
            }
        }

        /// <summary>
        /// Defines the test method RenameTable_WhenOriginalTableContainsMultipleRows_ShouldNotFailToMigrate.
        /// </summary>
        [Test]
        public void RenameTable_WhenOriginalTableContainsMultipleRows_ShouldNotFailToMigrate()
        {
            //---------------Set up test pack-------------------
            using (var tempDb = new TemporaryDatabase(IntegrationTestOptions.Firebird, _firebirdLibraryProber))
            {
                using (var serviceProvider = CreateServiceProvider(tempDb.ConnectionString, "FluentMigrator.Tests.Integration.Migrations"))
                {
                    var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
                    runner.Up(new CreateTableMigration());
                    runner.Up(new AddDataMigration(id: 1));
                    runner.Up(new AddDataMigration(2));
                    runner.Up(new AddDataMigration(3));
                    //---------------Assert Precondition----------------
                    var processor = serviceProvider.GetRequiredService<IMigrationProcessor>();
                    Assert.That(CountRowsWith("select count(*) as TheCount from TheTable", processor), Is.EqualTo(3));

                    //---------------Execute Test ----------------------
                    Exception thrown = null;
                    try
                    {
                        runner.Up(new RenameTableMigration());
                    }
                    catch (Exception ex)
                    {
                        thrown = ex;
                    }

                    //---------------Test Result -----------------------
                    Assert.IsNull(thrown);
                }
            }
        }

        /// <summary>
        /// Class DeleteDataMigration.
        /// Implements the <see cref="FluentMigrator.Migration" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Migration" />
        private class DeleteDataMigration : Migration
        {
            /// <summary>
            /// The ids
            /// </summary>
            private readonly int[] _ids;

            /// <summary>
            /// Initializes a new instance of the <see cref="DeleteDataMigration"/> class.
            /// </summary>
            /// <param name="forIds">For ids.</param>
            public DeleteDataMigration(params int[] forIds)
            {
                _ids = forIds;
            }

            /// <summary>
            /// Collect the UP migration expressions
            /// </summary>
            public override void Up()
            {
                var start = Delete.FromTable("TheTable").Row(new { Id = _ids.First() });
                foreach (var id in _ids.Skip(1))
                    start.Row(new { Id = id });
            }

            /// <summary>
            /// Collects the DOWN migration expressions
            /// </summary>
            public override void Down()
            {
            }
        }

        /// <summary>
        /// Defines the test method OneMigrationWithOneDelete_ShouldDeleteAffectedRow.
        /// </summary>
        [Test]
        public void OneMigrationWithOneDelete_ShouldDeleteAffectedRow()
        {
            //---------------Set up test pack-------------------
            using (var tempDb = new TemporaryDatabase(IntegrationTestOptions.Firebird, _firebirdLibraryProber))
            {
                using (var serviceProvider = CreateServiceProvider(tempDb.ConnectionString, "FluentMigrator.Tests.Integration.Migrations"))
                {
                    var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
                    runner.Up(new CreateTableMigration());
                    runner.Up(new AddDataMigration(1));
                    runner.Up(new AddDataMigration(2));
                    runner.Up(new AddDataMigration(3));

                    //---------------Assert Precondition----------------
                    const string countSql = "select count(*) as TheCount from TheTable";
                    var processor = serviceProvider.GetRequiredService<FirebirdProcessor>();
                    Assert.That(CountRowsWith(countSql, processor), Is.EqualTo(3));

                    //---------------Execute Test ----------------------
                    Exception thrown = null;
                    try
                    {
                        runner.Up(new DeleteDataMigration(1));
                        processor.CommitTransaction();

                        Assert.That(CountRowsWith(countSql, processor), Is.EqualTo(2));
                    }
                    catch (Exception ex)
                    {
                        thrown = ex;
                    }


                    //---------------Test Result -----------------------
                    Assert.IsNull(thrown);
                }
            }
        }

        /// <summary>
        /// Defines the test method OneMigrationWithMultipleDeletes_ShouldDeleteAffectedRow.
        /// </summary>
        [Test]
        public void OneMigrationWithMultipleDeletes_ShouldDeleteAffectedRow()
        {
            //---------------Set up test pack-------------------
            using (var tempDb = new TemporaryDatabase(IntegrationTestOptions.Firebird, _firebirdLibraryProber))
            {
                using (var serviceProvider = CreateServiceProvider(tempDb.ConnectionString, "FluentMigrator.Tests.Integration.Migrations"))
                {
                    var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
                    runner.Up(new CreateTableMigration());
                    runner.Up(new AddDataMigration(1));
                    runner.Up(new AddDataMigration(2));
                    runner.Up(new AddDataMigration(3));
                    //---------------Assert Precondition----------------
                    const string countSql = "select count(*) as TheCount from TheTable";
                    var processor = serviceProvider.GetRequiredService<FirebirdProcessor>();
                    Assert.That(CountRowsWith(countSql, processor), Is.EqualTo(3));

                    //---------------Execute Test ----------------------
                    Exception thrown = null;
                    try
                    {
                        runner.Up(new DeleteDataMigration(1, 2));
                        processor.CommitTransaction();

                        Assert.That(CountRowsWith(countSql, processor), Is.EqualTo(1));
                    }
                    catch (Exception ex)
                    {
                        thrown = ex;
                    }


                    //---------------Test Result -----------------------
                    Assert.IsNull(thrown);
                }
            }
        }

        /// <summary>
        /// Class DeleteAllRowsMigration.
        /// Implements the <see cref="FluentMigrator.Migration" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Migration" />
        private class DeleteAllRowsMigration : Migration
        {
            /// <summary>
            /// Collect the UP migration expressions
            /// </summary>
            public override void Up()
            {
                Delete.FromTable("TheTable").AllRows();
            }

            /// <summary>
            /// Collects the DOWN migration expressions
            /// </summary>
            public override void Down()
            {
            }
        }

        /// <summary>
        /// Defines the test method MigrationWithcAllRowsDelete_ShouldDeleteAllRows.
        /// </summary>
        [Test]
        public void MigrationWithcAllRowsDelete_ShouldDeleteAllRows()
        {
            //---------------Set up test pack-------------------
            using (var tempDb = new TemporaryDatabase(IntegrationTestOptions.Firebird, _firebirdLibraryProber))
            {
                using (var serviceProvider = CreateServiceProvider(tempDb.ConnectionString, "FluentMigrator.Tests.Integration.Migrations"))
                {
                    var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
                    runner.Up(new CreateTableMigration());
                    runner.Up(new AddDataMigration(1));
                    runner.Up(new AddDataMigration(2));
                    runner.Up(new AddDataMigration(3));
                    //---------------Assert Precondition----------------
                    const string countSql = "select count(*) as TheCount from TheTable";
                    var processor = serviceProvider.GetRequiredService<FirebirdProcessor>();
                    Assert.That(CountRowsWith(countSql, processor), Is.EqualTo(3));

                    //---------------Execute Test ----------------------
                    Exception thrown = null;
                    try
                    {
                        runner.Up(new DeleteAllRowsMigration());
                        processor.CommitTransaction();

                        Assert.That(CountRowsWith(countSql, processor), Is.EqualTo(0));
                    }
                    catch (Exception ex)
                    {
                        thrown = ex;
                    }


                    //---------------Test Result -----------------------
                    Assert.IsNull(thrown);
                }
            }
        }

        /// <summary>
        /// Defines the test method OneMigrationWithOneSpecificUpdate_ShouldUpdateAffectedRow.
        /// </summary>
        [Test]
        public void OneMigrationWithOneSpecificUpdate_ShouldUpdateAffectedRow()
        {
            //---------------Set up test pack-------------------
            using (var tempDb = new TemporaryDatabase(IntegrationTestOptions.Firebird, _firebirdLibraryProber))
            {
                using (var serviceProvider = CreateServiceProvider(tempDb.ConnectionString, "FluentMigrator.Tests.Integration.Migrations"))
                {
                    var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
                    runner.Up(new CreateTableMigration());
                    runner.Up(new AddDataMigration(1));
                    runner.Up(new AddDataMigration(2));
                    runner.Up(new AddDataMigration(3));
                    //---------------Assert Precondition----------------
                    const string countSql = "select count(*) as TheCount from TheTable where Id = {0}";
                    var processor = serviceProvider.GetRequiredService<FirebirdProcessor>();
                    Assert.That(CountRowsWith(countSql, processor, 1), Is.EqualTo(1));

                    //---------------Execute Test ----------------------
                    Exception thrown = null;
                    try
                    {
                        runner.Up(new UpdateMigration(4, 1));
                        processor.CommitTransaction();

                        Assert.That(CountRowsWith(countSql, processor, 4), Is.EqualTo(1));
                    }
                    catch (Exception ex)
                    {
                        thrown = ex;
                    }


                    //---------------Test Result -----------------------
                    Assert.IsNull(thrown);
                }
            }
        }

        /// <summary>
        /// Defines the test method TwoMigrationsWithSpecificUpdates_ShouldUpdateAffectedRows.
        /// </summary>
        [Test]
        public void TwoMigrationsWithSpecificUpdates_ShouldUpdateAffectedRows()
        {
            //---------------Set up test pack-------------------
            using (var tempDb = new TemporaryDatabase(IntegrationTestOptions.Firebird, _firebirdLibraryProber))
            {
                using (var serviceProvider = CreateServiceProvider(tempDb.ConnectionString, "FluentMigrator.Tests.Integration.Migrations"))
                {
                    var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
                    runner.Up(new CreateTableMigration());
                    runner.Up(new AddDataMigration(1));
                    runner.Up(new AddDataMigration(2));
                    runner.Up(new AddDataMigration(3));
                    //---------------Assert Precondition----------------
                    const string countSql = "select count(*) as TheCount from TheTable where Id = {0}";
                    var processor = serviceProvider.GetRequiredService<FirebirdProcessor>();
                    Assert.That(CountRowsWith(countSql, processor, 1), Is.EqualTo(1));

                    //---------------Execute Test ----------------------
                    Exception thrown = null;
                    try
                    {
                        runner.Up(new UpdateMigration(4, 1));
                        runner.Up(new UpdateMigration(5, 2));
                        processor.CommitTransaction();
                        Assert.That(CountRowsWith(countSql, processor, 4), Is.EqualTo(1));
                        Assert.That(CountRowsWith(countSql, processor, 5), Is.EqualTo(1));
                    }
                    catch (Exception ex)
                    {
                        thrown = ex;
                    }


                    //---------------Test Result -----------------------
                    Assert.IsNull(thrown);
                }
            }
        }

        /// <summary>
        /// Defines the test method OneMigrationWithOneBlanketUpdate_ShouldUpdateAffectedRow.
        /// </summary>
        [Test]
        public void OneMigrationWithOneBlanketUpdate_ShouldUpdateAffectedRow()
        {
            //---------------Set up test pack-------------------
            using (var tempDb = new TemporaryDatabase(IntegrationTestOptions.Firebird, _firebirdLibraryProber))
            {
                using (var serviceProvider = CreateServiceProvider(tempDb.ConnectionString, "FluentMigrator.Tests.Integration.Migrations"))
                {
                    var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
                    runner.Up(new CreateTableMigration());
                    runner.Up(new AddDataMigration(1));
                    runner.Up(new AddDataMigration(2));
                    runner.Up(new AddDataMigration(3));
                    //---------------Assert Precondition----------------
                    const string countSql = "select count(*) as TheCount from TheTable where SomeValue = {0}";
                    var processor = serviceProvider.GetRequiredService<FirebirdProcessor>();
                    Assert.That(CountRowsWith(countSql, processor, 1), Is.EqualTo(1));
                    Assert.That(CountRowsWith(countSql, processor, 2), Is.EqualTo(1));
                    Assert.That(CountRowsWith(countSql, processor, 3), Is.EqualTo(1));

                    //---------------Execute Test ----------------------
                    Exception thrown = null;
                    try
                    {
                        runner.Up(new UpdateMigration(4));
                        processor.CommitTransaction();
                        Assert.That(CountRowsWith(countSql, processor, 4), Is.EqualTo(3));
                    }
                    catch (Exception ex)
                    {
                        thrown = ex;
                    }


                    //---------------Test Result -----------------------
                    Assert.IsNull(thrown);
                }
            }
        }

        /// <summary>
        /// Defines the test method RenamingTable_WhenTableHasTextBlobs_ShouldCreateNewTableWithTextBlobsNotBinaryBlobs.
        /// </summary>
        [Test]
        public void RenamingTable_WhenTableHasTextBlobs_ShouldCreateNewTableWithTextBlobsNotBinaryBlobs()
        {
            using (var tempDb = new TemporaryDatabase(IntegrationTestOptions.Firebird, _firebirdLibraryProber))
            {
                using (var serviceProvider = CreateServiceProvider(tempDb.ConnectionString, "FluentMigrator.Tests.Integration.Migrations"))
                {
                    var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
                    runner.Up(new MigrationWhichCreatesTableWithTextBlob());
                    //---------------Assert Precondition----------------
                    var fieldName = "TheColumn";
                    var tableName = "TheTable";
                    var expectedFieldType = 261;
                    var expectedFieldSubType = 1;
                    var processor = serviceProvider.GetRequiredService<FirebirdProcessor>();
                    AssertThatFieldHasCorrectTypeAndSubType(fieldName, tableName, processor, expectedFieldType, expectedFieldSubType);
                    //---------------Execute Test ----------------------
                    runner.Up(new MigrationWhichRenamesTableWithTextBlob());
                    //---------------Test Result -----------------------
                    tableName = "TheNewTable";
                    AssertThatFieldHasCorrectTypeAndSubType(fieldName, tableName, processor, expectedFieldType, expectedFieldSubType);
                }
            }
        }

        /// <summary>
        /// Defines the test method ObsoleteAlterTable_MigrationRequiresAutomaticDelete_AndProcessorHasUndoDisabled_ShouldNotThrow.
        /// </summary>
        [Test]
        [Obsolete]
        public void ObsoleteAlterTable_MigrationRequiresAutomaticDelete_AndProcessorHasUndoDisabled_ShouldNotThrow()
        {
            using (var tempDb = new TemporaryDatabase(IntegrationTestOptions.Firebird, _firebirdLibraryProber))
            {
                var connectionString = tempDb.ConnectionString;

                var runnerContext = new RunnerContext(new TextWriterAnnouncer(TestContext.Out))
                {
                    Namespace = "FluentMigrator.Tests.Integration.Migrations"
                };

                using (var connection = new FbConnection(connectionString))
                {
                    var announcer = new TextWriterAnnouncer(TestContext.Out) { ShowSql = true };
                    var options = FirebirdOptions.AutoCommitBehaviour();
                    options.TruncateLongNames = false;
                    var processor = new FirebirdProcessor(connection, new FirebirdGenerator(options), announcer,
                        new ProcessorOptions(), new FirebirdDbFactory(), options);
                    var runner = new MigrationRunner(Assembly.GetExecutingAssembly(), runnerContext, processor);
                    runner.Up(new MigrationWhichCreatesTwoRelatedTables());
                    processor.CommitTransaction();
                    FbConnection.ClearPool(connection);
                }

                //---------------Assert Precondition----------------
                Assert.IsTrue(ForeignKeyExists(connectionString, MigrationWhichCreatesTwoRelatedTables.ForeignKeyName),
                    "Foreign key does not exist after first migration");
                using (var connection = new FbConnection(connectionString))
                {
                    var announcer = new TextWriterAnnouncer(TestContext.Out) { ShowSql = true };
                    var options = FirebirdOptions.AutoCommitBehaviour();
                    var processor = new FirebirdProcessor(connection, new FirebirdGenerator(options), announcer,
                        new ProcessorOptions(), new FirebirdDbFactory(), options);
                    var runner = new MigrationRunner(Assembly.GetExecutingAssembly(), runnerContext, processor);
                    runner.Up(new MigrationWhichAltersTableWithFK());
                    processor.CommitTransaction();
                }

                Assert.IsTrue(ForeignKeyExists(connectionString, MigrationWhichCreatesTwoRelatedTables.ForeignKeyName),
                    "Foreign key does not exist after second migration");
            }
        }

        /// <summary>
        /// Defines the test method AlterTable_MigrationRequiresAutomaticDelete_AndProcessorHasUndoDisabled_ShouldNotThrow.
        /// </summary>
        [Test]
        public void AlterTable_MigrationRequiresAutomaticDelete_AndProcessorHasUndoDisabled_ShouldNotThrow()
        {
            using (var tempDb = new TemporaryDatabase(IntegrationTestOptions.Firebird, _firebirdLibraryProber))
            {
                using (var serviceProvider = CreateServiceProvider(tempDb.ConnectionString, "FluentMigrator.Tests.Integration.Migrations"))
                {
                    var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
                    var processor = serviceProvider.GetRequiredService<FirebirdProcessor>();
                    runner.Up(new MigrationWhichCreatesTwoRelatedTables());
                    processor.CommitTransaction();
                    FbConnection.ClearPool((FbConnection)processor.Connection);
                }

                //---------------Assert Precondition----------------
                Assert.IsTrue(ForeignKeyExists(tempDb.ConnectionString, MigrationWhichCreatesTwoRelatedTables.ForeignKeyName),
                    "Foreign key does not exist after first migration");
                using (var serviceProvider = CreateServiceProvider(tempDb.ConnectionString, "FluentMigrator.Tests.Integration.Migrations"))
                {
                    var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
                    var processor = serviceProvider.GetRequiredService<FirebirdProcessor>();
                    runner.Up(new MigrationWhichAltersTableWithFK());
                    processor.CommitTransaction();
                }

                Assert.IsTrue(ForeignKeyExists(tempDb.ConnectionString, MigrationWhichCreatesTwoRelatedTables.ForeignKeyName),
                    "Foreign key does not exist after second migration");
            }
        }

        /// <summary>
        /// Asserts the type of the that field has correct type and sub.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="processor">The processor.</param>
        /// <param name="expectedFieldType">Expected type of the field.</param>
        /// <param name="expectedFieldSubType">Expected type of the field sub.</param>
        private static void AssertThatFieldHasCorrectTypeAndSubType(string fieldName, string tableName, IMigrationProcessor processor,
            int expectedFieldType, int expectedFieldSubType)
        {
            var sql =
                "select RDB$FIELD_TYPE fieldType, RDB$FIELD_SUB_TYPE subType from RDB$RELATION_FIELDS rf " +
                "inner join RDB$FIELDS f on rf.RDB$FIELD_SOURCE = f.RDB$FIELD_NAME where rf.RDB$FIELD_NAME = '" +
                fieldName.ToUpper() + "' " +
                "and rf.RDB$RELATION_NAME = '" + tableName.ToUpper() + "'";
            var result = processor.Read(sql);
            Assert.That(result.Tables.Count, Is.GreaterThan(0), "Unable to query schema for table '" + tableName + "'");
            var table = result.Tables[0];

            Assert.That(table.Rows.Count, Is.GreaterThan(0), "Unable to query schema for table '" + tableName + "'");
            var row = table.Rows[0];
            var fieldType = row["fieldType"];
            var fieldSubType = row["subType"];
            Assert.AreEqual(expectedFieldType, fieldType, "Field type mismatch");
            Assert.AreEqual(expectedFieldSubType, fieldSubType, "Field subtype mismatch");
        }

        /// <summary>
        /// Counts the rows with.
        /// </summary>
        /// <param name="countSql">The count SQL.</param>
        /// <param name="processor">The processor.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>System.Int32.</returns>
        private static int CountRowsWith(string countSql, IMigrationProcessor processor, params object[] args)
        {
            var result = processor.Read(countSql, args);

            Assert.That(result.Tables.Count, Is.GreaterThan(0));
            var table = result.Tables[0];

            Assert.That(table.Rows.Count, Is.GreaterThan(0));
            var row = table.Rows[0];

            Assert.IsTrue(table.Columns.Contains("TheCount"));
            return Convert.ToInt32(row["TheCount"]);
        }

        /// <summary>
        /// Creates the service provider.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="namespace">The namespace.</param>
        /// <returns>ServiceProvider.</returns>
        private ServiceProvider CreateServiceProvider(string connectionString, string @namespace)
        {
            return ServiceCollectionExtensions.CreateServices()
                .ConfigureRunner(cfg => cfg.AddFirebird())
                .AddScoped<IConnectionStringReader>(_ => new PassThroughConnectionStringReader(connectionString))
                .WithMigrationsIn(@namespace)
                .BuildServiceProvider();
        }

        /// <summary>
        /// Foreigns the key exists.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="withName">Name of the with.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool ForeignKeyExists(string connectionString, string withName)
        {
            using (var connection = new FbConnection(connectionString))
            {
                connection.Open();
                var keyQuery = @"
SELECT rc.RDB$CONSTRAINT_NAME AS constraint_name,
i.RDB$RELATION_NAME AS table_name,
s.RDB$FIELD_NAME AS field_name,
i.RDB$DESCRIPTION AS description,
rc.RDB$DEFERRABLE AS is_deferrable,
rc.RDB$INITIALLY_DEFERRED AS is_deferred,
refc.RDB$UPDATE_RULE AS on_update,
refc.RDB$DELETE_RULE AS on_delete,
refc.RDB$MATCH_OPTION AS match_type,
i2.RDB$RELATION_NAME AS references_table,
s2.RDB$FIELD_NAME AS references_field,
(s.RDB$FIELD_POSITION + 1) AS field_position
FROM RDB$INDEX_SEGMENTS s
LEFT JOIN RDB$INDICES i ON i.RDB$INDEX_NAME = s.RDB$INDEX_NAME
LEFT JOIN RDB$RELATION_CONSTRAINTS rc ON rc.RDB$INDEX_NAME = s.RDB$INDEX_NAME
LEFT JOIN RDB$REF_CONSTRAINTS refc ON rc.RDB$CONSTRAINT_NAME = refc.RDB$CONSTRAINT_NAME
LEFT JOIN RDB$RELATION_CONSTRAINTS rc2 ON rc2.RDB$CONSTRAINT_NAME = refc.RDB$CONST_NAME_UQ
LEFT JOIN RDB$INDICES i2 ON i2.RDB$INDEX_NAME = rc2.RDB$INDEX_NAME
LEFT JOIN RDB$INDEX_SEGMENTS s2 ON i2.RDB$INDEX_NAME = s2.RDB$INDEX_NAME
WHERE rc.RDB$CONSTRAINT_TYPE = 'FOREIGN KEY'
ORDER BY s.RDB$FIELD_POSITION";
                var cmd = connection.CreateCommand();
                cmd.CommandText = keyQuery;
                var result = false;
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        try
                        {
                            var constraintName = rdr["CONSTRAINT_NAME"];
                            if (constraintName == null) continue;
                            if (constraintName is DBNull) continue;
                            if (constraintName.ToString().Trim() == withName.ToUpper())
                            {
                                result = true;
                                break;
                            }
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                }
                connection.Close();
                FbConnection.ClearPool(connection);
                return result;
            }
        }

        /// <summary>
        /// Class MigrationWhichCreatesTableWithTextBlob.
        /// Implements the <see cref="FluentMigrator.Migration" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Migration" />
        public class MigrationWhichCreatesTableWithTextBlob : Migration
        {
            /// <summary>
            /// Collect the UP migration expressions
            /// </summary>
            public override void Up()
            {
                Create.Table("TheTable")
                    .WithColumn("TheColumn").AsString(int.MaxValue);
            }

            /// <summary>
            /// Collects the DOWN migration expressions
            /// </summary>
            public override void Down()
            {
            }
        }

        /// <summary>
        /// Class MigrationWhichRenamesTableWithTextBlob.
        /// Implements the <see cref="FluentMigrator.Migration" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Migration" />
        public class MigrationWhichRenamesTableWithTextBlob : Migration
        {
            /// <summary>
            /// Collect the UP migration expressions
            /// </summary>
            public override void Up()
            {
                Rename.Table("TheTable").To("TheNewTable");
            }

            /// <summary>
            /// Collects the DOWN migration expressions
            /// </summary>
            public override void Down()
            {
            }
        }

        /// <summary>
        /// Class MigrationWhichCreatesTwoRelatedTables.
        /// Implements the <see cref="FluentMigrator.Migration" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Migration" />
        public class MigrationWhichCreatesTwoRelatedTables : Migration
        {
            /// <summary>
            /// The foreign key name
            /// </summary>
            public const string ForeignKeyName = "FK_table2_table1";
            /// <summary>
            /// Collect the UP migration expressions
            /// </summary>
            public override void Up()
            {
                Create.Table("table1")
                        .WithColumn("id").AsInt32().PrimaryKey().Identity();
                Create.Table("table2")
                        .WithColumn("table1_id").AsInt32();
                Create.ForeignKey(ForeignKeyName)
                        .FromTable("table2").ForeignColumn("table1_id")
                        .ToTable("table1").PrimaryColumn("id");
            }

            /// <summary>
            /// Collects the DOWN migration expressions
            /// </summary>
            public override void Down()
            {
            }
        }

        /// <summary>
        /// Class MigrationWhichAltersTableWithFK.
        /// Implements the <see cref="FluentMigrator.Migration" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Migration" />
        public class MigrationWhichAltersTableWithFK : Migration
        {
            /// <summary>
            /// Collect the UP migration expressions
            /// </summary>
            public override void Up()
            {
                Alter.Table("table1").AddColumn("Value").AsDouble().Nullable();
            }

            /// <summary>
            /// Collects the DOWN migration expressions
            /// </summary>
            public override void Down()
            {
            }
        }

        /// <summary>
        /// Class UpdateMigration.
        /// Implements the <see cref="FluentMigrator.Migration" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Migration" />
        private class UpdateMigration : Migration
        {
            /// <summary>
            /// From
            /// </summary>
            private readonly int? _from;

            /// <summary>
            /// To
            /// </summary>
            private readonly int _to;

            /// <summary>
            /// Initializes a new instance of the <see cref="UpdateMigration"/> class.
            /// </summary>
            /// <param name="to">To.</param>
            /// <param name="from">From.</param>
            public UpdateMigration(int to, int? from = null)
            {
                _from = from;
                _to = to;
            }
            /// <summary>
            /// Collect the UP migration expressions
            /// </summary>
            public override void Up()
            {
                if (_from.HasValue)
                    Update.Table("TheTable").Set(new { Id = _to, Name = "foo" }).Where(new { Id = _from.Value });
                else
                    Update.Table("TheTable").Set(new { SomeValue = _to }).AllRows();
            }

            /// <summary>
            /// Collects the DOWN migration expressions
            /// </summary>
            public override void Down()
            {
            }
        }

        /// <summary>
        /// Class CreateTableMigration.
        /// Implements the <see cref="FluentMigrator.Migration" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Migration" />
        public class CreateTableMigration : Migration
        {
            /// <summary>
            /// Collect the UP migration expressions
            /// </summary>
            public override void Up()
            {
                Create.Table("TheTable")
                    .WithColumn("Id").AsInt32().PrimaryKey()
                    .WithColumn("Name").AsString(100).Nullable()
                    .WithColumn("SomeValue").AsInt32().Nullable();
            }

            /// <summary>
            /// Collects the DOWN migration expressions
            /// </summary>
            public override void Down()
            {
            }
        }

        /// <summary>
        /// Class AddDataMigration.
        /// Implements the <see cref="FluentMigrator.Migration" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Migration" />
        public class AddDataMigration : Migration
        {
            /// <summary>
            /// The identifier
            /// </summary>
            private readonly int _id;

            // ReSharper disable once UnusedMember.Global
            /// <summary>
            /// Initializes a new instance of the <see cref="AddDataMigration"/> class.
            /// </summary>
            public AddDataMigration()
                : this(1)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="AddDataMigration"/> class.
            /// </summary>
            /// <param name="id">The identifier.</param>
            public AddDataMigration(int id)
            {
                _id = id;
            }

            /// <summary>
            /// Collect the UP migration expressions
            /// </summary>
            public override void Up()
            {
                Insert.IntoTable("TheTable").Row(new { Id = _id, SomeValue = _id });
            }

            /// <summary>
            /// Collects the DOWN migration expressions
            /// </summary>
            public override void Down()
            {
                Delete.FromTable("TheTable").Row(new { Id = _id });
            }
        }

        /// <summary>
        /// Class RenameTableMigration.
        /// Implements the <see cref="FluentMigrator.Migration" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Migration" />
        public class RenameTableMigration : Migration
        {
            /// <summary>
            /// Collect the UP migration expressions
            /// </summary>
            public override void Up()
            {
                Rename.Table("TheTable").To("TheNewTable");
            }

            /// <summary>
            /// Downs this instance.
            /// </summary>
            public override void Down()
            {
            }
        }
    }
}
