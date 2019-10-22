// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="TextWriterAnnouncerTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License

// Copyright (c) 2007-2018, Sean Chambers <schambers80@gmail.com>
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
using System.IO;

using FluentMigrator.Runner;
using FluentMigrator.Runner.Logging;

using Microsoft.Extensions.Logging;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Loggers
{
    /// <summary>
    /// Defines test class TextWriterAnnouncerTests.
    /// </summary>
    [TestFixture]
    public class TextWriterAnnouncerTests
    {
        /// <summary>
        /// The logger factory
        /// </summary>
        private ILoggerFactory _loggerFactory;

        /// <summary>
        /// The logger
        /// </summary>
        private ILogger _logger;

        /// <summary>
        /// The options
        /// </summary>
        private SqlScriptFluentMigratorLoggerOptions _options;

        /// <summary>
        /// The string writer
        /// </summary>
        private StringWriter _stringWriter;

        /// <summary>
        /// Gets the output.
        /// </summary>
        /// <value>The output.</value>
        private string Output => _stringWriter.ToString();

        /// <summary>
        /// Sets up.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            _stringWriter = new StringWriter();
            _options = new SqlScriptFluentMigratorLoggerOptions();
            _loggerFactory = new LoggerFactory();
            _loggerFactory.AddProvider(new SqlScriptFluentMigratorLoggerProvider(_stringWriter, _options));
            _logger = _loggerFactory.CreateLogger("Test");
        }

        /// <summary>
        /// Defines the test method CanAnnounceAndPadWithEquals.
        /// </summary>
        [Test]
        public void CanAnnounceAndPadWithEquals()
        {
            _logger.LogHeader("Test");
            Output.ShouldBe("/* Test ====================================================================== */" + Environment.NewLine + Environment.NewLine);
        }

        /// <summary>
        /// Defines the test method CanSay.
        /// </summary>
        [Test]
        public void CanSay()
        {
            _logger.LogSay("Create table");
            Output.ShouldBe("/* Create table */" + Environment.NewLine);
        }

        /// <summary>
        /// Defines the test method CanSaySql.
        /// </summary>
        [Test]
        public void CanSaySql()
        {
            _logger.LogSql("DELETE Blah");
            Output.ShouldBe("DELETE Blah" + Environment.NewLine);
        }

        /// <summary>
        /// Defines the test method CanSayTimeSpan.
        /// </summary>
        [Test]
        public void CanSayTimeSpan()
        {
            _options.ShowElapsedTime = true;
            _logger.LogElapsedTime(new TimeSpan(0, 0, 5));
            Output.ShouldBe("/* => 5s */" + Environment.NewLine + Environment.NewLine);
        }
    }
}
