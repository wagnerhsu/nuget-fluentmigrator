// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="OracleIndexTestsBase.cs" company="FluentMigrator Project">
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

using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors.Oracle;
using FluentMigrator.Tests.Helpers;

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Integration.Processors.Oracle
{
    /// <summary>
    /// Class OracleIndexTestsBase.
    /// Implements the <see cref="FluentMigrator.Tests.Integration.Processors.BaseIndexTests" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Integration.Processors.BaseIndexTests" />
    [Category("Integration")]
    public abstract class OracleIndexTestsBase : BaseIndexTests
    {
        /// <summary>
        /// The schema name
        /// </summary>
        private const string SchemaName = "FMTEST";

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
        private OracleProcessorBase Processor { get; set; }

        /// <summary>
        /// Defines the test method CallingIndexExistsCanAcceptIndexNameWithSingleQuote.
        /// </summary>
        [Test]
        public override void CallingIndexExistsCanAcceptIndexNameWithSingleQuote()
        {
            using (var table = new OracleTestTable(Processor, null, "id int"))
            {
                table.WithIndexOn("ID", "UI'id");
                Processor.IndexExists(null, table.Name, "UI'id").ShouldBeTrue();
            }
        }

        /// <summary>
        /// Defines the test method CallingIndexExistsCanAcceptTableNameWithSingleQuote.
        /// </summary>
        [Test]
        public override void CallingIndexExistsCanAcceptTableNameWithSingleQuote()
        {
            using (var table = new OracleTestTable("Test'Table", Processor, null, "id int"))
            {
                table.WithIndexOn("ID");
                Processor.IndexExists(null, table.Name, "UI_id").ShouldBeTrue();
            }
        }

        /// <summary>
        /// Defines the test method CallingIndexExistsReturnsFalseIfIndexDoesNotExist.
        /// </summary>
        [Test]
        public override void CallingIndexExistsReturnsFalseIfIndexDoesNotExist()
        {
            using (var table = new OracleTestTable(Processor, null, "id int"))
            {
                table.WithIndexOn("ID");
                Processor.IndexExists(null, table.Name, "DoesNotExist").ShouldBeFalse();
            }
        }

        /// <summary>
        /// Defines the test method CallingIndexExistsReturnsFalseIfIndexDoesNotExistWithSchema.
        /// </summary>
        [Test]
        public override void CallingIndexExistsReturnsFalseIfIndexDoesNotExistWithSchema()
        {
            using (var table = new OracleTestTable(Processor, SchemaName, "id int"))
            {
                table.WithIndexOn("ID");
                Processor.IndexExists(SchemaName, table.Name, "DoesNotExist").ShouldBeFalse();
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
            Processor.IndexExists(SchemaName, "DoesNotExist", "DoesNotExist").ShouldBeFalse();
        }

        /// <summary>
        /// Defines the test method CallingIndexExistsReturnsTrueIfIndexExists.
        /// </summary>
        [Test]
        public override void CallingIndexExistsReturnsTrueIfIndexExists()
        {
            using (var table = new OracleTestTable(Processor, null, "id int"))
            {
                table.WithIndexOn("ID");
                Processor.IndexExists(null, table.Name, "UI_id").ShouldBeTrue();
            }
        }

        /// <summary>
        /// Defines the test method CallingIndexExistsReturnsTrueIfIndexExistsWithSchema.
        /// </summary>
        [Test]
        public override void CallingIndexExistsReturnsTrueIfIndexExistsWithSchema()
        {
            using (var table = new OracleTestTable(Processor, SchemaName, "id int"))
            {
                table.WithIndexOn("ID");
                Processor.IndexExists(SchemaName, table.Name, "UI_id").ShouldBeTrue();
            }
        }

        /// <summary>
        /// Classes the set up.
        /// </summary>
        [OneTimeSetUp]
        public void ClassSetUp()
        {
            if (!IntegrationTestOptions.Oracle.IsEnabled)
            {
                Assert.Ignore();
            }

            var serivces = AddOracleServices(ServiceCollectionExtensions.CreateServices())
                .AddScoped<IConnectionStringReader>(
                    _ => new PassThroughConnectionStringReader(IntegrationTestOptions.Oracle.ConnectionString));
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
            Processor = ServiceScope.ServiceProvider.GetRequiredService<OracleProcessorBase>();
        }

        /// <summary>
        /// Tears down.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            ServiceScope?.Dispose();
        }

        /// <summary>
        /// Adds the oracle services.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns>IServiceCollection.</returns>
        protected abstract IServiceCollection AddOracleServices(IServiceCollection services);
    }
}
