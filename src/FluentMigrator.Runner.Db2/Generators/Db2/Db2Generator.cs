// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Db2
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="Db2Generator.cs" company="FluentMigrator Project">
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

using FluentMigrator.Model;
using FluentMigrator.Runner.Generators.Generic;

using Microsoft.Extensions.Options;

namespace FluentMigrator.Runner.Generators.DB2
{
    /// <summary>
    /// Class Db2Generator.
    /// Implements the <see cref="FluentMigrator.Runner.Generators.Generic.GenericGenerator" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Generators.Generic.GenericGenerator" />
    public class Db2Generator : GenericGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Db2Generator"/> class.
        /// </summary>
        public Db2Generator()
            : this(new Db2Quoter())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Db2Generator"/> class.
        /// </summary>
        /// <param name="quoter">The quoter.</param>
        public Db2Generator(
            Db2Quoter quoter)
            : this(
                quoter,
                new OptionsWrapper<GeneratorOptions>(new GeneratorOptions()))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Db2Generator"/> class.
        /// </summary>
        /// <param name="quoter">The quoter.</param>
        /// <param name="generatorOptions">The generator options.</param>
        public Db2Generator(
            Db2Quoter quoter,
            IOptions<GeneratorOptions> generatorOptions)
            : base(new Db2Column(quoter), quoter, new EmptyDescriptionGenerator(), generatorOptions)
        {
        }

        /// <summary>
        /// Generates an SQL statement to alter a DEFAULT constraint
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(Expressions.AlterDefaultConstraintExpression expression)
        {
            return string.Format(
                "ALTER TABLE {0} ALTER COLUMN {1} SET DEFAULT {2}",
                Quoter.QuoteTableName(expression.TableName, expression.SchemaName),
                Quoter.QuoteColumnName(expression.ColumnName),
                ((Db2Column)Column).FormatAlterDefaultValue(expression.ColumnName, expression.DefaultValue));
        }

        /// <summary>
        /// Generates an SQL statement to drop a default constraint
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(Expressions.DeleteDefaultConstraintExpression expression)
        {
            return string.Format(
                "ALTER TABLE {0} ALTER COLUMN {1} DROP DEFAULT",
                Quoter.QuoteTableName(expression.TableName, expression.SchemaName),
                Quoter.QuoteColumnName(expression.ColumnName));
        }

        /// <summary>
        /// Generates an SQL statement to rename a table
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(Expressions.RenameTableExpression expression)
        {
            return string.Format(
                "RENAME TABLE {0} TO {1}",
                Quoter.QuoteTableName(expression.OldName, expression.SchemaName),
                Quoter.QuoteTableName(expression.NewName));
        }

        /// <summary>
        /// Generates a <c>ALTER TABLE DROP COLUMN</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(Expressions.DeleteColumnExpression expression)
        {
            var builder = new StringBuilder();
            if (expression.ColumnNames.Count == 0 || string.IsNullOrEmpty(expression.ColumnNames.First()))
            {
                return string.Empty;
            }

            builder.AppendFormat("ALTER TABLE {0}", Quoter.QuoteTableName(expression.TableName, expression.SchemaName));
            foreach (var column in expression.ColumnNames)
            {
                builder.AppendFormat(" DROP COLUMN {0}", Quoter.QuoteColumnName(column));
            }

            return builder.ToString();
        }

        /// <summary>
        /// Generates a <c>ALTER TABLE ADD COLUMN</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(Expressions.CreateColumnExpression expression)
        {
            expression.Column.AdditionalFeatures.Add(new KeyValuePair<string, object>("IsCreateColumn", true));

            return string.Format(
                "ALTER TABLE {0} ADD COLUMN {1}",
                Quoter.QuoteTableName(expression.TableName, expression.SchemaName),
                Column.Generate(expression.Column));
        }

        /// <summary>
        /// Generates an SQL statement to create a foreign key
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        /// <exception cref="ArgumentException">Number of primary columns and secondary columns must be equal</exception>
        public override string Generate(Expressions.CreateForeignKeyExpression expression)
        {
            if (expression.ForeignKey.PrimaryColumns.Count != expression.ForeignKey.ForeignColumns.Count)
            {
                throw new ArgumentException("Number of primary columns and secondary columns must be equal");
            }

            var keyName = string.IsNullOrEmpty(expression.ForeignKey.Name)
                ? Column.GenerateForeignKeyName(expression.ForeignKey)
                : expression.ForeignKey.Name;
            var keyWithSchema = Quoter.QuoteConstraintName(keyName, expression.ForeignKey.ForeignTableSchema);

            var primaryColumns = expression.ForeignKey.PrimaryColumns.Aggregate(new StringBuilder(), (acc, col) =>
            {
                var separator = acc.Length == 0 ? string.Empty : ", ";
                return acc.AppendFormat("{0}{1}", separator, Quoter.QuoteColumnName(col));
            });

            var foreignColumns = expression.ForeignKey.ForeignColumns.Aggregate(new StringBuilder(), (acc, col) =>
            {
                var separator = acc.Length == 0 ? string.Empty : ", ";
                return acc.AppendFormat("{0}{1}", separator, Quoter.QuoteColumnName(col));
            });

            return string.Format(
                "ALTER TABLE {0} ADD CONSTRAINT {1} FOREIGN KEY ({2}) REFERENCES {3} ({4}){5}",
                Quoter.QuoteTableName(expression.ForeignKey.ForeignTable, expression.ForeignKey.ForeignTableSchema),
                keyWithSchema,
                foreignColumns,
                Quoter.QuoteTableName(expression.ForeignKey.PrimaryTable, expression.ForeignKey.PrimaryTableSchema),
                primaryColumns,
                Column.FormatCascade("DELETE", expression.ForeignKey.OnDelete));
        }

