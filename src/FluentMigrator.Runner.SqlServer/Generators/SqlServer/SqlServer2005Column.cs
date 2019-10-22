// ***********************************************************************
// Assembly         : FluentMigrator.Runner.SqlServer
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SqlServer2005Column.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
// Copyright (c) 2007-2018, Sean Chambers and the FluentMigrator Project
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
using FluentMigrator.SqlServer;

namespace FluentMigrator.Runner.Generators.SqlServer
{
    /// <summary>
    /// Class SqlServer2005Column.
    /// Implements the <see cref="FluentMigrator.Runner.Generators.SqlServer.SqlServer2000Column" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Generators.SqlServer.SqlServer2000Column" />
    internal class SqlServer2005Column : SqlServer2000Column
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServer2005Column"/> class.
        /// </summary>
        /// <param name="typeMap">The type map</param>
        /// <param name="quoter">The quoter</param>
        public SqlServer2005Column(ITypeMap typeMap, IQuoter quoter)
            : base(typeMap, quoter)
        {
            ClauseOrder.Add(FormatRowGuid);
        }

        /// <inheritdoc />
        protected override string FormatNullable(ColumnDefinition column)
        {
            if (column.IsNullable == true && column.Type == null && !string.IsNullOrEmpty(column.CustomType))
            {
                return "NULL";
            }

            return base.FormatNullable(column);
        }

        /// <summary>
        /// Add <c>ROWGUIDCOL</c> when <see cref="SqlServerExtensions.RowGuidColumn" /> is set.
        /// </summary>
        /// <param name="column">The column to create the definition part for</param>
        /// <returns>The generated SQL string part</returns>
        protected virtual string FormatRowGuid(ColumnDefinition column)
        {
            if (column.AdditionalFeatures.ContainsKey(SqlServerExtensions.RowGuidColumn))
            {
                return "ROWGUIDCOL";
            }

            return string.Empty;
        }
    }
}
