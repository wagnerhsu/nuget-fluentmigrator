// ***********************************************************************
// Assembly         : FluentMigrator.Runner.SqlServer
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SqlServer2000Column.cs" company="FluentMigrator Project">
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

using FluentMigrator.Infrastructure.Extensions;
using FluentMigrator.Model;
using FluentMigrator.Runner.Generators.Base;
using FluentMigrator.SqlServer;

namespace FluentMigrator.Runner.Generators.SqlServer
{
    /// <summary>
    /// Class SqlServer2000Column.
    /// Implements the <see cref="FluentMigrator.Runner.Generators.Base.ColumnBase" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Generators.Base.ColumnBase" />
    internal class SqlServer2000Column : ColumnBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServer2000Column"/> class.
        /// </summary>
        /// <param name="typeMap">The type map</param>
        /// <param name="quoter">The quoter</param>
        public SqlServer2000Column(ITypeMap typeMap, IQuoter quoter)
            : base(typeMap, quoter)
        {
        }

        /// <inheritdoc />
        protected override string FormatDefaultValue(ColumnDefinition column)
        {
            if (DefaultValueIsSqlFunction(column.DefaultValue))
                return "DEFAULT " + column.DefaultValue;

            var defaultValue = base.FormatDefaultValue(column);

            if (column.ModificationType == ColumnModificationType.Create && !string.IsNullOrEmpty(defaultValue))
                return "CONSTRAINT " + Quoter.QuoteConstraintName(GetDefaultConstraintName(column.TableName, column.Name)) + " " + defaultValue;

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

        /// <inheritdoc />
        protected override string FormatIdentity(ColumnDefinition column)
        {
            return column.IsIdentity ? GetIdentityString(column) : string.Empty;
        }

        /// <summary>
        /// Gets the identity string.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns>System.String.</returns>
        private static string GetIdentityString(ColumnDefinition column)
        {
            return string.Format("IDENTITY({0},{1})",
                column.GetAdditionalFeature(SqlServerExtensions.IdentitySeed, 1),
                column.GetAdditionalFeature(SqlServerExtensions.IdentityIncrement, 1));
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
            return string.Format("DF_{0}_{1}", tableName, columnName);
        }
    }
}
