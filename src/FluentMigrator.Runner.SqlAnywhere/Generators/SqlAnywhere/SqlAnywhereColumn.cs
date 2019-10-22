// ***********************************************************************
// Assembly         : FluentMigrator.Runner.SqlAnywhere
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SqlAnywhereColumn.cs" company="FluentMigrator Project">
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

namespace FluentMigrator.Runner.Generators.SqlAnywhere
{
    /// <summary>
    /// Class SqlAnywhereColumn.
    /// Implements the <see cref="FluentMigrator.Runner.Generators.Base.ColumnBase" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Generators.Base.ColumnBase" />
    internal class SqlAnywhereColumn : ColumnBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlAnywhereColumn"/> class.
        /// </summary>
        /// <param name="typeMap">The type map.</param>
        public SqlAnywhereColumn(ITypeMap typeMap)
            : base(typeMap, new SqlAnywhereQuoter())
        {
            // Add UNIQUE before IDENTITY and after PRIMARY KEY
            ClauseOrder.Insert(ClauseOrder.Count - 2, FormatUniqueConstraint);
        }

        /// <inheritdoc />
        protected override string FormatNullable(ColumnDefinition column)
        {
            if (column.IsNullable.HasValue && column.IsNullable.Value)
            {
                return "NULL";
            }

            return "NOT NULL";
        }

        /// <inheritdoc />
        protected override string FormatDefaultValue(ColumnDefinition column)
        {
            if (DefaultValueIsSqlFunction(column.DefaultValue))
                return "DEFAULT " + column.DefaultValue;

            var defaultValue = base.FormatDefaultValue(column);

            if (!string.IsNullOrEmpty(defaultValue))
                return defaultValue;

            return string.Empty;
        }

        /// <summary>
        /// Defaults the value is SQL function.
        /// </summary>
        /// <param name="defaultValue">The default value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private static bool DefaultValueIsSqlFunction(object defaultValue)
        {
            return defaultValue is string && defaultValue.ToString().EndsWith("()");
        }

        /// <summary>
        /// Formats the unique constraint.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns>System.String.</returns>
        protected virtual string FormatUniqueConstraint(ColumnDefinition column)
        {
            // Define unique constraints on columns in addition to creating a unique index
            return column.IsUnique ? "UNIQUE" : string.Empty;
        }

        /// <inheritdoc />
        protected override string FormatIdentity(ColumnDefinition column)
        {
            return column.IsIdentity ? GetIdentityString() : string.Empty;
        }

        /// <summary>
        /// Gets the identity string.
        /// </summary>
        /// <returns>System.String.</returns>
        private static string GetIdentityString()
        {
            return "DEFAULT AUTOINCREMENT";
        }

        /// <summary>
        /// Formats the default value.
        /// </summary>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="quoter">The quoter.</param>
        /// <returns>System.String.</returns>
        public static string FormatDefaultValue(object defaultValue, IQuoter quoter)
        {
            if (DefaultValueIsSqlFunction(defaultValue))
                return defaultValue.ToString();

            return quoter.QuoteValue(defaultValue);
        }

        /// <summary>
        /// Gets the default name of the constraint.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>System.String.</returns>
        public static string GetDefaultConstraintName(string tableName, string columnName)
        {
            return $"DF_{tableName}_{columnName}";
        }
    }
}
