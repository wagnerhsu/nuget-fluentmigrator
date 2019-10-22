// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SqlServerSequenceTests.cs" company="FluentMigrator Project">
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
using FluentMigrator.Runner.Processors.SqlServer;
using FluentMigrator.Tests.Helpers;

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Integration.Processors.SqlServer.SqlServer2016
{
    /// <summary>
    /// Defines test class SqlServerSequenceTests.
    /// Implements the <see cref="FluentMigrator.Tests.Integration.Processors.BaseSequenceTests" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Integration.Processors.BaseSequenceTests" />
    [TestFixture]
    [Category("Integration")]
    [Category("SqlServer2016")]
    public class SqlServerSequenceTests : BaseSequenceTests
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
        /// Defines the test method CallingSequenceExistsReturnsFalseIfSequenceDoesNotExist.
        /// </summary>
        [Test]
        public override void CallingSequenceExistsReturnsFalseIfSequenceDoesNotExist()
        {
            Processor.SequenceExists(null, "DoesNotExist").ShouldBeFalse();
        }

        /// <summary>
        /// Defines the test method CallingSequenceExistsReturnsFalseIfSequenceDoesNotExistWithSchema.
        /// </summary>
        [Test]
        public override void CallingSequenceExistsReturnsFalseIfSequenceDoesNotExistWithSchema()
        {
            Processor.SequenceExists("test_schema", "DoesNotExist").ShouldBeFalse();
        }

        /// <summary>
        /// Defines the test method CallingSequenceExistsReturnsTrueIfSequenceExists.
        /// </summary>
        [Test]
        public override void CallingSequenceExistsReturnsTrueIfSequenceExists()
        {
            using (new SqlServerTestSequence(Processor, null, "test_sequence"))
                Processor.SequenceExists(null, "test_sequence").ShouldBeTrue();
        }

        /// <summary>
        /// Defines the test method CallingSequenceExistsReturnsTrueIfSequenceExistsWithSchema.
        /// </summary>
        [Test]
        public override void CallingSequenceExistsReturnsTrueIfSequenceExistsWithSchema()
        {
            using (new SqlServerTestSequence(Processor, "test_schema", "test_sequence"))
                Processor.SequenceExists("test_schema", "test_sequence").ShouldBeTrue();
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
