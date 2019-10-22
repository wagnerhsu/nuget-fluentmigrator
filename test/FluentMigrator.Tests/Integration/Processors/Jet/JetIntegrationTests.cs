// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="JetIntegrationTests.cs" company="FluentMigrator Project">
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
using System.Data.OleDb;
using System.IO;

using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors.Jet;

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

namespace FluentMigrator.Tests.Integration.Processors.Jet
{
    /// <summary>
    /// Class JetIntegrationTests.
    /// </summary>
    [Category("Integration")]
    [Category("Jet")]
    public abstract class JetIntegrationTests
    {
        /// <summary>
        /// The temporary data directory
        /// </summary>
        private string _tempDataDirectory;

        /// <summary>
        /// Gets or sets the database filename.
        /// </summary>
        /// <value>The database filename.</value>
        private string DatabaseFilename { get; set; }
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
        protected JetProcessor Processor { get; private set; }

        /// <summary>
        /// Classes the set up.
        /// </summary>
        [OneTimeSetUp]
        public void ClassSetUp()
        {
            if (!IntegrationTestOptions.Jet.IsEnabled)
                Assert.Ignore();

            var serivces = ServiceCollectionExtensions.CreateServices()
                .ConfigureRunner(builder => builder.AddJet())
                .AddScoped<IConnectionStringReader>(
                    _ => new PassThroughConnectionStringReader(IntegrationTestOptions.Jet.ConnectionString));
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
            if (!HostUtilities.TryGetJetCatalogType(out var jetCatalogType))
                Assert.Ignore("ADOX.Catalog could not be found - running from .NET Core?");

            _tempDataDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempDataDirectory);
            AppDomain.CurrentDomain.SetData("DataDirectory", _tempDataDirectory);

            var csb = new OleDbConnectionStringBuilder(IntegrationTestOptions.Jet.ConnectionString);
            csb.DataSource = DatabaseFilename = HostUtilities.ReplaceDataDirectory(csb.DataSource);

            try
            {
                RecreateDatabase(jetCatalogType, csb.ConnectionString);
            }
            catch (Exception ex)
            {
                try
                {
                    Directory.Delete(_tempDataDirectory);
                }
                catch
                {
                    // Ignore errors
                }

                TestContext.Error.WriteLine(ex.ToString());
                Assert.Ignore(ex.Message);
            }

            ServiceScope = ServiceProvider.CreateScope();
            Processor = ServiceScope.ServiceProvider.GetRequiredService<JetProcessor>();
        }

        /// <summary>
        /// Tears down.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            ServiceScope?.Dispose();

            if (!string.IsNullOrEmpty(_tempDataDirectory) && Directory.Exists(_tempDataDirectory))
            {
                try
                {
                    Directory.Delete(_tempDataDirectory, true);
                }
                catch
                {
                    // Ignore exceptions - we need to find out later why this happens
                }
            }
        }

        /// <summary>
        /// Recreates the database.
        /// </summary>
        /// <param name="jetCatalogType">Type of the jet catalog.</param>
        /// <param name="connString">The connection string.</param>
        private void RecreateDatabase(Type jetCatalogType, string connString)
        {
            if (File.Exists(DatabaseFilename))
            {
                File.Delete(DatabaseFilename);
            }

            if (jetCatalogType != null)
            {
                dynamic cat = Activator.CreateInstance(jetCatalogType);
                cat.Create(connString);
            }
        }
    }
}
