// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Hana
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="HanaColumn.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using FluentMigrator.Exceptions;
using FluentMigrator.Model;
using FluentMigrator.Runner.Generators.Base;

namespace FluentMigrator.Runner.Generators.Hana
{
    /// <summary>
    /// Class HanaColumn.
    /// Implements the <see cref="FluentMigrator.Runner.Generators.Base.ColumnBase" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Generators.Base.ColumnBase" />
    internal class HanaColumn : ColumnBase
    {
        /// <summary>
        /// The hana object name maximum length
        /// </summary>
        private const int HanaObjectNameMaxLength = 30;

        /// <summary>
        /// Initializes a new instance of the <see cref="HanaColumn"/> class.
        /// </summary>
        /// <param name="quoter">The quoter.</param>
        public HanaColumn(IQuoter quoter)
            : base(new HanaTypeMap(), quoter)
        {

            var a = ClauseOrder.IndexOf(FormatDefaultValue);
            var b = ClauseOrder.IndexOf(FormatNullable);

            // Hana requires DefaultValue before nullable
            if (a <= b) return;

            ClauseOrder[b] = FormatDefaultValue;
            ClauseOrder[a] = FormatNullable;
        }

        /// <summary>
        /// Formats the identity SQL fragment
        /// </summary>
        /// <param name="column">The column definition</param>
        /// <returns>The formatted identity SQL fragment</returns>
        protected override string FormatIdentity(ColumnDefinition column)
        {
            return column.IsIdentity ? "GENERATED ALWAYS AS IDENTITY" : string.Empty;
        }


        /// <summary>
        /// Formats the (not) null constraint
        /// </summary>
        /// <param name="column">The column definition</param>
        /// <returns>The formatted (not) null constraint</returns>
        protected override string FormatNullable(ColumnDefinition column)
        {
            if (!(column.DefaultValue is ColumnDefinition.UndefinedDefaultValue))
                return string.Empty;

            return column.IsNullable.HasValue
                ? (column.IsNullable.Value ? "NULL" : "NOT NULL")
                : string.Empty;
        }

        /// <summary>
        /// Gets the name of the primary key constraint. Some Generators may need to override if the constraint name is limited
        /// </summary>
        /// <param name="primaryKeyColumns">The primary key columns</param>
        /// <param name="tableName">The table name</param>
        /// <returns>The constraint clause</returns>
        /// <exception cref="ArgumentNullException">primaryKeyColumns</exception>
        /// <exception cref="ArgumentNullException">tableName</exception>
        /// <exception cref="DatabaseOperationNotSupportedException"></exception>
        protected override string GetPrimaryKeyConstraintName(IEnumerable<ColumnDefinition> primaryKeyColumns, string tableName)
        {
            if (primaryKeyColumns == null)
                throw new ArgumentNullException("primaryKeyColumns");
            if (tableName == null)
                throw new ArgumentNullException("tableName");

            var primaryKeyName = primaryKeyColumns.First().PrimaryKeyName;

            if (string.IsNullOrEmpty(primaryKeyName))
            {
                return string.Empty;
            }

            if (primaryKeyName.Length > HanaObjectNameMaxLength)
                throw new DatabaseOperationNotSupportedException(
                    string.Format(
                        "Hana does not support length of primary key name greater than {0} characters. Reduce length of primary key name. ({1})",
                        HanaObjectNameMaxLength, primaryKeyName));

            var result = string.Format("CONSTRAINT {0} ", Quoter.QuoteConstraintName(primaryKeyName));

            return result;
        }

        /// <summary>
        /// Creates the primary key constraint SQL fragment
        /// </summary>
        /// <param name="tableName">The table name</param>
        /// <param name="primaryKeyColumns">The primary key column defintions</param>
        /// <returns>The SQL fragment</returns>
        public override string AddPrimaryKeyConstraint(string tableName, IEnumerable<ColumnDefinition> primaryKeyColumns)
        {
            var keyColumns = string.Join(", ", primaryKeyColumns.Select(x => Quoter.QuoteColumnName(x.Name)).ToArray());

            return string.Format(", PRIMARY KEY ({0})", keyColumns);
        }
    }
}