        /// <summary>
        /// Generates an SQL statement to create a constraint
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(Expressions.CreateConstraintExpression expression)
        {
            var constraintName = Quoter.QuoteConstraintName(expression.Constraint.ConstraintName, expression.Constraint.SchemaName);

            var constraintType = expression.Constraint.IsPrimaryKeyConstraint ? "PRIMARY KEY" : "UNIQUE";
            var quotedNames = expression.Constraint.Columns.Select(q => Quoter.QuoteColumnName(q));
            var columnList = string.Join(", ", quotedNames.ToArray());

            return string.Format(
                "ALTER TABLE {0} ADD CONSTRAINT {1} {2} ({3})",
                Quoter.QuoteTableName(expression.Constraint.TableName, expression.Constraint.SchemaName),
                constraintName,
                constraintType,
                columnList);
        }

        /// <summary>
        /// Generates the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>System.String.</returns>
        /// <inheritdoc />
        public override string Generate(Expressions.CreateIndexExpression expression)
        {
            var indexWithSchema = Quoter.QuoteIndexName(expression.Index.Name, expression.Index.SchemaName);

            var columnList = expression.Index.Columns.Aggregate(new StringBuilder(), (item, itemToo) =>
            {
                var accumulator = item.Length == 0 ? string.Empty : ", ";
                var direction = itemToo.Direction == Direction.Ascending ? string.Empty : " DESC";

                return item.AppendFormat("{0}{1}{2}", accumulator, Quoter.QuoteColumnName(itemToo.Name), direction);
            });

            return string.Format(
                "CREATE {0}INDEX {1} ON {2} ({3})",
                expression.Index.IsUnique ? "UNIQUE " : string.Empty,
                indexWithSchema,
                Quoter.QuoteTableName(expression.Index.TableName, expression.Index.SchemaName),
                columnList);
        }

        /// <summary>
        /// Generates a <c>CREATE SCHEMA</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(Expressions.CreateSchemaExpression expression)
        {
            return string.Format("CREATE SCHEMA {0}", Quoter.QuoteSchemaName(expression.SchemaName));
        }

        /// <summary>
        /// Generates a <c>DROP TABLE</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(Expressions.DeleteTableExpression expression)
        {
            return string.Format("DROP TABLE {0}", Quoter.QuoteTableName(expression.TableName, expression.SchemaName));
        }

        /// <summary>
        /// Generates an SQL statement to drop an index
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(Expressions.DeleteIndexExpression expression)
        {
            var indexWithSchema = Quoter.QuoteIndexName(expression.Index.Name, expression.Index.SchemaName);
            return string.Format("DROP INDEX {0}", indexWithSchema);
        }

        /// <summary>
        /// Generates a <c>DROP SCHEMA</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(Expressions.DeleteSchemaExpression expression)
        {
            return string.Format("DROP SCHEMA {0} RESTRICT", Quoter.QuoteSchemaName(expression.SchemaName));
        }

        /// <summary>
        /// Generates an SQL statement to drop a constraint
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(Expressions.DeleteConstraintExpression expression)
        {
            var constraintName = Quoter.QuoteConstraintName(expression.Constraint.ConstraintName, expression.Constraint.SchemaName);

            return string.Format(
                "ALTER TABLE {0} DROP CONSTRAINT {1}",
                Quoter.QuoteTableName(expression.Constraint.TableName, expression.Constraint.SchemaName),
                constraintName);
        }

        /// <summary>
        /// Generates an SQL statement to delete a foreign key
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(Expressions.DeleteForeignKeyExpression expression)
        {
            var constraintName = Quoter.QuoteConstraintName(expression.ForeignKey.Name, expression.ForeignKey.ForeignTableSchema);

            return string.Format(
                "ALTER TABLE {0} DROP FOREIGN KEY {1}",
                Quoter.QuoteTableName(expression.ForeignKey.ForeignTable, expression.ForeignKey.ForeignTableSchema),
                constraintName);
        }

