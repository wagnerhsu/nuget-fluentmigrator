// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="TimestampedMigrationAttributeTests.cs" company="FluentMigrator Project">
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

using NUnit.Framework;

namespace FluentMigrator.Tests.Unit
{
    /// <summary>
    /// Defines test class TimestampedMigrationAttributeTests.
    /// </summary>
    [TestFixture]
    [SetUICulture("en-US")]
    public class TimestampedMigrationAttributeTests
    {
        /// <summary>
        /// The day of month
        /// </summary>
        private const int DayOfMonth = 15;

        /// <summary>
        /// The description
        /// </summary>
        private const string Description = "Description";

        /// <summary>
        /// The hour
        /// </summary>
        private const int Hour = 12;

        /// <summary>
        /// The invalid date exception message
        /// </summary>
        private const string InvalidDateExceptionMessage = "Year, Month, and Day parameters describe an un-representable DateTime.";

        /// <summary>
        /// The invalid time exception message
        /// </summary>
        private const string InvalidTimeExceptionMessage = "Hour, Minute, and Second parameters describe an un-representable DateTime.";

        /// <summary>
        /// The minute
        /// </summary>
        private const int Minute = 30;

        /// <summary>
        /// The month
        /// </summary>
        private const int Month = 06;

        /// <summary>
        /// The second
        /// </summary>
        private const int Second = 30;

        /// <summary>
        /// The year
        /// </summary>
        private const int Year = 2000;

        /// <summary>
        /// Defines the test method CanCreateOneAccurateToTheMinute.
        /// </summary>
        [Test]
        public void CanCreateOneAccurateToTheMinute()
        {
            Assert.That(() => new TimestampedMigrationAttribute(Year, Month, DayOfMonth, Hour, Minute), Throws.Nothing);
        }

        /// <summary>
        /// Defines the test method CanCreateOneAccurateToTheMinuteWithDescription.
        /// </summary>
        [Test]
        public void CanCreateOneAccurateToTheMinuteWithDescription()
        {
            Assert.That(() => new TimestampedMigrationAttribute(Year, Month, DayOfMonth, Hour, Minute, Description), Throws.Nothing);
        }

        /// <summary>
        /// Defines the test method CanCreateOneAccurateToTheMinuteWithTransactionBehavior.
        /// </summary>
        [Test]
        public void CanCreateOneAccurateToTheMinuteWithTransactionBehavior()
        {
            Assert.That(() => new TimestampedMigrationAttribute(Year, Month, DayOfMonth, Hour, Minute, TransactionBehavior.Default), Throws.Nothing);
        }

        /// <summary>
        /// Defines the test method CanCreateOneAccurateToTheMinuteWithTransactionBehaviorAndDescription.
        /// </summary>
        [Test]
        public void CanCreateOneAccurateToTheMinuteWithTransactionBehaviorAndDescription()
        {
            Assert.That(() => new TimestampedMigrationAttribute(Year, Month, DayOfMonth, Hour, Minute, TransactionBehavior.Default, Description), Throws.Nothing);
        }

        /// <summary>
        /// Defines the test method CanCreateOneAccurateToTheSecond.
        /// </summary>
        [Test]
        public void CanCreateOneAccurateToTheSecond()
        {
            Assert.That(() => new TimestampedMigrationAttribute(Year, Month, DayOfMonth, Hour, Minute, Second), Throws.Nothing);
        }

        /// <summary>
        /// Defines the test method CanCreateOneAccurateToTheSecondWithDescription.
        /// </summary>
        [Test]
        public void CanCreateOneAccurateToTheSecondWithDescription()
        {
            Assert.That(() => new TimestampedMigrationAttribute(Year, Month, DayOfMonth, Hour, Minute, Second, Description), Throws.Nothing);
        }

        /// <summary>
        /// Defines the test method CanCreateOneAccurateToTheSecondWithTransactionBehavior.
        /// </summary>
        [Test]
        public void CanCreateOneAccurateToTheSecondWithTransactionBehavior()
        {
            Assert.That(
                () => new TimestampedMigrationAttribute(Year, Month, DayOfMonth, Hour, Minute, Second, TransactionBehavior.Default),
                Throws.Nothing);
        }

