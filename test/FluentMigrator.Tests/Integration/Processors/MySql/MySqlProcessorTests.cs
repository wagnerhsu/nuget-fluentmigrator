// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="MySqlProcessorTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.IO;

using FluentMigrator.Expressions;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors.MySql;
using FluentMigrator.Tests.Logging;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Integration.Processors.MySql
{
    /// <summary>
    /// Defines test class MySqlProcessorTests.
    /// </summary>
    [TestFixture]
    [Category("Integration")]
    [Category("MySql")]
    public class MySqlProcessorTests
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
        private MySql4Processor Processor { get; set; }

        /// <summary>
        /// Defines the test method CallingProcessWithPerformDBOperationExpressionWhenInPreviewOnlyModeWillNotMakeDbChanges.
        /// </summary>
        [Test]
        public void CallingProcessWithPerformDBOperationExpressionWhenInPreviewOnlyModeWillNotMakeDbChanges()
        {
            var output = new StringWriter();

            var sp = CreateProcessorServices(sc => sc
                .AddSingleton<ILoggerProvider>(new TextWriterLoggerProvider(output))
                .ConfigureRunner(r => r.AsGlobalPreview()));

            using (sp)
            {
                using (var scope = sp.CreateScope())
                {
                    var processor = scope.ServiceProvider.GetRequiredService<IMigrationProcessor>();
                    bool tableExists;

                    try
                    {
                        var expression =
                            new PerformDBOperationExpression
                            {
                                Operation = (con, trans) =>
                                {
                                    var command = con.CreateCommand();
                                    command.CommandText = "CREATE TABLE processtesttable (test int NULL) ";
                                    command.Transaction = trans;

                                    command.ExecuteNonQuery();
                                }
                            };

                        processor.Process(expression);

                        tableExists = processor.TableExists("", "processtesttable");
                    }
                    finally
                    {
                        processor.RollbackTransaction();
                    }

                    tableExists.ShouldBeFalse();
                }
            }
        }

        /// <summary>
        /// Defines the test method CallingExecuteWithPerformDBOperationExpressionWhenInPreviewOnlyModeWillNotMakeDbChanges.
        /// </summary>
        [Test]
        public void CallingExecuteWithPerformDBOperationExpressionWhenInPreviewOnlyModeWillNotMakeDbChanges()
        {
            var output = new StringWriter();

            var sp = CreateProcessorServices(sc => sc
                .AddSingleton<ILoggerProvider>(new TextWriterLoggerProvider(output))
                .ConfigureRunner(r => r.AsGlobalPreview()));

            using (sp)
            {
                using (var scope = sp.CreateScope())
                {
                    var processor = scope.ServiceProvider.GetRequiredService<IMigrationProcessor>();
                    bool tableExists;

                    try
                    {
                        processor.Execute("CREATE TABLE processtesttable (test int NULL) ");

                        tableExists = processor.TableExists("", "processtesttable");
                    }
                    finally
                    {
                        processor.RollbackTransaction();
                    }

                    tableExists.ShouldBeFalse();
                }
            }
        }

        /// <summary>
        /// Defines the test method CallingDefaultValueExistsReturnsTrueWhenMatches.
        /// </summary>
        [Test]
        public void CallingDefaultValueExistsReturnsTrueWhenMatches()
        {
            try
            {
                Processor.Execute("CREATE TABLE dftesttable (test int NULL DEFAULT 1) ");
                Processor.DefaultValueExists(null, "dftesttable", "test", 1).ShouldBeTrue();
            }
            finally
            {
                Processor.Execute("DROP TABLE dftesttable");
            }
        }

        /// <summary>
        /// Defines the test method CallingReadTableDataQuotesTableName.
        /// </summary>
        [Test]
        public void CallingReadTableDataQuotesTableName()
        {
            try
            {
                Processor.Execute("CREATE TABLE `infrastructure.version` (test int null) ");
                Processor.ReadTableData(null, "infrastructure.version");
            }
            finally
            {
                Processor.Execute("DROP TABLE `infrastructure.version`");
            }
        }

        /// <summary>
        /// Creates the processor services.
        /// </summary>
        /// <param name="initAction">The initialize action.</param>
        /// <returns>ServiceProvider.</returns>
        private static ServiceProvider CreateProcessorServices([CanBeNull] Action<IServiceCollection> initAction)
        {
            if (!IntegrationTestOptions.MySql.IsEnabled)
                Assert.Ignore();

            var serivces = ServiceCollectionExtensions.CreateServices()
                .ConfigureRunner(builder => builder.AddMySql4())
                .AddScoped<IConnectionStringReader>(
                    _ => new PassThroughConnectionStringReader(IntegrationTestOptions.MySql.ConnectionString));
            initAction?.Invoke(serivces);
            return serivces.BuildServiceProvider();
        }

        /// <summary>
        /// Classes the set up.
        /// </summary>
        [OneTimeSetUp]
        public void ClassSetUp()
        {
            ServiceProvider = CreateProcessorServices(null);
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
            Processor = ServiceScope.ServiceProvider.GetRequiredService<MySql4Processor>();
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
