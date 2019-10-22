// ***********************************************************************
// Assembly         : FluentMigrator.Runner.MySql
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="MySqlQuoter.cs" company="FluentMigrator Project">
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

namespace FluentMigrator.Runner.Generators.MySql
{
    /// <summary>
    /// Class MySqlQuoter.
    /// Implements the <see cref="FluentMigrator.Runner.Generators.Generic.GenericQuoter" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Generators.Generic.GenericQuoter" />
    public class MySqlQuoter : GenericQuoter
    {
        /// <summary>
        /// Returns the opening quote identifier - " is the standard according to the specification
        /// </summary>
        /// <value>The open quote.</value>
        public override string OpenQuote { get { return "`"; } }

        /// <summary>
        /// Returns the closing quote identifier - " is the standard according to the specification
        /// </summary>
        /// <value>The close quote.</value>
        public override string CloseQuote { get { return "`"; } }

        /// <summary>
        /// Quotes the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        /// <inheritdoc />
        public override string QuoteValue(object value)
        {
            return base.QuoteValue(value).Replace(@"\", @"\\");
        }

        /// <summary>
        /// Froms the time span.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public override string FromTimeSpan(System.TimeSpan value)
        {
            return string.Format("{0}{1:00}:{2:00}:{3:00}{0}"
                , ValueQuote
                , value.Hours + (value.Days * 24)
                , value.Minutes
                , value.Seconds);
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
                case SystemMethods.NewGuid:
                case SystemMethods.NewSequentialId:
                    return "(SELECT UUID())";
                case SystemMethods.CurrentDateTime:
                    return "CURRENT_TIMESTAMP";
                case SystemMethods.CurrentUTCDateTime:
                    return "UTC_TIMESTAMP";
                case SystemMethods.CurrentUser:
                    return "CURRENT_USER()";
            }

            return base.FormatSystemMethods(value);
        }

        /// <summary>
        /// Quotes the name of the schema.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <returns>System.String.</returns>
        public override string QuoteSchemaName(string schemaName)
        {
            // This database doesn't support schemata
            return string.Empty;
        }
    }
}
