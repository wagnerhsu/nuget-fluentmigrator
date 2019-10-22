// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="OracleProcessorFactoryTestsBase.cs" company="FluentMigrator Project">
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

using System;

using FluentMigrator.Runner;
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Generators.Oracle;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Runner.Processors.Oracle;

using NUnit.Framework;

namespace FluentMigrator.Tests.Integration.Processors.Oracle
{
    /// <summary>
    /// Class OracleProcessorFactoryTestsBase.
    /// </summary>
    [Category("Integration")]
    [Obsolete]
    public abstract class OracleProcessorFactoryTestsBase
    {
        /// <summary>
        /// The factory
        /// </summary>
        private IMigrationProcessorFactory _factory;
        /// <summary>
        /// The connection string
        /// </summary>
        private string _connectionString;
        /// <summary>
        /// The announcer
        /// </summary>
        private IAnnouncer _announcer;
        /// <summary>
        /// The options
        /// </summary>
        private ProcessorOptions _options;

        /// <summary>
        /// Sets up.
        /// </summary>
        /// <param name="processorFactory">The processor factory.</param>
        protected void SetUp(IMigrationProcessorFactory processorFactory)
        {
            if (!IntegrationTestOptions.Oracle.IsEnabled)
            {
                Assert.Ignore();
            }

            _factory = processorFactory;
            _connectionString = "Data Source=localhost/XE;User Id=Something;Password=Something";
            _announcer = new NullAnnouncer();
            _options = new ProcessorOptions();
        }

        /// <summary>
        /// Defines the test method CreateProcessorWithNoProviderSwitchesShouldUseOracleQuoter.
        /// </summary>
        /// <param name="providerSwitches">The provider switches.</param>
        [TestCase("")]
        [TestCase(null)]
        public void CreateProcessorWithNoProviderSwitchesShouldUseOracleQuoter(string providerSwitches)
        {
            _options.ProviderSwitches = providerSwitches;
            var processor = _factory.Create(_connectionString, _announcer, _options);
            Assert.That(((OracleProcessor) processor).Quoter, Is.InstanceOf<OracleQuoter>());
        }

        /// <summary>
        /// Defines the test method CreateProcessorWithProviderSwitchIndicatingQuotedShouldUseOracleQuoterQuotedIdentifier.
        /// </summary>
        /// <param name="providerSwitches">The provider switches.</param>
        [TestCase("QuotedIdentifiers=true")]
        [TestCase("QuotedIdentifiers=TRUE;")]
        [TestCase("QuotedIDENTIFIERS=TRUE;")]
        [TestCase("QuotedIdentifiers=true;somethingelse=1")]
        [TestCase("somethingelse=1;QuotedIdentifiers=true")]
        [TestCase("somethingelse=1;QuotedIdentifiers=true;sometingOther='special thingy'")]
        public void CreateProcessorWithProviderSwitchIndicatingQuotedShouldUseOracleQuoterQuotedIdentifier(
            string providerSwitches)
        {
            _options.ProviderSwitches = providerSwitches;
            var processor = _factory.Create(_connectionString, _announcer, _options);
            Assert.That(((OracleProcessor) processor).Quoter, Is.InstanceOf<OracleQuoterQuotedIdentifier>());
        }
    }
}
