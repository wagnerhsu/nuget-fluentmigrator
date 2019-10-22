// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="BatchParserTests.cs" company="FluentMigrator Project">
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
using System.Data.Common;
using System.Linq;

using FluentMigrator.Runner.BatchParser;
using FluentMigrator.Runner.Generators.SqlServer;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Runner.Processors.SqlServer;
using FluentMigrator.Tests.Logging;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Moq;

using NUnit.Framework;

namespace FluentMigrator.Tests.Unit.Processors.SqlServer2000
{
    /// <summary>
    /// Class BatchParserTests.
    /// Implements the <see cref="FluentMigrator.Tests.Unit.Processors.ProcessorBatchParserTestsBase" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Unit.Processors.ProcessorBatchParserTestsBase" />
    [Category("SqlServer2000")]
    public class BatchParserTests : ProcessorBatchParserTestsBase
    {
        /// <summary>
        /// Creates the processor.
        /// </summary>
        /// <returns>IMigrationProcessor.</returns>
        protected override IMigrationProcessor CreateProcessor()
        {
            var mockedConnStringReader = new Mock<IConnectionStringReader>();
            mockedConnStringReader.SetupGet(r => r.Priority).Returns(0);
            mockedConnStringReader.Setup(r => r.GetConnectionString(It.IsAny<string>())).Returns("server=this");

            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddSingleton<ILoggerProvider, TestLoggerProvider>()
                .AddTransient<SqlServerBatchParser>()
                .BuildServiceProvider();

            var logger = serviceProvider.GetRequiredService<ILogger<SqlServer2000Processor>>();

            var opt = new OptionsManager<ProcessorOptions>(new OptionsFactory<ProcessorOptions>(
                Enumerable.Empty<IConfigureOptions<ProcessorOptions>>(),
                Enumerable.Empty<IPostConfigureOptions<ProcessorOptions>>()));
            return new Processor(
                MockedDbProviderFactory.Object,
                logger,
                new SqlServer2000Generator(),
                opt,
                MockedConnectionStringAccessor.Object,
                serviceProvider);
        }

        /// <summary>
        /// Class Processor.
        /// Implements the <see cref="FluentMigrator.Runner.Processors.SqlServer.SqlServer2000Processor" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Runner.Processors.SqlServer.SqlServer2000Processor" />
        private class Processor : SqlServer2000Processor
        {
            /// <inheritdoc />
            public Processor(DbProviderFactory factory, [NotNull] ILogger logger, [NotNull] SqlServer2000Generator generator, [NotNull] IOptionsSnapshot<ProcessorOptions> options, [NotNull] IConnectionStringAccessor connectionStringAccessor, [NotNull] IServiceProvider serviceProvider)
                : base(factory, logger, generator, options, connectionStringAccessor, serviceProvider)
            {
            }
        }
    }
}
