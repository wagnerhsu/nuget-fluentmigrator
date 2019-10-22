// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="OracleQuoterTest.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using FluentMigrator.Runner.Generators;
using FluentMigrator.Runner.Generators.Oracle;
using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Generators.Oracle
{
    /// <summary>
    /// Defines test class OracleQuoterTest.
    /// </summary>
    [TestFixture]
    public class OracleQuoterTest
    {
        /// <summary>
        /// The quoter
        /// </summary>
        private IQuoter _quoter;

        /// <summary>
        /// Sets up.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            _quoter = new OracleQuoter();
        }

        /// <summary>
        /// Defines the test method TimeSpanIsFormattedQuotes.
        /// </summary>
        [Test]
        public void TimeSpanIsFormattedQuotes()
        {
            _quoter.QuoteValue(new TimeSpan(1, 2, 13, 65))
                   .ShouldBe("'1 2:14:5.0'");
        }

        /// <summary>
        /// Defines the test method GuidIsFormattedAsOracleAndQuoted.
        /// </summary>
        [Test]
        public void GuidIsFormattedAsOracleAndQuoted()
        {
            Guid givenValue = new Guid("CC28B6C7-9260-4800-9C1F-A5243960C087");

            _quoter.QuoteValue(givenValue)
                   .ShouldBe("'C7B628CC609200489C1FA5243960C087'");
        }
    }
}