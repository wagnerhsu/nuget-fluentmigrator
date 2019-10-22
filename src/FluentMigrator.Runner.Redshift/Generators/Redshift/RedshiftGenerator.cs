// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Redshift
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="RedshiftGenerator.cs" company="FluentMigrator Project">
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
using System.Text;

using FluentMigrator.Expressions;
using FluentMigrator.Runner.Generators.Generic;

using JetBrains.Annotations;

using Microsoft.Extensions.Options;

namespace FluentMigrator.Runner.Generators.Redshift
{
    /// <summary>
    /// Class RedshiftGenerator.
    /// Implements the <see cref="FluentMigrator.Runner.Generators.Generic.GenericGenerator" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Generators.Generic.GenericGenerator" />
    public class RedshiftGenerator : GenericGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RedshiftGenerator"/> class.
        /// </summary>
        public RedshiftGenerator()
            : this(new RedshiftQuoter())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedshiftGenerator"/> class.
        /// </summary>
        /// <param name="quoter">The quoter.</param>
        public RedshiftGenerator(
            [NotNull] RedshiftQuoter quoter)
            : this(quoter, new OptionsWrapper<GeneratorOptions>(new GeneratorOptions()))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedshiftGenerator"/> class.
        /// </summary>
        /// <param name="quoter">The quoter.</param>
        /// <param name="generatorOptions">The generator options.</param>
        public RedshiftGenerator(
            [NotNull] RedshiftQuoter quoter,
            [NotNull] IOptions<GeneratorOptions> generatorOptions)
            : base(new RedshiftColumn(), quoter, new RedshiftDescriptionGenerator(), generatorOptions)
        {
        }

        /// <summary>
        /// Generates a <c>ALTER TABLE</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(AlterTableExpression expression)
        {
            var alterStatement = new StringBuilder();
            var descriptionStatement = DescriptionGenerator.GenerateDescriptionStatement(expression);
            alterStatement.Append(base.Generate(expression));
            if (string.IsNullOrEmpty(descriptionStatement))
            {
                alterStatement.Append(descriptionStatement);
            }
            return alterStatement.ToString();
        }

        /// <summary>
        /// Generates a <c>CREATE SCHEMA</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(CreateSchemaExpression expression)
        {
            return string.Format("CREATE SCHEMA {0};", Quoter.QuoteSchemaName(expression.SchemaName));
        }

        /// <summary>
        /// Generates a <c>DROP SCHEMA</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(DeleteSchemaExpression expression)
        {
            return string.Format("DROP SCHEMA {0};", Quoter.QuoteSchemaName(expression.SchemaName));
        }

        /// <summary>
        /// Outputs a create table string
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(CreateTableExpression expression)
        {
            var createStatement = new StringBuilder();
            var tableName = Quoter.Quote(expression.TableName);
            createStatement.AppendFormat("CREATE TABLE {0} ({1})", Quoter.QuoteTableName(expression.TableName, expression.SchemaName), Column.Generate(expression.Columns, tableName));
            var descriptionStatement = DescriptionGenerator.GenerateDescriptionStatements(expression)
                ?.ToList();
            createStatement.Append(";");

            if (descriptionStatement != null && descriptionStatement.Count != 0)
            {
                createStatement.Append(string.Join(";", descriptionStatement.ToArray()));
                createStatement.Append(";");
            }
            return createStatement.ToString();
        }

        /// <summary>
        /// Generates a <c>ALTER TABLE ALTER COLUMN</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(AlterColumnExpression expression)
        {
            var alterStatement = new StringBuilder();
            alterStatement.AppendFormat("ALTER TABLE {0} {1};", Quoter.QuoteTableName(expression.TableName, expression.SchemaName), ((RedshiftColumn)Column).GenerateAlterClauses(expression.Column));
            var descriptionStatement = DescriptionGenerator.GenerateDescriptionStatement(expression);
            if (!string.IsNullOrEmpty(descriptionStatement))
            {
                alterStatement.Append(";");
                alterStatement.Append(descriptionStatement);
            }
            return alterStatement.ToString();
        }

        /// <summary>
        /// Generates a <c>ALTER TABLE ADD COLUMN</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(CreateColumnExpression expression)
        {
            var createStatement = new StringBuilder();
            createStatement.AppendFormat("ALTER TABLE {0} ADD {1};", Quoter.QuoteTableName(expression.TableName, expression.SchemaName), Column.Generate(expression.Column));
            var descriptionStatement = DescriptionGenerator.GenerateDescriptionStatement(expression);
            if (!string.IsNullOrEmpty(descriptionStatement))
            {
                createStatement.Append(";");
                createStatement.Append(descriptionStatement);
            }
            return createStatement.ToString();
        }

        /// <summary>
        /// Generates a <c>DROP TABLE</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(DeleteTableExpression expression)
        {
            return string.Format("DROP TABLE {0};", Quoter.QuoteTableName(expression.TableName, expression.SchemaName));
        }

