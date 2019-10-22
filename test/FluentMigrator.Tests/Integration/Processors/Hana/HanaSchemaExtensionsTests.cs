// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="HanaSchemaExtensionsTests.cs" company="FluentMigrator Project">
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
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors.Hana;
using FluentMigrator.Tests.Helpers;

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Integration.Processors.Hana
{
    /// <summary>
    /// Defines test class HanaSchemaExtensionsTests.
    /// Implements the <see cref="FluentMigrator.Tests.Integration.Processors.BaseSchemaExtensionsTests" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Integration.Processors.BaseSchemaExtensionsTests" />
    [TestFixture]
    [Category("Integration")]
    [Category("Hana")]
    public class HanaSchemaExtensionsTests : BaseSchemaExtensionsTests
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
        private HanaProcessor Processor { get; set; }

        /// <summary>
        /// Defines the test method CallingColumnExistsCanAcceptSchemaNameWithSingleQuote.
        /// </summary>
        [Test]
        public override void CallingColumnExistsCanAcceptSchemaNameWithSingleQuote()
        {
            using (var table = new HanaTestTable(Processor, "test'schema", "id int"))
                Processor.ColumnExists("test'schema", table.Name, "id").ShouldBeTrue();
        }

        /// <summary>
        /// Defines the test method CallingConstraintExistsCanAcceptSchemaNameWithSingleQuote.
        /// </summary>
        [Test]
        public override void CallingConstraintExistsCanAcceptSchemaNameWithSingleQuote()
        {
            Assert.Ignore("Hana doesn't support check constraints");
        }

        /// <summary>
        /// Defines the test method CallingIndexExistsCanAcceptSchemaNameWithSingleQuote.
        /// </summary>
        [Test]
        public override void CallingIndexExistsCanAcceptSchemaNameWithSingleQuote()
        {
            using (var table = new HanaTestTable(Processor, "test'schema", "\"id\" int"))
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
            Assert.Ignore("Schemas aren't supported by this SAP Hana runner");
        }

        /// <summary>
        /// Defines the test method CallingTableExistsCanAcceptSchemaNameWithSingleQuote.
        /// </summary>
        [Test]
        public override void CallingTableExistsCanAcceptSchemaNameWithSingleQuote()
        {
            using (var table = new HanaTestTable(Processor, "test'schema", "id int"))
                Processor.TableExists("test'schema", table.Name).ShouldBeTrue();
        }

        /// <summary>
        /// Defines the test method CallingDefaultValueExistsCanAcceptSchemaNameWithSingleQuote.
        /// </summary>
        [Test]
        public void CallingDefaultValueExistsCanAcceptSchemaNameWithSingleQuote()
        {
            Assert.Ignore("Hana doesn't support changing a columns default constraint");
        }

        /// <summary>
        /// Classes the set up.
        /// </summary>
        [OneTimeSetUp]
        public void ClassSetUp()
        {
            if (!IntegrationTestOptions.Hana.IsEnabled)
                Assert.Ignore();

            var serivces = ServiceCollectionExtensions.CreateServices()
                .ConfigureRunner(builder => builder.AddHana())
                .AddScoped<IConnectionStringReader>(
                    _ => new PassThroughConnectionStringReader(IntegrationTestOptions.Hana.ConnectionString));
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
            Processor = ServiceScope.ServiceProvider.GetRequiredService<HanaProcessor>();
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
