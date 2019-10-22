// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Postgres
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="PostgresOptions.cs" company="FluentMigrator Project">
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
using System.Text.RegularExpressions;

namespace FluentMigrator.Runner.Processors.Postgres
{
    /// <summary>
    /// Class PostgresOptions.
    /// Implements the <see cref="System.ICloneable" />
    /// </summary>
    /// <seealso cref="System.ICloneable" />
    public class PostgresOptions : ICloneable
    {
        /// <summary>
        /// Gets or sets a value indicating whether all names should be quoted unconditionally.
        /// </summary>
        /// <value><c>true</c> if [force quote]; otherwise, <c>false</c>.</value>
        public bool ForceQuote { get; set; } = true;

        /// <summary>
        /// Parses the provider switches.
        /// </summary>
        /// <param name="providerSwitches">The provider switches.</param>
        /// <returns>PostgresOptions.</returns>
        public static PostgresOptions ParseProviderSwitches(string providerSwitches)
        {
            var retval = new PostgresOptions();

            var switchesParsed = Regex.Matches(providerSwitches ?? string.Empty, @"(?<key>[^=]+)=(?<value>[^\s]+)");
            foreach (Match match in switchesParsed)
            {
                if (!match.Success)
                {
                    continue;
                }

                var key = match.Groups["key"].Value;
                var value = match.Groups["value"].Value;

                if ("Force Quote".Equals(key, StringComparison.OrdinalIgnoreCase) && bool.TryParse(value, out var forceQuoteParsed))
                {
                    retval.ForceQuote = forceQuoteParsed;
                }
            }

            return retval;
        }

        /// <inheritdoc />
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
