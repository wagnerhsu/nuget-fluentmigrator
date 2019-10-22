// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="QuoterTests.cs" company="FluentMigrator Project">
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
using System.Globalization;
using System.Threading;

using FluentMigrator.Runner.Generators;
using FluentMigrator.Runner.Generators.Generic;
using FluentMigrator.Runner.Generators.Jet;
using FluentMigrator.Runner.Generators.MySql;
using FluentMigrator.Runner.Generators.Oracle;
using FluentMigrator.Runner.Generators.SqlServer;
using FluentMigrator.Runner.Generators.SQLite;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Generators.GenericGenerator
{
    /// <summary>
    /// Defines test class ConstantFormatterTests.
    /// </summary>
    [TestFixture]
    public class ConstantFormatterTests
    {
        /// <summary>
        /// Sets up.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            _quoter = new GenericQuoter();
        }

        /// <summary>
        /// The quoter
        /// </summary>
        private IQuoter _quoter;
        /// <summary>
        /// The current culture
        /// </summary>
        private readonly CultureInfo _currentCulture = Thread.CurrentThread.CurrentCulture;

        /// <summary>
        /// Restores the culture.
        /// </summary>
        private void RestoreCulture()
        {
            Thread.CurrentThread.CurrentCulture = _currentCulture;
        }

        /// <summary>
        /// Changes the culture.
        /// </summary>
        private void ChangeCulture()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("nb-NO");
        }

        /// <summary>
        /// Enum Foo
        /// </summary>
        private enum Foo
        {
            /// <summary>
            /// The bar
            /// </summary>
            Bar,
            // ReSharper disable once UnusedMember.Local
            /// <summary>
            /// The baz
            /// </summary>
            Baz
        }

        /// <summary>
        /// Class CustomClass.
        /// </summary>
        private class CustomClass
        {
            /// <summary>
            /// Returns a <see cref="System.String" /> that represents this instance.
            /// </summary>
            /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
            public override string ToString()
            {
                return "CustomClass";
            }
        }

        /// <summary>
        /// Defines the test method CanEscapeAString.
        /// </summary>
        [Test]
        public void CanEscapeAString()
        {
            _quoter.Quote("Test\"String").ShouldBe("\"Test\"\"String\"");
        }

        /// <summary>
        /// Defines the test method CanHandleAnAlreadyQuotedColumnName.
        /// </summary>
        [Test]
        public void CanHandleAnAlreadyQuotedColumnName()
        {
            _quoter.QuoteColumnName("\"ColumnName\"").ShouldBe("\"ColumnName\"");
        }

        /// <summary>
        /// Defines the test method CanHandleAnAlreadyQuotedSchemaName.
        /// </summary>
        [Test]
        public void CanHandleAnAlreadyQuotedSchemaName()
        {
            _quoter.QuoteColumnName("\"SchemaName\"").ShouldBe("\"SchemaName\"");
        }

        /// <summary>
        /// Defines the test method CanHandleAnAlreadyQuotedTableName.
        /// </summary>
        [Test]
        public void CanHandleAnAlreadyQuotedTableName()
        {
            _quoter.QuoteColumnName("\"TableName\"").ShouldBe("\"TableName\"");
        }

        /// <summary>
        /// Defines the test method CanHandleAnUnQuotedColumnName.
        /// </summary>
        [Test]
        public void CanHandleAnUnQuotedColumnName()
        {
            _quoter.QuoteColumnName("ColumnName").ShouldBe("\"ColumnName\"");
        }

        /// <summary>
        /// Defines the test method CanHandleAnUnQuotedSchemaName.
        /// </summary>
        [Test]
        public void CanHandleAnUnQuotedSchemaName()
        {
            _quoter.QuoteColumnName("SchemaName").ShouldBe("\"SchemaName\"");
        }

        /// <summary>
        /// Defines the test method CanHandleAnUnQuotedTableName.
        /// </summary>
        [Test]
        public void CanHandleAnUnQuotedTableName()
        {
            _quoter.QuoteColumnName("TableName").ShouldBe("\"TableName\"");
        }

        /// <summary>
        /// Defines the test method CanQuoteAString.
        /// </summary>
        [Test]
        public void CanQuoteAString()
        {
            _quoter.Quote("TestString").ShouldBe("\"TestString\"");
        }

        /// <summary>
        /// Defines the test method CanRecogniseAQuotedString.
        /// </summary>
        [Test]
        public void CanRecogniseAQuotedString()
        {
            _quoter.IsQuoted("\"QuotedString\"").ShouldBeTrue();
        }

        /// <summary>
        /// Defines the test method CanRecogniseAnUnQuotedString.
        /// </summary>
        [Test]
        public void CanRecogniseAnUnQuotedString()
        {
            _quoter.IsQuoted("UnQuotedString").ShouldBeFalse();
        }

        /// <summary>
        /// Defines the test method CharIsFormattedWithQuotes.
        /// </summary>
        [Test]
        public void CharIsFormattedWithQuotes()
        {
            _quoter.QuoteValue('A')
                .ShouldBe("'A'");
        }

        /// <summary>
        /// Defines the test method CustomTypeIsBare.
        /// </summary>
        [Test]
        public void CustomTypeIsBare()
        {
            _quoter.QuoteValue(new CustomClass())
                .ShouldBe("CustomClass");
        }

        /// <summary>
        /// Defines the test method DateTimeIsFormattedIso8601WithQuotes.
        /// </summary>
        [Test]
        public void DateTimeIsFormattedIso8601WithQuotes()
        {
            ChangeCulture();
            DateTime date = new DateTime(2010, 1, 2, 18, 4, 5, 123);
            _quoter.QuoteValue(date)
                .ShouldBe("'2010-01-02T18:04:05'");
        }

        /// <summary>
        /// Defines the test method DateTimeIsFormattedIso8601WithQuotes_WithItalyAsCulture.
        /// </summary>
        [Test]
        public void DateTimeIsFormattedIso8601WithQuotes_WithItalyAsCulture()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("it-IT");
            DateTime date = new DateTime(2010, 1, 2, 18, 4, 5, 123);
            _quoter.QuoteValue(date)
                .ShouldBe("'2010-01-02T18:04:05'");
        }

        /// <summary>
        /// Defines the test method DateTimeOffsetIsFormattedIso8601WithQuotes.
        /// </summary>
        [Test]
        public void DateTimeOffsetIsFormattedIso8601WithQuotes()
        {
            ChangeCulture();
            DateTimeOffset date = new DateTimeOffset(2010, 1, 2, 18, 4, 5, 123, TimeSpan.FromHours(-4));
            _quoter.QuoteValue(date).ShouldBe("'2010-01-02T18:04:05-04:00'");
        }

        /// <summary>
        /// Defines the test method DateTimeOffsetIsFormattedIso8601WithQuotes_WithItalyAsCulture.
        /// </summary>
        [Test]
        public void DateTimeOffsetIsFormattedIso8601WithQuotes_WithItalyAsCulture()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("it-IT");
            DateTimeOffset date = new DateTimeOffset(2010, 1, 2, 18, 4, 5, 123, TimeSpan.FromHours(-4));
            _quoter.QuoteValue(date)
                .ShouldBe("'2010-01-02T18:04:05-04:00'");
        }

        /// <summary>
        /// Defines the test method EnumIsFormattedAsString.
        /// </summary>
        [Test]
        public void EnumIsFormattedAsString()
        {
            _quoter.QuoteValue(Foo.Bar)
                .ShouldBe("'Bar'");
        }

        /// <summary>
        /// Defines the test method FalseIsFormattedAsZero.
        /// </summary>
        [Test]
        public void FalseIsFormattedAsZero()
        {
            _quoter.QuoteValue(false)
                .ShouldBe("0");
        }

        /// <summary>
        /// Defines the test method GuidIsFormattedWithQuotes.
        /// </summary>
        [Test]
        public void GuidIsFormattedWithQuotes()
        {
            Guid guid = new Guid("00000000-0000-0000-0000-000000000000");
            _quoter.QuoteValue(guid)
                .ShouldBe("'00000000-0000-0000-0000-000000000000'");
        }

        /// <summary>
        /// Defines the test method Int32IsBare.
        /// </summary>
        [Test]
        public void Int32IsBare()
        {
            _quoter.QuoteValue(1234)
                .ShouldBe("1234");
        }

        /// <summary>
        /// Defines the test method NullIsFormattedAsLiteral.
        /// </summary>
        [Test]
        public void NullIsFormattedAsLiteral()
        {
            _quoter.QuoteValue(null)
                .ShouldBe("NULL");
        }

        /// <summary>
        /// Defines the test method ShouldEscapeJetObjectNames.
        /// </summary>
        [Test]
        public void ShouldEscapeJetObjectNames()
        {
            //This will throw and error on the Jet Engine if special characters are used.
            //We do nothing.
            JetQuoter quoter = new JetQuoter();
            quoter.Quote("[Table]Name").ShouldBe("[[Table]Name]");
        }

        /// <summary>
        /// Defines the test method ShouldEscapeMySqlObjectNames.
        /// </summary>
        [Test]
        public void ShouldEscapeMySqlObjectNames()
        {
            MySqlQuoter quoter = new MySqlQuoter();
            quoter.Quote("`Table`Name").ShouldBe("```Table``Name`");
        }

        /// <summary>
        /// Defines the test method ShouldEscapeOracleObjectNames.
        /// </summary>
        [Test]
        public void ShouldEscapeOracleObjectNames()
        {
            //Do Nothing at the moment due to case sensitivity issues with oracle
            OracleQuoterQuotedIdentifier quoter = new OracleQuoterQuotedIdentifier();
            quoter.Quote("Table\"Name").ShouldBe("\"Table\"\"Name\"");
        }

        /// <summary>
        /// Defines the test method ShouldEscapeSqlServerObjectNames.
        /// </summary>
        [Test]
        public void ShouldEscapeSqlServerObjectNames()
        {
            SqlServer2000Quoter quoter = new SqlServer2000Quoter();
            quoter.Quote("[Table]Name").ShouldBe("[[Table]]Name]");
        }

        /// <summary>
        /// Defines the test method ShouldEscapeSqliteObjectNames.
        /// </summary>
        [Test]
        public void ShouldEscapeSqliteObjectNames()
        {
            SQLiteQuoter quoter = new SQLiteQuoter();
            quoter.Quote("Table\"Name").ShouldBe("\"Table\"\"Name\"");
        }

        /// <summary>
        /// Defines the test method ShouldHandleDecimalToStringConversionInAnyCulture.
        /// </summary>
        [Test]
        public void ShouldHandleDecimalToStringConversionInAnyCulture()
        {
            ChangeCulture();
            _quoter.QuoteValue(new decimal(123.4d)).ShouldBe("123.4");
            RestoreCulture();
        }

        /// <summary>
        /// Defines the test method ShouldHandleDoubleToStringConversionInAnyCulture.
        /// </summary>
        [Test]
        public void ShouldHandleDoubleToStringConversionInAnyCulture()
        {
            ChangeCulture();
            _quoter.QuoteValue(123.4d).ShouldBe("123.4");
            RestoreCulture();
        }

        /// <summary>
        /// Defines the test method ShouldHandleFloatToStringConversionInAnyCulture.
        /// </summary>
        [Test]
        public void ShouldHandleFloatToStringConversionInAnyCulture()
        {
            ChangeCulture();
            _quoter.QuoteValue(123.4f).ShouldBe("123.4");
            RestoreCulture();
        }

        /// <summary>
        /// Defines the test method StringIsFormattedWithQuotes.
        /// </summary>
        [Test]
        public void StringIsFormattedWithQuotes()
        {
            _quoter.QuoteValue("value")
                .ShouldBe("'value'");
        }

        /// <summary>
        /// Defines the test method StringWithQuoteIsFormattedWithDoubleQuote.
        /// </summary>
        [Test]
        public void StringWithQuoteIsFormattedWithDoubleQuote()
        {
            _quoter.QuoteValue("val'ue")
                .ShouldBe("'val''ue'");
        }

        /// <summary>
        /// Defines the test method TrueIsFormattedAsOne.
        /// </summary>
        [Test]
        public void TrueIsFormattedAsOne()
        {
            _quoter.QuoteValue(true)
                .ShouldBe("1");
        }

        /// <summary>
        /// Defines the test method ByteArrayIsFormattedWithQuotes.
        /// </summary>
        [Test]
        public void ByteArrayIsFormattedWithQuotes()
        {
            _quoter.QuoteValue(new byte[] { 0, 254, 13, 18, 125, 17 })
                .ShouldBe("0x00fe0d127d11");
        }

        /// <summary>
        /// Defines the test method TimeSpanIsFormattedQuotes.
        /// </summary>
        [Test]
        public void TimeSpanIsFormattedQuotes()
        {
            _quoter.QuoteValue(new TimeSpan(2, 13, 65))
                .ShouldBe("'02:14:05'");
        }

        /// <summary>
        /// Defines the test method NonUnicodeStringIsFormattedAsNormalString.
        /// </summary>
        [Test]
        public void NonUnicodeStringIsFormattedAsNormalString()
        {
            _quoter.QuoteValue(new NonUnicodeString("Test String")).ShouldBe("'Test String'");
        }

        /// <summary>
        /// Defines the test method NonUnicodeStringIsFormattedAsNormalStringQuotes.
        /// </summary>
        [Test]
        public void NonUnicodeStringIsFormattedAsNormalStringQuotes()
        {
            _quoter.QuoteValue(new NonUnicodeString("Test ' String")).ShouldBe("'Test '' String'");
        }

        /// <summary>
        /// Defines the test method ExplicitUnicodeStringIsFormattedAsNormalString.
        /// </summary>
        [Test]
        [Obsolete]
        public void ExplicitUnicodeStringIsFormattedAsNormalString()
        {
            _quoter.QuoteValue(new ExplicitUnicodeString("Test String")).ShouldBe("'Test String'");
        }

        /// <summary>
        /// Defines the test method ExplicitUnicodeStringIsFormattedAsNormalStringQuotes.
        /// </summary>
        [Test]
        [Obsolete]
        public void ExplicitUnicodeStringIsFormattedAsNormalStringQuotes()
        {
            _quoter.QuoteValue(new ExplicitUnicodeString("Test ' String")).ShouldBe("'Test '' String'");
        }
    }
}
