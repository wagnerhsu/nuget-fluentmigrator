// ***********************************************************************
// Assembly         : FluentMigrator.Runner.SqlServer
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SqlServer2000Generator.cs" company="FluentMigrator Project">
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
using FluentMigrator.Runner.Generators.Generic;
using FluentMigrator.SqlServer;

using JetBrains.Annotations;

using Microsoft.Extensions.Options;

namespace FluentMigrator.Runner.Generators.SqlServer
{
    /// <summary>
    /// Class SqlServer2000Generator.
    /// Implements the <see cref="FluentMigrator.Runner.Generators.Generic.GenericGenerator" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Generators.Generic.GenericGenerator" />
    public class SqlServer2000Generator : GenericGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServer2000Generator"/> class.
        /// </summary>
        public SqlServer2000Generator()
            : this(new SqlServer2000Quoter())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServer2000Generator"/> class.
        /// </summary>
        /// <param name="quoter">The quoter.</param>
        public SqlServer2000Generator(
            [NotNull] SqlServer2000Quoter quoter)
            : this(quoter, new OptionsWrapper<GeneratorOptions>(new GeneratorOptions()))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServer2000Generator"/> class.
        /// </summary>
        /// <param name="quoter">The quoter.</param>
        /// <param name="generatorOptions">The generator options.</param>
        public SqlServer2000Generator(
            [NotNull] SqlServer2000Quoter quoter,
            [NotNull] IOptions<GeneratorOptions> generatorOptions)
            : this(new SqlServer2000Column(new SqlServer2000TypeMap(), quoter), quoter, new EmptyDescriptionGenerator(), generatorOptions)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServer2000Generator"/> class.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="quoter">The quoter.</param>
        /// <param name="descriptionGenerator">The description generator.</param>
        /// <param name="generatorOptions">The generator options.</param>
        protected SqlServer2000Generator(
            [NotNull] IColumn column,
            [NotNull] IQuoter quoter,
            [NotNull] IDescriptionGenerator descriptionGenerator,
            [NotNull] IOptions<GeneratorOptions> generatorOptions)
            : base(column, quoter, descriptionGenerator, generatorOptions)
        {
        }

        /// <summary>
        /// Gets the rename table.
        /// </summary>
        /// <value>The rename table.</value>
        public override string RenameTable { get { return "sp_rename {0}, {1}"; } }

        /// <summary>
        /// Gets the rename column.
        /// </summary>
        /// <value>The rename column.</value>
        public override string RenameColumn { get { return "sp_rename {0}, {1}"; } }

        /// <summary>
        /// Gets the index of the drop.
        /// </summary>
        /// <value>The index of the drop.</value>
        public override string DropIndex { get { return "DROP INDEX {1}.{0}"; } }

        /// <summary>
        /// Gets the add column.
        /// </summary>
        /// <value>The add column.</value>
        public override string AddColumn { get { return "ALTER TABLE {0} ADD {1}"; } }

        /// <summary>
        /// Gets the identity insert.
        /// </summary>
        /// <value>The identity insert.</value>
        public virtual string IdentityInsert { get { return "SET IDENTITY_INSERT {0} {1}"; } }

        /// <summary>
        /// Gets the create constraint.
        /// </summary>
        /// <value>The create constraint.</value>
        public override string CreateConstraint { get { return "ALTER TABLE {0} ADD CONSTRAINT {1} {2}{3} ({4})"; } }

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
        protected virtual string GetConstraintClusteringString(CreateConstraintExpression constraint)
        {
            object indexType;

            if (!constraint.Constraint.AdditionalFeatures.TryGetValue(
                SqlServerExtensions.ConstraintType, out indexType)) return string.Empty;

            return (indexType.Equals(SqlServerConstraintType.Clustered)) ? " CLUSTERED" : " NONCLUSTERED";
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

            return string.Format(CreateConstraint,
                Quoter.QuoteTableName(expression.Constraint.TableName, expression.Constraint.SchemaName),
                Quoter.Quote(expression.Constraint.ConstraintName),
                constraintType,
                constraintClustering,
                columns);
        }

        /// <summary>
        /// Generates an SQL statement to rename a table
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(RenameTableExpression expression)
        {
            var sourceParam = Quoter.QuoteValue(Quoter.QuoteTableName(expression.OldName, expression.SchemaName));
            var destinationParam = Quoter.QuoteValue(expression.NewName);
            return string.Format(RenameTable, sourceParam, destinationParam);
        }

        /// <summary>
        /// Generates an SQL statement to rename a column
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(RenameColumnExpression expression)
        {
            var tableName = Quoter.QuoteTableName(expression.TableName, expression.SchemaName);
            var columnName = Quoter.QuoteColumnName(expression.OldName);
            var sourceParam = Quoter.QuoteValue($"{tableName}.{columnName}");
            var destinationParam = Quoter.QuoteValue(expression.NewName);
            return string.Format(RenameColumn, sourceParam, destinationParam);
        }

