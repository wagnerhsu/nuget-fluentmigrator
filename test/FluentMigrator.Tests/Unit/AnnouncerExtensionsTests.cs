// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="AnnouncerExtensionsTests.cs" company="FluentMigrator Project">
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

using FluentMigrator.Runner;

using Moq;

using NUnit.Framework;

namespace FluentMigrator.Tests.Unit
{
    /// <summary>
    /// Defines test class AnnouncerExtensionsTests.
    /// </summary>
    [TestFixture]
    [Obsolete]
    public class AnnouncerExtensionsTests
    {
        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _announcer = new Mock<IAnnouncer>(MockBehavior.Strict).Object;
        }

        /// <summary>
        /// The announcer
        /// </summary>
        private IAnnouncer _announcer;

        /// <summary>
        /// Defines the test method ErrorShouldErrorStringFormattedMessage.
        /// </summary>
        [Test]
        public void ErrorShouldErrorStringFormattedMessage()
        {
            Mock.Get(_announcer).Setup(a => a.Error("Hello Error"));

            _announcer.Error("Hello {0}", "Error");
        }

        /// <summary>
        /// Defines the test method HeadingShouldHeadingStringFormattedMessage.
        /// </summary>
        [Test]
        public void HeadingShouldHeadingStringFormattedMessage()
        {
            Mock.Get(_announcer).Setup(a => a.Heading("Hello Heading"));

            _announcer.Heading("Hello {0}", "Heading");
        }

        /// <summary>
        /// Defines the test method SayShouldSayStringFormattedMessage.
        /// </summary>
        [Test]
        public void SayShouldSayStringFormattedMessage()
        {
            Mock.Get(_announcer).Setup(a => a.Say("Hello Say"));

            _announcer.Say("Hello {0}", "Say");
        }
    }
}
