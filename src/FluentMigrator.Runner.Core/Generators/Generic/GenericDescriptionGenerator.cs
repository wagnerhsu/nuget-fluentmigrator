// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Core
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="GenericDescriptionGenerator.cs" company="FluentMigrator Project">
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

using System.Collections.Generic;

namespace FluentMigrator.Runner.Generators.Generic
{
    /// <summary>
    /// Base class to generate descriptions for tables/classes
    /// </summary>
    public abstract class GenericDescriptionGenerator : IDescriptionGenerator
    {
        /// <summary>
        /// Generates the table description.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="tableDescription">The table description.</param>
        /// <returns>System.String.</returns>
        protected abstract string GenerateTableDescription(
            string schemaName, string tableName, string tableDescription);
        /// <summary>
        /// Generates the column description.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="columnDescription">The column description.</param>
        /// <returns>System.String.</returns>
        protected abstract string GenerateColumnDescription(
            string schemaName, string tableName, string columnName, string columnDescription);

        /// <summary>
        /// Generates the description statements.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public virtual IEnumerable<string> GenerateDescriptionStatements(Expressions.CreateTableExpression expression)
        {
            var statements = new List<string>();

            if (!string.IsNullOrEmpty(expression.TableDescription))
                statements.Add(GenerateTableDescription(expression.SchemaName, expression.TableName, expression.TableDescription));

            foreach (var column in expression.Columns)
            {
                if (string.IsNullOrEmpty(column.ColumnDescription))
                    continue;

                statements.Add(GenerateColumnDescription(
                    expression.SchemaName,
                    expression.TableName,
                    column.Name,
                    column.ColumnDescription));
            }

            return statements;
        }

        /// <summary>
        /// Generates the description statement.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>System.String.</returns>
        public virtual string GenerateDescriptionStatement(Expressions.AlterTableExpression expression)
        {
            if (string.IsNullOrEmpty(expression.TableDescription))
                return string.Empty;

            return GenerateTableDescription(
                expression.SchemaName, expression.TableName, expression.TableDescription);
        }

        /// <summary>
        /// Generates the description statement.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>System.String.</returns>
        public virtual string GenerateDescriptionStatement(Expressions.CreateColumnExpression expression)
        {
            if (string.IsNullOrEmpty(expression.Column.ColumnDescription))
                return string.Empty;

            return GenerateColumnDescription(
                expression.SchemaName, expression.TableName, expression.Column.Name, expression.Column.ColumnDescription);
        }

        /// <summary>
        /// Generates the description statement.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>System.String.</returns>
        public virtual string GenerateDescriptionStatement(Expressions.AlterColumnExpression expression)
        {
            if (string.IsNullOrEmpty(expression.Column.ColumnDescription))
                return string.Empty;

            return GenerateColumnDescription(expression.SchemaName, expression.TableName, expression.Column.Name, expression.Column.ColumnDescription);
        }
    }
}
