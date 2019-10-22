// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="AnnouncerTests.cs" company="FluentMigrator Project">
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
using System.Collections.Generic;
using System.IO;

using FluentMigrator.Runner;
using FluentMigrator.Tests.Logging;

using Microsoft.Extensions.Logging;

using NUnit.Framework;

namespace FluentMigrator.Tests.Unit.Loggers
{
    /// <summary>
    /// Defines test class AnnouncerTests.
    /// </summary>
    [TestFixture]
    public class AnnouncerTests
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
        private FluentMigratorLoggerOptions _options;

        /// <summary>
        /// The output
        /// </summary>
        private StringWriter _output;

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _options = new FluentMigratorLoggerOptions();
            _output = new StringWriter();
            _loggerFactory = new LoggerFactory();
            _loggerFactory.AddProvider(new TextWriterLoggerProvider(_output, _options));
            _logger = _loggerFactory.CreateLogger("Test");
        }

        /// <summary>
        /// Defines the test method ElapsedTime_Should_Not_Write_When_ShowElapsedTime_Is_False.
        /// </summary>
        [Test]
        public void ElapsedTime_Should_Not_Write_When_ShowElapsedTime_Is_False()
        {
            var time = new TimeSpan(0, 1, 40);

            _logger.LogElapsedTime(time);

            Assert.IsEmpty(_output.ToString());
        }

        /// <summary>
        /// Defines the test method ElapsedTime_Should_Write_When_ShowElapsedTime_Is_True.
        /// </summary>
        [Test]
        public void ElapsedTime_Should_Write_When_ShowElapsedTime_Is_True()
        {
            var time = new TimeSpan(0, 1, 40);

            _options.ShowElapsedTime = true;

            _logger.LogElapsedTime(time);

            Assert.AreEqual("=> 100s", _output.ToString().Trim());
        }

        /// <summary>
        /// Defines the test method Error_Should_Write.
        /// </summary>
        [Test]
        public void Error_Should_Write()
        {
            var message = "TheMessage";

            _logger.LogError(message);

            Assert.AreEqual($"!!! {message}", _output.ToString().Trim());
        }

        /// <summary>
        /// Defines the test method Heading_Should_Write.
        /// </summary>
        [Test]
        public void Heading_Should_Write()
        {
            var message = "TheMessage";

            _logger.LogHeader(message);

            var lines = GetLines();
            Assert.GreaterOrEqual(3, lines.Count);

            Assert.AreEqual(message, lines[1]);
        }

        /// <summary>
        /// Defines the test method Say_Should_Write.
        /// </summary>
        [Test]
        public void Say_Should_Write()
        {
            var message = "TheMessage";

            _logger.LogSay(message);

            Assert.AreEqual(message, _output.ToString().Trim());
        }

        /// <summary>
        /// Defines the test method Sql_Should_Not_Write_When_Show_Sql_Is_False.
        /// </summary>
        [Test]
        public void Sql_Should_Not_Write_When_Show_Sql_Is_False()
        {
            var sql = "INSERT INTO table(Id,Name) VALUES (1, 'Test');";

            _logger.LogSql(sql);

            Assert.IsEmpty(_output.ToString());
        }

        /// <summary>
        /// Defines the test method Sql_Should_Write_When_Show_Sql_Is_True.
        /// </summary>
        [Test]
        public void Sql_Should_Write_When_Show_Sql_Is_True()
        {
            var sql = "INSERT INTO table(Id,Name) VALUES (1, 'Test');";

            _options.ShowSql = true;

            _logger.LogSql(sql);

            Assert.AreEqual(sql, _output.ToString().Trim());
        }

        /// <summary>
        /// Defines the test method Sql_Should_Write_When_Show_Sql_Is_True_And_Sql_Is_Empty.
        /// </summary>
        [Test]
        public void Sql_Should_Write_When_Show_Sql_Is_True_And_Sql_Is_Empty()
        {
            var sql = string.Empty;

            _options.ShowSql = true;

            _logger.LogSql(sql);

            Assert.AreEqual("No SQL statement executed.", _output.ToString().Trim());
        }

        /// <summary>
        /// Gets the lines.
        /// </summary>
        /// <returns>IReadOnlyList&lt;System.String&gt;.</returns>
        private IReadOnlyList<string> GetLines()
        {
            var lines = new List<string>();

            using (var reader = new StringReader(_output.ToString()))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }

            return lines;
        }
    }
}
