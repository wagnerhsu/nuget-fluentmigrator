// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="Db2ISeriesProcessorTests.cs" company="FluentMigrator Project">
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
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors.DB2.iSeries;
using FluentMigrator.Tests.Helpers;

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Integration.Processors.Db2ISeries
{
    /// <summary>
    /// Defines test class Db2ISeriesProcessorTests.
    /// </summary>
    [TestFixture]
    [Category("Integration")]
    [Category("DB2 iSeries")]
    public class Db2ISeriesProcessorTests
    {
        /// <summary>
        /// Initializes static members of the <see cref="Db2ISeriesProcessorTests"/> class.
        /// </summary>
        static Db2ISeriesProcessorTests()
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
        /// Defines the test method CallingColumnExistsReturnsFalseIfColumnExistsInDifferentSchema.
        /// </summary>
        [Test]
        public void CallingColumnExistsReturnsFalseIfColumnExistsInDifferentSchema()
        {
            using (var table = new Db2ISeriesTestTable(Processor, "TstSchma", "ID INT"))
            {
                Processor.ColumnExists("DNE", table.Name, "ID").ShouldBeFalse();
            }
        }

        /// <summary>
        /// Defines the test method CallingConstraintExistsReturnsFalseIfConstraintExistsInDifferentSchema.
        /// </summary>
        [Test]
        public void CallingConstraintExistsReturnsFalseIfConstraintExistsInDifferentSchema()
        {
            using (var table = new Db2ISeriesTestTable(Processor, "TstSchma", "ID INT NOT NULL"))
            {
                table.WithUniqueConstraintOn("ID", "c1");
                Processor.ConstraintExists("DNE", table.Name, "c1").ShouldBeFalse();
            }
        }

        /// <summary>
        /// Defines the test method CallingTableExistsReturnsFalseIfTableExistsInDifferentSchema.
        /// </summary>
        [Test]
        public void CallingTableExistsReturnsFalseIfTableExistsInDifferentSchema()
        {
            using (var table = new Db2ISeriesTestTable(Processor, "TstSchma", "ID INT"))
            {
                Processor.TableExists("DNE", table.Name).ShouldBeFalse();
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
