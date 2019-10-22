// ***********************************************************************
// Assembly         : FluentMigrator.Runner.SqlAnywhere
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SqlAnywhere16Generator.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
//
// Copyright (c) 2007-2018, Sean Chambers <schambers80@gmail.com>
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
using FluentMigrator.Infrastructure.Extensions;
using FluentMigrator.Runner.Generators.Generic;
using FluentMigrator.Model;
using FluentMigrator.SqlAnywhere;

using JetBrains.Annotations;

using Microsoft.Extensions.Options;

namespace FluentMigrator.Runner.Generators.SqlAnywhere
{
    /// <summary>
    /// Class SqlAnywhere16Generator.
    /// Implements the <see cref="FluentMigrator.Runner.Generators.Generic.GenericGenerator" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Generators.Generic.GenericGenerator" />
    public class SqlAnywhere16Generator : GenericGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlAnywhere16Generator"/> class.
        /// </summary>
        public SqlAnywhere16Generator()
            : this(new SqlAnywhereQuoter())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlAnywhere16Generator"/> class.
        /// </summary>
        /// <param name="quoter">The quoter.</param>
        public SqlAnywhere16Generator(
            [NotNull] SqlAnywhereQuoter quoter)
            : this(quoter, new OptionsWrapper<GeneratorOptions>(new GeneratorOptions()))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlAnywhere16Generator"/> class.
        /// </summary>
        /// <param name="quoter">The quoter.</param>
        /// <param name="generatorOptions">The generator options.</param>
        public SqlAnywhere16Generator(
            [NotNull] SqlAnywhereQuoter quoter,
            [NotNull] IOptions<GeneratorOptions> generatorOptions)
            : this(new SqlAnywhereColumn(new SqlAnywhere16TypeMap()), quoter, new EmptyDescriptionGenerator(), generatorOptions)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlAnywhere16Generator"/> class.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="quoter">The quoter.</param>
        /// <param name="descriptionGenerator">The description generator.</param>
        /// <param name="generatorOptions">The generator options.</param>
        protected SqlAnywhere16Generator(
            [NotNull] IColumn column,
            [NotNull] IQuoter quoter,
            [NotNull] IDescriptionGenerator descriptionGenerator,
            [NotNull] IOptions<GeneratorOptions> generatorOptions)
            : base(column, quoter, descriptionGenerator, generatorOptions)
        {
        }

        /// <summary>
        /// Gets the create schema.
        /// </summary>
        /// <value>The create schema.</value>
        public override string CreateSchema => "CREATE SCHEMA AUTHORIZATION {0}";

        /// <summary>
        /// Gets the drop schema.
        /// </summary>
        /// <value>The drop schema.</value>
        public override string DropSchema => "DROP USER {0}";

        /// <summary>
        /// Gets the rename table.
        /// </summary>
        /// <value>The rename table.</value>
        public override string RenameTable => "ALTER TABLE {0} RENAME {1}";

        /// <summary>
        /// Gets the index of the create.
        /// </summary>
        /// <value>The index of the create.</value>
        public override string CreateIndex => "CREATE {0}{1}INDEX {2} ON {3} ({4}){5}";

        /// <summary>
        /// Gets the index of the drop.
        /// </summary>
        /// <value>The index of the drop.</value>
        public override string DropIndex => "DROP INDEX {0}.{1}";

        /// <summary>
        /// Gets the add column.
        /// </summary>
        /// <value>The add column.</value>
        public override string AddColumn => "ALTER TABLE {0} ADD {1}";

        /// <summary>
        /// Gets the drop column.
        /// </summary>
        /// <value>The drop column.</value>
        public override string DropColumn => "ALTER TABLE {0} DROP {1}";

        /// <summary>
        /// Gets the alter column.
        /// </summary>
        /// <value>The alter column.</value>
        public override string AlterColumn => "ALTER TABLE {0} ALTER {1}";

        /// <summary>
        /// Gets the rename column.
        /// </summary>
        /// <value>The rename column.</value>
        public override string RenameColumn => "ALTER TABLE {0} RENAME {1} TO {2}";

        /// <summary>
        /// Gets the create foreign key constraint.
        /// </summary>
        /// <value>The create foreign key constraint.</value>
        public override string CreateForeignKeyConstraint => "ALTER TABLE {0} ADD CONSTRAINT {1} FOREIGN KEY ({2}) REFERENCES {3} ({4}){5}{6}";

        /// <summary>
        /// Gets the create constraint.
        /// </summary>
        /// <value>The create constraint.</value>
        public override string CreateConstraint => "ALTER TABLE {0} ADD CONSTRAINT {1} {2}{3} ({4})";

        //Not need for the nonclusted keyword as it is the default mode
        /// <summary>
        /// Gets the cluster type string.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns>System.String.</returns>
        public override string GetClusterTypeString(CreateIndexExpression column)
        {
            return column.Index.IsClustered ? "CLUSTERED " : string.Empty;
        }

