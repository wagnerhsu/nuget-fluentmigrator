// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="TagsExtensionsTests.cs" company="FluentMigrator Project">
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

using System.Collections.Generic;

using FluentMigrator.Runner.Extensions;

using NUnit.Framework;

namespace FluentMigrator.Tests.Unit.Runners
{
    /// <summary>
    /// Defines test class TagsExtensionsTests.
    /// </summary>
    [TestFixture]
    public class TagsExtensionsTests
    {
        /// <summary>
        /// Defines the test method ToTags_WithOneTag_ShouldReturnListWithOneTag.
        /// </summary>
        [Test]
        public void ToTags_WithOneTag_ShouldReturnListWithOneTag()
        {
            List<string> tags = "Test".ToTags();

            Assert.That(tags[0], Is.EqualTo("Test"));
        }

        /// <summary>
        /// Defines the test method ToTags_WithNullString_ShouldReturnEmptyList.
        /// </summary>
        [Test]
        public void ToTags_WithNullString_ShouldReturnEmptyList()
        {
            var tags = ((string) null).ToTags();

            Assert.That(tags, Is.Not.Null);
        }

        /// <summary>
        /// Defines the test method ToTags_WithThreeTags_ShouldReturnListWithThreeTags.
        /// </summary>
        [Test]
        public void ToTags_WithThreeTags_ShouldReturnListWithThreeTags()
        {
            List<string> tags = "Dev,Test,Prod".ToTags();

            var expectedTags = new[] { "Dev", "Test", "Prod" };
            CollectionAssert.AreEquivalent(expectedTags, tags);
        }
    }
}
