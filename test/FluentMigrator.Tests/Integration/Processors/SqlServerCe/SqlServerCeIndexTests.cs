// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SqlServerCeIndexTests.cs" company="FluentMigrator Project">
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
using System.Data.SqlServerCe;
using System.IO;

using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors.SqlServer;
using FluentMigrator.Tests.Helpers;

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Integration.Processors.SqlServerCe
{
    /// <summary>
    /// Defines test class SqlServerCeIndexTests.
    /// Implements the <see cref="FluentMigrator.Tests.Integration.Processors.BaseIndexTests" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Integration.Processors.BaseIndexTests" />
    [TestFixture]
    [Category("Integration")]
    [Category("SqlServerCe")]
    public class SqlServerCeIndexTests : BaseIndexTests
    {
        /// <summary>
        /// The temporary data directory
        /// </summary>
        private string _tempDataDirectory;

        /// <summary>
        /// Gets or sets the database filename.
        /// </summary>
        /// <value>The database filename.</value>
        private string DatabaseFilename { get; set; }
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
        private SqlServerCeProcessor Processor { get; set; }

        /// <summary>
        /// Defines the test method CallingIndexExistsCanAcceptIndexNameWithSingleQuote.
        /// </summary>
        [Test]
        public override void CallingIndexExistsCanAcceptIndexNameWithSingleQuote()
        {
            using (var table = new SqlServerCeTestTable(Processor, "id int"))
            {
                table.WithIndexOn("id", "UI'id");
                Processor.IndexExists("NOTUSED", table.Name, "UI'id").ShouldBeTrue();
            }
        }

        /// <summary>
        /// Defines the test method CallingIndexExistsCanAcceptTableNameWithSingleQuote.
        /// </summary>
        [Test]
        public override void CallingIndexExistsCanAcceptTableNameWithSingleQuote()
        {
            using (var table = new SqlServerCeTestTable("Test'Table", Processor, "id int"))
            {
                table.WithIndexOn("id");
                Processor.IndexExists("NOTUSED", table.Name, "UI_id").ShouldBeTrue();
            }
        }

        /// <summary>
        /// Defines the test method CallingIndexExistsReturnsFalseIfIndexDoesNotExist.
        /// </summary>
        [Test]
        public override void CallingIndexExistsReturnsFalseIfIndexDoesNotExist()
        {
            using (var table = new SqlServerCeTestTable(Processor, "id int"))
            {
                table.WithIndexOn("id");
                Processor.IndexExists(null, table.Name, "DoesNotExist").ShouldBeFalse();
            }
        }

        /// <summary>
        /// Defines the test method CallingIndexExistsReturnsFalseIfIndexDoesNotExistWithSchema.
        /// </summary>
        [Test]
        public override void CallingIndexExistsReturnsFalseIfIndexDoesNotExistWithSchema()
        {
            using (var table = new SqlServerCeTestTable(Processor, "id int"))
            {
                table.WithIndexOn("id");
                Processor.IndexExists("NOTUSED", table.Name, "DoesNotExist").ShouldBeFalse();
            }
        }

        /// <summary>
        /// Defines the test method CallingIndexExistsReturnsFalseIfTableDoesNotExist.
        /// </summary>
        [Test]
        public override void CallingIndexExistsReturnsFalseIfTableDoesNotExist()
        {
            Processor.IndexExists(null, "DoesNotExist", "DoesNotExist").ShouldBeFalse();
        }

        /// <summary>
        /// Defines the test method CallingIndexExistsReturnsFalseIfTableDoesNotExistWithSchema.
        /// </summary>
        [Test]
        public override void CallingIndexExistsReturnsFalseIfTableDoesNotExistWithSchema()
        {
            Processor.IndexExists("NOTUSED", "DoesNotExist", "DoesNotExist").ShouldBeFalse();
        }


        /// <summary>
        /// Defines the test method CallingIndexExistsReturnsTrueIfIndexExists.
        /// </summary>
        [Test]
        public override void CallingIndexExistsReturnsTrueIfIndexExists()
        {
            using (var table = new SqlServerCeTestTable(Processor, "id int"))
            {
                table.WithIndexOn("id");
                Processor.IndexExists(null, table.Name, "UI_id").ShouldBeTrue();
            }
        }

        /// <summary>
        /// Defines the test method CallingIndexExistsReturnsTrueIfIndexExistsWithSchema.
        /// </summary>
        [Test]
        public override void CallingIndexExistsReturnsTrueIfIndexExistsWithSchema()
        {
            using (var table = new SqlServerCeTestTable(Processor, "id int"))
            {
                table.WithIndexOn("id");
                Processor.IndexExists("NOTUSED", table.Name, "UI_id").ShouldBeTrue();
            }
        }

        /// <summary>
        /// Classes the set up.
        /// </summary>
        [OneTimeSetUp]
        public void ClassSetUp()
        {
            if (!IntegrationTestOptions.SqlServerCe.IsEnabled)
                Assert.Ignore();

            if (!HostUtilities.ProbeSqlServerCeBehavior())
            {
                Assert.Ignore("SQL Server CE binaries not found");
            }

            var serivces = ServiceCollectionExtensions.CreateServices()
                .ConfigureRunner(r => r.AddSqlServerCe())
                .AddScoped<IConnectionStringReader>(
                    _ => new PassThroughConnectionStringReader(IntegrationTestOptions.SqlServerCe.ConnectionString));
            ServiceProvider = serivces.BuildServiceProvider();
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
            _tempDataDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempDataDirectory);
            AppDomain.CurrentDomain.SetData("DataDirectory", _tempDataDirectory);

            var csb = new SqlCeConnectionStringBuilder(IntegrationTestOptions.SqlServerCe.ConnectionString);
            DatabaseFilename = HostUtilities.ReplaceDataDirectory(csb.DataSource);
            RecreateDatabase();

            ServiceScope = ServiceProvider.CreateScope();
            Processor = ServiceScope.ServiceProvider.GetRequiredService<SqlServerCeProcessor>();
        }

        /// <summary>
        /// Tears down.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            ServiceScope?.Dispose();

            if (!string.IsNullOrEmpty(_tempDataDirectory) && Directory.Exists(_tempDataDirectory))
            {
                Directory.Delete(_tempDataDirectory, true);
            }
        }

        /// <summary>
        /// Recreates the database.
        /// </summary>
        private void RecreateDatabase()
        {
            if (File.Exists(DatabaseFilename))
            {
                File.Delete(DatabaseFilename);
            }

            new SqlCeEngine(IntegrationTestOptions.SqlServerCe.ConnectionString).CreateDatabase();
        }
    }
}
