// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SqlServerCeTypeMapTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Data;

using FluentMigrator.Runner.Generators.SqlServer;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Generators.SqlServerCe
{
    /// <summary>
    /// Defines test class SqlServerCeTypeMapTests.
    /// </summary>
    [TestFixture]
    [Category("SqlServerCe")]
    [Category("Generator")]
    [Category("TypeMap")]
    public abstract class SqlServerCeTypeMapTests
    {
        /// <summary>
        /// Gets or sets the type map.
        /// </summary>
        /// <value>The type map.</value>
        private SqlServerCeTypeMap TypeMap { get; set; }

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            TypeMap = new SqlServerCeTypeMap();
        }

        /// <summary>
        /// Defines test class AnsistringTests.
        /// Implements the <see cref="FluentMigrator.Tests.Unit.Generators.SqlServerCe.SqlServerCeTypeMapTests" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Tests.Unit.Generators.SqlServerCe.SqlServerCeTypeMapTests" />
        [TestFixture]
        public class AnsistringTests : SqlServerCeTypeMapTests
        {
            /// <summary>
            /// Defines the test method ItMapsAnsistringByDefaultToNvarchar255.
            /// </summary>
            [Test]
            public void ItMapsAnsistringByDefaultToNvarchar255()
            {
                var template = TypeMap.GetTypeMap(DbType.AnsiString, size: null, precision: null);

                template.ShouldBe("NVARCHAR(255)");
            }

            /// <summary>
            /// Defines the test method ItMapsAnsistringWithSizeToNvarcharOfSize.
            /// </summary>
            /// <param name="size">The size.</param>
            [Test]
            [TestCase(1)]
            [TestCase(2000)]
            [TestCase(4000)]
            public void ItMapsAnsistringWithSizeToNvarcharOfSize(int size)
            {
                var template = TypeMap.GetTypeMap(DbType.AnsiString, size, precision: null);

                template.ShouldBe($"NVARCHAR({size})");
            }

            /// <summary>
            /// Defines the test method ItMapsAnsistringWithMaxSizeToNtext.
            /// </summary>
            [Test]
            public void ItMapsAnsistringWithMaxSizeToNtext()
            {
                var template = TypeMap.GetTypeMap(DbType.AnsiString, int.MaxValue, precision: null);
                template.ShouldBe("NTEXT");
            }
        }

        /// <summary>
        /// Defines test class AnsistringFixedLengthTests.
        /// Implements the <see cref="FluentMigrator.Tests.Unit.Generators.SqlServerCe.SqlServerCeTypeMapTests" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Tests.Unit.Generators.SqlServerCe.SqlServerCeTypeMapTests" />
        [TestFixture]
        public class AnsistringFixedLengthTests : SqlServerCeTypeMapTests
        {
            /// <summary>
            /// Defines the test method ItMapsAnsistringFixedLengthByDefaultToNchar255.
            /// </summary>
            [Test]
            public void ItMapsAnsistringFixedLengthByDefaultToNchar255()
            {
                var template = TypeMap.GetTypeMap(DbType.AnsiStringFixedLength, size: null, precision: null);

                template.ShouldBe("NCHAR(255)");
            }

            /// <summary>
            /// Defines the test method ItMapsAnsistringFixedLengthWithSizeToNcharOfSize.
            /// </summary>
            /// <param name="size">The size.</param>
            [Test]
            [TestCase(1)]
            [TestCase(2000)]
            [TestCase(4000)]
            public void ItMapsAnsistringFixedLengthWithSizeToNcharOfSize(int size)
            {
                var template = TypeMap.GetTypeMap(DbType.AnsiStringFixedLength, size, precision: null);

                template.ShouldBe($"NCHAR({size})");
            }

            /// <summary>
            /// Defines the test method ItThrowsIfAnsistringFixedLengthHasSizeAbove4000.
            /// </summary>
            [Test]
            public void ItThrowsIfAnsistringFixedLengthHasSizeAbove4000()
            {
                Should.Throw<NotSupportedException>(
                    () => TypeMap.GetTypeMap(DbType.AnsiStringFixedLength, 4001, precision: null));
            }
        }

        /// <summary>
        /// Defines test class StringTests.
        /// Implements the <see cref="FluentMigrator.Tests.Unit.Generators.SqlServerCe.SqlServerCeTypeMapTests" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Tests.Unit.Generators.SqlServerCe.SqlServerCeTypeMapTests" />
        [TestFixture]
        public class StringTests : SqlServerCeTypeMapTests
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
            /// Defines the test method ItThrowsIfStringHasSizeAbove4000.
            /// </summary>
            [Test]
            public void ItThrowsIfStringHasSizeAbove4000()
            {
                Should.Throw<NotSupportedException>(
                    () => TypeMap.GetTypeMap(DbType.AnsiStringFixedLength, 4001, precision: null));
            }
        }

        /// <summary>
        /// Defines test class StringFixedLengthTests.
        /// Implements the <see cref="FluentMigrator.Tests.Unit.Generators.SqlServerCe.SqlServerCeTypeMapTests" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Tests.Unit.Generators.SqlServerCe.SqlServerCeTypeMapTests" />
        [TestFixture]
        public class StringFixedLengthTests : SqlServerCeTypeMapTests
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
                    () => TypeMap.GetTypeMap(DbType.StringFixedLength, 4001, precision: null));
            }
        }

        /// <summary>
        /// Defines test class BinaryTests.
        /// Implements the <see cref="FluentMigrator.Tests.Unit.Generators.SqlServerCe.SqlServerCeTypeMapTests" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Tests.Unit.Generators.SqlServerCe.SqlServerCeTypeMapTests" />
        [TestFixture]
        public class BinaryTests : SqlServerCeTypeMapTests
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
            [TestCase(1073741823)]
            public void ItMapsBinaryWithSizeAbove8000ToImage(int size)
            {
                var template = TypeMap.GetTypeMap(DbType.Binary, size, precision: null);

                template.ShouldBe("IMAGE");
            }

            /// <summary>
            /// Defines the test method ItThrowsIfBinarySizeIsAbove1073741823.
            /// </summary>
            /// <param name="size">The size.</param>
            [Test]
            [TestCase(1073741824)]
            public void ItThrowsIfBinarySizeIsAbove1073741823(int size)
            {
                Should.Throw<NotSupportedException>(
                    () => TypeMap.GetTypeMap(DbType.Binary, size, precision: null));
            }
        }

        /// <summary>
        /// Defines test class NumericTests.
        /// Implements the <see cref="FluentMigrator.Tests.Unit.Generators.SqlServerCe.SqlServerCeTypeMapTests" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Tests.Unit.Generators.SqlServerCe.SqlServerCeTypeMapTests" />
        [TestFixture]
        public class NumericTests : SqlServerCeTypeMapTests
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
            /// Defines the test method ItMapsInt32ToInt.
            /// </summary>
            [Test]
            public void ItMapsInt32ToInt()
            {
                var template = TypeMap.GetTypeMap(DbType.Int32, size: null, precision: null);

                template.ShouldBe("INT");
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

                template.ShouldBe("FLOAT");
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
            /// Defines the test method ItMapsDecimalByDefaultToDecimal195.
            /// </summary>
            [Test]
            public void ItMapsDecimalByDefaultToDecimal195()
            {
                var template = TypeMap.GetTypeMap(DbType.Decimal, size: null, precision: null);

                template.ShouldBe("NUMERIC(19,5)");
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
                var template = TypeMap.GetTypeMap(DbType.Decimal, (int?) precision, precision: 1);

                template.ShouldBe($"NUMERIC({precision},1)");
            }

            /// <summary>
            /// Defines the test method ItThrowsIfDecimalPrecisionIsAbove38.
            /// </summary>
            [Test]
            public void ItThrowsIfDecimalPrecisionIsAbove38()
            {
                Should.Throw<NotSupportedException>(
                    () => TypeMap.GetTypeMap(DbType.Decimal, 39, precision: null));
            }
        }

        /// <summary>
        /// Defines test class GuidTests.
        /// Implements the <see cref="FluentMigrator.Tests.Unit.Generators.SqlServerCe.SqlServerCeTypeMapTests" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Tests.Unit.Generators.SqlServerCe.SqlServerCeTypeMapTests" />
        [TestFixture]
        public class GuidTests : SqlServerCeTypeMapTests
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
        /// Implements the <see cref="FluentMigrator.Tests.Unit.Generators.SqlServerCe.SqlServerCeTypeMapTests" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Tests.Unit.Generators.SqlServerCe.SqlServerCeTypeMapTests" />
        [TestFixture]
        public class DateTimeTests : SqlServerCeTypeMapTests
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
            /// Defines the test method ItMapsDateToDatetime.
            /// </summary>
            [Test]
            public void ItMapsDateToDatetime()
            {
                var template = TypeMap.GetTypeMap(DbType.Date, size: null, precision: null);

                template.ShouldBe("DATETIME");
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

        /// <summary>
        /// Defines test class XmlTests.
        /// Implements the <see cref="FluentMigrator.Tests.Unit.Generators.SqlServerCe.SqlServerCeTypeMapTests" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Tests.Unit.Generators.SqlServerCe.SqlServerCeTypeMapTests" />
        [TestFixture]
        public class XmlTests : SqlServerCeTypeMapTests
        {
            /// <summary>
            /// Defines the test method ItMapsXmlToNtext.
            /// </summary>
            [Test]
            public void ItMapsXmlToNtext()
            {
                var template = TypeMap.GetTypeMap(DbType.Xml, size: null, precision: null);

                template.ShouldBe("NTEXT");
            }
        }
    }
}
