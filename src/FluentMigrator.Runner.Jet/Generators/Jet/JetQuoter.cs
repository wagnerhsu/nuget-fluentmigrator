// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Jet
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="JetQuoter.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using FluentMigrator.Runner.Generators.Generic;

namespace FluentMigrator.Runner.Generators.Jet
{
    /// <summary>
    /// Class JetQuoter.
    /// Implements the <see cref="FluentMigrator.Runner.Generators.Generic.GenericQuoter" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Generators.Generic.GenericQuoter" />
    public class JetQuoter : GenericQuoter
    {
        /// <summary>
        /// Returns the opening quote identifier - " is the standard according to the specification
        /// </summary>
        /// <value>The open quote.</value>
        public override string OpenQuote { get { return "["; } }

        /// <summary>
        /// Returns the closing quote identifier - " is the standard according to the specification
        /// </summary>
        /// <value>The close quote.</value>
        public override string CloseQuote { get { return "]"; } }

        /// <summary>
        /// Gets the close quote escape string.
        /// </summary>
        /// <value>The close quote escape string.</value>
        public override string CloseQuoteEscapeString { get { return string.Empty; } }

        /// <summary>
        /// Gets the open quote escape string.
        /// </summary>
        /// <value>The open quote escape string.</value>
        public override string OpenQuoteEscapeString { get { return string.Empty; } }

        /// <summary>
        /// Formats the date time.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public override string FormatDateTime(DateTime value)
        {
            return ValueQuote + (value).ToString("yyyy-MM-dd HH:mm:ss") + ValueQuote;
        }

        /// <summary>
        /// Quotes the name of the schema.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <returns>System.String.</returns>
        public override string QuoteSchemaName(string schemaName)
        {
            return string.Empty;
        }
    }
}
