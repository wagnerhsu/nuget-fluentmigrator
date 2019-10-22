// ***********************************************************************
// Assembly         : FluentMigrator.Runner.SqlServer
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SqlServer2005DescriptionGenerator.cs" company="FluentMigrator Project">
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

using FluentMigrator.Expressions;
using FluentMigrator.Runner.Generators.Generic;

namespace FluentMigrator.Runner.Generators.SqlServer
{
    /// <summary>
    /// Class SqlServer2005DescriptionGenerator.
    /// Implements the <see cref="FluentMigrator.Runner.Generators.Generic.GenericDescriptionGenerator" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Generators.Generic.GenericDescriptionGenerator" />
    public class SqlServer2005DescriptionGenerator : GenericDescriptionGenerator
    {
        #region Constants

        /// <summary>
        /// The table description template
        /// </summary>
        private const string TableDescriptionTemplate =
            "EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'{0}', @level0type=N'SCHEMA', @level0name='{1}', @level1type=N'TABLE', @level1name='{2}'";
        /// <summary>
        /// The column description template
        /// </summary>
        private const string ColumnDescriptionTemplate =
            "EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'{0}', @level0type = N'SCHEMA', @level0name = '{1}', @level1type = N'Table', @level1name = '{2}', @level2type = N'Column',  @level2name = '{3}'";
        /// <summary>
        /// The remove table description template
        /// </summary>
        private const string RemoveTableDescriptionTemplate = "EXEC sys.sp_dropextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name='{0}', @level1type=N'TABLE', @level1name='{1}'";
        /// <summary>
        /// The remove column description template
        /// </summary>
        private const string RemoveColumnDescriptionTemplate = "EXEC sys.sp_dropextendedproperty @name=N'MS_Description', @level0type = N'SCHEMA', @level0name = '{0}', @level1type = N'Table', @level1name = '{1}', @level2type = N'Column',  @level2name = '{2}'";
        /// <summary>
        /// The table description verification template
        /// </summary>
        private const string TableDescriptionVerificationTemplate = "IF EXISTS ( SELECT * FROM fn_listextendedproperty(N'MS_Description', N'SCHEMA', N'{0}', N'TABLE', N'{1}', NULL, NULL))";
        /// <summary>
        /// The column description verification template
        /// </summary>
        private const string ColumnDescriptionVerificationTemplate = "IF EXISTS (SELECT * FROM fn_listextendedproperty(N'MS_Description', N'SCHEMA', N'{0}', N'TABLE', N'{1}', N'Column', N'{2}' ))";

        #endregion

        /// <summary>
        /// Generates the description statement.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>System.String.</returns>
        public override string GenerateDescriptionStatement(AlterTableExpression expression)
        {
            if (string.IsNullOrEmpty(expression.TableDescription))
                return string.Empty;

            var formattedSchemaName = FormatSchemaName(expression.SchemaName);

            // For this, we need to remove the extended property first if exists (or implement verification and use sp_updateextendedproperty)
            var tableVerificationStatement = string.Format(TableDescriptionVerificationTemplate, formattedSchemaName, expression.TableName);
            var removalStatement = string.Format("{0} {1}", tableVerificationStatement, GenerateTableDescriptionRemoval(formattedSchemaName, expression.TableName));
            var newDescriptionStatement = GenerateTableDescription(formattedSchemaName, expression.TableName, expression.TableDescription);

            return string.Join(";", new[] { removalStatement, newDescriptionStatement });
        }

        /// <summary>
        /// Generates the description statement.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>System.String.</returns>
        public override string GenerateDescriptionStatement(AlterColumnExpression expression)
        {
            if (string.IsNullOrEmpty(expression.Column.ColumnDescription))
                return string.Empty;

            var formattedSchemaName = FormatSchemaName(expression.SchemaName);

            // For this, we need to remove the extended property first if exists (or implement verification and use sp_updateextendedproperty)
            var columnVerificationStatement = string.Format(ColumnDescriptionVerificationTemplate, formattedSchemaName, expression.TableName, expression.Column.Name);
            var removalStatement = string.Format("{0} {1}", columnVerificationStatement, GenerateColumnDescriptionRemoval(formattedSchemaName, expression.TableName, expression.Column.Name));
            var newDescriptionStatement = GenerateColumnDescription(formattedSchemaName, expression.TableName, expression.Column.Name, expression.Column.ColumnDescription);

            return string.Join(";", new[] { removalStatement, newDescriptionStatement });
        }

        /// <summary>
        /// Generates the table description.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="tableDescription">The table description.</param>
        /// <returns>System.String.</returns>
        protected override string GenerateTableDescription(string schemaName, string tableName, string tableDescription)
        {
            if (string.IsNullOrEmpty(tableDescription))
                return string.Empty;

            var formattedSchemaName = FormatSchemaName(schemaName);

            return string.Format(TableDescriptionTemplate,
                tableDescription.Replace("'", "''"),
                formattedSchemaName,
                tableName);
        }

        /// <summary>
        /// Generates the column description.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="columnDescription">The column description.</param>
        /// <returns>System.String.</returns>
        protected override string GenerateColumnDescription(string schemaName, string tableName, string columnName, string columnDescription)
        {
            if (string.IsNullOrEmpty(columnDescription))
                return string.Empty;

            var formattedSchemaName = FormatSchemaName(schemaName);

            return string.Format(ColumnDescriptionTemplate, columnDescription.Replace("'", "''"), formattedSchemaName, tableName, columnName);
        }

        /// <summary>
        /// Generates the table description removal.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>System.String.</returns>
        private string GenerateTableDescriptionRemoval(string schemaName, string tableName)
        {
            return string.Format(RemoveTableDescriptionTemplate, schemaName, tableName);
        }

        /// <summary>
        /// Generates the column description removal.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>System.String.</returns>
        private string GenerateColumnDescriptionRemoval(string schemaName, string tableName, string columnName)
        {
            return string.Format(RemoveColumnDescriptionTemplate, schemaName, tableName, columnName);
        }

        /// <summary>
        /// Formats the name of the schema.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <returns>System.String.</returns>
        private string FormatSchemaName(string schemaName)
        {
            return (string.IsNullOrEmpty(schemaName)) ? "dbo" : schemaName;
        }
    }
}
