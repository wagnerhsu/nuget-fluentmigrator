// ***********************************************************************
// Assembly         : FluentMigrator.Runner.MySql
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="MySqlColumn.cs" company="FluentMigrator Project">
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

using FluentMigrator.Model;
using FluentMigrator.Runner.Generators.Base;

namespace FluentMigrator.Runner.Generators.MySql
{
    /// <summary>
    /// Class MySqlColumn.
    /// Implements the <see cref="FluentMigrator.Runner.Generators.Base.ColumnBase" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Generators.Base.ColumnBase" />
    internal class MySqlColumn : ColumnBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlColumn"/> class.
        /// </summary>
        /// <param name="typeMap">The type map</param>
        /// <param name="quoter">The quoter</param>
        public MySqlColumn(ITypeMap typeMap, IQuoter quoter)
            : base(typeMap, quoter)
        {
            ClauseOrder.Add(FormatDescription);
        }

        /// <summary>
        /// Formats the default value.
        /// </summary>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>System.String.</returns>
        internal string FormatDefaultValue(object defaultValue)
        {
            string formatDefaultValue = base.FormatDefaultValue(new ColumnDefinition { DefaultValue = defaultValue });
            return formatDefaultValue;
        }

        /// <summary>
        /// Formats the description.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns>System.String.</returns>
        protected string FormatDescription(ColumnDefinition column)
        {
            return string.IsNullOrEmpty(column.ColumnDescription)
                ? string.Empty
                : string.Format("COMMENT {0}", Quoter.QuoteValue(column.ColumnDescription));
        }

        /// <inheritdoc />
        protected override string FormatIdentity(ColumnDefinition column)
        {
            return column.IsIdentity ? "AUTO_INCREMENT" : string.Empty;
        }
    }
}
