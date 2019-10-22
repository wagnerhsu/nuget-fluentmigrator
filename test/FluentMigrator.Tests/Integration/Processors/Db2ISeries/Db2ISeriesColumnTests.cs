// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="Db2ISeriesColumnTests.cs" company="FluentMigrator Project">
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

using System.Diagnostics;

using FluentMigrator.Runner;
using FluentMigrator.Runner.Generators.DB2.iSeries;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors.DB2.iSeries;
using FluentMigrator.Tests.Helpers;

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Integration.Processors.Db2ISeries
{
    /// <summary>
    /// Defines test class Db2ISeriesColumnTests.
    /// Implements the <see cref="FluentMigrator.Tests.Integration.Processors.BaseColumnTests" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Integration.Processors.BaseColumnTests" />
    [TestFixture]
    [Category("DB2 iSeries")]
    public class Db2ISeriesColumnTests : BaseColumnTests
    {
        /// <summary>
        /// Initializes static members of the <see cref="Db2ISeriesColumnTests"/> class.
        /// </summary>
        static Db2ISeriesColumnTests()
        {
            try { EnsureReference(); } catch { /* ignore */ }
        }

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
        private Db2ISeriesProcessor Processor { get; set; }
        /// <summary>
        /// Gets or sets the quoter.
        /// </summary>
        /// <value>The quoter.</value>
        private Db2ISeriesQuoter Quoter { get; set; }

        /// <summary>
        /// Defines the test method CallingColumnExistsCanAcceptColumnNameWithSingleQuote.
        /// </summary>
        [Test]
        public override void CallingColumnExistsCanAcceptColumnNameWithSingleQuote()
        {
            var columnName = Quoter.Quote("I'D") + " INT";
            using (var table = new Db2ISeriesTestTable(Processor, null, columnName))
            {
                Processor.ColumnExists(null, table.Name, "I'D").ShouldBeTrue();
            }
        }

        /// <summary>
        /// Defines the test method CallingColumnExistsCanAcceptTableNameWithSingleQuote.
        /// </summary>
        [Test]
        public override void CallingColumnExistsCanAcceptTableNameWithSingleQuote()
        {
            using (var table = new Db2ISeriesTestTable("Test'Table", Processor, null, "ID INT"))
            {
                Processor.ColumnExists(null, table.Name, "ID").ShouldBeTrue();
            }
        }

        /// <summary>
        /// Defines the test method CallingColumnExistsReturnsFalseIfColumnDoesNotExist.
        /// </summary>
        [Test]
        public override void CallingColumnExistsReturnsFalseIfColumnDoesNotExist()
        {
            using (var table = new Db2ISeriesTestTable(Processor, null, "ID INT"))
            {
                Processor.ColumnExists(null, table.Name, "DoesNotExist").ShouldBeFalse();
            }
        }

        /// <summary>
        /// Defines the test method CallingColumnExistsReturnsFalseIfColumnDoesNotExistWithSchema.
        /// </summary>
        [Test]
        public override void CallingColumnExistsReturnsFalseIfColumnDoesNotExistWithSchema()
        {
            using (var table = new Db2ISeriesTestTable(Processor, "TstSchma", "ID INT"))
            {
                Processor.ColumnExists("TstSchma", table.Name, "DoesNotExist").ShouldBeFalse();
            }
        }

        /// <summary>
        /// Defines the test method CallingColumnExistsReturnsFalseIfTableDoesNotExist.
        /// </summary>
        [Test]
        public override void CallingColumnExistsReturnsFalseIfTableDoesNotExist()
        {
            Processor.ColumnExists(null, "DoesNotExist", "DoesNotExist").ShouldBeFalse();
        }

        /// <summary>
        /// Defines the test method CallingColumnExistsReturnsFalseIfTableDoesNotExistWithSchema.
        /// </summary>
        [Test]
        public override void CallingColumnExistsReturnsFalseIfTableDoesNotExistWithSchema()
        {
            Processor.ColumnExists("TstSchma", "DoesNotExist", "DoesNotExist").ShouldBeFalse();
        }

        /// <summary>
        /// Defines the test method CallingColumnExistsReturnsTrueIfColumnExists.
        /// </summary>
        [Test]
        public override void CallingColumnExistsReturnsTrueIfColumnExists()
        {
            using (var table = new Db2ISeriesTestTable(Processor, null, "ID INT"))
            {
                Processor.ColumnExists(null, table.Name, "ID").ShouldBeTrue();
            }
        }

        /// <summary>
        /// Defines the test method CallingColumnExistsReturnsTrueIfColumnExistsWithSchema.
        /// </summary>
        [Test]
        public override void CallingColumnExistsReturnsTrueIfColumnExistsWithSchema()
        {
            using (var table = new Db2ISeriesTestTable(Processor, "TstSchma", "ID INT"))
            {
                Processor.ColumnExists("TstSchma", table.Name, "ID").ShouldBeTrue();
            }
        }

        /// <summary>
        /// Classes the set up.
        /// </summary>
        [OneTimeSetUp]
        public void ClassSetUp()
        {
            if (!IntegrationTestOptions.Db2ISeries.IsEnabled)
                Assert.Ignore();

            var serivces = ServiceCollectionExtensions.CreateServices()
                .ConfigureRunner(builder => builder.AddDb2ISeries())
                .AddScoped<IConnectionStringReader>(
                    _ => new PassThroughConnectionStringReader(IntegrationTestOptions.Db2ISeries.ConnectionString));
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
            Processor = ServiceScope.ServiceProvider.GetRequiredService<Db2ISeriesProcessor>();
            Quoter = ServiceScope.ServiceProvider.GetRequiredService<Db2ISeriesQuoter>();
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
        /// Ensures the reference.
        /// </summary>
        private static void EnsureReference()
        {
            // This is here to avoid the removal of the referenced assembly
            Debug.WriteLine(typeof(IBM.Data.DB2.DB2Factory));
        }
    }
}
