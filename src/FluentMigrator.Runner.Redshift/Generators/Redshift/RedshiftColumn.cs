// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Redshift
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="RedshiftColumn.cs" company="FluentMigrator Project">
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
using System.Collections.Generic;
using System.Linq;

using FluentMigrator.Model;
using FluentMigrator.Runner.Generators.Base;

namespace FluentMigrator.Runner.Generators.Redshift
{
    /// <summary>
    /// Class RedshiftColumn.
    /// Implements the <see cref="FluentMigrator.Runner.Generators.Base.ColumnBase" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Generators.Base.ColumnBase" />
    internal class RedshiftColumn : ColumnBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RedshiftColumn"/> class.
        /// </summary>
        public RedshiftColumn() : base(new RedshiftTypeMap(), new RedshiftQuoter())
        {
            AlterClauseOrder = new List<Func<ColumnDefinition, string>> { FormatAlterType, FormatAlterNullable };
        }

        /// <summary>
        /// Formats the alter default value.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>System.String.</returns>
        public string FormatAlterDefaultValue(string column, object defaultValue)
        {
            string formatDefaultValue = FormatDefaultValue(new ColumnDefinition { Name = column, DefaultValue = defaultValue});

            return string.Format("SET {0}", formatDefaultValue);
        }

        /// <summary>
        /// Formats the alter nullable.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns>System.String.</returns>
        private string FormatAlterNullable(ColumnDefinition column)
        {
            if (!column.IsNullable.HasValue)
                return "";

            if (column.IsNullable.Value)
                return "DROP NOT NULL";

            return "SET NOT NULL";
        }

        /// <summary>
        /// Formats the type of the alter.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns>System.String.</returns>
        private string FormatAlterType(ColumnDefinition column)
        {
            return string.Format("TYPE {0}", GetColumnType(column));
        }

        /// <summary>
        /// Gets or sets the alter clause order.
        /// </summary>
        /// <value>The alter clause order.</value>
        protected IList<Func<ColumnDefinition, string>> AlterClauseOrder { get; set; }

        /// <summary>
        /// Generates the alter clauses.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns>System.String.</returns>
        public string GenerateAlterClauses(ColumnDefinition column)
        {
            var clauses = new List<string>();
            foreach (var action in AlterClauseOrder)
            {
                string columnClause = action(column);
                if (!string.IsNullOrEmpty(columnClause))
                    clauses.Add(string.Format("ALTER {0} {1}", Quoter.QuoteColumnName(column.Name), columnClause));
            }

            return string.Join(", ", clauses.ToArray());
        }

        /// <inheritdoc />
        protected override string FormatIdentity(ColumnDefinition column)
        {
            return string.Empty;
        }

        /// <inheritdoc />
        public override string AddPrimaryKeyConstraint(string tableName, IEnumerable<ColumnDefinition> primaryKeyColumns)
        {
            var pkColDef = primaryKeyColumns.ToList();
            string pkName = GetPrimaryKeyConstraintName(pkColDef, tableName);

            var cols = string.Join(",", pkColDef.Select(c => Quoter.QuoteColumnName(c.Name)));

            if (string.IsNullOrEmpty(pkName))
                return string.Format(", PRIMARY KEY ({0})", cols);

            return string.Format(", {0}PRIMARY KEY ({1})", pkName, cols);
        }

        /// <summary>
        /// Gets the type of the column.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns>System.String.</returns>
        public string GetColumnType(ColumnDefinition column)
        {
            return FormatType(column);
        }
    }
}
