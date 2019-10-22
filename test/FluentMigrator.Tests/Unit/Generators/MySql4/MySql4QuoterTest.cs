// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="MySql4QuoterTest.cs" company="FluentMigrator Project">
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

using FluentMigrator.Runner.Generators;
using FluentMigrator.Runner.Generators.MySql;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Generators.MySql4
{
    /// <summary>
    /// Defines test class MySql4QuoterTest.
    /// </summary>
    [TestFixture]
    public class MySql4QuoterTest
    {
        /// <summary>
        /// The quoter
        /// </summary>
        private IQuoter _quoter = default(MySqlQuoter);

        /// <summary>
        /// Sets up.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            _quoter = new MySqlQuoter();
        }

        /// <summary>
        /// Defines the test method TimeSpanIsFormattedQuotes.
        /// </summary>
        [Test]
        public void TimeSpanIsFormattedQuotes()
        {
            _quoter.QuoteValue(new TimeSpan(1,2, 13, 65))
                .ShouldBe("'26:14:05'");
        }
    }
}
