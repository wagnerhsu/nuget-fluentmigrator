// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SqlAnywhere16TypeMapTests.cs" company="FluentMigrator Project">
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
using System.Data;

using FluentMigrator.Runner.Generators.SqlAnywhere;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Generators.SqlAnywhere
{
    /// <summary>
    /// Defines test class SqlAnywhere16TypeMapTests.
    /// </summary>
    [TestFixture]
    [Category("SqlAnywhere")]
    [Category("SqlAnywhere16")]
    [Category("Generator")]
    [Category("TypeMap")]
    public abstract class SqlAnywhere16TypeMapTests
    {
        /// <summary>
        /// Gets or sets the type map.
        /// </summary>
        /// <value>The type map.</value>
        private SqlAnywhere16TypeMap TypeMap { get; set; }

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            TypeMap = new SqlAnywhere16TypeMap();
        }

        /// <summary>
        /// Defines test class AnsistringTests.
        /// Implements the <see cref="FluentMigrator.Tests.Unit.Generators.SqlAnywhere.SqlAnywhere16TypeMapTests" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Tests.Unit.Generators.SqlAnywhere.SqlAnywhere16TypeMapTests" />
        [TestFixture]
        public class AnsistringTests : SqlAnywhere16TypeMapTests
        {
            /// <summary>
            /// Defines the test method ItMapsAnsistringByDefaultToVarchar255.
            /// </summary>
            [Test]
            public void ItMapsAnsistringByDefaultToVarchar255()
            {
                var template = TypeMap.GetTypeMap(DbType.AnsiString, size: null, precision: null);

                template.ShouldBe("VARCHAR(255)");
            }

            /// <summary>
            /// Defines the test method ItMapsAnsistringWithSizeToVarcharOfSize.
            /// </summary>
            /// <param name="size">The size.</param>
            [Test]
            [TestCase(1)]
            [TestCase(4000)]
            [TestCase(8000)]
            public void ItMapsAnsistringWithSizeToVarcharOfSize(int size)
            {
                var template = TypeMap.GetTypeMap(DbType.AnsiString, size, precision: null);

                template.ShouldBe($"VARCHAR({size})");
            }

            /// <summary>
            /// Defines the test method ItMapsAnsistringWithSizeAbove8000ToText.
            /// </summary>
            /// <param name="size">The size.</param>
            [Test]
            [TestCase(8001)]
            [TestCase(2147483647)]
            public void ItMapsAnsistringWithSizeAbove8000ToText(int size)
            {
                var template = TypeMap.GetTypeMap(DbType.AnsiString, size, precision: null);

                template.ShouldBe("TEXT");
            }
        }

        /// <summary>
        /// Defines test class AnsistringFixedLengthTests.
        /// Implements the <see cref="FluentMigrator.Tests.Unit.Generators.SqlAnywhere.SqlAnywhere16TypeMapTests" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Tests.Unit.Generators.SqlAnywhere.SqlAnywhere16TypeMapTests" />
        [TestFixture]
        public class AnsistringFixedLengthTests : SqlAnywhere16TypeMapTests
        {
            /// <summary>
            /// Defines the test method ItMapsAnsistringFixedLengthByDefaultToChar255.
            /// </summary>
            [Test]
            public void ItMapsAnsistringFixedLengthByDefaultToChar255()
            {
                var template = TypeMap.GetTypeMap(DbType.AnsiStringFixedLength, size: null, precision: null);

                template.ShouldBe("CHAR(255)");
            }

            /// <summary>
            /// Defines the test method ItMapsAnsistringFixedLengthWithSizeToCharOfSize.
            /// </summary>
            /// <param name="size">The size.</param>
            [Test]
            [TestCase(1)]
            [TestCase(4000)]
            [TestCase(8000)]
            public void ItMapsAnsistringFixedLengthWithSizeToCharOfSize(int size)
            {
                var template = TypeMap.GetTypeMap(DbType.AnsiStringFixedLength, size, precision: null);

                template.ShouldBe($"CHAR({size})");
            }

            /// <summary>
            /// Defines the test method ItThrowsIfAnsistringFixedLengthHasSizeAbove8000.
            /// </summary>
            [Test]
            public void ItThrowsIfAnsistringFixedLengthHasSizeAbove8000()
            {
                Should.Throw<NotSupportedException>(
                    () => TypeMap.GetTypeMap(DbType.AnsiStringFixedLength, size: 8001, precision: null));
            }
        }

        /// <summary>
        /// Defines test class StringTests.
        /// Implements the <see cref="FluentMigrator.Tests.Unit.Generators.SqlAnywhere.SqlAnywhere16TypeMapTests" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Tests.Unit.Generators.SqlAnywhere.SqlAnywhere16TypeMapTests" />
        [TestFixture]
        public class StringTests : SqlAnywhere16TypeMapTests
        {
            /// <summary>
            /// Defines the test method ItMapsStringByDefaultToNvarchar255.
            /// </summary>
            [Test]
            public void ItMapsStringByDefaultToNvarchar255()
            {
                var template = TypeMap.GetTypeMap(DbType.String, size: null, precision: null);

                template.ShouldBe("NVARCHAR(255)");
            }

            /// <summary>
            /// Defines the test method ItMapsStringWithSizeToNvarcharOfSize.
            /// </summary>
            /// <param name="size">The size.</param>
            [Test]
            [TestCase(1)]
            [TestCase(4000)]
            public void ItMapsStringWithSizeToNvarcharOfSize(int size)
            {
                var template = TypeMap.GetTypeMap(DbType.String, size, precision: null);

                template.ShouldBe($"NVARCHAR({size})");
            }

            /// <summary>
            /// Defines the test method ItMapsStringWithSizeAbove4000ToNtext.
            /// </summary>
            /// <param name="size">The size.</param>
            [Test]
            [TestCase(4001)]
            [TestCase(1073741823)]
            public void ItMapsStringWithSizeAbove4000ToNtext(int size)
            {
                var template = TypeMap.GetTypeMap(DbType.String, size, precision: null);

                template.ShouldBe("NTEXT");
            }

            /// <summary>
            /// Defines the test method ItMapsStringWithSizeAbove1073741823ToNtextToAllowIntMaxvalueConvention.
            /// </summary>
            [Test]
            public void ItMapsStringWithSizeAbove1073741823ToNtextToAllowIntMaxvalueConvention()
            {
                var template = TypeMap.GetTypeMap(DbType.String, int.MaxValue, precision: null);

                template.ShouldBe("NTEXT");
            }
        }

        /// <summary>
        /// Defines test class StringFixedLengthTests.
        /// Implements the <see cref="FluentMigrator.Tests.Unit.Generators.SqlAnywhere.SqlAnywhere16TypeMapTests" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Tests.Unit.Generators.SqlAnywhere.SqlAnywhere16TypeMapTests" />
        [TestFixture]
        public class StringFixedLengthTests : SqlAnywhere16TypeMapTests
        {
            /// <summary>
            /// Defines the test method ItMapsStringFixedLengthByDefaultToNchar255.
            /// </summary>
            [Test]
            public void ItMapsStringFixedLengthByDefaultToNchar255()
            {
                var template = TypeMap.GetTypeMap(DbType.StringFixedLength, size: null, precision: null);

                template.ShouldBe("NCHAR(255)");
            }


            /// <summary>
            /// Defines the test method ItMapsStringFixedLengthWithSizeToNcharOfSize.
            /// </summary>
            /// <param name="size">The size.</param>
            [Test]
            [TestCase(1)]
            [TestCase(4000)]
            public void ItMapsStringFixedLengthWithSizeToNcharOfSize(int size)
            {
                var template = TypeMap.GetTypeMap(DbType.StringFixedLength, size, precision: null);

                template.ShouldBe($"NCHAR({size})");
            }

            /// <summary>
            /// Defines the test method ItThrowsIfStringFixedLengthHasSizeAbove4000.
            /// </summary>
            [Test]
            public void ItThrowsIfStringFixedLengthHasSizeAbove4000()
            {
                Should.Throw<NotSupportedException>(
                    () => TypeMap.GetTypeMap(DbType.StringFixedLength, size: 4001, precision: null));
            }
        }

        /// <summary>
        /// Defines test class BinaryTests.
        /// Implements the <see cref="FluentMigrator.Tests.Unit.Generators.SqlAnywhere.SqlAnywhere16TypeMapTests" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Tests.Unit.Generators.SqlAnywhere.SqlAnywhere16TypeMapTests" />
        [TestFixture]
        public class BinaryTests : SqlAnywhere16TypeMapTests
        {
            /// <summary>
            /// Defines the test method ItMapsBinaryByDefaultToVarbinary8000.
            /// </summary>
            [Test]
            public void ItMapsBinaryByDefaultToVarbinary8000()
            {
                var template = TypeMap.GetTypeMap(DbType.Binary, size: null, precision: null);

                template.ShouldBe("VARBINARY(8000)");
            }

            /// <summary>
            /// Defines the test method ItMapsBinaryWithSizeToVarbinaryOfSize.
            /// </summary>
            /// <param name="size">The size.</param>
            [Test]
            [TestCase(1)]
            [TestCase(4000)]
            [TestCase(8000)]
            public void ItMapsBinaryWithSizeToVarbinaryOfSize(int size)
            {
                var template = TypeMap.GetTypeMap(DbType.Binary, size, precision: null);

                template.ShouldBe($"VARBINARY({size})");
            }

            /// <summary>
            /// Defines the test method ItMapsBinaryWithSizeAbove8000ToImage.
            /// </summary>
            /// <param name="size">The size.</param>
            [Test]
            [TestCase(8001)]
            [TestCase(int.MaxValue)]
            public void ItMapsBinaryWithSizeAbove8000ToImage(int size)
            {
                var template = TypeMap.GetTypeMap(DbType.Binary, size, precision: null);

                template.ShouldBe("IMAGE");
            }
        }

        /// <summary>
        /// Defines test class NumericTests.
        /// Implements the <see cref="FluentMigrator.Tests.Unit.Generators.SqlAnywhere.SqlAnywhere16TypeMapTests" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Tests.Unit.Generators.SqlAnywhere.SqlAnywhere16TypeMapTests" />
        [TestFixture]
        public class NumericTests : SqlAnywhere16TypeMapTests
        {
            /// <summary>
            /// Defines the test method ItMapsBooleanToBit.
            /// </summary>
            [Test]
            public void ItMapsBooleanToBit()
            {
                var template = TypeMap.GetTypeMap(DbType.Boolean, size: null, precision: null);

                template.ShouldBe("BIT");
            }

            /// <summary>
            /// Defines the test method ItMapsByteToTinyint.
            /// </summary>
            [Test]
            public void ItMapsByteToTinyint()
            {
                var template = TypeMap.GetTypeMap(DbType.Byte, size: null, precision: null);

                template.ShouldBe("TINYINT");
            }

            /// <summary>
            /// Defines the test method ItMapsInt16ToSmallint.
            /// </summary>
            [Test]
            public void ItMapsInt16ToSmallint()
            {
                var template = TypeMap.GetTypeMap(DbType.Int16, size: null, precision: null);

                template.ShouldBe("SMALLINT");
            }

            /// <summary>
            /// Defines the test method ItMapsInt32ToInteger.
            /// </summary>
            [Test]
            public void ItMapsInt32ToInteger()
            {
                var template = TypeMap.GetTypeMap(DbType.Int32, size: null, precision: null);

                template.ShouldBe("INTEGER");
            }

            /// <summary>
            /// Defines the test method ItMapsInt64ToBigint.
            /// </summary>
            [Test]
            public void ItMapsInt64ToBigint()
            {
                var template = TypeMap.GetTypeMap(DbType.Int64, size: null, precision: null);

                template.ShouldBe("BIGINT");
            }

            /// <summary>
            /// Defines the test method ItMapsSingleToReal.
            /// </summary>
            [Test]
            public void ItMapsSingleToReal()
            {
                var template = TypeMap.GetTypeMap(DbType.Single, size: null, precision: null);

                template.ShouldBe("REAL");
            }

            /// <summary>
            /// Defines the test method ItMapsDoubleToDoublePrecision.
            /// </summary>
            [Test]
            public void ItMapsDoubleToDoublePrecision()
            {
                var template = TypeMap.GetTypeMap(DbType.Double, size: null, precision: null);

                template.ShouldBe("DOUBLE PRECISION");
            }

            /// <summary>
            /// Defines the test method ItMapsCurrencyToMoney.
            /// </summary>
            [Test]
            public void ItMapsCurrencyToMoney()
            {
                var template = TypeMap.GetTypeMap(DbType.Currency, size: null, precision: null);

                template.ShouldBe("MONEY");
            }

            /// <summary>
            /// Defines the test method ItMapsDecimalByDefaultToDecimal306.
            /// </summary>
            [Test]
            public void ItMapsDecimalByDefaultToDecimal306()
            {
                var template = TypeMap.GetTypeMap(DbType.Decimal, size: null, precision: null);

                template.ShouldBe("DECIMAL(30,6)");
            }

            /// <summary>
            /// Defines the test method ItMapsDecimalWithPrecisionToDecimal.
            /// </summary>
            /// <param name="precision">The precision.</param>
            [Test]
            [TestCase(1)]
            [TestCase(20)]
            [TestCase(38)]
            public void ItMapsDecimalWithPrecisionToDecimal(int precision)
            {
                var template = TypeMap.GetTypeMap(DbType.Decimal, (int?)precision, precision: 1);

                template.ShouldBe($"DECIMAL({precision},1)");
            }

            /// <summary>
            /// Defines the test method ItThrowsIfDecimalPrecisionIsAbove127.
            /// </summary>
            [Test]
            public void ItThrowsIfDecimalPrecisionIsAbove127()
            {
                Should.Throw<NotSupportedException>(
                    () => TypeMap.GetTypeMap(DbType.Decimal, size: 128, precision: null));
            }

            /// <summary>
            /// Defines the test method ItMapsVarnumericByDefaultToNumeric195.
            /// </summary>
            [Test]
            public void ItMapsVarnumericByDefaultToNumeric195()
            {
                var template = TypeMap.GetTypeMap(DbType.VarNumeric, size: null, precision: null);

                template.ShouldBe("NUMERIC(30,6)");
            }

            /// <summary>
            /// Defines the test method ItMapsVarnumericWithPrecisionToNumeric.
            /// </summary>
            /// <param name="precision">The precision.</param>
            [Test]
            [TestCase(1)]
            [TestCase(20)]
            [TestCase(38)]
            public void ItMapsVarnumericWithPrecisionToNumeric(int precision)
            {
                var template = TypeMap.GetTypeMap(DbType.VarNumeric, (int?)precision, 1);

                template.ShouldBe($"NUMERIC({precision},1)");
            }

            /// <summary>
            /// Defines the test method ItThrowsIfVarnumericPrecisionIsAbove127.
            /// </summary>
            [Test]
            public void ItThrowsIfVarnumericPrecisionIsAbove127()
            {
                Should.Throw<NotSupportedException>(
                    () => TypeMap.GetTypeMap(DbType.VarNumeric, size: 128, precision: null));
            }
        }

        /// <summary>
        /// Defines test class GuidTests.
        /// Implements the <see cref="FluentMigrator.Tests.Unit.Generators.SqlAnywhere.SqlAnywhere16TypeMapTests" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Tests.Unit.Generators.SqlAnywhere.SqlAnywhere16TypeMapTests" />
        [TestFixture]
        public class GuidTests : SqlAnywhere16TypeMapTests
        {
            /// <summary>
            /// Defines the test method ItMapsGUIDToUniqueidentifier.
            /// </summary>
            [Test]
            public void ItMapsGUIDToUniqueidentifier()
            {
                var template = TypeMap.GetTypeMap(DbType.Guid, size: null, precision: null);

                template.ShouldBe("UNIQUEIDENTIFIER");
            }
        }

        /// <summary>
        /// Defines test class DateTimeTests.
        /// Implements the <see cref="FluentMigrator.Tests.Unit.Generators.SqlAnywhere.SqlAnywhere16TypeMapTests" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Tests.Unit.Generators.SqlAnywhere.SqlAnywhere16TypeMapTests" />
        [TestFixture]
        public class DateTimeTests : SqlAnywhere16TypeMapTests
        {
            /// <summary>
            /// Defines the test method ItMapsTimeToDatetime.
            /// </summary>
            [Test]
            public void ItMapsTimeToDatetime()
            {
                var template = TypeMap.GetTypeMap(DbType.Time, size: null, precision: null);

                template.ShouldBe("DATETIME");
            }

            /// <summary>
            /// Defines the test method ItMapsDateToDate.
            /// </summary>
            [Test]
            public void ItMapsDateToDate()
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
                var template = TypeMap.GetTypeMap(DbType.DateTime, size: null, precision: null);

                template.ShouldBe("DATETIME");
            }
        }
    }
}
