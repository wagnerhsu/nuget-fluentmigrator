// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="Db2TableTests.cs" company="FluentMigrator Project">
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
using FluentMigrator.Runner.Processors.DB2;
using FluentMigrator.Tests.Helpers;

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Integration.Processors.Db2
{
    /// <summary>
    /// Defines test class Db2TableTests.
    /// Implements the <see cref="FluentMigrator.Tests.Integration.Processors.BaseTableTests" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Integration.Processors.BaseTableTests" />
    [TestFixture]
    [Category("Integration")]
    [Category("Db2")]
    public class Db2TableTests : BaseTableTests
    {
        /// <summary>
        /// Initializes static members of the <see cref="Db2TableTests"/> class.
        /// </summary>
        static Db2TableTests()
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
        private Db2Processor Processor { get; set; }

        /// <summary>
        /// Defines the test method CallingTableExistsCanAcceptTableNameWithSingleQuote.
        /// </summary>
        [Test]
        public override void CallingTableExistsCanAcceptTableNameWithSingleQuote()
        {
            using (var table = new Db2TestTable("Test'Table", Processor, null, "ID INT"))
            {
                Processor.TableExists(null, table.Name).ShouldBeTrue();
            }
        }

        /// <summary>
        /// Defines the test method CallingTableExistsReturnsFalseIfTableDoesNotExist.
        /// </summary>
        [Test]
        public override void CallingTableExistsReturnsFalseIfTableDoesNotExist()
        {
            Processor.TableExists(null, "DoesNotExist").ShouldBeFalse();
        }

        /// <summary>
        /// Defines the test method CallingTableExistsReturnsFalseIfTableDoesNotExistWithSchema.
        /// </summary>
        [Test]
        public override void CallingTableExistsReturnsFalseIfTableDoesNotExistWithSchema()
        {
            Processor.TableExists("TstSchma", "DoesNotExist").ShouldBeFalse();
        }

        /// <summary>
        /// Defines the test method CallingTableExistsReturnsTrueIfTableExists.
        /// </summary>
        [Test]
        public override void CallingTableExistsReturnsTrueIfTableExists()
        {
            using (var table = new Db2TestTable(Processor, null, "ID INT"))
            {
                Processor.TableExists(null, table.Name).ShouldBeTrue();
            }
        }

        /// <summary>
        /// Defines the test method CallingTableExistsReturnsTrueIfTableExistsWithSchema.
        /// </summary>
        [Test]
        public override void CallingTableExistsReturnsTrueIfTableExistsWithSchema()
        {
            using (var table = new Db2TestTable(Processor, "TstSchma", "ID INT"))
            {
                Processor.TableExists("TstSchma", table.Name).ShouldBeTrue();
            }
        }

        /// <summary>
        /// Classes the set up.
        /// </summary>
        [OneTimeSetUp]
        public void ClassSetUp()
        {
            if (!IntegrationTestOptions.Db2.IsEnabled)
                Assert.Ignore();

            var serivces = ServiceCollectionExtensions.CreateServices()
                .ConfigureRunner(builder => builder.AddDb2())
                .AddScoped<IConnectionStringReader>(
                    _ => new PassThroughConnectionStringReader(IntegrationTestOptions.Db2.ConnectionString));
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
            Processor = ServiceScope.ServiceProvider.GetRequiredService<Db2Processor>();
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
