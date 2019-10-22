// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SqlServer2008TypeMapTests.cs" company="FluentMigrator Project">
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

using System.Data;
using FluentMigrator.Runner.Generators.SqlServer;
using NUnit.Framework;
using Shouldly;

namespace FluentMigrator.Tests.Unit.Generators.SqlServer2008
{
    /// <summary>
    /// Defines test class SqlServer2008TypeMapTests.
    /// </summary>
    [TestFixture]
    [Category("SqlServer2008")]
    [Category("Generator")]
    [Category("TypeMap")]
    public abstract class SqlServer2008TypeMapTests
    {
        /// <summary>
        /// Gets or sets the type map.
        /// </summary>
        /// <value>The type map.</value>
        private SqlServer2005TypeMap TypeMap { get; set; }

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            TypeMap = new SqlServer2008TypeMap();
        }

        /// <summary>
        /// Defines test class DateTimeTests.
        /// Implements the <see cref="FluentMigrator.Tests.Unit.Generators.SqlServer2008.SqlServer2008TypeMapTests" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Tests.Unit.Generators.SqlServer2008.SqlServer2008TypeMapTests" />
        [TestFixture]
        public class DateTimeTests : SqlServer2008TypeMapTests
        {
            /// <summary>
            /// Defines the test method ItMapsTimeToDatetime.
            /// </summary>
            [Test]
            public void ItMapsTimeToDatetime()
            {
                var template = TypeMap.GetTypeMap(DbType.Time, size: null, precision: null);

                template.ShouldBe("TIME");
            }

            /// <summary>
            /// Defines the test method ItMapsDateToDatetime.
            /// </summary>
            [Test]
            public void ItMapsDateToDatetime()
            {
                var template = TypeMap.GetTypeMap(DbType.Date, size: null, precision: null);

                template.ShouldBe("DATE");
            }

            /// <summary>
            /// Defines the test method ItMapsDatetimeToDatetime.
            /// </summary>
            [Test]
            public void ItMapsDatetimeToDatetime()
            {
                var template = TypeMap.GetTypeMap(DbType.DateTime2, size: null, precision: null);

                template.ShouldBe("DATETIME2");
            }

            /// <summary>
            /// Defines the test method ItMapsDatetimeToDatetimeOffset.
            /// </summary>
            [Test]
            public void ItMapsDatetimeToDatetimeOffset()
            {
                var template = TypeMap.GetTypeMap(DbType.DateTimeOffset, size: null, precision: null);

                template.ShouldBe("DATETIMEOFFSET");
            }

            /// <summary>
            /// Defines the test method ItMapsDatetimeToDatetimeOffset.
            /// </summary>
            /// <param name="precision">The precision.</param>
            [Test]
            [TestCase(0)]
            [TestCase(7)]
            public void ItMapsDatetimeToDatetimeOffset(int precision)
            {
                var template = TypeMap.GetTypeMap(DbType.DateTimeOffset, precision, precision: null);

                template.ShouldBe($"DATETIMEOFFSET({precision})");
            }
        }
    }
}
