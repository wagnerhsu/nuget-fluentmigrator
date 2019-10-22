// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="OracleProcessorTestsBase.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
// Copyright (c) 2018, Fluent Migrator Project
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using FluentMigrator.Runner.Generators.Oracle;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors.Oracle;
using FluentMigrator.Tests.Helpers;

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Integration.Processors.Oracle
{
    /// <summary>
    /// Class OracleProcessorTestsBase.
    /// </summary>
    [Category("Integration")]
    public abstract class OracleProcessorTestsBase
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
        /// Gets or sets the quoter.
        /// </summary>
        /// <value>The quoter.</value>
        private OracleQuoterBase Quoter { get; set; }

        /// <summary>
        /// Defines the test method CallingColumnExistsReturnsFalseIfColumnExistsInDifferentSchema.
        /// </summary>
        [Test]
        public void CallingColumnExistsReturnsFalseIfColumnExistsInDifferentSchema()
        {
            using (var table = new OracleTestTable(Processor, SchemaName, "id int"))
            {
                Processor.ColumnExists("testschema", table.Name, "ID").ShouldBeFalse();
            }
        }

        /// <summary>
        /// Defines the test method CallingConstraintExistsReturnsFalseIfConstraintExistsInDifferentSchema.
        /// </summary>
        [Test]
        public void CallingConstraintExistsReturnsFalseIfConstraintExistsInDifferentSchema()
        {
            using (var table = new OracleTestTable(Processor, SchemaName, "id int"))
            {
                table.WithUniqueConstraintOn("ID");
                Processor.ConstraintExists("testschema", table.Name, "UC_id").ShouldBeFalse();
            }
        }

        /// <summary>
        /// Defines the test method CallingTableExistsReturnsFalseIfTableExistsInDifferentSchema.
        /// </summary>
        [Test]
        public void CallingTableExistsReturnsFalseIfTableExistsInDifferentSchema()
        {
            using (var table = new OracleTestTable(Processor, SchemaName, "id int"))
            {
                Processor.TableExists("testschema", table.Name).ShouldBeFalse();
            }
        }

        /// <summary>
        /// Defines the test method CallingColumnExistsWithIncorrectCaseReturnsTrueIfColumnExists.
        /// </summary>
        [Test]
        public void CallingColumnExistsWithIncorrectCaseReturnsTrueIfColumnExists()
        {
            //the ColumnExisits() function is'nt case sensitive
            using (var table = new OracleTestTable(Processor, null, "id int"))
            {
                Processor.ColumnExists(null, table.Name, "Id").ShouldBeTrue();
            }
        }

        /// <summary>
        /// Defines the test method CallingConstraintExistsWithIncorrectCaseReturnsTrueIfConstraintExists.
        /// </summary>
        [Test]
        public void CallingConstraintExistsWithIncorrectCaseReturnsTrueIfConstraintExists()
        {
            //the ConstraintExists() function is'nt case sensitive
            using (var table = new OracleTestTable(Processor, null, "id int"))
            {
                table.WithUniqueConstraintOn("ID", "uc_id");
                Processor.ConstraintExists(null, table.Name, "Uc_Id").ShouldBeTrue();
            }
        }

        /// <summary>
        /// Defines the test method CallingIndexExistsWithIncorrectCaseReturnsFalseIfIndexExist.
        /// </summary>
        [Test]
        public void CallingIndexExistsWithIncorrectCaseReturnsFalseIfIndexExist()
        {
            //the IndexExists() function is'nt case sensitive
            using (var table = new OracleTestTable(Processor, null, "id int"))
            {
                table.WithIndexOn("ID", "ui_id");
                Processor.IndexExists(null, table.Name, "Ui_Id").ShouldBeTrue();
            }
        }

        /// <summary>
        /// Defines the test method TestQuery.
        /// </summary>
        [Test]
        public void TestQuery()
        {
            var sql = "SELECT SYSDATE FROM " + Quoter.QuoteTableName("DUAL");
            var ds = Processor.Read(sql);
            Assert.Greater(ds.Tables.Count, 0);
            Assert.Greater(ds.Tables[0].Columns.Count, 0);
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
            Quoter = ServiceScope.ServiceProvider.GetRequiredService<OracleQuoterBase>();
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