        /// <summary>
        /// Generates an SQL statement to DELETE data
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(Expressions.DeleteDataExpression expression)
        {
            if (expression.IsAllRows)
            {
                return string.Format("DELETE FROM {0}", Quoter.QuoteTableName(expression.TableName, expression.SchemaName));
            }
            else
            {
                var deleteExpressions = new StringBuilder();
                foreach (var row in expression.Rows)
                {
                    var clauses = row.Aggregate(new StringBuilder(), (acc, rowVal) =>
                    {
                        var accumulator = acc.Length == 0 ? string.Empty : " AND ";
                        var clauseOperator = rowVal.Value == null || rowVal.Value == DBNull.Value
                            ? "IS"
                            : "=";

                        return acc.AppendFormat("{0}{1} {2} {3}", accumulator, Quoter.QuoteColumnName(rowVal.Key), clauseOperator, Quoter.QuoteValue(rowVal.Value));
                    });

                    var separator = deleteExpressions.Length > 0 ? " " : string.Empty;
                    deleteExpressions.AppendFormat("{0}DELETE FROM {1} WHERE {2}", separator, Quoter.QuoteTableName(expression.TableName, expression.SchemaName), clauses);
                }

                return deleteExpressions.ToString();
            }
        }

        /// <summary>
        /// Generates an SQL statement to rename a column
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(Expressions.RenameColumnExpression expression)
        {
            return CompatibilityMode.HandleCompatibilty("This feature not directly supported by most versions of DB2.");
        }

        /// <summary>
        /// Generates an SQL statement to INSERT data
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(Expressions.InsertDataExpression expression)
        {
            var sb = new StringBuilder();
            foreach (var row in expression.Rows)
            {
                var columnList = row.Aggregate(new StringBuilder(), (acc, rowVal) =>
                {
                    var accumulator = acc.Length == 0 ? string.Empty : ", ";
                    return acc.AppendFormat("{0}{1}", accumulator, Quoter.QuoteColumnName(rowVal.Key));
                });

                var dataList = row.Aggregate(new StringBuilder(), (acc, rowVal) =>
                {
                    var accumulator = acc.Length == 0 ? string.Empty : ", ";
                    return acc.AppendFormat("{0}{1}", accumulator, Quoter.QuoteValue(rowVal.Value));
                });

                var separator = sb.Length == 0 ? string.Empty : " ";

                sb.AppendFormat(
                    "{0}INSERT INTO {1} ({2}) VALUES ({3})",
                    separator,
                    Quoter.QuoteTableName(expression.TableName, expression.SchemaName),
                    columnList,
                    dataList);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Generates an SQL statement to UPDATE data
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(Expressions.UpdateDataExpression expression)
        {
            var updateClauses = expression.Set.Aggregate(new StringBuilder(), (acc, newRow) =>
            {
                var accumulator = acc.Length == 0 ? string.Empty : ", ";
                return acc.AppendFormat("{0}{1} = {2}", accumulator, Quoter.QuoteColumnName(newRow.Key), Quoter.QuoteValue(newRow.Value));
            });

            if (expression.IsAllRows)
            {
                return string.Format("UPDATE {0} SET {1}", Quoter.QuoteTableName(expression.TableName, expression.SchemaName), updateClauses);
            }

            var whereClauses = expression.Where.Aggregate(new StringBuilder(), (acc, rowVal) =>
            {
                var accumulator = acc.Length == 0 ? string.Empty : " AND ";
                var clauseOperator = rowVal.Value == null || rowVal.Value == DBNull.Value ? "IS" : "=";

                return acc.AppendFormat("{0}{1} {2} {3}", accumulator, Quoter.QuoteColumnName(rowVal.Key), clauseOperator, Quoter.QuoteValue(rowVal.Value));
            });

            return string.Format("UPDATE {0} SET {1} WHERE {2}", Quoter.QuoteTableName(expression.TableName, expression.SchemaName), updateClauses, whereClauses);
        }

        /// <summary>
        /// Outputs a create table string
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(Expressions.CreateTableExpression expression)
        {
            return string.Format("CREATE TABLE {0} ({1})", Quoter.QuoteTableName(expression.TableName, expression.SchemaName), Column.Generate(expression.Columns, expression.TableName));
        }

        /// <summary>
        /// Generates a <c>ALTER TABLE ALTER COLUMN</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(Expressions.AlterColumnExpression expression)
        {
            try
            {
                // throws an exception of an attempt is made to alter an identity column, as it is not supported by most version of DB2.
                return string.Format("ALTER TABLE {0} {1}", Quoter.QuoteTableName(expression.TableName, expression.SchemaName), ((Db2Column)Column).GenerateAlterClause(expression.Column));
            }
            catch (NotSupportedException e)
            {
                return CompatibilityMode.HandleCompatibilty(e.Message);
            }
        }
        /// <summary>
        /// Generates the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>System.String.</returns>
        public override string Generate(Expressions.AlterSchemaExpression expression)
        {
            return CompatibilityMode.HandleCompatibilty("This feature not directly supported by most versions of DB2.");
        }
    }
}
