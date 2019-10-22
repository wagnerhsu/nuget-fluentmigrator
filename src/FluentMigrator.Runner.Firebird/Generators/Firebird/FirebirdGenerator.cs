// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Firebird
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="FirebirdGenerator.cs" company="FluentMigrator Project">
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
using System.Linq;
using System.Text;

using FluentMigrator.Expressions;
using FluentMigrator.Model;
using FluentMigrator.Runner.Generators.Generic;
using FluentMigrator.Runner.Processors.Firebird;

using JetBrains.Annotations;

using Microsoft.Extensions.Options;

namespace FluentMigrator.Runner.Generators.Firebird
{

    /// <summary>
    /// Class FirebirdGenerator.
    /// Implements the <see cref="FluentMigrator.Runner.Generators.Generic.GenericGenerator" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Generators.Generic.GenericGenerator" />
    public class FirebirdGenerator : GenericGenerator
    {
        // ReSharper disable once InconsistentNaming
        /// <summary>
        /// The truncator
        /// </summary>
        [Obsolete("Use the Truncator property")]
        protected readonly FirebirdTruncator truncator;

        /// <summary>
        /// Initializes a new instance of the <see cref="FirebirdGenerator"/> class.
        /// </summary>
        /// <param name="fbOptions">The fb options.</param>
        public FirebirdGenerator(
            [NotNull] FirebirdOptions fbOptions)
            : this(fbOptions, new OptionsWrapper<GeneratorOptions>(new GeneratorOptions()))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FirebirdGenerator"/> class.
        /// </summary>
        /// <param name="fbOptions">The fb options.</param>
        /// <param name="generatorOptions">The generator options.</param>
        public FirebirdGenerator(
            [NotNull] FirebirdOptions fbOptions,
            [NotNull] IOptions<GeneratorOptions> generatorOptions)
            : this(new FirebirdQuoter(fbOptions.ForceQuote), fbOptions, generatorOptions)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FirebirdGenerator"/> class.
        /// </summary>
        /// <param name="quoter">The quoter.</param>
        /// <param name="fbOptions">The fb options.</param>
        /// <param name="generatorOptions">The generator options.</param>
        /// <exception cref="ArgumentNullException">fbOptions</exception>
        public FirebirdGenerator(
            [NotNull] FirebirdQuoter quoter,
            [NotNull] FirebirdOptions fbOptions,
            [NotNull] IOptions<GeneratorOptions> generatorOptions)
            : base(new FirebirdColumn(fbOptions), quoter, new EmptyDescriptionGenerator(), generatorOptions)
        {
            FBOptions = fbOptions ?? throw new ArgumentNullException(nameof(fbOptions));
#pragma warning disable 618
            truncator = new FirebirdTruncator(FBOptions.TruncateLongNames, FBOptions.PackKeyNames);
#pragma warning restore 618
        }

        //It's kind of a hack to mess with system tables, but this is the cleanest and time-tested method to alter the nullable constraint.
        // It's even mentioned in the firebird official FAQ.
        // Currently the only drawback is that the integrity is not checked by the engine, you have to ensure it manually
        /// <summary>
        /// Gets the alter column set nullable pre3.
        /// </summary>
        /// <value>The alter column set nullable pre3.</value>
        public string AlterColumnSetNullablePre3 { get { return "UPDATE RDB$RELATION_FIELDS SET RDB$NULL_FLAG = {0} WHERE lower(rdb$relation_name) = lower({1}) AND lower(RDB$FIELD_NAME) = lower({2})"; } }

        /// <summary>
        /// ALTER TABLE table_name ALTER column_name { DROP | SET } [NOT] NULL
        /// </summary>
        /// <value>The alter column set nullable3.</value>
        public string AlterColumnSetNullable3 { get { return "ALTER TABLE {0} ALTER {1} {2} {3}"; } }

        /// <summary>
        /// Gets the type of the alter column set.
        /// </summary>
        /// <value>The type of the alter column set.</value>
        public string AlterColumnSetType { get { return "ALTER TABLE {0} ALTER COLUMN {1} TYPE {2}"; } }

        /// <summary>
        /// Gets the add column.
        /// </summary>
        /// <value>The add column.</value>
        public override string AddColumn { get { return "ALTER TABLE {0} ADD {1}"; } }
        /// <summary>
        /// Gets the drop column.
        /// </summary>
        /// <value>The drop column.</value>
        public override string DropColumn { get { return "ALTER TABLE {0} DROP {1}"; } }
        /// <summary>
        /// Gets the rename column.
        /// </summary>
        /// <value>The rename column.</value>
        public override string RenameColumn { get { return "ALTER TABLE {0} ALTER COLUMN {1} TO {2}"; } }

        /// <summary>
        /// Gets the fb options.
        /// </summary>
        /// <value>The fb options.</value>
        protected FirebirdOptions FBOptions { get; }

#pragma warning disable 618
        /// <summary>
        /// Gets the truncator.
        /// </summary>
        /// <value>The truncator.</value>
        public FirebirdTruncator Truncator => truncator;
#pragma warning restore 618

