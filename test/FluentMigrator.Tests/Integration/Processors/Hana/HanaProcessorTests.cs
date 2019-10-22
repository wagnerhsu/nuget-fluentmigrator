// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="HanaProcessorTests.cs" company="FluentMigrator Project">
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

using System.IO;

using FluentMigrator.Expressions;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Logging;
using FluentMigrator.Runner.Processors.Hana;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Integration.Processors.Hana
{
    /// <summary>
    /// Defines test class HanaProcessorTests.
    /// </summary>
    [TestFixture]
    [Category("Integration")]
    [Category("Hana")]
    public class HanaProcessorTests
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
        /// Gets or sets the output.
        /// </summary>
        /// <value>The output.</value>
        private StringWriter Output { get; set; }

        /// <summary>
        /// Defines the test method CallingProcessWithPerformDbOperationExpressionWhenInPreviewOnlyModeWillNotMakeDbChanges.
        /// </summary>
        [Test]
        public void CallingProcessWithPerformDbOperationExpressionWhenInPreviewOnlyModeWillNotMakeDbChanges()
        {
            var expression =
                new PerformDBOperationExpression
                {
                    Operation = (con, trans) =>
                    {
                        var command = con.CreateCommand();
                        command.CommandText = "CREATE TABLE ProcessTestTable (test int NULL) ";
                        command.Transaction = trans;

                        command.ExecuteNonQuery();
                    }
                };

            Processor.Process(expression);

            var tableExists = Processor.TableExists("", "ProcessTestTable");

            tableExists.ShouldBeFalse();

            var fmOutput = Output.ToString();
            Assert.That(fmOutput, Does.Contain("/* Performing DB Operation */"));
        }

        /// <summary>
        /// Sets up.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            if (!IntegrationTestOptions.Hana.IsEnabled)
                Assert.Ignore();

            Output = new StringWriter();
            var serivces = ServiceCollectionExtensions.CreateServices()
                .ConfigureRunner(builder => builder.AddHana().AsGlobalPreview())
                .AddSingleton<ILoggerProvider>(new SqlScriptFluentMigratorLoggerProvider(Output))
                .AddScoped<IConnectionStringReader>(
                    _ => new PassThroughConnectionStringReader(IntegrationTestOptions.Hana.ConnectionString));
            ServiceProvider = serivces.BuildServiceProvider();
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
            ServiceProvider?.Dispose();
        }
    }
}
