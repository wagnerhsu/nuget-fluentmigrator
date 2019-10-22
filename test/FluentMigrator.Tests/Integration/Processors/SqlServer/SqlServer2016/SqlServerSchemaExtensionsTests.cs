// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SqlServerSchemaExtensionsTests.cs" company="FluentMigrator Project">
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

using FluentMigrator.Runner;
using FluentMigrator.Runner.Generators.SqlServer;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors.SqlServer;
using FluentMigrator.Tests.Helpers;

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Integration.Processors.SqlServer.SqlServer2016
{
    /// <summary>
    /// Defines test class SqlServerSchemaExtensionsTests.
    /// Implements the <see cref="FluentMigrator.Tests.Integration.Processors.BaseSchemaExtensionsTests" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Integration.Processors.BaseSchemaExtensionsTests" />
    [TestFixture]
    [Category("Integration")]
    [Category("SqlServer2016")]
    public class SqlServerSchemaExtensionsTests : BaseSchemaExtensionsTests
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
        private SqlServer2016Processor Processor { get; set; }
        /// <summary>
        /// Gets or sets the quoter.
        /// </summary>
        /// <value>The quoter.</value>
        private SqlServer2008Quoter Quoter { get; set; }

        /// <summary>
        /// Defines the test method CallingColumnExistsCanAcceptSchemaNameWithSingleQuote.
        /// </summary>
        [Test]
        public override void CallingColumnExistsCanAcceptSchemaNameWithSingleQuote()
        {
            using (var table = new SqlServerTestTable(Processor, "test'schema", "id int"))
                Processor.ColumnExists("test'schema", table.Name, "id").ShouldBeTrue();
        }

        /// <summary>
        /// Defines the test method CallingConstraintExistsCanAcceptSchemaNameWithSingleQuote.
        /// </summary>
        [Test]
        public override void CallingConstraintExistsCanAcceptSchemaNameWithSingleQuote()
        {
            using (var table = new SqlServerTestTable(Processor, "test'schema", "id int", "wibble int CONSTRAINT c1 CHECK(wibble > 0)"))
                Processor.ConstraintExists("test'schema", table.Name, "c1").ShouldBeTrue();
        }

        /// <summary>
        /// Defines the test method CallingIndexExistsCanAcceptSchemaNameWithSingleQuote.
        /// </summary>
        [Test]
        public override void CallingIndexExistsCanAcceptSchemaNameWithSingleQuote()
        {
            using (var table = new SqlServerTestTable(Processor, "test'schema", "id int"))
            {
                var indexName = table.WithIndexOn("id");
                Processor.IndexExists("test'schema", table.Name, indexName).ShouldBeTrue();
            }
        }

        /// <summary>
        /// Defines the test method CallingSchemaExistsCanAcceptSchemaNameWithSingleQuote.
        /// </summary>
        [Test]
        public override void CallingSchemaExistsCanAcceptSchemaNameWithSingleQuote()
        {
            using (new SqlServerTestTable(Processor, "test'schema", Quoter.QuoteColumnName("id") + " int"))
                Processor.SchemaExists("test'schema").ShouldBeTrue();
        }

        /// <summary>
        /// Defines the test method CallingTableExistsCanAcceptSchemaNameWithSingleQuote.
        /// </summary>
        [Test]
        public override void CallingTableExistsCanAcceptSchemaNameWithSingleQuote()
        {
            using (var table = new SqlServerTestTable(Processor, "test'schema", "id int"))
                Processor.TableExists("test'schema", table.Name).ShouldBeTrue();
        }

        /// <summary>
        /// Defines the test method CallingDefaultValueExistsCanAcceptSchemaNameWithSingleQuote.
        /// </summary>
        [Test]
        public void CallingDefaultValueExistsCanAcceptSchemaNameWithSingleQuote()
        {
            using (var table = new SqlServerTestTable(Processor, "test'schema", "id int"))
            {
                table.WithDefaultValueOn("id");
                Processor.DefaultValueExists("test'schema", table.Name, "id", 1).ShouldBeTrue();
            }
        }

        /// <summary>
        /// Classes the set up.
        /// </summary>
        [OneTimeSetUp]
        public void ClassSetUp()
        {
            if (!IntegrationTestOptions.SqlServer2016.IsEnabled)
                Assert.Ignore();

            var serivces = ServiceCollectionExtensions.CreateServices()
                .ConfigureRunner(r => r.AddSqlServer2016())
                .AddScoped<IConnectionStringReader>(
                    _ => new PassThroughConnectionStringReader(IntegrationTestOptions.SqlServer2016.ConnectionString));
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
            ServiceScope = ServiceProvider.CreateScope();
            Processor = ServiceScope.ServiceProvider.GetRequiredService<SqlServer2016Processor>();
            Quoter = ServiceScope.ServiceProvider.GetRequiredService<SqlServer2008Quoter>();
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