        /// <summary>
        /// Gets the constraint clustering string.
        /// </summary>
        /// <param name="constraint">The constraint.</param>
        /// <returns>System.String.</returns>
        protected string GetConstraintClusteringString(CreateConstraintExpression constraint)
        {
            if (!constraint.Constraint.AdditionalFeatures.TryGetValue(
                SqlAnywhereExtensions.ConstraintType, out var indexType)) return string.Empty;

            return (indexType.Equals(SqlAnywhereConstraintType.Clustered)) ? " CLUSTERED" : " NONCLUSTERED";
        }

        /// <summary>
        /// Outputs a create table string
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(CreateTableExpression expression)
        {
            var descriptionStatements = DescriptionGenerator.GenerateDescriptionStatements(expression);
            var createTableStatement = base.Generate(expression);
            var descriptionStatementsArray = descriptionStatements as string[] ?? descriptionStatements.ToArray();

            if (!descriptionStatementsArray.Any())
                return createTableStatement;

            return ComposeStatements(createTableStatement, descriptionStatementsArray);
        }

        /// <summary>
        /// Generates a <c>ALTER TABLE ADD COLUMN</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(CreateColumnExpression expression)
        {
            var alterTableStatement = base.Generate(expression);
            var descriptionStatement = DescriptionGenerator.GenerateDescriptionStatement(expression);

            if (string.IsNullOrEmpty(descriptionStatement))
                return alterTableStatement;

            return ComposeStatements(alterTableStatement, new[] { descriptionStatement });
        }

        /// <summary>
        /// Generates a <c>ALTER TABLE ALTER COLUMN</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(AlterColumnExpression expression)
        {
            var alterTableStatement = base.Generate(expression);
            var descriptionStatement = DescriptionGenerator.GenerateDescriptionStatement(expression);

            if (string.IsNullOrEmpty(descriptionStatement))
                return alterTableStatement;

            return ComposeStatements(alterTableStatement, new[] { descriptionStatement });
        }

        /// <summary>
        /// Generates an SQL statement to create a constraint
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(CreateConstraintExpression expression)
        {
            var constraintType = (expression.Constraint.IsPrimaryKeyConstraint) ? "PRIMARY KEY" : "UNIQUE";

            var constraintClustering = GetConstraintClusteringString(expression);

            string columns = string.Join(", ", expression.Constraint.Columns.Select(x => Quoter.QuoteColumnName(x)).ToArray());

            string schemaTableName = Quoter.QuoteTableName(expression.Constraint.TableName, expression.Constraint.SchemaName);

            return string.Format(CreateConstraint, schemaTableName,
                Quoter.Quote(expression.Constraint.ConstraintName),
                constraintType,
                constraintClustering,
                columns);
        }

        /// <summary>
        /// Generates an SQL statement to alter a DEFAULT constraint
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(AlterDefaultConstraintExpression expression)
        {
            // before we alter a default constraint on a column, we have to drop any default value constraints in SQL Anywhere
            var builder = new StringBuilder();
            var deleteDefault = Generate(new DeleteDefaultConstraintExpression
            {
                ColumnName = expression.ColumnName,
                SchemaName = expression.SchemaName,
                TableName = expression.TableName
            }) + ";";
            builder.AppendLine(deleteDefault);

            builder.Append(string.Format("-- create alter table command to create new default constraint as string and run it" + Environment.NewLine + "ALTER TABLE {0} ALTER {1} DEFAULT {2};",
                Quoter.QuoteTableName(expression.TableName, expression.SchemaName),
                Quoter.QuoteColumnName(expression.ColumnName),
                SqlAnywhereColumn.FormatDefaultValue(expression.DefaultValue, Quoter)));

            return builder.ToString();
        }

        /// <summary>
        /// Generates an SQL statement to create a foreign key
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        /// <exception cref="ArgumentException">Number of primary columns and secondary columns must be equal</exception>
        public override string Generate(CreateForeignKeyExpression expression)
        {
            if (expression.ForeignKey.PrimaryColumns.Count != expression.ForeignKey.ForeignColumns.Count)
            {
                throw new ArgumentException("Number of primary columns and secondary columns must be equal");
            }

            List<string> primaryColumns = new List<string>();
            List<string> foreignColumns = new List<string>();
            foreach (var column in expression.ForeignKey.PrimaryColumns)
            {
                primaryColumns.Add(Quoter.QuoteColumnName(column));
            }

            foreach (var column in expression.ForeignKey.ForeignColumns)
            {
                foreignColumns.Add(Quoter.QuoteColumnName(column));
            }

            return string.Format(
                CreateForeignKeyConstraint,
                Quoter.QuoteTableName(expression.ForeignKey.ForeignTable, expression.ForeignKey.ForeignTableSchema),
                Quoter.QuoteColumnName(expression.ForeignKey.Name),
                string.Join(", ", foreignColumns.ToArray()),
                Quoter.QuoteTableName(expression.ForeignKey.PrimaryTable, expression.ForeignKey.PrimaryTableSchema),
                string.Join(", ", primaryColumns.ToArray()),
                Column.FormatCascade("DELETE", expression.ForeignKey.OnDelete),
                Column.FormatCascade("UPDATE", expression.ForeignKey.OnUpdate)
                );
        }

