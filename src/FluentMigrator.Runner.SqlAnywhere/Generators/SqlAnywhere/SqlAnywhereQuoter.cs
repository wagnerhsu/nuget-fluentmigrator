// ***********************************************************************
// Assembly         : FluentMigrator.Runner.SqlAnywhere
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SqlAnywhereQuoter.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
// Copyright (c) 2007-2018, FluentMigrator Project
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

using FluentMigrator.Runner.Generators.Generic;

namespace FluentMigrator.Runner.Generators.SqlAnywhere
{
    /// <summary>
    /// Class SqlAnywhereQuoter.
    /// Implements the <see cref="FluentMigrator.Runner.Generators.Generic.GenericQuoter" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Generators.Generic.GenericQuoter" />
    public class SqlAnywhereQuoter : GenericQuoter
    {
        /// <summary>
        /// Returns the opening quote identifier - " is the standard according to the specification
        /// </summary>
        /// <value>The open quote.</value>
        public override string OpenQuote => "[";

        /// <summary>
        /// Returns the closing quote identifier - " is the standard according to the specification
        /// </summary>
        /// <value>The close quote.</value>
        public override string CloseQuote => "]";

        /// <summary>
        /// Gets the close quote escape string.
        /// </summary>
        /// <value>The close quote escape string.</value>
        public override string CloseQuoteEscapeString => "]]";

        /// <summary>
        /// Gets the open quote escape string.
        /// </summary>
        /// <value>The open quote escape string.</value>
        public override string OpenQuoteEscapeString => string.Empty;

        /// <summary>
        /// Quotes the name of the schema.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <returns>System.String.</returns>
        /// <inheritdoc />
        public override string QuoteSchemaName(string schemaName)
        {
            return (string.IsNullOrEmpty(schemaName)) ? "[dbo]" : Quote(schemaName);
        }

        /// <summary>
        /// Formats the national string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public override string FormatNationalString(string value)
        {
            return $"N{FormatAnsiString(value)}";
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
                case SystemMethods.NewSequentialId:
                case SystemMethods.NewGuid:
                    return "NEWID()";
                case SystemMethods.CurrentDateTimeOffset:
                case SystemMethods.CurrentDateTime:
                    return "CURRENT TIMESTAMP";
                case SystemMethods.CurrentUTCDateTime:
                    return "CURRENT UTC TIMESTAMP";
                case SystemMethods.CurrentUser:
                    return "CURRENT USER";
            }

            return base.FormatSystemMethods(value);
        }
    }
}
