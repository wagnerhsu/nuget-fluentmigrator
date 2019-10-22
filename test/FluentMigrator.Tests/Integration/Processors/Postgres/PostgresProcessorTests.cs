// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="PostgresProcessorTests.cs" company="FluentMigrator Project">
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
using System.Data;
using System.IO;

using FluentMigrator.Expressions;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Logging;
using FluentMigrator.Runner.Processors.Postgres;
using FluentMigrator.Tests.Helpers;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Integration.Processors.Postgres
{
    /// <summary>
    /// Defines test class PostgresProcessorTests.
    /// </summary>
    [TestFixture]
    [Category("Integration")]
    [Category("Postgres")]
    public class PostgresProcessorTests
    {
        /// <summary>
        /// Gets or sets the service provider.
        /// </summary>
        /// <value>The service provider.</value>
        private ServiceProvider ServiceProvider { get; set; }
        /// <summary>
        /// Gets or sets the service scope.
        /// </summary>
        /// <value>The service scope.</value>
        private IServiceScope ServiceScope { get; set; }
        /// <summary>
        /// Gets or sets the processor.
        /// </summary>
        /// <value>The processor.</value>
        private PostgresProcessor Processor { get; set; }

        /// <summary>
        /// Defines the test method CallingColumnExistsReturnsFalseIfColumnExistsInDifferentSchema.
        /// </summary>
        [Test]
        public void CallingColumnExistsReturnsFalseIfColumnExistsInDifferentSchema()
        {
            using (var table = new PostgresTestTable(Processor, "TestSchema1", "id int"))
                Processor.ColumnExists("TestSchema2", table.Name, "id").ShouldBeFalse();
        }

        /// <summary>
        /// Defines the test method CallingConstraintExistsReturnsFalseIfConstraintExistsInDifferentSchema.
        /// </summary>
        [Test]
        public void CallingConstraintExistsReturnsFalseIfConstraintExistsInDifferentSchema()
        {
            using (var table = new PostgresTestTable(Processor, "TestSchema1", "id int",
                "wibble int CONSTRAINT c1 CHECK(wibble > 0)"))
                Processor.ConstraintExists("TestSchema2", table.Name, "c1").ShouldBeFalse();
        }

        /// <summary>
        /// Defines the test method CallingTableExistsReturnsFalseIfTableExistsInDifferentSchema.
        /// </summary>
        [Test]
        public void CallingTableExistsReturnsFalseIfTableExistsInDifferentSchema()
        {
            using (var table = new PostgresTestTable(Processor, "TestSchema1", "id int"))
                Processor.TableExists("TestSchema2", table.Name).ShouldBeFalse();
        }

        /// <summary>
        /// Defines the test method CanReadData.
        /// </summary>
        [Test]
        public void CanReadData()
        {
            using (var table = new PostgresTestTable(Processor, null, "id int"))
            {
                AddTestData(table);

                DataSet ds = Processor.Read("SELECT * FROM \"{0}\"", table.Name);

                ds.ShouldNotBeNull();
                ds.Tables.Count.ShouldBe(1);
                ds.Tables[0].Rows.Count.ShouldBe(3);
                ds.Tables[0].Rows[2][0].ShouldBe(2);
            }
        }

        /// <summary>
        /// Defines the test method CanReadTableData.
        /// </summary>
        [Test]
        public void CanReadTableData()
        {
            using (var table = new PostgresTestTable(Processor, null, "id int"))
            {
                AddTestData(table);

                DataSet ds = Processor.ReadTableData(null, table.Name);

                ds.ShouldNotBeNull();
                ds.Tables.Count.ShouldBe(1);
                ds.Tables[0].Rows.Count.ShouldBe(3);
                ds.Tables[0].Rows[2][0].ShouldBe(2);
            }
        }

        /// <summary>
        /// Adds the test data.
        /// </summary>
        /// <param name="table">The table.</param>
        private void AddTestData(PostgresTestTable table)
        {
            for (int i = 0; i < 3; i++)
            {
                var cmd = table.Connection.CreateCommand();
                cmd.Transaction = table.Transaction;
                cmd.CommandText = $"INSERT INTO {table.NameWithSchema} (id) VALUES ({i})";
                cmd.ExecuteNonQuery();
            }
        }


        /// <summary>
        /// Defines the test method CanReadDataWithSchema.
        /// </summary>
        [Test]
        public void CanReadDataWithSchema()
        {
            using (var table = new PostgresTestTable(Processor, "TestSchema", "id int"))
            {
                AddTestData(table);

                DataSet ds = Processor.Read("SELECT * FROM {0}", table.NameWithSchema);

                ds.ShouldNotBeNull();
                ds.Tables.Count.ShouldBe(1);
                ds.Tables[0].Rows.Count.ShouldBe(3);
                ds.Tables[0].Rows[2][0].ShouldBe(2);
            }
        }

        /// <summary>
        /// Defines the test method CanReadTableDataWithSchema.
        /// </summary>
        [Test]
        public void CanReadTableDataWithSchema()
        {
            using (var table = new PostgresTestTable(Processor, "TestSchema", "id int"))
            {
                AddTestData(table);

                DataSet ds = Processor.ReadTableData("TestSchema", table.Name);

                ds.ShouldNotBeNull();
                ds.Tables.Count.ShouldBe(1);
                ds.Tables[0].Rows.Count.ShouldBe(3);
                ds.Tables[0].Rows[2][0].ShouldBe(2);
            }
        }

        /// <summary>
        /// Defines the test method CallingProcessWithPerformDbOperationExpressionWhenInPreviewOnlyModeWillNotMakeDbChanges.
        /// </summary>
        [Test]
        public void CallingProcessWithPerformDbOperationExpressionWhenInPreviewOnlyModeWillNotMakeDbChanges()
        {
            var output = new StringWriter();

            var sp = CreateProcessorServices(
                services => services
                    .AddSingleton<ILoggerProvider>(new SqlScriptFluentMigratorLoggerProvider(output))
                    .ConfigureRunner(r => r.AsGlobalPreview()));
            using (sp)
            {
                using (var scope = sp.CreateScope())
                {
                    var processor = scope.ServiceProvider.GetRequiredService<IMigrationProcessor>();

                    bool tableExists;

                    try
                    {
                        var expression =
                            new PerformDBOperationExpression
                            {
                                Operation = (con, trans) =>
                                {
                                    var command = con.CreateCommand();
                                    command.CommandText = "CREATE TABLE processtesttable (test int NULL) ";
                                    command.Transaction = trans;

                                    command.ExecuteNonQuery();
                                }
                            };

                        processor.BeginTransaction();
                        processor.Process(expression);

                        tableExists = processor.TableExists("public", "processtesttable");
                    }
                    finally
                    {
                        processor.RollbackTransaction();
                    }

                    tableExists.ShouldBeFalse();
                    output.ToString().ShouldBe(
                        @"/* Beginning Transaction */
/* Performing DB Operation */
/* Rolling back transaction */
");
                }
            }
        }

        /// <summary>
        /// Creates the processor services.
        /// </summary>
        /// <param name="initAction">The initialize action.</param>
        /// <returns>ServiceProvider.</returns>
        private ServiceProvider CreateProcessorServices([CanBeNull] Action<IServiceCollection> initAction)
        {
            if (!IntegrationTestOptions.Postgres.IsEnabled)
                Assert.Ignore();

            var serivces = ServiceCollectionExtensions.CreateServices()
                .ConfigureRunner(r => r.AddPostgres())
                .AddScoped<IConnectionStringReader>(
                    _ => new PassThroughConnectionStringReader(IntegrationTestOptions.Postgres.ConnectionString));

            initAction?.Invoke(serivces);

            return serivces.BuildServiceProvider();
        }

        /// <summary>
        /// Classes the set up.
        /// </summary>
        [OneTimeSetUp]
        public void ClassSetUp()
        {
            ServiceProvider = CreateProcessorServices(initAction: null);
        }

        /// <summary>
        /// Classes the tear down.
        /// </summary>
        [OneTimeTearDown]
        public void ClassTearDown()
        {
            ServiceProvider?.Dispose();
        }

        /// <summary>
        /// Sets up.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            ServiceScope = ServiceProvider.CreateScope();
            Processor = ServiceScope.ServiceProvider.GetRequiredService<PostgresProcessor>();
        }

        /// <summary>
        /// Tears down.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            ServiceScope?.Dispose();
        }
    }
}
