// ***********************************************************************
// Assembly         : FluentMigrator.Runner.SqlServer
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SqlServer2008Column.cs" company="FluentMigrator Project">
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
    /// Class SqlServer2008Column.
    /// Implements the <see cref="FluentMigrator.Runner.Generators.SqlServer.SqlServer2005Column" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Generators.SqlServer.SqlServer2005Column" />
    internal class SqlServer2008Column : SqlServer2005Column
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServer2008Column"/> class.
        /// </summary>
        /// <param name="typeMap">The type map</param>
        /// <param name="quoter">The quoter</param>
        public SqlServer2008Column(ITypeMap typeMap, IQuoter quoter)
           : base(typeMap, quoter)
        {
            ClauseOrder.Add(FormatSparse);
        }

        /// <summary>
        /// Add <c>SPARSE</c> when <see cref="SqlServerExtensions.SparseColumn" /> is set.
        /// </summary>
        /// <param name="column">The column to create the definition part for</param>
        /// <returns>The generated SQL string part</returns>
        protected virtual string FormatSparse(ColumnDefinition column)
        {
            if (column.AdditionalFeatures.ContainsKey(SqlServerExtensions.SparseColumn)
                && (column.IsNullable ?? false))
            {
                return "SPARSE";
            }

            return string.Empty;
        }

    }
}
