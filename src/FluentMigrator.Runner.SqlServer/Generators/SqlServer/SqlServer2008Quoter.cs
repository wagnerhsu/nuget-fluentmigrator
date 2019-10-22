// ***********************************************************************
// Assembly         : FluentMigrator.Runner.SqlServer
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SqlServer2008Quoter.cs" company="FluentMigrator Project">
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

namespace FluentMigrator.Runner.Generators.SqlServer
{
    /// <summary>
    /// Class SqlServer2008Quoter.
    /// Implements the <see cref="FluentMigrator.Runner.Generators.SqlServer.SqlServer2005Quoter" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Generators.SqlServer.SqlServer2005Quoter" />
    public class SqlServer2008Quoter : SqlServer2005Quoter
    {
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
                    return "SYSDATETIMEOFFSET()";
            }

            return base.FormatSystemMethods(value);
        }
    }
}
