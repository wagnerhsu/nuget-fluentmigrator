// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Db2
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="Db2Quoter.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
// Copyright (c) 2018, Fluent Migrator Project
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;

using FluentMigrator.Runner.Generators.Generic;

namespace FluentMigrator.Runner.Generators.DB2
{
    /// <summary>
    /// Class Db2Quoter.
    /// Implements the <see cref="FluentMigrator.Runner.Generators.Generic.GenericQuoter" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Generators.Generic.GenericQuoter" />
    public class Db2Quoter : GenericQuoter
    {
        /// <summary>
        /// The special chars
        /// </summary>
        public readonly char[] SpecialChars = "\"%'()*+|,{}-./:;<=>?^[]".ToCharArray();

        /// <summary>
        /// Formats the date time.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public override string FormatDateTime(DateTime value)
        {
            return ValueQuote + value.ToString("yyyy-MM-dd-HH.mm.ss") + ValueQuote;
        }

        /// <summary>
        /// Shoulds the quote.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected override bool ShouldQuote(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;
            // Quotes are only included if the name contains a special character, in order to preserve case insensitivity where possible.
            return name.IndexOfAny(SpecialChars) != -1;
        }

        /// <summary>
        /// Quotes the name of the index.
        /// </summary>
        /// <param name="indexName">Name of the index.</param>
        /// <param name="schemaName">Name of the schema.</param>
        /// <returns>System.String.</returns>
        /// <inheritdoc />
        public override string QuoteIndexName(string indexName, string schemaName)
        {
            return CreateSchemaPrefixedQuotedIdentifier(
                QuoteSchemaName(schemaName),
                IsQuoted(indexName) ? indexName : Quote(indexName));
        }

        /// <summary>
        /// Formats the system methods.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public override string FormatSystemMethods(SystemMethods value)
        {
            switch (value)
            {
                case SystemMethods.CurrentUTCDateTime:
                    return "(CURRENT_TIMESTAMP - CURRENT_TIMEZONE)";
                case SystemMethods.CurrentDateTime:
                    return "CURRENT_TIMESTAMP";
                case SystemMethods.CurrentUser:
                    return "USER";
            }

            return base.FormatSystemMethods(value);
        }
    }
}
