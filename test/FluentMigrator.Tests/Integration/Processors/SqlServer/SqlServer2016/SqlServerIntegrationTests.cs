// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SqlServerIntegrationTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
// Copyright (c) 2018, FluentMigrator Project
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

using System;

using FluentMigrator.Runner;
using FluentMigrator.Runner.Generators.SqlServer;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors.SqlServer;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

namespace FluentMigrator.Tests.Integration.Processors.SqlServer.SqlServer2016
{
    /// <summary>
    /// Class SqlServerIntegrationTests.
    /// </summary>
    [Category("Integration")]
    [Category("SqlServer2016")]
    public abstract class SqlServerIntegrationTests
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
        /// Gets the processor.
        /// </summary>
        /// <value>The processor.</value>
        protected SqlServer2016Processor Processor { get; private set; }
        /// <summary>
        /// Gets the quoter.
        /// </summary>
        /// <value>The quoter.</value>
        protected SqlServer2008Quoter Quoter { get; private set; }

        /// <summary>
        /// Classes the set up.
        /// </summary>
        [OneTimeSetUp]
        public void ClassSetUp()
        {
            ServiceProvider = CreateProcessorServices(initAction: null);
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

        /// <summary>
        /// Creates the processor services.
        /// </summary>
        /// <param name="initAction">The initialize action.</param>
        /// <returns>ServiceProvider.</returns>
        protected ServiceProvider CreateProcessorServices([CanBeNull] Action<IServiceCollection> initAction)
        {
            if (!IntegrationTestOptions.SqlServer2016.IsEnabled)
                Assert.Ignore();

            var serivces = ServiceCollectionExtensions.CreateServices()
                .ConfigureRunner(r => r.AddSqlServer2016())
                .AddScoped<IConnectionStringReader>(
                    _ => new PassThroughConnectionStringReader(IntegrationTestOptions.SqlServer2016.ConnectionString));

            initAction?.Invoke(serivces);

            return serivces.BuildServiceProvider();
        }
    }
}
