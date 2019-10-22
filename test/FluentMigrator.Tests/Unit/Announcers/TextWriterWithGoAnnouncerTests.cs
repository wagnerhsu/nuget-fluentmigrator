// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="TextWriterWithGoAnnouncerTests.cs" company="FluentMigrator Project">
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
using System.IO;

using FluentMigrator.Runner.Announcers;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Announcers
{
    /// <summary>
    /// Defines test class TextWriterWithGoAnnouncerTests.
    /// </summary>
    [TestFixture]
    [Obsolete]
    public class TextWriterWithGoAnnouncerTests
    {
        /// <summary>
        /// The string writer
        /// </summary>
        private StringWriter _stringWriter;
        /// <summary>
        /// The announcer
        /// </summary>
        private TextWriterWithGoAnnouncer _announcer;

        /// <summary>
        /// Tests the setup.
        /// </summary>
        [SetUp]
        public void TestSetup()
        {
            _stringWriter = new StringWriter();
            _announcer = new TextWriterWithGoAnnouncer(_stringWriter)
            {
                ShowElapsedTime = true,
                ShowSql = true
            };
        }

        /// <summary>
        /// Defines the test method Adds_Go_StatementAfterSqlAnouncement.
        /// </summary>
        [Test]
        public void Adds_Go_StatementAfterSqlAnouncement()
        {
            _announcer.Sql("DELETE Blah");
            Output.ShouldBe("DELETE Blah" + Environment.NewLine +
                "GO" + Environment.NewLine);
        }

        /// <summary>
        /// Defines the test method Sql_Should_Not_Write_When_Show_Sql_Is_False.
        /// </summary>
        [Test]
        public void Sql_Should_Not_Write_When_Show_Sql_Is_False()
        {
            _announcer.ShowSql = false;

            _announcer.Sql("SQL");
            Output.ShouldBe(string.Empty);
        }

        /// <summary>
        /// Defines the test method Sql_Should_Not_Write_Go_When_Sql_Is_Empty.
        /// </summary>
        [Test]
        public void Sql_Should_Not_Write_Go_When_Sql_Is_Empty()
        {
            _announcer.Sql("");
            Assert.IsFalse(Output.Contains("GO"));
        }

        /// <summary>
        /// Gets the output.
        /// </summary>
        /// <value>The output.</value>
        public string Output
        {
            get { return _stringWriter.GetStringBuilder().ToString(); }
        }
    }
}
