// ***********************************************************************
// Assembly         : FluentMigrator.Runner.SqlServer
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SqlServer2005Generator.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
//
// Copyright (c) 2010, Nathan Brown
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
using FluentMigrator.Infrastructure;
using FluentMigrator.Infrastructure.Extensions;
using FluentMigrator.Model;
using FluentMigrator.SqlServer;

using JetBrains.Annotations;

using Microsoft.Extensions.Options;

namespace FluentMigrator.Runner.Generators.SqlServer
{
    /// <summary>
    /// Class SqlServer2005Generator.
    /// Implements the <see cref="FluentMigrator.Runner.Generators.SqlServer.SqlServer2000Generator" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Generators.SqlServer.SqlServer2000Generator" />
    public class SqlServer2005Generator : SqlServer2000Generator
    {
        /// <summary>
        /// The supported additional features
        /// </summary>
        private static readonly HashSet<string> _supportedAdditionalFeatures = new HashSet<string>
        {
            SqlServerExtensions.IncludesList,
            SqlServerExtensions.OnlineIndex,
            SqlServerExtensions.RowGuidColumn,
            SqlServerExtensions.SchemaAuthorization,
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServer2005Generator"/> class.
        /// </summary>
        public SqlServer2005Generator()
            : this(new SqlServer2005Quoter())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServer2005Generator"/> class.
        /// </summary>
        /// <param name="quoter">The quoter.</param>
        public SqlServer2005Generator(
            [NotNull] SqlServer2005Quoter quoter)
            : this(quoter, new OptionsWrapper<GeneratorOptions>(new GeneratorOptions()))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServer2005Generator"/> class.
        /// </summary>
        /// <param name="quoter">The quoter.</param>
        /// <param name="generatorOptions">The generator options.</param>
        public SqlServer2005Generator(
            [NotNull] SqlServer2005Quoter quoter,
            [NotNull] IOptions<GeneratorOptions> generatorOptions)
            : this(
                new SqlServer2005Column(new SqlServer2005TypeMap(), quoter),
                quoter,
                new SqlServer2005DescriptionGenerator(),
                generatorOptions)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServer2005Generator"/> class.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="quoter">The quoter.</param>
        /// <param name="descriptionGenerator">The description generator.</param>
        /// <param name="generatorOptions">The generator options.</param>
        protected SqlServer2005Generator(
            [NotNull] IColumn column,
            [NotNull] IQuoter quoter,
            [NotNull] IDescriptionGenerator descriptionGenerator,
            [NotNull] IOptions<GeneratorOptions> generatorOptions)
            : base(column, quoter, descriptionGenerator, generatorOptions)
        {
        }

        /// <summary>
        /// Gets the add column.
        /// </summary>
        /// <value>The add column.</value>
        public override string AddColumn { get { return "ALTER TABLE {0} ADD {1}"; } }

        /// <summary>
        /// Gets the create schema.
        /// </summary>
        /// <value>The create schema.</value>
        public override string CreateSchema { get { return "CREATE SCHEMA {0}{1}"; } }

        /// <summary>
        /// Gets the index of the create.
        /// </summary>
        /// <value>The index of the create.</value>
        public override string CreateIndex { get { return "CREATE {0}{1}INDEX {2} ON {3} ({4}{5}{6}){7}"; } }
        /// <summary>
        /// Gets the index of the drop.
        /// </summary>
        /// <value>The index of the drop.</value>
        public override string DropIndex { get { return "DROP INDEX {0} ON {1}{2}"; } }

        /// <summary>
        /// Gets the identity insert.
        /// </summary>
        /// <value>The identity insert.</value>
        public override string IdentityInsert { get { return "SET IDENTITY_INSERT {0} {1}"; } }

        /// <summary>
        /// Gets the create foreign key constraint.
        /// </summary>
        /// <value>The create foreign key constraint.</value>
        public override string CreateForeignKeyConstraint { get { return "ALTER TABLE {0} ADD CONSTRAINT {1} FOREIGN KEY ({2}) REFERENCES {3} ({4}){5}{6}"; } }

        /// <summary>
        /// Gets the include string.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns>System.String.</returns>
        public virtual string GetIncludeString(CreateIndexExpression column)
        {
            var includes = column.GetAdditionalFeature<IList<IndexIncludeDefinition>>(SqlServerExtensions.IncludesList);
            return includes?.Count > 0 ? ") INCLUDE (" : string.Empty;
        }

        /// <summary>
        /// Gets the with options.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>System.String.</returns>
        public virtual string GetWithOptions(ISupportAdditionalFeatures expression)
        {
            var items = new List<string>();
            var isOnline = expression.GetAdditionalFeature(SqlServerExtensions.OnlineIndex, (bool?)null);
            if (isOnline.HasValue)
            {
                items.Add($"ONLINE={(isOnline.Value ? "ON" : "OFF")}");
            }

            return string.Join(", ", items);
        }

        /// <summary>
        /// Determines whether [is additional feature supported] [the specified feature].
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <returns><c>true</c> if [is additional feature supported] [the specified feature]; otherwise, <c>false</c>.</returns>
        public override bool IsAdditionalFeatureSupported(string feature)
        {
            return _supportedAdditionalFeatures.Contains(feature)
             || base.IsAdditionalFeatureSupported(feature);
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
        /// Generates a <c>ALTER TABLE</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(AlterTableExpression expression)
        {
            var descriptionStatement = DescriptionGenerator.GenerateDescriptionStatement(expression);

            if (string.IsNullOrEmpty(descriptionStatement))
                return base.Generate(expression);

            return descriptionStatement;
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
            IndexColumnDefinition columnDef;


            for (int i = 0; i < expression.Index.Columns.Count; i++)
            {
                columnDef = expression.Index.Columns.ElementAt(i);
                if (columnDef.Direction == Direction.Ascending)
                {
                    indexColumns[i] = Quoter.QuoteColumnName(columnDef.Name) + " ASC";
                }
                else
                {
                    indexColumns[i] = Quoter.QuoteColumnName(columnDef.Name) + " DESC";
                }
            }

            var includes = expression.Index.GetAdditionalFeature<IList<IndexIncludeDefinition>>(SqlServerExtensions.IncludesList);
            string[] indexIncludes = new string[includes?.Count ?? 0];

            if (includes != null)
            {
                for (int i = 0; i != includes.Count; i++)
                {
                    var includeDef = includes[i];
                    indexIncludes[i] = Quoter.QuoteColumnName(includeDef.Name);
                }
            }

            var withParts = GetWithOptions(expression);
            var withPart = !string.IsNullOrEmpty(withParts)
                ? $" WITH ({withParts})"
                : string.Empty;

            var result = string.Format(
                CreateIndex,
                GetUniqueString(expression),
                GetClusterTypeString(expression),
                Quoter.QuoteIndexName(expression.Index.Name),
                Quoter.QuoteTableName(expression.Index.TableName, expression.Index.SchemaName),
                string.Join(", ", indexColumns),
                GetIncludeString(expression),
                string.Join(", ", indexIncludes),
                withPart);

            return result;
        }

        /// <summary>
        /// Generates an SQL statement to drop an index
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(DeleteIndexExpression expression)
        {
            var withParts = GetWithOptions(expression);
            var withPart = !string.IsNullOrEmpty(withParts)
                ? $" WITH ({withParts})"
                : string.Empty;

            return string.Format(
                DropIndex,
                Quoter.QuoteIndexName(expression.Index.Name),
                Quoter.QuoteTableName(expression.Index.TableName, expression.Index.SchemaName),
                withPart);
        }

        /// <summary>
        /// Generates an SQL statement to create a constraint
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(CreateConstraintExpression expression)
        {
            var withParts = GetWithOptions(expression);
            var withPart = !string.IsNullOrEmpty(withParts)
                ? $" WITH ({withParts})"
                : string.Empty;

            return $"{base.Generate(expression)}{withPart}";
        }

        /// <summary>
        /// Generates an SQL statement to drop a default constraint
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(DeleteDefaultConstraintExpression expression)
        {
            string sql =
                "DECLARE @default sysname, @sql nvarchar(max);" + Environment.NewLine + Environment.NewLine +
                "-- get name of default constraint" + Environment.NewLine +
                "SELECT @default = name" + Environment.NewLine +
                "FROM sys.default_constraints" + Environment.NewLine +
                "WHERE parent_object_id = object_id('{0}')" + Environment.NewLine +
                "AND type = 'D'" + Environment.NewLine +
                "AND parent_column_id = (" + Environment.NewLine +
                "SELECT column_id" + Environment.NewLine +
                "FROM sys.columns" + Environment.NewLine +
                "WHERE object_id = object_id('{0}')" + Environment.NewLine +
                "AND name = '{1}'" + Environment.NewLine +
                ");" + Environment.NewLine + Environment.NewLine +
                "-- create alter table command to drop constraint as string and run it" + Environment.NewLine +
                "SET @sql = N'ALTER TABLE {0} DROP CONSTRAINT ' + QUOTENAME(@default);" + Environment.NewLine +
                "EXEC sp_executesql @sql;";
            return string.Format(sql, Quoter.QuoteTableName(expression.TableName, expression.SchemaName), expression.ColumnName);
        }

        /// <summary>
        /// Generates an SQL statement to drop a constraint
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(DeleteConstraintExpression expression)
        {
            var withParts = GetWithOptions(expression);
            var withPart = !string.IsNullOrEmpty(withParts)
                ? $" WITH ({withParts})"
                : string.Empty;

            return $"{base.Generate(expression)}{withPart}";
        }

        /// <summary>
        /// Generates a <c>CREATE SCHEMA</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(CreateSchemaExpression expression)
        {
            string authFragment;
            if (expression.AdditionalFeatures.TryGetValue(SqlServerExtensions.SchemaAuthorization, out var authorization))
            {
                authFragment = $" AUTHORIZATION {Quoter.QuoteSchemaName((string) authorization)}";
            }
            else
            {
                authFragment = string.Empty;
            }

            return string.Format(CreateSchema, Quoter.QuoteSchemaName(expression.SchemaName), authFragment);
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
        /// Generates an SQL statement to move a table from one schema to another
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(AlterSchemaExpression expression)
        {
            return string.Format(AlterSchema, Quoter.QuoteSchemaName(expression.DestinationSchemaName), Quoter.QuoteTableName(expression.TableName, expression.SourceSchemaName));
        }

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
