// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="AlterColumnExpressionBuilderTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
//
// Copyright (c) 2007-2018, Sean Chambers <schambers80@gmail.com>
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

using FluentMigrator.Builders;
using FluentMigrator.Builders.Alter.Column;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using FluentMigrator.Model;
using FluentMigrator.SqlServer;

using Moq;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Builders.Alter
{
    /// <summary>
    /// Defines test class AlterColumnExpressionBuilderTests.
    /// </summary>
    [TestFixture]
    public class AlterColumnExpressionBuilderTests
    {
        /// <summary>
        /// Verifies the column property.
        /// </summary>
        /// <param name="columnExpression">The column expression.</param>
        /// <param name="callToTest">The call to test.</param>
        private void VerifyColumnProperty(Action<ColumnDefinition> columnExpression, Action<AlterColumnExpressionBuilder> callToTest)
        {
            var columnMock = new Mock<ColumnDefinition>();

            var expressionMock = new Mock<AlterColumnExpression>();
            expressionMock.SetupProperty(e => e.Column);

            var expression = expressionMock.Object;
            expression.Column = columnMock.Object;

            var contextMock = new Mock<IMigrationContext>();
            contextMock.SetupGet(mc => mc.Expressions).Returns(new Collection<IMigrationExpression>());

            callToTest(new AlterColumnExpressionBuilder(expression, contextMock.Object));

            columnMock.VerifySet(columnExpression);
        }

        /// <summary>
        /// Verifies the type of the column database.
        /// </summary>
        /// <param name="expected">The expected.</param>
        /// <param name="callToTest">The call to test.</param>
        private void VerifyColumnDbType(DbType expected, Action<AlterColumnExpressionBuilder> callToTest)
        {
            var columnMock = new Mock<ColumnDefinition>();

            var expressionMock = new Mock<AlterColumnExpression>();
            expressionMock.SetupProperty(e => e.Column);

            var expression = expressionMock.Object;
            expression.Column = columnMock.Object;

            var contextMock = new Mock<IMigrationContext>();

            callToTest(new AlterColumnExpressionBuilder(expression, contextMock.Object));

            columnMock.VerifySet(c => c.Type = expected);
        }

        /// <summary>
        /// Verifies the size of the column.
        /// </summary>
        /// <param name="expected">The expected.</param>
        /// <param name="callToTest">The call to test.</param>
        private void VerifyColumnSize(int expected, Action<AlterColumnExpressionBuilder> callToTest)
        {
            var columnMock = new Mock<ColumnDefinition>();

            var expressionMock = new Mock<AlterColumnExpression>();
            expressionMock.SetupProperty(e => e.Column);

            var expression = expressionMock.Object;
            expression.Column = columnMock.Object;

            var contextMock = new Mock<IMigrationContext>();

            callToTest(new AlterColumnExpressionBuilder(expression, contextMock.Object));

            columnMock.VerifySet(c => c.Size = expected);
        }

        /// <summary>
        /// Verifies the column precision.
        /// </summary>
        /// <param name="expected">The expected.</param>
        /// <param name="callToTest">The call to test.</param>
        private void VerifyColumnPrecision(int expected, Action<AlterColumnExpressionBuilder> callToTest)
        {
            var columnMock = new Mock<ColumnDefinition>();

            var expressionMock = new Mock<AlterColumnExpression>();
            expressionMock.SetupProperty(e => e.Column);

            var expression = expressionMock.Object;
            expression.Column = columnMock.Object;

            var contextMock = new Mock<IMigrationContext>();

            callToTest(new AlterColumnExpressionBuilder(expression, contextMock.Object));

            columnMock.VerifySet(c => c.Precision = expected);
        }

        /// <summary>
        /// Verifies the column collation.
        /// </summary>
        /// <param name="expected">The expected.</param>
        /// <param name="callToTest">The call to test.</param>
        private void VerifyColumnCollation(string expected, Action<AlterColumnExpressionBuilder> callToTest)
        {
            var columnMock = new Mock<ColumnDefinition>();

            var expressionMock = new Mock<AlterColumnExpression>();
            expressionMock.SetupProperty(e => e.Column);

            var expression = expressionMock.Object;
            expression.Column = columnMock.Object;

            var contextMock = new Mock<IMigrationContext>();

            callToTest(new AlterColumnExpressionBuilder(expression, contextMock.Object));

            columnMock.VerifySet(c => c.CollationName = expected);
        }

        /// <summary>
        /// Defines the test method CallingAsAnsiStringSetsColumnDbTypeToAnsiString.
        /// </summary>
        [Test]
        public void CallingAsAnsiStringSetsColumnDbTypeToAnsiString()
        {
            VerifyColumnDbType(DbType.AnsiString, b => b.AsAnsiString());
        }

        /// <summary>
        /// Defines the test method CallingAsAnsiStringWithSizeSetsColumnDbTypeToAnsiString.
        /// </summary>
        [Test]
        public void CallingAsAnsiStringWithSizeSetsColumnDbTypeToAnsiString()
        {
            VerifyColumnDbType(DbType.AnsiString, b => b.AsAnsiString(42));
        }

        /// <summary>
        /// Defines the test method CallingAsAnsiStringWithSizeSetsColumnSizeToSpecifiedValue.
        /// </summary>
        [Test]
        public void CallingAsAnsiStringWithSizeSetsColumnSizeToSpecifiedValue()
        {
            VerifyColumnSize(42, b => b.AsAnsiString(42));
        }

        /// <summary>
        /// Defines the test method CallingAsBinarySetsColumnDbTypeToBinary.
        /// </summary>
        [Test]
        public void CallingAsBinarySetsColumnDbTypeToBinary()
        {
            VerifyColumnDbType(DbType.Binary, b => b.AsBinary());
        }

        /// <summary>
        /// Defines the test method CallingAsBinaryWithSizeSetsColumnDbTypeToBinary.
        /// </summary>
        [Test]
        public void CallingAsBinaryWithSizeSetsColumnDbTypeToBinary()
        {
            VerifyColumnDbType(DbType.Binary, b => b.AsBinary(42));
        }

        /// <summary>
        /// Defines the test method CallingAsBinaryWithSizeSetsColumnSizeToSpecifiedValue.
        /// </summary>
        [Test]
        public void CallingAsBinaryWithSizeSetsColumnSizeToSpecifiedValue()
        {
            VerifyColumnSize(42, b => b.AsBinary(42));
        }

        /// <summary>
        /// Defines the test method CallingAsBooleanSetsColumnDbTypeToBoolean.
        /// </summary>
        [Test]
        public void CallingAsBooleanSetsColumnDbTypeToBoolean()
        {
            VerifyColumnDbType(DbType.Boolean, b => b.AsBoolean());
        }

        /// <summary>
        /// Defines the test method CallingAsByteSetsColumnDbTypeToByte.
        /// </summary>
        [Test]
        public void CallingAsByteSetsColumnDbTypeToByte()
        {
            VerifyColumnDbType(DbType.Byte, b => b.AsByte());
        }

        /// <summary>
        /// Defines the test method CallingAsCurrencySetsColumnDbTypeToCurrency.
        /// </summary>
        [Test]
        public void CallingAsCurrencySetsColumnDbTypeToCurrency()
        {
            VerifyColumnDbType(DbType.Currency, b => b.AsCurrency());
        }

        /// <summary>
        /// Defines the test method CallingAsCustomSetsTypeToNullAndSetsCustomType.
        /// </summary>
        [Test]
        public void CallingAsCustomSetsTypeToNullAndSetsCustomType()
        {
            VerifyColumnProperty(c => c.Type = null, b => b.AsCustom("Test"));
            VerifyColumnProperty(c => c.CustomType = "Test", b => b.AsCustom("Test"));
        }

        /// <summary>
        /// Defines the test method CallingAsDateSetsColumnDbTypeToDate.
        /// </summary>
        [Test]
        public void CallingAsDateSetsColumnDbTypeToDate()
        {
            VerifyColumnDbType(DbType.Date, b => b.AsDate());
        }

        /// <summary>
        /// Defines the test method CallingAsDateTimeSetsColumnDbTypeToDateTime.
        /// </summary>
        [Test]
        public void CallingAsDateTimeSetsColumnDbTypeToDateTime()
        {
            VerifyColumnDbType(DbType.DateTime, b => b.AsDateTime());
        }

        /// <summary>
        /// Defines the test method CallingAsDateTime2SetsColumnDbTypeToDateTime2.
        /// </summary>
        [Test]
        public void CallingAsDateTime2SetsColumnDbTypeToDateTime2()
        {
            VerifyColumnDbType(DbType.DateTime2, b => b.AsDateTime2());
        }

        /// <summary>
        /// Defines the test method CallingAsDateTimeOffsetSetsColumnDbTypeToDateTimeOffset.
        /// </summary>
        [Test]
        public void CallingAsDateTimeOffsetSetsColumnDbTypeToDateTimeOffset()
        {
            VerifyColumnDbType(DbType.DateTimeOffset, b => b.AsDateTimeOffset());
        }

        /// <summary>
        /// Defines the test method CallingAsDecimalSetsColumnDbTypeToDecimal.
        /// </summary>
        [Test]
        public void CallingAsDecimalSetsColumnDbTypeToDecimal()
        {
            VerifyColumnDbType(DbType.Decimal, b => b.AsDecimal());
        }

        /// <summary>
        /// Defines the test method CallingAsDecimalStringSetsColumnPrecisionToSpecifiedValue.
        /// </summary>
        [Test]
        public void CallingAsDecimalStringSetsColumnPrecisionToSpecifiedValue()
        {
            VerifyColumnPrecision(2, b => b.AsDecimal(1, 2));
        }

        /// <summary>
        /// Defines the test method CallingAsDecimalStringSetsColumnSizeToSpecifiedValue.
        /// </summary>
        [Test]
        public void CallingAsDecimalStringSetsColumnSizeToSpecifiedValue()
        {
            VerifyColumnSize(1, b => b.AsDecimal(1, 2));
        }

        /// <summary>
        /// Defines the test method CallingAsDecimalWithSizeAndPrecisionSetsColumnDbTypeToDecimal.
        /// </summary>
        [Test]
        public void CallingAsDecimalWithSizeAndPrecisionSetsColumnDbTypeToDecimal()
        {
            VerifyColumnDbType(DbType.Decimal, b => b.AsDecimal(1, 2));
        }

        /// <summary>
        /// Defines the test method CallingAsDoubleSetsColumnDbTypeToDouble.
        /// </summary>
        [Test]
        public void CallingAsDoubleSetsColumnDbTypeToDouble()
        {
            VerifyColumnDbType(DbType.Double, b => b.AsDouble());
        }

        /// <summary>
        /// Defines the test method CallingAsFixedLengthAnsiStringSetsColumnDbTypeToAnsiStringFixedLength.
        /// </summary>
        [Test]
        public void CallingAsFixedLengthAnsiStringSetsColumnDbTypeToAnsiStringFixedLength()
        {
            VerifyColumnDbType(DbType.AnsiStringFixedLength, b => b.AsFixedLengthAnsiString(255));
        }

        /// <summary>
        /// Defines the test method CallingAsFixedLengthAnsiStringSetsColumnSizeToSpecifiedValue.
        /// </summary>
        [Test]
        public void CallingAsFixedLengthAnsiStringSetsColumnSizeToSpecifiedValue()
        {
            VerifyColumnSize(255, b => b.AsFixedLengthAnsiString(255));
        }

        /// <summary>
        /// Defines the test method CallingAsFixedLengthStringSetsColumnDbTypeToStringFixedLength.
        /// </summary>
        [Test]
        public void CallingAsFixedLengthStringSetsColumnDbTypeToStringFixedLength()
        {
            VerifyColumnDbType(DbType.StringFixedLength, e => e.AsFixedLengthString(255));
        }

        /// <summary>
        /// Defines the test method CallingAsFixedLengthStringSetsColumnSizeToSpecifiedValue.
        /// </summary>
        [Test]
        public void CallingAsFixedLengthStringSetsColumnSizeToSpecifiedValue()
        {
            VerifyColumnSize(255, b => b.AsFixedLengthString(255));
        }

        /// <summary>
        /// Defines the test method CallingAsFloatSetsColumnDbTypeToSingle.
        /// </summary>
        [Test]
        public void CallingAsFloatSetsColumnDbTypeToSingle()
        {
            VerifyColumnDbType(DbType.Single, b => b.AsFloat());
        }

        /// <summary>
        /// Defines the test method CallingAsGuidSetsColumnDbTypeToGuid.
        /// </summary>
        [Test]
        public void CallingAsGuidSetsColumnDbTypeToGuid()
        {
            VerifyColumnDbType(DbType.Guid, b => b.AsGuid());
        }

        /// <summary>
        /// Defines the test method CallingAsInt16SetsColumnDbTypeToInt16.
        /// </summary>
        [Test]
        public void CallingAsInt16SetsColumnDbTypeToInt16()
        {
            VerifyColumnDbType(DbType.Int16, b => b.AsInt16());
        }

        /// <summary>
        /// Defines the test method CallingAsInt32SetsColumnDbTypeToInt32.
        /// </summary>
        [Test]
        public void CallingAsInt32SetsColumnDbTypeToInt32()
        {
            VerifyColumnDbType(DbType.Int32, b => b.AsInt32());
        }

        /// <summary>
        /// Defines the test method CallingAsInt64SetsColumnDbTypeToInt64.
        /// </summary>
        [Test]
        public void CallingAsInt64SetsColumnDbTypeToInt64()
        {
            VerifyColumnDbType(DbType.Int64, b => b.AsInt64());
        }

        /// <summary>
        /// Defines the test method CallingAsStringSetsColumnDbTypeToString.
        /// </summary>
        [Test]
        public void CallingAsStringSetsColumnDbTypeToString()
        {
            VerifyColumnDbType(DbType.String, b => b.AsString());
        }

        /// <summary>
        /// Defines the test method CallingAsStringSetsColumnSizeToSpecifiedValue.
        /// </summary>
        [Test]
        public void CallingAsStringSetsColumnSizeToSpecifiedValue()
        {
            VerifyColumnSize(255, b => b.AsFixedLengthAnsiString(255));
        }

        /// <summary>
        /// Defines the test method CallingAsStringWithLengthSetsColumnDbTypeToString.
        /// </summary>
        [Test]
        public void CallingAsStringWithLengthSetsColumnDbTypeToString()
        {
            VerifyColumnDbType(DbType.String, b => b.AsString(255));
        }

        /// <summary>
        /// Defines the test method CallingAsAnsiStringWithCollation.
        /// </summary>
        [Test]
        public void CallingAsAnsiStringWithCollation()
        {
            VerifyColumnCollation(Generators.GeneratorTestHelper.TestColumnCollationName, b => b.AsAnsiString(Generators.GeneratorTestHelper.TestColumnCollationName));
        }

        /// <summary>
        /// Defines the test method CallingAsFixedLengthAnsiStringWithCollation.
        /// </summary>
        [Test]
        public void CallingAsFixedLengthAnsiStringWithCollation()
        {
            VerifyColumnCollation(Generators.GeneratorTestHelper.TestColumnCollationName, b => b.AsFixedLengthAnsiString(255, Generators.GeneratorTestHelper.TestColumnCollationName));
        }

        /// <summary>
        /// Defines the test method CallingAsFixedLengthStringWithCollation.
        /// </summary>
        [Test]
        public void CallingAsFixedLengthStringWithCollation()
        {
            VerifyColumnCollation(Generators.GeneratorTestHelper.TestColumnCollationName, b => b.AsFixedLengthString(255, Generators.GeneratorTestHelper.TestColumnCollationName));
        }

        /// <summary>
        /// Defines the test method CallingAsStringWithCollation.
        /// </summary>
        [Test]
        public void CallingAsStringWithCollation()
        {
            VerifyColumnCollation(Generators.GeneratorTestHelper.TestColumnCollationName, b => b.AsString(Generators.GeneratorTestHelper.TestColumnCollationName));
        }

        /// <summary>
        /// Defines the test method CallingAsTimeSetsColumnDbTypeToTime.
        /// </summary>
        [Test]
        public void CallingAsTimeSetsColumnDbTypeToTime()
        {
            VerifyColumnDbType(DbType.Time, b => b.AsTime());
        }

        /// <summary>
        /// Defines the test method CallingAsXmlSetsColumnDbTypeToXml.
        /// </summary>
        [Test]
        public void CallingAsXmlSetsColumnDbTypeToXml()
        {
            VerifyColumnDbType(DbType.Xml, b => b.AsXml());
        }

        /// <summary>
        /// Defines the test method CallingAsXmlSetsColumnSizeToSpecifiedValue.
        /// </summary>
        [Test]
        public void CallingAsXmlSetsColumnSizeToSpecifiedValue()
        {
            VerifyColumnSize(255, b => b.AsXml(255));
        }

        /// <summary>
        /// Defines the test method CallingAsXmlWithSizeSetsColumnDbTypeToXml.
        /// </summary>
        [Test]
        public void CallingAsXmlWithSizeSetsColumnDbTypeToXml()
        {
            VerifyColumnDbType(DbType.Xml, b => b.AsXml(255));
        }

        /// <summary>
        /// Defines the test method CallingForeignKeyAddsNewForeignKeyExpressionToContext.
        /// </summary>
        [Test]
        public void CallingForeignKeyAddsNewForeignKeyExpressionToContext()
        {
            var collectionMock = new Mock<ICollection<IMigrationExpression>>();

            var contextMock = new Mock<IMigrationContext>();
            contextMock.Setup(x => x.Expressions).Returns(collectionMock.Object);

            var columnMock = new Mock<ColumnDefinition>();
            columnMock.SetupGet(x => x.Name).Returns("BaconId");

            var expressionMock = new Mock<AlterColumnExpression>();
            expressionMock.SetupGet(x => x.TableName).Returns("Bacon");
            expressionMock.SetupGet(x => x.Column).Returns(columnMock.Object);

            var builder = new AlterColumnExpressionBuilder(expressionMock.Object, contextMock.Object);

            builder.ForeignKey("fk_foo", "FooTable", "BarColumn");

            collectionMock.Verify(x => x.Add(It.Is<CreateForeignKeyExpression>(
                fk => fk.ForeignKey.Name == "fk_foo" &&
                      fk.ForeignKey.PrimaryTable == "FooTable" &&
                      fk.ForeignKey.PrimaryColumns.Contains("BarColumn") &&
                      fk.ForeignKey.PrimaryColumns.Count == 1 &&
                      fk.ForeignKey.ForeignTable == "Bacon" &&
                      fk.ForeignKey.ForeignColumns.Contains("BaconId") &&
                      fk.ForeignKey.ForeignColumns.Count == 1
                                                 )));

            contextMock.VerifyGet(x => x.Expressions);
        }

        /// <summary>
        /// Defines the test method CallingForeignKeySetsIsForeignKeyToTrue.
        /// </summary>
        [Test]
        public void CallingForeignKeySetsIsForeignKeyToTrue()
        {
            VerifyColumnProperty(c => c.IsForeignKey = true, b => b.ForeignKey());
        }

        /// <summary>
        /// Defines the test method CallingIdentitySetsIsIdentityToTrue.
        /// </summary>
        [Test]
        public void CallingIdentitySetsIsIdentityToTrue()
        {
            VerifyColumnProperty(c => c.IsIdentity = true, b => b.Identity());
        }

        /// <summary>
        /// Defines the test method CallingSeededIdentitySetsAdditionalProperties.
        /// </summary>
        [Test]
        public void CallingSeededIdentitySetsAdditionalProperties()
        {
            var contextMock = new Mock<IMigrationContext>();

            var columnMock = new Mock<ColumnDefinition>();
            columnMock.SetupGet(x => x.Name).Returns("BaconId");

            var expressionMock = new Mock<AlterColumnExpression>();
            expressionMock.SetupGet(x => x.Column).Returns(columnMock.Object);

            var builder = new AlterColumnExpressionBuilder(expressionMock.Object, contextMock.Object);
            builder.Identity(23, 44);

            columnMock.Object.AdditionalFeatures.ShouldContain(
                new KeyValuePair<string, object>(SqlServerExtensions.IdentitySeed, 23));
            columnMock.Object.AdditionalFeatures.ShouldContain(
                new KeyValuePair<string, object>(SqlServerExtensions.IdentityIncrement, 44));
        }

        /// <summary>
        /// Defines the test method CallingSeededLongIdentitySetsAdditionalProperties.
        /// </summary>
        [Test]
        public void CallingSeededLongIdentitySetsAdditionalProperties()
        {
            var contextMock = new Mock<IMigrationContext>();

            var columnMock = new Mock<ColumnDefinition>();
            columnMock.SetupGet(x => x.Name).Returns("BaconId");

            var expressionMock = new Mock<AlterColumnExpression>();
            expressionMock.SetupGet(x => x.Column).Returns(columnMock.Object);

            var builder = new AlterColumnExpressionBuilder(expressionMock.Object, contextMock.Object);
            builder.Identity(long.MaxValue, 44);

            columnMock.Object.AdditionalFeatures.ShouldContain(
                new KeyValuePair<string, object>(SqlServerExtensions.IdentitySeed, long.MaxValue));
            columnMock.Object.AdditionalFeatures.ShouldContain(
                new KeyValuePair<string, object>(SqlServerExtensions.IdentityIncrement, 44));
        }

        /// <summary>
        /// Defines the test method CallingIndexedCallsHelperWithNullIndexName.
        /// </summary>
        [Test]
        public void CallingIndexedCallsHelperWithNullIndexName()
        {
            VerifyColumnHelperCall(c => c.Indexed(), h => h.Indexed(null));
        }

        /// <summary>
        /// Defines the test method CallingIndexedNamedCallsHelperWithName.
        /// </summary>
        [Test]
        public void CallingIndexedNamedCallsHelperWithName()
        {
            VerifyColumnHelperCall(c => c.Indexed("MyIndexName"), h => h.Indexed("MyIndexName"));
        }

        /// <summary>
        /// Defines the test method NullableUsesHelper.
        /// </summary>
        [Test]
        public void NullableUsesHelper()
        {
            VerifyColumnHelperCall(c => c.Nullable(), h => h.SetNullable(true));
        }

        /// <summary>
        /// Defines the test method NotNullableUsesHelper.
        /// </summary>
        [Test]
        public void NotNullableUsesHelper()
        {
            VerifyColumnHelperCall(c => c.NotNullable(), h => h.SetNullable(false));
        }

        /// <summary>
        /// Defines the test method UniqueUsesHelper.
        /// </summary>
        [Test]
        public void UniqueUsesHelper()
        {
            VerifyColumnHelperCall(c => c.Unique(), h => h.Unique(null));
        }

        /// <summary>
        /// Defines the test method NamedUniqueUsesHelper.
        /// </summary>
        [Test]
        public void NamedUniqueUsesHelper()
        {
            VerifyColumnHelperCall(c => c.Unique("asdf"), h => h.Unique("asdf"));
        }

        /// <summary>
        /// Defines the test method CallingOnTableSetsTableName.
        /// </summary>
        [Test]
        public void CallingOnTableSetsTableName()
        {
            var expressionMock = new Mock<AlterColumnExpression>();

            var contextMock = new Mock<IMigrationContext>();

            var builder = new AlterColumnExpressionBuilder(expressionMock.Object, contextMock.Object);
            builder.OnTable("Bacon");

            expressionMock.VerifySet(x => x.TableName = "Bacon");
        }

        /// <summary>
        /// Defines the test method CallingPrimaryKeySetsIsPrimaryKeyToTrue.
        /// </summary>
        [Test]
        public void CallingPrimaryKeySetsIsPrimaryKeyToTrue()
        {
            VerifyColumnProperty(c => c.IsPrimaryKey = true, b => b.PrimaryKey());
        }

        /// <summary>
        /// Defines the test method CallingReferencedByAddsNewForeignKeyExpressionToContext.
        /// </summary>
        [Test]
        public void CallingReferencedByAddsNewForeignKeyExpressionToContext()
        {
            var collectionMock = new Mock<ICollection<IMigrationExpression>>();

            var contextMock = new Mock<IMigrationContext>();
            contextMock.Setup(x => x.Expressions).Returns(collectionMock.Object);

            var columnMock = new Mock<ColumnDefinition>();
            columnMock.SetupGet(x => x.Name).Returns("BaconId");

            var expressionMock = new Mock<AlterColumnExpression>();
            expressionMock.SetupGet(x => x.TableName).Returns("Bacon");
            expressionMock.SetupGet(x => x.Column).Returns(columnMock.Object);

            var builder = new AlterColumnExpressionBuilder(expressionMock.Object, contextMock.Object);

            builder.ReferencedBy("fk_foo", "FooTable", "BarColumn");

            collectionMock.Verify(x => x.Add(It.Is<CreateForeignKeyExpression>(
                fk => fk.ForeignKey.Name == "fk_foo" &&
                      fk.ForeignKey.ForeignTable == "FooTable" &&
                      fk.ForeignKey.ForeignColumns.Contains("BarColumn") &&
                      fk.ForeignKey.ForeignColumns.Count == 1 &&
                      fk.ForeignKey.PrimaryTable == "Bacon" &&
                      fk.ForeignKey.PrimaryColumns.Contains("BaconId") &&
                      fk.ForeignKey.PrimaryColumns.Count == 1
                                                 )));

            contextMock.VerifyGet(x => x.Expressions);
        }

        /// <summary>
        /// Defines the test method CallingOnUpdateSetsOnUpdateOnForeignKeyExpression.
        /// </summary>
        /// <param name="rule">The rule.</param>
        [TestCase(Rule.Cascade), TestCase(Rule.SetDefault), TestCase(Rule.SetNull), TestCase(Rule.None)]
        public void CallingOnUpdateSetsOnUpdateOnForeignKeyExpression(Rule rule)
        {
            var builder = new AlterColumnExpressionBuilder(null, null) { CurrentForeignKey = new ForeignKeyDefinition() };
            builder.OnUpdate(rule);
            Assert.That(builder.CurrentForeignKey.OnUpdate, Is.EqualTo(rule));
            Assert.That(builder.CurrentForeignKey.OnDelete, Is.EqualTo(Rule.None));
        }

        /// <summary>
        /// Defines the test method CallingOnDeleteSetsOnDeleteOnForeignKeyExpression.
        /// </summary>
        /// <param name="rule">The rule.</param>
        [TestCase(Rule.Cascade), TestCase(Rule.SetDefault), TestCase(Rule.SetNull), TestCase(Rule.None)]
        public void CallingOnDeleteSetsOnDeleteOnForeignKeyExpression(Rule rule)
        {
            var builder = new AlterColumnExpressionBuilder(null, null) { CurrentForeignKey = new ForeignKeyDefinition() };
            builder.OnDelete(rule);
            Assert.That(builder.CurrentForeignKey.OnUpdate, Is.EqualTo(Rule.None));
            Assert.That(builder.CurrentForeignKey.OnDelete, Is.EqualTo(rule));
        }

        /// <summary>
        /// Defines the test method CallingOnDeleteOrUpdateSetsOnUpdateAndOnDeleteOnForeignKeyExpression.
        /// </summary>
        /// <param name="rule">The rule.</param>
        [TestCase(Rule.Cascade), TestCase(Rule.SetDefault), TestCase(Rule.SetNull), TestCase(Rule.None)]
        public void CallingOnDeleteOrUpdateSetsOnUpdateAndOnDeleteOnForeignKeyExpression(Rule rule)
        {
            var builder = new AlterColumnExpressionBuilder(null, null) { CurrentForeignKey = new ForeignKeyDefinition() };
            builder.OnDeleteOrUpdate(rule);
            Assert.That(builder.CurrentForeignKey.OnUpdate, Is.EqualTo(rule));
            Assert.That(builder.CurrentForeignKey.OnDelete, Is.EqualTo(rule));
        }

        /// <summary>
        /// Defines the test method CallingWithDefaultValueAddsAlterDefaultConstraintExpression.
        /// </summary>
        [Test]
        public void CallingWithDefaultValueAddsAlterDefaultConstraintExpression()
        {
            const int value = 42;

            var columnMock = new Mock<ColumnDefinition>();

            var expressionMock = new Mock<AlterColumnExpression>();
            expressionMock.SetupProperty(e => e.Column);

            var expression = expressionMock.Object;
            expression.Column = columnMock.Object;

            var collectionMock = new Mock<ICollection<IMigrationExpression>>();

            var contextMock = new Mock<IMigrationContext>();
            contextMock.Setup(x => x.Expressions).Returns(collectionMock.Object);

            var builder = new AlterColumnExpressionBuilder(expressionMock.Object, contextMock.Object);
            builder.WithDefaultValue(value);

            columnMock.VerifySet(c => c.DefaultValue = value);
            collectionMock.Verify(x => x.Add(It.Is<AlterDefaultConstraintExpression>(e => e.DefaultValue.Equals(value))));
            contextMock.VerifyGet(x => x.Expressions);
        }

        /// <summary>
        /// Defines the test method CallingWithDefaultAddsAlterDefaultConstraintExpression.
        /// </summary>
        [Test]
        public void CallingWithDefaultAddsAlterDefaultConstraintExpression()
        {
            var columnMock = new Mock<ColumnDefinition>();

            var expressionMock = new Mock<AlterColumnExpression>();
            expressionMock.SetupProperty(e => e.Column);

            var expression = expressionMock.Object;
            expression.Column = columnMock.Object;

            var collectionMock = new Mock<ICollection<IMigrationExpression>>();

            var contextMock = new Mock<IMigrationContext>();
            contextMock.Setup(x => x.Expressions).Returns(collectionMock.Object);

            var builder = new AlterColumnExpressionBuilder(expressionMock.Object, contextMock.Object);
            builder.WithDefault(SystemMethods.NewGuid);

            columnMock.VerifySet(c => c.DefaultValue = SystemMethods.NewGuid);
            collectionMock.Verify(x => x.Add(It.Is<AlterDefaultConstraintExpression>(e => e.DefaultValue.Equals(SystemMethods.NewGuid))));
            contextMock.VerifyGet(x => x.Expressions);
        }

        /// <summary>
        /// Defines the test method ColumnHelperSetOnCreation.
        /// </summary>
        [Test]
        public void ColumnHelperSetOnCreation()
        {
            var expressionMock = new Mock<AlterColumnExpression>();
            var contextMock = new Mock<IMigrationContext>();

            var builder = new AlterColumnExpressionBuilder(expressionMock.Object, contextMock.Object);

            Assert.IsNotNull(builder.ColumnHelper);
        }

        /// <summary>
        /// Defines the test method ColumnExpressionBuilderUsesExpressionSchemaAndTableName.
        /// </summary>
        [Test]
        public void ColumnExpressionBuilderUsesExpressionSchemaAndTableName()
        {
            var expressionMock = new Mock<AlterColumnExpression>();
            var contextMock = new Mock<IMigrationContext>();
            expressionMock.SetupGet(n => n.SchemaName).Returns("Fred");
            expressionMock.SetupGet(n => n.TableName).Returns("Flinstone");

            var builder = new AlterColumnExpressionBuilder(expressionMock.Object, contextMock.Object);
            var builderAsInterface = (IColumnExpressionBuilder)builder;

            Assert.AreEqual("Fred", builderAsInterface.SchemaName);
            Assert.AreEqual("Flinstone", builderAsInterface.TableName);
        }

        /// <summary>
        /// Defines the test method ColumnExpressionBuilderUsesExpressionColumn.
        /// </summary>
        [Test]
        public void ColumnExpressionBuilderUsesExpressionColumn()
        {
            var expressionMock = new Mock<AlterColumnExpression>();
            var contextMock = new Mock<IMigrationContext>();
            var curColumn = new Mock<ColumnDefinition>().Object;
            expressionMock.SetupGet(n => n.Column).Returns(curColumn);

            var builder = new AlterColumnExpressionBuilder(expressionMock.Object, contextMock.Object);
            var builderAsInterface = (IColumnExpressionBuilder)builder;

            Assert.AreSame(curColumn, builderAsInterface.Column);
        }

        /// <summary>
        /// Verifies the column helper call.
        /// </summary>
        /// <param name="callToTest">The call to test.</param>
        /// <param name="expectedHelperAction">The expected helper action.</param>
        private void VerifyColumnHelperCall(Action<AlterColumnExpressionBuilder> callToTest, System.Linq.Expressions.Expression<Action<ColumnExpressionBuilderHelper>> expectedHelperAction)
        {
            var expressionMock = new Mock<AlterColumnExpression>();
            var contextMock = new Mock<IMigrationContext>();
            var helperMock = new Mock<ColumnExpressionBuilderHelper>();

            var builder = new AlterColumnExpressionBuilder(expressionMock.Object, contextMock.Object);
            builder.ColumnHelper = helperMock.Object;

            callToTest(builder);

            helperMock.Verify(expectedHelperAction);
        }
    }
}