        /// <summary>
        /// Generates a <c>ALTER TABLE DROP COLUMN</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(DeleteColumnExpression expression)
        {
            // before we drop a column, we have to drop any default value constraints in SQL Server
            var builder = new StringBuilder();

            foreach (string column in expression.ColumnNames)
            {
                if (expression.ColumnNames.First() != column) builder.AppendLine("GO");
                BuildDelete(expression, column, builder);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Builds the delete.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="builder">The builder.</param>
        protected virtual void BuildDelete(DeleteColumnExpression expression, string columnName, StringBuilder builder)
        {
            builder.AppendLine(
                Generate(
                    new DeleteDefaultConstraintExpression
                    {
                        ColumnName = columnName,
                        SchemaName = expression.SchemaName,
                        TableName = expression.TableName
                    }));

            builder.AppendLine();

            builder.AppendLine(string.Format("-- now we can finally drop column" + Environment.NewLine + "ALTER TABLE {0} DROP COLUMN {1};",
                                         Quoter.QuoteTableName(expression.TableName, expression.SchemaName),
                                         Quoter.QuoteColumnName(columnName)));
        }

        /// <summary>
        /// Generates an SQL statement to alter a DEFAULT constraint
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(AlterDefaultConstraintExpression expression)
        {
            // before we alter a default constraint on a column, we have to drop any default value constraints in SQL Server
            var builder = new StringBuilder();

            builder.AppendLine(Generate(new DeleteDefaultConstraintExpression
                                            {
                                                ColumnName = expression.ColumnName,
                                                SchemaName = expression.SchemaName,
                                                TableName = expression.TableName
                                            }));

            builder.AppendLine();

            builder.AppendFormat("-- create alter table command to create new default constraint as string and run it" + Environment.NewLine +"ALTER TABLE {0} WITH NOCHECK ADD CONSTRAINT {3} DEFAULT({2}) FOR {1};",
                Quoter.QuoteTableName(expression.TableName, expression.SchemaName),
                Quoter.QuoteColumnName(expression.ColumnName),
                SqlServer2000Column.FormatDefaultValue(expression.DefaultValue, Quoter),
                Quoter.QuoteConstraintName(SqlServer2000Column.GetDefaultConstraintName(expression.TableName, expression.ColumnName)));

            return builder.ToString();
        }

        /// <summary>
        /// Generates an SQL statement to INSERT data
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(InsertDataExpression expression)
        {
            if (IsUsingIdentityInsert(expression))
            {
                return string.Format("{0}; {1}; {2}",
                            string.Format(IdentityInsert, Quoter.QuoteTableName(expression.TableName, expression.SchemaName), "ON"),
                            base.Generate(expression),
                            string.Format(IdentityInsert, Quoter.QuoteTableName(expression.TableName, expression.SchemaName), "OFF"));
            }
            return base.Generate(expression);
        }

        /// <summary>
        /// Determines whether [is using identity insert] [the specified expression].
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns><c>true</c> if [is using identity insert] [the specified expression]; otherwise, <c>false</c>.</returns>
        protected static bool IsUsingIdentityInsert(InsertDataExpression expression)
        {
            if (expression.AdditionalFeatures.ContainsKey(SqlServerExtensions.IdentityInsert))
            {
                return (bool)expression.AdditionalFeatures[SqlServerExtensions.IdentityInsert];
            }

            return false;
        }

        /// <summary>
        /// Generates a <c>CREATE SEQUENCE</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(CreateSequenceExpression expression)
        {
            return CompatibilityMode.HandleCompatibilty("Sequences are not supported in SqlServer2000");
        }

        /// <summary>
        /// Generates the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>System.String.</returns>
        public override string Generate(DeleteSequenceExpression expression)
        {
            return CompatibilityMode.HandleCompatibilty("Sequences are not supported in SqlServer2000");
        }

        /// <summary>
        /// Generates an SQL statement to drop a default constraint
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(DeleteDefaultConstraintExpression expression)
        {
            string sql =
                "DECLARE @default sysname, @sql nvarchar(4000);" + Environment.NewLine + Environment.NewLine +
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
        /// Determines whether [is additional feature supported] [the specified feature].
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <returns><c>true</c> if [is additional feature supported] [the specified feature]; otherwise, <c>false</c>.</returns>
        public override bool IsAdditionalFeatureSupported(string feature)
        {
            return _supportedAdditionalFeatures.Any(x => x == feature);
        }

        /// <summary>
        /// The supported additional features
        /// </summary>
        private readonly IEnumerable<string> _supportedAdditionalFeatures = new List<string>
        {
            SqlServerExtensions.IdentityInsert,
            SqlServerExtensions.IdentitySeed,
            SqlServerExtensions.IdentityIncrement,
            SqlServerExtensions.ConstraintType
        };
    }
}
