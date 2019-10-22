// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Redshift
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="RedshiftQuoter.cs" company="FluentMigrator Project">
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

using System.Linq;

using FluentMigrator.Runner.Generators.Generic;

namespace FluentMigrator.Runner.Generators.Redshift
{
    /// <summary>
    /// Class RedshiftQuoter.
    /// Implements the <see cref="FluentMigrator.Runner.Generators.Generic.GenericQuoter" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Generators.Generic.GenericQuoter" />
    public class RedshiftQuoter : GenericQuoter
    {
        /// <summary>
        /// Formats the bool.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <returns>System.String.</returns>
        public override string FormatBool(bool value) { return value ? "true" : "false"; }

        /// <summary>
        /// Quotes the name of the schema.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <returns>System.String.</returns>
        /// <inheritdoc />
        public override string QuoteSchemaName(string schemaName)
        {
            if (string.IsNullOrEmpty(schemaName))
                schemaName = "public";
            return base.QuoteSchemaName(schemaName);
        }

        /// <summary>
        /// Formats the byte array.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <returns>System.String.</returns>
        protected override string FormatByteArray(byte[] array)
        {
            var arrayAsHex = array.Select(b => b.ToString("X2")).ToArray();
            return @"E'\\x" + string.Concat(arrayAsHex) + "'";
        }

        /// <summary>
        /// Uns the name of the quote schema.
        /// </summary>
        /// <param name="quoted">The quoted.</param>
        /// <returns>System.String.</returns>
        public string UnQuoteSchemaName(string quoted)
        {
            if (string.IsNullOrEmpty(quoted))
                return "public";

            return UnQuote(quoted);
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
                case SystemMethods.CurrentDateTimeOffset:
                case SystemMethods.CurrentDateTime:
                    return "SYSDATE";
                case SystemMethods.CurrentUTCDateTime:
                    return "(SYSDATE at time zone 'UTC')";
                case SystemMethods.CurrentUser:
                    return "current_user";
            }

            return base.FormatSystemMethods(value);
        }
    }
}
