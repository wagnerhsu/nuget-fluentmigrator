// ***********************************************************************
// Assembly         : FluentMigrator.Runner.MySql
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="MySql4Generator.cs" company="FluentMigrator Project">
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
using System.Linq;

using FluentMigrator.Expressions;
using FluentMigrator.Runner.Generators.Generic;

using JetBrains.Annotations;

using Microsoft.Extensions.Options;

namespace FluentMigrator.Runner.Generators.MySql
{
    /// <summary>
    /// Class MySql4Generator.
    /// Implements the <see cref="FluentMigrator.Runner.Generators.Generic.GenericGenerator" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Generators.Generic.GenericGenerator" />
    public class MySql4Generator : GenericGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MySql4Generator"/> class.
        /// </summary>
        public MySql4Generator()
            : this(new MySqlQuoter())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MySql4Generator"/> class.
        /// </summary>
        /// <param name="quoter">The quoter.</param>
        public MySql4Generator(
            [NotNull] MySqlQuoter quoter)
            : this(quoter, new OptionsWrapper<GeneratorOptions>(new GeneratorOptions()))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MySql4Generator"/> class.
        /// </summary>
        /// <param name="quoter">The quoter.</param>
        /// <param name="generatorOptions">The generator options.</param>
        public MySql4Generator(
            [NotNull] MySqlQuoter quoter,
            [NotNull] IOptions<GeneratorOptions> generatorOptions)
            : this(
                new MySqlColumn(new MySql4TypeMap(), quoter),
                quoter,
                new EmptyDescriptionGenerator(),
                generatorOptions)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MySql4Generator"/> class.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="quoter">The quoter.</param>
        /// <param name="descriptionGenerator">The description generator.</param>
        /// <param name="generatorOptions">The generator options.</param>
        protected MySql4Generator(
            [NotNull] IColumn column,
            [NotNull] IQuoter quoter,
            [NotNull] IDescriptionGenerator descriptionGenerator,
            [NotNull] IOptions<GeneratorOptions> generatorOptions)
            : base(column, quoter, descriptionGenerator, generatorOptions)
        {
        }

        /// <summary>
        /// Gets the alter column.
        /// </summary>
        /// <value>The alter column.</value>
        public override string AlterColumn { get { return "ALTER TABLE {0} MODIFY COLUMN {1}"; } }
        /// <summary>
        /// Gets the delete constraint.
        /// </summary>
        /// <value>The delete constraint.</value>
        public override string DeleteConstraint { get { return "ALTER TABLE {0} DROP {1}{2}"; } }
        //public override string DeleteConstraint { get { return "ALTER TABLE {0} DROP FOREIGN KEY {1}"; } }

        /// <summary>
        /// Outputs a create table string
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        /// <exception cref="ArgumentNullException">expression - expression.TableName cannot be empty</exception>
        /// <exception cref="ArgumentException">You must specifiy at least one column</exception>
        public override string Generate(CreateTableExpression expression)
        {
            if (string.IsNullOrEmpty(expression.TableName)) throw new ArgumentNullException(nameof(expression), @"expression.TableName cannot be empty");
            if (expression.Columns.Count == 0) throw new ArgumentException("You must specifiy at least one column");

            string errors = ValidateAdditionalFeatureCompatibility(expression.Columns.SelectMany(x => x.AdditionalFeatures));
            if (!string.IsNullOrEmpty(errors)) return errors;

            string quotedTableName = Quoter.QuoteTableName(expression.TableName);

            string tableDescription = string.IsNullOrEmpty(expression.TableDescription)
                ? string.Empty
                : string.Format(" COMMENT {0}", Quoter.QuoteValue(expression.TableDescription));

            return string.Format("CREATE TABLE {0} ({1}){2} ENGINE = INNODB",
                quotedTableName,
                Column.Generate(expression.Columns, quotedTableName),
                tableDescription);
        }

        /// <summary>
        /// Generates a <c>ALTER TABLE</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(AlterTableExpression expression)
        {
            if (string.IsNullOrEmpty(expression.TableDescription))
                return base.Generate(expression);

            return string.Format("ALTER TABLE {0} COMMENT {1}", Quoter.QuoteTableName(expression.TableName), Quoter.QuoteValue(expression.TableDescription));
        }

        /// <summary>
        /// Generates an SQL statement to drop an index
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(DeleteIndexExpression expression)
        {
            return string.Format("DROP INDEX {0} ON {1}", Quoter.QuoteIndexName(expression.Index.Name), Quoter.QuoteTableName(expression.Index.TableName));
        }

        /// <summary>
        /// Generates an SQL statement to rename a column
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(RenameColumnExpression expression)
        {
            return string.Format("ALTER TABLE {0} CHANGE {1} {2} ", Quoter.QuoteTableName(expression.TableName), Quoter.QuoteColumnName(expression.OldName), Quoter.QuoteColumnName(expression.NewName));
        }

        /// <summary>
        /// Generates an SQL statement to alter a DEFAULT constraint
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(AlterDefaultConstraintExpression expression)
        {
            // Available since MySQL 4.0.22 (2005)
            var defaultValue = ((MySqlColumn)Column).FormatDefaultValue(expression.DefaultValue);
            return string.Format(
                "ALTER TABLE {0} ALTER {1} SET {2}",
                Quoter.QuoteTableName(expression.TableName),
                Quoter.QuoteColumnName(expression.ColumnName),
                defaultValue);
        }

        /// <summary>
        /// Generates a <c>CREATE SEQUENCE</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(CreateSequenceExpression expression)
        {
            return CompatibilityMode.HandleCompatibilty("Sequences is not supporteed for MySql");
        }

        /// <summary>
        /// Generates the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>System.String.</returns>
        public override string Generate(DeleteSequenceExpression expression)
        {
            return CompatibilityMode.HandleCompatibilty("Sequences is not supporteed for MySql");
        }

        /// <summary>
        /// Generates an SQL statement to drop a constraint
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(DeleteConstraintExpression expression)
        {
            if (expression.Constraint.IsPrimaryKeyConstraint)
            {
                return string.Format(DeleteConstraint, Quoter.QuoteTableName(expression.Constraint.TableName), "PRIMARY KEY", "");
            }
            return string.Format(DeleteConstraint, Quoter.QuoteTableName(expression.Constraint.TableName), "INDEX ", Quoter.Quote(expression.Constraint.ConstraintName));
        }

        /// <summary>
        /// Generates an SQL statement to delete a foreign key
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(DeleteForeignKeyExpression expression)
        {
            return string.Format(DeleteConstraint, Quoter.QuoteTableName(expression.ForeignKey.ForeignTable), "FOREIGN KEY ", Quoter.QuoteColumnName(expression.ForeignKey.Name));
        }

        /// <summary>
        /// Generates the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>System.String.</returns>
        public override string Generate(DeleteDefaultConstraintExpression expression)
        {
            // Available since MySQL 4.0.22 (2005)
            return string.Format(
                "ALTER TABLE {0} ALTER {1} DROP DEFAULT",
                Quoter.QuoteTableName(expression.TableName),
                Quoter.QuoteColumnName(expression.ColumnName));
        }
    }
}