        /// <summary>
        /// Generates an SQL statement to alter a DEFAULT constraint
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(AlterDefaultConstraintExpression expression)
        {
            Truncator.Truncate(expression);
            return string.Format("ALTER TABLE {0} ALTER COLUMN {1} SET DEFAULT {2}",
                Quoter.QuoteTableName(expression.TableName),
                Quoter.QuoteColumnName(expression.ColumnName),
                Quoter.QuoteValue(expression.DefaultValue)
                );
        }

        /// <summary>
        /// Generates an SQL statement to drop a default constraint
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(DeleteDefaultConstraintExpression expression)
        {
            Truncator.Truncate(expression);
            return string.Format("ALTER TABLE {0} ALTER COLUMN {1} DROP DEFAULT",
                Quoter.QuoteTableName(expression.TableName),
                Quoter.QuoteColumnName(expression.ColumnName)
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
            //Firebird doesn't have particular asc or desc order per column, only per the whole index
            // CREATE [UNIQUE] [ASC[ENDING] | [DESC[ENDING]] INDEX indexname
            //  ON tablename  { (<col> [, <col> ...]) | COMPUTED BY (expression) }
            //  <col>  ::=  a column not of type ARRAY, BLOB or COMPUTED BY
            //
            // Assuming the first column's direction for the index's direction.

            Truncator.Truncate(expression);

            StringBuilder indexColumns = new StringBuilder("");
            Direction indexDirection = Direction.Ascending;
            int columnCount = expression.Index.Columns.Count;
            for (int i = 0; i < columnCount; i++)
            {
                IndexColumnDefinition columnDef = expression.Index.Columns.ElementAt(i);

                if (i > 0)
                    indexColumns.Append(", ");
                else indexDirection = columnDef.Direction;

                indexColumns.Append(Quoter.QuoteColumnName(columnDef.Name));

            }

            return string.Format(CreateIndex
                , GetUniqueString(expression)
                , indexDirection == Direction.Ascending ? "ASC " : "DESC "
                , Quoter.QuoteIndexName(expression.Index.Name)
                , Quoter.QuoteTableName(expression.Index.TableName)
                , indexColumns);
        }

        /// <summary>
        /// Generates a <c>ALTER TABLE ALTER COLUMN</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(AlterColumnExpression expression)
        {
            Truncator.Truncate(expression);
            return CompatibilityMode.HandleCompatibilty("Alter column is not supported as expected");
        }


        /// <summary>
        /// Generates a <c>CREATE SEQUENCE</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(CreateSequenceExpression expression)
        {
            Truncator.Truncate(expression);
            return string.Format("CREATE SEQUENCE {0}", Quoter.QuoteSequenceName(expression.Sequence.Name));
        }

        /// <summary>
        /// Generates the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>System.String.</returns>
        public override string Generate(DeleteSequenceExpression expression)
        {
            Truncator.Truncate(expression);
            return string.Format("DROP SEQUENCE {0}", Quoter.QuoteSequenceName(expression.SequenceName));
        }

        /// <summary>
        /// Generates the alter sequence.
        /// </summary>
        /// <param name="sequence">The sequence.</param>
        /// <returns>System.String.</returns>
        public string GenerateAlterSequence(SequenceDefinition sequence)
        {
            Truncator.Truncate(sequence);
            if (sequence.StartWith != null)
                return string.Format("ALTER SEQUENCE {0} RESTART WITH {1}", Quoter.QuoteSequenceName(sequence.Name), sequence.StartWith.ToString());

            return string.Empty;
        }

        /// <summary>
        /// Outputs a create table string
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(CreateTableExpression expression)
        {
            Truncator.Truncate(expression);
            return base.Generate(expression);
        }

        /// <summary>
        /// Generates a <c>DROP TABLE</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(DeleteTableExpression expression)
        {
            Truncator.Truncate(expression);
            return base.Generate(expression);
        }

        /// <summary>
        /// Generates an SQL statement to rename a table
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(RenameTableExpression expression)
        {
            Truncator.Truncate(expression);
            return CompatibilityMode.HandleCompatibilty("Rename table is not supported");
        }

        /// <summary>
        /// Generates a <c>ALTER TABLE ADD COLUMN</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(CreateColumnExpression expression)
        {
            Truncator.Truncate(expression);
            return base.Generate(expression);
        }

        /// <summary>
        /// Generates a <c>ALTER TABLE DROP COLUMN</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(DeleteColumnExpression expression)
        {
            Truncator.Truncate(expression);
            return base.Generate(expression);
        }

        /// <summary>
        /// Generates an SQL statement to rename a column
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(RenameColumnExpression expression)
        {
            Truncator.Truncate(expression);
            return base.Generate(expression);
        }

        /// <summary>
        /// Generates an SQL statement to drop an index
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(DeleteIndexExpression expression)
        {
            Truncator.Truncate(expression);
            return base.Generate(expression);
        }

        /// <summary>
        /// Generates an SQL statement to create a constraint
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(CreateConstraintExpression expression)
        {
            Truncator.Truncate(expression);
            return base.Generate(expression);
        }

        /// <summary>
        /// Generates an SQL statement to drop a constraint
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(DeleteConstraintExpression expression)
        {
            Truncator.Truncate(expression);
            return base.Generate(expression);
        }

        /// <summary>
        /// Generates an SQL statement to create a foreign key
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(CreateForeignKeyExpression expression)
        {
            Truncator.Truncate(expression);
            return base.Generate(expression);
        }

        /// <summary>
        /// Generates the name of the foreign key.
        /// </summary>
        /// <param name="foreignKey">The foreign key.</param>
        /// <returns>System.String.</returns>
        public override string GenerateForeignKeyName(ForeignKeyDefinition foreignKey)
        {
            Truncator.Truncate(foreignKey);
            return Truncator.Truncate(base.GenerateForeignKeyName(foreignKey));
        }

        /// <summary>
        /// Generates an SQL statement to delete a foreign key
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(DeleteForeignKeyExpression expression)
        {
            Truncator.Truncate(expression);
            return base.Generate(expression);
        }

        /// <summary>
        /// Generates an SQL statement to INSERT data
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(InsertDataExpression expression)
        {
            Truncator.Truncate(expression);
            return base.Generate(expression);
        }

        /// <summary>
        /// Generates an SQL statement to UPDATE data
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(UpdateDataExpression expression)
        {
            Truncator.Truncate(expression);
            return base.Generate(expression);
        }

        /// <summary>
        /// Generates an SQL statement to DELETE data
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(DeleteDataExpression expression)
        {
            Truncator.Truncate(expression);
            return base.Generate(expression);
        }

        /// <summary>
        /// Generates the set null pre3.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="column">The column.</param>
        /// <returns>System.String.</returns>
        public virtual string GenerateSetNullPre3(string tableName, ColumnDefinition column)
        {
            Truncator.Truncate(column);
            return string.Format(AlterColumnSetNullablePre3,
                !column.IsNullable.HasValue || !column.IsNullable.Value  ? "1" : "NULL",
                Quoter.QuoteValue(tableName),
                Quoter.QuoteValue(column.Name)
                );
        }

        /// <summary>
        /// Generates the set null3.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="column">The column.</param>
        /// <returns>System.String.</returns>
        public virtual string GenerateSetNull3(string tableName, ColumnDefinition column)
        {
            Truncator.Truncate(column);
            var dropSet = !column.IsNullable.HasValue ? "DROP" : "SET";
            var nullable = column.IsNullable.GetValueOrDefault() ? "NULL" : "NOT NULL";
            return string.Format(AlterColumnSetNullable3,
                Quoter.QuoteTableName(tableName),
                Quoter.QuoteColumnName(column.Name),
                dropSet,
                nullable
            );
        }

        /// <summary>
        /// Generates the type of the set.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="column">The column.</param>
        /// <returns>System.String.</returns>
        public virtual string GenerateSetType(string tableName, ColumnDefinition column)
        {
            Truncator.Truncate(column);
            return string.Format(AlterColumnSetType,
                Quoter.QuoteTableName(tableName),
                Quoter.QuoteColumnName(column.Name),
                ((FirebirdColumn) Column).GenerateForTypeAlter(column)
                );
        }

        /// <summary>
        /// Columns the types match.
        /// </summary>
        /// <param name="col1">The col1.</param>
        /// <param name="col2">The col2.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool ColumnTypesMatch(ColumnDefinition col1, ColumnDefinition col2)
        {
            FirebirdColumn column = new FirebirdColumn(new FirebirdOptions());
            string colDef1 = column.GenerateForTypeAlter(col1);
            string colDef2 = column.GenerateForTypeAlter(col2);
            return colDef1 == colDef2;
        }

        /// <summary>
        /// Defaults the values match.
        /// </summary>
        /// <param name="col1">The col1.</param>
        /// <param name="col2">The col2.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool DefaultValuesMatch(ColumnDefinition col1, ColumnDefinition col2)
        {
            if (col1.DefaultValue is ColumnDefinition.UndefinedDefaultValue && col2.DefaultValue is ColumnDefinition.UndefinedDefaultValue)
                return true;
            if (col1.DefaultValue is ColumnDefinition.UndefinedDefaultValue || col2.DefaultValue is ColumnDefinition.UndefinedDefaultValue)
                return true;
            FirebirdColumn column = new FirebirdColumn(new FirebirdOptions());
            string col1Value = column.GenerateForDefaultAlter(col1);
            string col2Value = column.GenerateForDefaultAlter(col2);
            return col1Value != col2Value;
        }
    }
}
