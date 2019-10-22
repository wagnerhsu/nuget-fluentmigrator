// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SourceTestsBase.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
// Copyright (c) 2018, Fluent Migrator Project
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

using FluentMigrator.Runner.BatchParser;

using NUnit.Framework;

namespace FluentMigrator.Tests.Unit.BatchParser
{
    /// <summary>
    /// Class SourceTestsBase.
    /// </summary>
    [Category("BatchParser")]
    public abstract class SourceTestsBase
    {
        /// <summary>
        /// Creates the source.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>ITextSource.</returns>
        public abstract ITextSource CreateSource(string content);

        /// <summary>
        /// Defines the test method TestInputs.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="lines">The lines.</param>
        [TestCase("")]
        [TestCase("a", "a")]
        [TestCase("a\n", "a")]
        [TestCase("a\nb", "a", "b")]
        [TestCase("a\nb\n", "a", "b")]
        [TestCase("a\n\nc", "a", "", "c")]
        [TestCase("\nb\n\nd", "", "b", "", "d")]
        public void TestInputs(string content, params string[] lines)
        {
            var source = CreateSource(content);
            Assert.IsNotNull(source);
            var reader = source.CreateReader();

            foreach (var line in lines)
            {
                Assert.NotNull(reader);
                Assert.AreEqual(line, reader.Line);
                Assert.AreEqual(0, reader.Index);

                var nextReader = reader.Advance(reader.Line.Length);
                Assert.AreNotSame(reader, nextReader);
                reader = nextReader;
            }

            Assert.IsNull(reader);
        }

        /// <summary>
        /// Defines the test method TestReadTooMuch.
        /// </summary>
        [Test]
        public void TestReadTooMuch()
        {
            var source = CreateSource("asdasdasd");
            Assert.IsNotNull(source);
            var reader = source.CreateReader();
            Assert.IsNotNull(reader);
            Assert.AreEqual("asdasdasd", reader.Line);
            Assert.AreEqual(0, reader.Index);
            Assert.AreEqual(9, reader.Length);
            Assert.Throws<ArgumentOutOfRangeException>(() => reader.ReadString(100));
        }

        /// <summary>
        /// Defines the test method TestFullLineAdvance.
        /// </summary>
        [Test]
        public void TestFullLineAdvance()
        {
            var source = CreateSource("asdasdasd");
            Assert.IsNotNull(source);
            var reader = source.CreateReader();
            Assert.IsNotNull(reader);
            Assert.AreEqual("asdasdasd", reader.Line);
            Assert.AreEqual(0, reader.Index);
            var newReader = reader.Advance(reader.Line.Length);
            Assert.AreNotSame(reader, newReader);
            Assert.IsNull(newReader);
        }

        /// <summary>
        /// Defines the test method TestPartialAdvance.
        /// </summary>
        [Test]
        public void TestPartialAdvance()
        {
            var source = CreateSource("asdasdasd");
            Assert.IsNotNull(source);
            var reader = source.CreateReader();
            Assert.IsNotNull(reader);
            Assert.AreEqual("asd", reader.ReadString(3));
            var newReader = reader.Advance(1);
            Assert.AreNotSame(reader, newReader);
            Assert.IsNotNull(newReader);
            reader = newReader;
            Assert.AreEqual("sda", reader.ReadString(3));
            Assert.AreEqual(1, reader.Index);
            Assert.AreEqual(8, reader.Length);
        }

        /// <summary>
        /// Defines the test method TestOverlappingAdvanceOneLine.
        /// </summary>
        [Test]
        public void TestOverlappingAdvanceOneLine()
        {
            var source = CreateSource("asd\nqwe");
            Assert.IsNotNull(source);
            var reader = source.CreateReader();
            Assert.IsNotNull(reader);
            reader = reader.Advance(4);
            Assert.IsNotNull(reader);
            Assert.AreEqual("we", reader.ReadString(2));
            Assert.AreEqual(1, reader.Index);
        }

        /// <summary>
        /// Defines the test method TestOverlappingAdvanceTwoLine.
        /// </summary>
        [Test]
        public void TestOverlappingAdvanceTwoLine()
        {
            var source = CreateSource("asd\n\nqwe");
            Assert.IsNotNull(source);
            var reader = source.CreateReader();
            Assert.IsNotNull(reader);
            reader = reader.Advance(4);
            Assert.IsNotNull(reader);
            Assert.AreEqual("we", reader.ReadString(2));
            Assert.AreEqual(1, reader.Index);
        }

        /// <summary>
        /// Defines the test method TestNonOverlappingAdvanceTwoLine.
        /// </summary>
        [Test]
        public void TestNonOverlappingAdvanceTwoLine()
        {
            var source = CreateSource("asd\n\nqwe");
            Assert.IsNotNull(source);
            var reader = source.CreateReader();
            Assert.IsNotNull(reader);
            reader = reader.Advance(3);
            Assert.IsNotNull(reader);
            Assert.AreEqual(string.Empty, reader.Line);
            Assert.AreEqual(0, reader.Index);
            reader = reader.Advance(0);
            Assert.IsNotNull(reader);
            Assert.AreEqual("qwe", reader.Line);
            Assert.AreEqual(0, reader.Index);
        }
    }
}
