// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="OracleTypeMapTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
//
// Copyright (c) 2007-2018, Fluent Migrator Project
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

using FluentMigrator.Runner.Generators.Oracle;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Generators.Oracle
{
    /// <summary>
    /// Defines test class OracleTypeMapTests.
    /// </summary>
    [TestFixture]
    [Category("Oracle")]
    [Category("Generator")]
    [Category("TypeMap")]
    public class OracleTypeMapTests
    {
        /// <summary>
        /// The type map
        /// </summary>
        private OracleTypeMap _typeMap;

        /// <summary>
        /// Sets up.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            _typeMap = new OracleTypeMap();
        }

        // See https://docs.oracle.com/cd/B28359_01/server.111/b28320/limits001.htm#i287903
        // and http://docs.oracle.com/cd/B19306_01/server.102/b14220/datatype.htm#i13446
        // for limits in Oracle data types.
        /// <summary>
        /// Defines the test method AnsiStringDefaultIsVarchar2_255.
        /// </summary>
        [Test]
        public void AnsiStringDefaultIsVarchar2_255()
        {
            _typeMap.GetTypeMap(DbType.AnsiString, size: null, precision: null).ShouldBe("VARCHAR2(255 CHAR)");
        }

        /// <summary>
        /// Defines the test method AnsiStringOfSizeIsVarchar2OfSize.
        /// </summary>
        [Test]
        public void AnsiStringOfSizeIsVarchar2OfSize()
        {
            _typeMap.GetTypeMap(DbType.AnsiString, size: 4000, precision: null).ShouldBe("VARCHAR2(4000 CHAR)");
        }

        /// <summary>
        /// Defines the test method AnsiStringOver4000IsClob.
        /// </summary>
        [Test]
        public void AnsiStringOver4000IsClob()
        {
            _typeMap.GetTypeMap(DbType.AnsiString, size: 4001, precision: null).ShouldBe("CLOB");
        }

        /// <summary>
        /// Defines the test method AnsiStringFixedDefaultIsChar_255.
        /// </summary>
        [Test]
        public void AnsiStringFixedDefaultIsChar_255()
        {
            _typeMap.GetTypeMap(DbType.AnsiStringFixedLength, size: null, precision: null).ShouldBe("CHAR(255 CHAR)");
        }

        /// <summary>
        /// Defines the test method AnsiStringFixedOfSizeIsCharOfSize.
        /// </summary>
        [Test]
        public void AnsiStringFixedOfSizeIsCharOfSize()
        {
            _typeMap.GetTypeMap(DbType.AnsiStringFixedLength, size: 2000, precision: null).ShouldBe("CHAR(2000 CHAR)");
        }


        /// <summary>
        /// Defines the test method BinaryDefaultIsRaw_2000.
        /// </summary>
        [Test]
        public void BinaryDefaultIsRaw_2000()
        {
            _typeMap.GetTypeMap(DbType.Binary, size: null, precision: null).ShouldBe("RAW(2000)");
        }

        /// <summary>
        /// Defines the test method BinaryOfSizeIsRawOfSize.
        /// </summary>
        [Test]
        public void BinaryOfSizeIsRawOfSize()
        {
            _typeMap.GetTypeMap(DbType.Binary, size: 2000, precision: null).ShouldBe("RAW(2000)");
        }


        /// <summary>
        /// Defines the test method BinaryOver2000IsBlob.
        /// </summary>
        [Test]
        public void BinaryOver2000IsBlob()
        {
            _typeMap.GetTypeMap(DbType.Binary, size: 2001, precision: null).ShouldBe("BLOB");
        }

        /// <summary>
        /// Defines the test method BooleanIsNumber.
        /// </summary>
        [Test]
        public void BooleanIsNumber()
        {
            _typeMap.GetTypeMap(DbType.Boolean, size: null, precision: null).ShouldBe("NUMBER(1,0)");
        }

        /// <summary>
        /// Defines the test method ByteIsNumber.
        /// </summary>
        [Test]
        public void ByteIsNumber()
        {
            _typeMap.GetTypeMap(DbType.Byte, size: null, precision: null).ShouldBe("NUMBER(3,0)");
        }

        /// <summary>
        /// Defines the test method CurrencyIsNumber.
        /// </summary>
        [Test]
        public void CurrencyIsNumber()
        {
            _typeMap.GetTypeMap(DbType.Currency, size: null, precision: null).ShouldBe("NUMBER(19,4)");
        }

        /// <summary>
        /// Defines the test method DateIsDate.
        /// </summary>
        [Test]
        public void DateIsDate()
        {
            _typeMap.GetTypeMap(DbType.Date, size: null, precision: null).ShouldBe("DATE");
        }

        /// <summary>
        /// Defines the test method DateTimeIsTimestamp.
        /// </summary>
        [Test]
        public void DateTimeIsTimestamp()
        {
            _typeMap.GetTypeMap(DbType.DateTime, size: null, precision: null).ShouldBe("TIMESTAMP(4)");
        }

        /// <summary>
        /// Defines the test method DateTimeOffsetIsTimestampWithTimeZone.
        /// </summary>
        [Test]
        public void DateTimeOffsetIsTimestampWithTimeZone()
        {
            _typeMap.GetTypeMap(DbType.DateTimeOffset, size: null, precision: null).ShouldBe("TIMESTAMP(4) WITH TIME ZONE");
        }

        /// <summary>
        /// Defines the test method DecimalDefaultIsNumber.
        /// </summary>
        [Test]
        public void DecimalDefaultIsNumber()
        {
            _typeMap.GetTypeMap(DbType.Decimal, size: null, precision: null).ShouldBe("NUMBER(19,5)");
        }

        /// <summary>
        /// Defines the test method DecimalOfPrecisionIsNumberWithPrecision.
        /// </summary>
        [Test]
        public void DecimalOfPrecisionIsNumberWithPrecision()
        {
            _typeMap.GetTypeMap(DbType.Decimal, (int?)8, precision: 3).ShouldBe("NUMBER(8,3)");
        }

        /// <summary>
        /// Defines the test method DoubleIsDouble.
        /// </summary>
        [Test]
        public void DoubleIsDouble()
        {
            _typeMap.GetTypeMap(DbType.Double, size: null, precision: null).ShouldBe("DOUBLE PRECISION");
        }

        /// <summary>
        /// Defines the test method GuidIsRaw.
        /// </summary>
        [Test]
        public void GuidIsRaw()
        {
            _typeMap.GetTypeMap(DbType.Guid, size: null, precision: null).ShouldBe("RAW(16)");
        }

        /// <summary>
        /// Defines the test method Int16IsNumber.
        /// </summary>
        [Test]
        public void Int16IsNumber()
        {
            _typeMap.GetTypeMap(DbType.Int16, size: null, precision: null).ShouldBe("NUMBER(5,0)");
        }

        /// <summary>
        /// Defines the test method In32IsNumber.
        /// </summary>
        [Test]
        public void In32IsNumber()
        {
            _typeMap.GetTypeMap(DbType.Int32, size: null, precision: null).ShouldBe("NUMBER(10,0)");
        }

        /// <summary>
        /// Defines the test method Int64IsNumber.
        /// </summary>
        [Test]
        public void Int64IsNumber()
        {
            _typeMap.GetTypeMap(DbType.Int64, size: null, precision: null).ShouldBe("NUMBER(19,0)");
        }

        /// <summary>
        /// Defines the test method SingleIsFloat.
        /// </summary>
        [Test]
        public void SingleIsFloat()
        {
            _typeMap.GetTypeMap(DbType.Single, size: null, precision: null).ShouldBe("FLOAT(24)");
        }

        /// <summary>
        /// Defines the test method StringFixedLengthDefaultIsNChar_255.
        /// </summary>
        [Test]
        public void StringFixedLengthDefaultIsNChar_255()
        {
            _typeMap.GetTypeMap(DbType.StringFixedLength, size: null, precision: null).ShouldBe("NCHAR(255)");
        }

        /// <summary>
        /// Defines the test method StringFixedLengthOfSizeIsNCharOfSize.
        /// </summary>
        [Test]
        public void StringFixedLengthOfSizeIsNCharOfSize()
        {
            _typeMap.GetTypeMap(DbType.StringFixedLength, size: 2000, precision: null).ShouldBe("NCHAR(2000)");
        }


        /// <summary>
        /// Defines the test method StringDefaultIsNVarchar2_255.
        /// </summary>
        [Test]
        public void StringDefaultIsNVarchar2_255()
        {
            _typeMap.GetTypeMap(DbType.String, size: null, precision: null).ShouldBe("NVARCHAR2(255)");
        }

        /// <summary>
        /// Defines the test method StringOfLengthIsNVarchar2Length.
        /// </summary>
        [Test]
        public void StringOfLengthIsNVarchar2Length()
        {
            _typeMap.GetTypeMap(DbType.String, size: 4000, precision: null).ShouldBe("NVARCHAR2(4000)");
        }

        /// <summary>
        /// Defines the test method TimeIsDate.
        /// </summary>
        [Test]
        public void TimeIsDate()
        {
            _typeMap.GetTypeMap(DbType.Time, size: null, precision: null).ShouldBe("DATE");
        }

        /// <summary>
        /// Defines the test method XmlIsXmltype.
        /// </summary>
        [Test]
        public void XmlIsXmltype()
        {
            _typeMap.GetTypeMap(DbType.Xml, size: null, precision: null).ShouldBe("XMLTYPE");
        }
    }
}
