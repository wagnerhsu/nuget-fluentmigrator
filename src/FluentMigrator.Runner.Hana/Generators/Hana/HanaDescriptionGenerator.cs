// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Hana
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="HanaDescriptionGenerator.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using FluentMigrator.Runner.Generators.Generic;

namespace FluentMigrator.Runner.Generators.Hana
{
    /// <summary>
    /// Class HanaDescriptionGenerator.
    /// Implements the <see cref="FluentMigrator.Runner.Generators.Generic.GenericDescriptionGenerator" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Generators.Generic.GenericDescriptionGenerator" />
    public class HanaDescriptionGenerator : GenericDescriptionGenerator
    {
        #region Constants

        /// <summary>
        /// The table description template
        /// </summary>
        private const string TableDescriptionTemplate = "COMMENT ON TABLE {0} IS '{1}'";
        /// <summary>
        /// The column description template
        /// </summary>
        private const string ColumnDescriptionTemplate = "COMMENT ON COLUMN {0}.{1} IS '{2}'";

        #endregion

        /// <summary>
        /// Gets the full name of the table.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>System.String.</returns>
        private string GetFullTableName(string schemaName, string tableName)
        {
            return string.IsNullOrEmpty(schemaName)
               ? tableName
               : string.Format("{0}.{1}", schemaName, tableName);
        }

        /// <summary>
        /// Generates the table description.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="tableDescription">The table description.</param>
        /// <returns>System.String.</returns>
        protected override string GenerateTableDescription(
            string schemaName, string tableName, string tableDescription)
        {
            if (string.IsNullOrEmpty(tableDescription))
                return string.Empty;

            return string.Format(TableDescriptionTemplate, GetFullTableName(schemaName, tableName), tableDescription);
        }

        /// <summary>
        /// Generates the column description.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="columnDescription">The column description.</param>
        /// <returns>System.String.</returns>
        protected override string GenerateColumnDescription(
            string schemaName, string tableName, string columnName, string columnDescription)
        {
            if (string.IsNullOrEmpty(columnDescription))
                return string.Empty;

            return string.Format(
                ColumnDescriptionTemplate,
                GetFullTableName(schemaName, tableName),
                columnName,
                columnDescription);
        }
    }
}