        /// <summary>
        /// Generates the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>System.String.</returns>
        /// <inheritdoc />
        public override string Generate(CreateIndexExpression expression)
        {
            string[] indexColumns = new string[expression.Index.Columns.Count];

            for (int i = 0; i < expression.Index.Columns.Count; i++)
            {
                var columnDef = expression.Index.Columns.ElementAt(i);
                if (columnDef.Direction == Direction.Ascending)
                {
                    indexColumns[i] = Quoter.QuoteColumnName(columnDef.Name) + " ASC";
                }
                else
                {
                    indexColumns[i] = Quoter.QuoteColumnName(columnDef.Name) + " DESC";
                }
            }

            return string.Format(CreateIndex
                , GetUniqueString(expression)
                , GetClusterTypeString(expression)
                , Quoter.QuoteIndexName(expression.Index.Name)
                , Quoter.QuoteTableName(expression.Index.TableName, expression.Index.SchemaName)
                , string.Join(", ", indexColumns)
                , GetWithNullsDistinctString(expression.Index));
        }

        /// <summary>
        /// Generates an SQL statement to drop an index
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(DeleteIndexExpression expression)
        {
            return string.Format(DropIndex, Quoter.QuoteTableName(expression.Index.TableName, expression.Index.SchemaName), Quoter.QuoteIndexName(expression.Index.Name));
        }

        /// <summary>
        /// Generates an SQL statement to move a table from one schema to another
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(AlterSchemaExpression expression)
        {
            return CompatibilityMode.HandleCompatibilty("AlterSchema is not supported in SqlAnywhere");
        }

        /// <summary>
        /// Generates a <c>CREATE SCHEMA</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(CreateSchemaExpression expression)
        {
            return string.Format(CreateSchema, Quoter.QuoteSchemaName(expression.SchemaName));
        }

        /// <summary>
        /// Generates a <c>DROP SCHEMA</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(DeleteSchemaExpression expression)
        {
            return string.Format(DropSchema, Quoter.QuoteSchemaName(expression.SchemaName));
        }

        /// <summary>
        /// Generates a <c>CREATE SEQUENCE</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(CreateSequenceExpression expression)
        {
            return CompatibilityMode.HandleCompatibilty("Sequences are not supported in SqlAnywhere");
        }

        /// <summary>
        /// Generates the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>System.String.</returns>
        public override string Generate(DeleteSequenceExpression expression)
        {
            return CompatibilityMode.HandleCompatibilty("Sequences are not supported in SqlAnywhere");
        }

        /// <summary>
        /// Generates an SQL statement to drop a default constraint
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(DeleteDefaultConstraintExpression expression)
        {
            string sql = "ALTER TABLE {0} ALTER {1} DROP DEFAULT";
            string schemaAndTable = Quoter.QuoteTableName(expression.TableName, expression.SchemaName);
            return string.Format(sql, schemaAndTable, Quoter.QuoteColumnName(expression.ColumnName));
        }

        /// <summary>
        /// Determines whether [is additional feature supported] [the specified feature].
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <returns><c>true</c> if [is additional feature supported] [the specified feature]; otherwise, <c>false</c>.</returns>
        public override bool IsAdditionalFeatureSupported(string feature)
        {
            return _supportedAdditionalFeatures.Any(x => x == feature);
        }

        /// <summary>
        /// Gets the with nulls distinct string.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>System.String.</returns>
        protected virtual string GetWithNullsDistinctString(IndexDefinition index)
        {
            var indexNullsDistinct = index.GetAdditionalFeature(SqlAnywhereExtensions.WithNullsDistinct, (bool?)null);

            if (indexNullsDistinct == null)
                return string.Empty;

            if (indexNullsDistinct.Value)
            {
                return " WITH NULLS DISTINCT";
            }

            return " WITH NULLS NOT DISTINCT";
        }

        /// <summary>
        /// The supported additional features
        /// </summary>
        private readonly IEnumerable<string> _supportedAdditionalFeatures = new List<string>
        {
            SqlAnywhereExtensions.ConstraintType,
            SqlAnywhereExtensions.SchemaPassword,
            SqlAnywhereExtensions.WithNullsDistinct,
        };

        /// <summary>
        /// Composes the statements.
        /// </summary>
        /// <param name="ddlStatement">The DDL statement.</param>
        /// <param name="otherStatements">The other statements.</param>
        /// <returns>System.String.</returns>
        private string ComposeStatements(string ddlStatement, IEnumerable<string> otherStatements)
        {
            var otherStatementsArray = otherStatements.ToArray();

            var statementsBuilder = new StringBuilder();
            statementsBuilder.AppendLine(ddlStatement);
            statementsBuilder.AppendLine("GO");
            statementsBuilder.AppendLine(string.Join(";", otherStatementsArray));

            return statementsBuilder.ToString();
        }
    }
}
