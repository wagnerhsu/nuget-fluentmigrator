// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Oracle
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="OracleQuoterBase.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
// Copyright (c) 2018, FluentMigrator Project
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

namespace FluentMigrator.Runner.Generators.Oracle
{
    /// <summary>
    /// Class OracleQuoterBase.
    /// Implements the <see cref="FluentMigrator.Runner.Generators.Generic.GenericQuoter" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Generators.Generic.GenericQuoter" />
    public class OracleQuoterBase : GenericQuoter
    {
        /// <summary>
        /// Formats the date time.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public override string FormatDateTime(DateTime value)
        {
            var result = string.Format("to_date({0}{1}{0}, {0}yyyy-mm-dd hh24:mi:ss{0})", ValueQuote, value.ToString("yyyy-MM-dd HH:mm:ss")); //ISO 8601 DATETIME FORMAT (EXCEPT 'T' CHAR)
            return result;
        }

        /// <summary>
        /// Froms the time span.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public override string FromTimeSpan(TimeSpan value)
        {
            return string.Format("{0}{1} {2}:{3}:{4}.{5}{0}"
                , ValueQuote
                , value.Days
                , value.Hours
                , value.Minutes
                , value.Seconds
                , value.Milliseconds);
        }

        /// <summary>
        /// Formats the unique identifier.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public override string FormatGuid(Guid value)
        {
            return string.Format("{0}{1}{0}", ValueQuote, BitConverter.ToString(value.ToByteArray()).Replace("-", string.Empty));
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
                    return "sys_guid()";
                case SystemMethods.CurrentDateTime:
                    return "LOCALTIMESTAMP";
                case SystemMethods.CurrentDateTimeOffset:
                    return "CURRENT_TIMESTAMP";
                case SystemMethods.CurrentUTCDateTime:
                    return "sys_extract_utc(SYSTIMESTAMP)";
                case SystemMethods.CurrentUser:
                    return "USER";
            }

            return base.FormatSystemMethods(value);
        }
    }
}
