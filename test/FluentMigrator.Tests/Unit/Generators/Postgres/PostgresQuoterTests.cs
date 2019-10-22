// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="PostgresQuoterTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using FluentMigrator.Runner.Generators;
using FluentMigrator.Runner.Generators.Postgres;
using FluentMigrator.Runner.Processors.Postgres;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Generators.Postgres
{
    /// <summary>
    /// Defines test class PostgresQuotesTests.
    /// </summary>
    [TestFixture]
    public class PostgresQuotesTests
    {
        /// <summary>
        /// Sets up.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            _quoter = new PostgresQuoter(new PostgresOptions());
        }

        /// <summary>
        /// The quoter
        /// </summary>
        private IQuoter _quoter = default(PostgresQuoter);

        /// <summary>
        /// Defines the test method ByteArrayIsFormattedWithQuotes.
        /// </summary>
        [Test]
        public void ByteArrayIsFormattedWithQuotes()
        {
            _quoter.QuoteValue(new byte[] { 0, 254, 13, 18, 125, 17 })
                .ShouldBe(@"E'\\x00FE0D127D11'");
        }

        /// <summary>
        /// Defines the test method DisableForceQuoteRemovesQuotes.
        /// </summary>
        [Test]
        public void DisableForceQuoteRemovesQuotes()
        {
            _quoter = new PostgresQuoter(new PostgresOptions() { ForceQuote = false });
            _quoter.Quote("TableName").ShouldBe("TableName");
        }

        /// <summary>
        /// Defines the test method DisableForceQuoteQuotesReservedKeyword.
        /// </summary>
        [Test]
        public void DisableForceQuoteQuotesReservedKeyword()
        {
            _quoter = new PostgresQuoter(new PostgresOptions() { ForceQuote = false });

            _quoter.Quote("between").ShouldBe(@"""between""");
            _quoter.Quote("BETWEEN").ShouldBe(@"""BETWEEN""");
        }
    }
}
