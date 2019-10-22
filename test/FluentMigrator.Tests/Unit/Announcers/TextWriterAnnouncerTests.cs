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
using FluentMigrator.Runner.Announcers;
using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Announcers
{
    /// <summary>
    /// Defines test class TextWriterAnnouncerTests.
    /// </summary>
    [TestFixture]
    [Obsolete]
    public class TextWriterAnnouncerTests
    {
        /// <summary>
        /// Sets up.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            _stringWriter = new StringWriter();
            _announcer = new TextWriterAnnouncer(_stringWriter)
                             {
                                 ShowElapsedTime = true,
                                 ShowSql = true
                             };
        }

        /// <summary>
        /// The announcer
        /// </summary>
        private TextWriterAnnouncer _announcer;
        /// <summary>
        /// The string writer
        /// </summary>
        private StringWriter _stringWriter;

        /// <summary>
        /// Gets the output.
        /// </summary>
        /// <value>The output.</value>
        public string Output
        {
            get { return _stringWriter.GetStringBuilder().ToString(); }
        }

        /// <summary>
        /// Defines the test method CanAnnounceAndPadWithEquals.
        /// </summary>
        [Test]
        public void CanAnnounceAndPadWithEquals()
        {
            _announcer.Heading("Test");
            Output.ShouldBe("/* Test ====================================================================== */" + Environment.NewLine + Environment.NewLine);
        }

        /// <summary>
        /// Defines the test method CanSay.
        /// </summary>
        [Test]
        public void CanSay()
        {
            _announcer.Say("Create table");
            Output.ShouldBe("/* Create table */" + Environment.NewLine);
        }

        /// <summary>
        /// Defines the test method CanSaySql.
        /// </summary>
        [Test]
        public void CanSaySql()
        {
            _announcer.Sql("DELETE Blah");
            Output.ShouldBe("DELETE Blah" + Environment.NewLine);
        }

        /// <summary>
        /// Defines the test method CanSayTimeSpan.
        /// </summary>
        [Test]
        public void CanSayTimeSpan()
        {
            _announcer.ElapsedTime(new TimeSpan(0, 0, 5));
            Output.ShouldBe("/* => 5s */" + Environment.NewLine + Environment.NewLine);
        }
    }
}
