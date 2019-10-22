// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="FirebirdTableTests.cs" company="FluentMigrator Project">
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
using FluentMigrator.Runner.Processors.Firebird;
using FluentMigrator.Tests.Helpers;

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Integration.Processors.Firebird
{
    /// <summary>
    /// Defines test class FirebirdTableTests.
    /// Implements the <see cref="FluentMigrator.Tests.Integration.Processors.BaseTableTests" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Integration.Processors.BaseTableTests" />
    [TestFixture]
    [Category("Integration")]
    [Category("Firebird")]
    public class FirebirdTableTests : BaseTableTests
    {
        /// <summary>
        /// The prober
        /// </summary>
        private readonly FirebirdLibraryProber _prober = new FirebirdLibraryProber();
        /// <summary>
        /// The temporary database
        /// </summary>
        private TemporaryDatabase _temporaryDatabase;

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
        private FirebirdProcessor Processor { get; set; }

        /// <summary>
        /// Defines the test method CallingTableExistsCanAcceptTableNameWithSingleQuote.
        /// </summary>
        [Test]
        public override void CallingTableExistsCanAcceptTableNameWithSingleQuote()
        {
            using (var table = new FirebirdTestTable("\"Test'Table\"", Processor, "id int"))
                Processor.TableExists(null, table.Name).ShouldBeTrue();
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
            Processor.TableExists("TestSchema", "DoesNotExist").ShouldBeFalse();
        }

        /// <summary>
        /// Defines the test method CallingTableExistsReturnsTrueIfTableExists.
        /// </summary>
        [Test]
        public override void CallingTableExistsReturnsTrueIfTableExists()
        {
            using (var table = new FirebirdTestTable(Processor, "id int"))
                Processor.TableExists(null, table.Name).ShouldBeTrue();
        }

        /// <summary>
        /// Defines the test method CallingTableExistsReturnsTrueIfTableExistsWithSchema.
        /// </summary>
        [Test]
        public override void CallingTableExistsReturnsTrueIfTableExistsWithSchema()
        {
            using (var table = new FirebirdTestTable(Processor, "id int"))
                Processor.TableExists("TestSchema", table.Name).ShouldBeTrue();
        }

        /// <summary>
        /// Sets up.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            if (!IntegrationTestOptions.Firebird.IsEnabled)
                Assert.Ignore();

            _temporaryDatabase = new TemporaryDatabase(
                IntegrationTestOptions.Firebird,
                _prober);

            var serivces = ServiceCollectionExtensions.CreateServices()
                .ConfigureRunner(builder => builder.AddFirebird())
                .AddScoped<IConnectionStringReader>(
                    _ => new PassThroughConnectionStringReader(_temporaryDatabase.ConnectionString));

            ServiceProvider = serivces.BuildServiceProvider();
            ServiceScope = ServiceProvider.CreateScope();
            Processor = ServiceScope.ServiceProvider.GetRequiredService<FirebirdProcessor>();
        }

        /// <summary>
        /// Tears down.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            ServiceScope?.Dispose();
            ServiceProvider?.Dispose();
            if (_temporaryDatabase != null)
            {
                var connString = _temporaryDatabase.ConnectionString;
                _temporaryDatabase = null;
                FbDatabase.DropDatabase(connString);
            }
        }
    }
}