        /// <summary>
        /// Generates a <c>ALTER TABLE DROP COLUMN</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(DeleteColumnExpression expression)
        {
            StringBuilder builder = new StringBuilder();
            foreach (string columnName in expression.ColumnNames) {
                if (expression.ColumnNames.First() != columnName) builder.AppendLine("");
                builder.AppendFormat("ALTER TABLE {0} DROP COLUMN {1};",
                    Quoter.QuoteTableName(expression.TableName, expression.SchemaName),
                    Quoter.QuoteColumnName(columnName));
            }
            return builder.ToString();
        }

        /// <summary>
        /// Generates an SQL statement to create a foreign key
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(CreateForeignKeyExpression expression)
        {
            var primaryColumns = GetColumnList(expression.ForeignKey.PrimaryColumns);
            var foreignColumns = GetColumnList(expression.ForeignKey.ForeignColumns);

            const string sql = "ALTER TABLE {0} ADD CONSTRAINT {1} FOREIGN KEY ({2}) REFERENCES {3} ({4}){5}{6};";

            return string.Format(sql,
                Quoter.QuoteTableName(expression.ForeignKey.ForeignTable, expression.ForeignKey.ForeignTableSchema),
                Quoter.Quote(expression.ForeignKey.Name),
                foreignColumns,
                Quoter.QuoteTableName(expression.ForeignKey.PrimaryTable, expression.ForeignKey.PrimaryTableSchema),
                primaryColumns,
                Column.FormatCascade("DELETE", expression.ForeignKey.OnDelete),
                Column.FormatCascade("UPDATE", expression.ForeignKey.OnUpdate)
            );
        }

        /// <summary>
        /// Generates an SQL statement to delete a foreign key
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(DeleteForeignKeyExpression expression)
        {
            return string.Format(
                "ALTER TABLE {0} DROP CONSTRAINT {1};",
                Quoter.QuoteTableName(expression.ForeignKey.ForeignTable, expression.ForeignKey.ForeignTableSchema),
                Quoter.Quote(expression.ForeignKey.Name));
        }

        /// <summary>
        /// Generates the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>System.String.</returns>
        /// <inheritdoc />
        public override string Generate(CreateIndexExpression expression)
        {
            return CompatibilityMode.HandleCompatibilty("Indices not supported");
        }

        /// <summary>
        /// Generates an SQL statement to drop an index
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(DeleteIndexExpression expression)
        {

            return CompatibilityMode.HandleCompatibilty("Indices not supported");
        }

        /// <summary>
        /// Generates an SQL statement to rename a table
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(RenameTableExpression expression)
        {
            return string.Format(
                "ALTER TABLE {0} RENAME TO {1};",
                Quoter.QuoteTableName(expression.OldName, expression.SchemaName),
                Quoter.Quote(expression.NewName));
        }

        /// <summary>
        /// Generates an SQL statement to rename a column
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(RenameColumnExpression expression)
        {
            return string.Format(
                "ALTER TABLE {0} RENAME COLUMN {1} TO {2};",
                Quoter.QuoteTableName(expression.TableName, expression.SchemaName),
                Quoter.QuoteColumnName(expression.OldName),
                Quoter.QuoteColumnName(expression.NewName));
        }

        /// <summary>
        /// Generates an SQL statement to INSERT data
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(InsertDataExpression expression)
        {
            var result = new StringBuilder();
            foreach (var row in expression.Rows)
            {
                var columnNames = new List<string>();
                var columnData = new List<object>();
                foreach (var item in row)
                {
                    columnNames.Add(item.Key);
                    columnData.Add(item.Value);
                }

                var columns = GetColumnList(columnNames);
                var data = GetDataList(columnData);
                result.AppendFormat("INSERT INTO {0} ({1}) VALUES ({2});", Quoter.QuoteTableName(expression.TableName, expression.SchemaName), columns, data);
            }
            return result.ToString();
        }

        /// <summary>
        /// Generates an SQL statement to alter a DEFAULT constraint
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(AlterDefaultConstraintExpression expression)
        {
            return string.Format(
                "ALTER TABLE {0} ALTER {1} DROP DEFAULT, ALTER {1} {2};",
                Quoter.QuoteTableName(expression.TableName, expression.SchemaName),
                Quoter.QuoteColumnName(expression.ColumnName),
                ((RedshiftColumn)Column).FormatAlterDefaultValue(expression.ColumnName, expression.DefaultValue));
        }