        /// <summary>
        /// Defines the test method CanCreateOneAccurateToTheSecondWithTransactionBehaviorAndDescription.
        /// </summary>
        [Test]
        public void CanCreateOneAccurateToTheSecondWithTransactionBehaviorAndDescription()
        {
            Assert.That(
                () => new TimestampedMigrationAttribute(Year, Month, DayOfMonth, Hour, Minute, Second, TransactionBehavior.Default, Description),
                Throws.Nothing);
        }

        /// <summary>
        /// Defines the test method CreatingOneSetsUnderlyingValues.
        /// </summary>
        [Test]
        public void CreatingOneSetsUnderlyingValues()
        {
            var attribute = new TimestampedMigrationAttribute(Year, Month, DayOfMonth, Hour, Minute, Second, TransactionBehavior.None, Description);
            Assert.That(attribute.Description, Is.EqualTo(Description));
            Assert.That(attribute.TransactionBehavior, Is.EqualTo(TransactionBehavior.None));
            Assert.That(attribute.Version, Is.EqualTo(20000615123030));
        }

        /// <summary>
        /// Defines the test method ExtendsMigrationAttribute.
        /// </summary>
        [Test]
        public void ExtendsMigrationAttribute()
        {
            var timestampedMigrationAttribute = new TimestampedMigrationAttribute(Year, Month, DayOfMonth, Hour, Minute);
            Assert.That(timestampedMigrationAttribute, Is.InstanceOf<MigrationAttribute>());
        }

        /// <summary>
        /// Defines the test method TryingToCreateWithInvalidDayOfMonthResultsInArgumentOutOfRangeException.
        /// </summary>
        [Test]
        public void TryingToCreateWithInvalidDayOfMonthResultsInArgumentOutOfRangeException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new TimestampedMigrationAttribute(Year, Month, 99, Hour, Minute));
            Assert.That(exception.Message, Is.EqualTo(InvalidDateExceptionMessage));
        }

        /// <summary>
        /// Defines the test method TryingToCreateWithInvalidHourResultsInArgumentOutOfRangeException.
        /// </summary>
        [Test]
        public void TryingToCreateWithInvalidHourResultsInArgumentOutOfRangeException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new TimestampedMigrationAttribute(Year, Month, DayOfMonth, 99, Minute));
            Assert.That(exception.Message, Is.EqualTo(InvalidTimeExceptionMessage));
        }

        /// <summary>
        /// Defines the test method TryingToCreateWithInvalidMinuteResultsInArgumentOutOfRangeException.
        /// </summary>
        [Test]
        public void TryingToCreateWithInvalidMinuteResultsInArgumentOutOfRangeException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new TimestampedMigrationAttribute(Year, Month, DayOfMonth, Hour, 99));
            Assert.That(exception.Message, Is.EqualTo(InvalidTimeExceptionMessage));
        }

        /// <summary>
        /// Defines the test method TryingToCreateWithInvalidMonthResultsInArgumentOutOfRangeException.
        /// </summary>
        [Test]
        public void TryingToCreateWithInvalidMonthResultsInArgumentOutOfRangeException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new TimestampedMigrationAttribute(Year, 99, DayOfMonth, Hour, Minute));
            Assert.That(exception.Message, Is.EqualTo(InvalidDateExceptionMessage));
        }

        /// <summary>
        /// Defines the test method TryingToCreateWithInvalidSecondResultsInArgumentOutOfRangeException.
        /// </summary>
        [Test]
        public void TryingToCreateWithInvalidSecondResultsInArgumentOutOfRangeException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new TimestampedMigrationAttribute(Year, Month, DayOfMonth, Hour, Minute, 99));
            Assert.That(exception.Message, Is.EqualTo(InvalidTimeExceptionMessage));
        }

        /// <summary>
        /// Defines the test method TryingToCreateWithInvalidYearResultsInArgumentOutOfRangeException.
        /// </summary>
        [Test]
        public void TryingToCreateWithInvalidYearResultsInArgumentOutOfRangeException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new TimestampedMigrationAttribute(0000, Month, DayOfMonth, Hour, Minute));
            Assert.That(exception.Message, Is.EqualTo(InvalidDateExceptionMessage));
        }
    }
}