        /// <summary>
        /// Generates an SQL statement to DELETE data
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(DeleteDataExpression expression)
        {
            var result = new StringBuilder();

            if (expression.IsAllRows)
            {
                result.AppendFormat("DELETE FROM {0};", Quoter.QuoteTableName(expression.TableName, expression.SchemaName));
            }
            else
            {
                foreach (var row in expression.Rows)
                {
                    var where = string.Empty;
                    var i = 0;

                    foreach (var item in row)
                    {
                        if (i != 0)
                        {
                            where += " AND ";
                        }

                        var op = item.Value == null || item.Value == DBNull.Value ? "IS" : "=";
                        where += string.Format("{0} {1} {2}", Quoter.QuoteColumnName(item.Key), op, Quoter.QuoteValue(item.Value));
                        i++;
                    }

                    result.AppendFormat("DELETE FROM {0} WHERE {1};", Quoter.QuoteTableName(expression.TableName, expression.SchemaName), where);
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Generates an SQL statement to UPDATE data
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(UpdateDataExpression expression)
        {
            var updateItems = new List<string>();
            var whereClauses = new List<string>();

            foreach (var item in expression.Set)
            {
                updateItems.Add(string.Format("{0} = {1}", Quoter.QuoteColumnName(item.Key), Quoter.QuoteValue(item.Value)));
            }

            if (expression.IsAllRows)
            {
                whereClauses.Add("1 = 1");
            }
            else
            {
                foreach (var item in expression.Where)
                {
                    var op = item.Value == null || item.Value == DBNull.Value ? "IS" : "=";
                    whereClauses.Add(string.Format("{0} {1} {2}", Quoter.QuoteColumnName(item.Key),
                                                   op, Quoter.QuoteValue(item.Value)));
                }
            }

            return string.Format(
                "UPDATE {0} SET {1} WHERE {2};",
                Quoter.QuoteTableName(expression.TableName, expression.SchemaName),
                string.Join(", ", updateItems.ToArray()),
                string.Join(" AND ", whereClauses.ToArray()));
        }

        /// <summary>
        /// Generates an SQL statement to move a table from one schema to another
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(AlterSchemaExpression expression)
        {
            return string.Format(
                "ALTER TABLE {0} SET SCHEMA {1};",
                Quoter.QuoteTableName(expression.TableName, expression.SourceSchemaName),
                Quoter.QuoteSchemaName(expression.DestinationSchemaName));
        }

        /// <summary>
        /// Generates an SQL statement to drop a default constraint
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(DeleteDefaultConstraintExpression expression)
        {
            return string.Format("ALTER TABLE {0} ALTER {1} DROP DEFAULT;", Quoter.QuoteTableName(expression.TableName, expression.SchemaName), Quoter.Quote(expression.ColumnName));
        }

        /// <summary>
        /// Generates an SQL statement to drop a constraint
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(DeleteConstraintExpression expression)
        {
            return string.Format(
                "ALTER TABLE {0} DROP CONSTRAINT {1};",
                Quoter.QuoteTableName(expression.Constraint.TableName, expression.Constraint.SchemaName),
                Quoter.Quote(expression.Constraint.ConstraintName));
        }

        /// <summary>
        /// Generates an SQL statement to create a constraint
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(CreateConstraintExpression expression)
        {
            var constraintType = (expression.Constraint.IsPrimaryKeyConstraint) ? "PRIMARY KEY" : "UNIQUE";

            string[] columns = new string[expression.Constraint.Columns.Count];

            for (int i = 0; i < expression.Constraint.Columns.Count; i++)
            {
                columns[i] = Quoter.QuoteColumnName(expression.Constraint.Columns.ElementAt(i));
            }

            return string.Format(
                "ALTER TABLE {0} ADD CONSTRAINT {1} {2} ({3});",
                Quoter.QuoteTableName(expression.Constraint.TableName, expression.Constraint.SchemaName),
                Quoter.QuoteConstraintName(expression.Constraint.ConstraintName),
                constraintType,
                string.Join(", ", columns));
        }

        /// <summary>
        /// Gets the column list.
        /// </summary>
        /// <param name="columns">The columns.</param>
        /// <returns>System.String.</returns>
        protected string GetColumnList(IEnumerable<string> columns)
        {
            var result = "";
            foreach (var column in columns)
            {
                result += Quoter.QuoteColumnName(column) + ",";
            }
            return result.TrimEnd(',');
        }

        /// <summary>
        /// Gets the data list.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>System.String.</returns>
        protected string GetDataList(List<object> data)
        {
            var result = "";
            foreach (var column in data)
            {
                result += Quoter.QuoteValue(column) + ",";
            }
            return result.TrimEnd(',');
        }

        /// <summary>
        /// Generates a <c>CREATE SEQUENCE</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(CreateSequenceExpression expression)
        {
            return CompatibilityMode.HandleCompatibilty("Sequences not supported");
        }

        /// <summary>
        /// Generates the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>System.String.</returns>
        public override string Generate(DeleteSequenceExpression expression)
        {
            return CompatibilityMode.HandleCompatibilty("Sequences not supported");
        }
    }
}
