// ***********************************************************************
// Assembly         : FluentMigrator.Runner.SqlServerCe
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SqlServerCeGenerator.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
//
// Copyright (c) 2007-2018, Sean Chambers <schambers80@gmail.com>
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

using System.Linq;
using FluentMigrator.Exceptions;
using FluentMigrator.Expressions;

using JetBrains.Annotations;

using Microsoft.Extensions.Options;

namespace FluentMigrator.Runner.Generators.SqlServer
{
    /// <summary>
    /// Class SqlServerCeGenerator.
    /// Implements the <see cref="FluentMigrator.Runner.Generators.SqlServer.SqlServer2000Generator" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Generators.SqlServer.SqlServer2000Generator" />
    public class SqlServerCeGenerator : SqlServer2000Generator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerCeGenerator"/> class.
        /// </summary>
        public SqlServerCeGenerator()
            : this(new SqlServer2000Quoter())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerCeGenerator"/> class.
        /// </summary>
        /// <param name="quoter">The quoter.</param>
        public SqlServerCeGenerator(SqlServer2000Quoter quoter)
            : this(quoter, new OptionsWrapper<GeneratorOptions>(new GeneratorOptions()))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerCeGenerator"/> class.
        /// </summary>
        /// <param name="quoter">The quoter.</param>
        /// <param name="generatorOptions">The generator options.</param>
        public SqlServerCeGenerator(
            [NotNull] SqlServer2000Quoter quoter,
            [NotNull] IOptions<GeneratorOptions> generatorOptions)
            : base(new SqlServerCeColumn(new SqlServerCeTypeMap(), quoter), quoter, new EmptyDescriptionGenerator(), generatorOptions)
        {
        }

        /// <summary>
        /// Gets the insert data.
        /// </summary>
        /// <value>The insert data.</value>
        public override string InsertData
        {
            get { return "INSERT INTO {0} ({1}) {2}"; }
        }

        /// <summary>
        /// Gets the cluster type string.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns>System.String.</returns>
        public override string GetClusterTypeString(CreateIndexExpression column)
        {
            // Only nonclusterd
            return string.Empty;
        }

        /// <summary>
        /// Gets the constraint clustering string.
        /// </summary>
        /// <param name="constraint">The constraint.</param>
        /// <returns>System.String.</returns>
        protected override string GetConstraintClusteringString(CreateConstraintExpression constraint)
        {
            // Only nonclustered
            return string.Empty;
        }

        /// <summary>
        /// Generates an SQL statement to rename a table
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(RenameTableExpression expression)
        {
            return string.Format("sp_rename {0}, {1}", Quoter.QuoteValue(expression.OldName), Quoter.QuoteValue(expression.NewName));
        }

        /// <summary>
        /// Generates an SQL statement to rename a column
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        /// <exception cref="DatabaseOperationNotSupportedException"></exception>
        public override string Generate(RenameColumnExpression expression)
        {
            throw new DatabaseOperationNotSupportedException();
        }

        //All Schema method throw by default as only Sql server 2005 and up supports them.
        /// <summary>
        /// Generates a <c>CREATE SCHEMA</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        /// <exception cref="DatabaseOperationNotSupportedException"></exception>
        public override string Generate(CreateSchemaExpression expression)
        {
            throw new DatabaseOperationNotSupportedException();
        }

        /// <summary>
        /// Generates a <c>DROP SCHEMA</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        /// <exception cref="DatabaseOperationNotSupportedException"></exception>
        public override string Generate(DeleteSchemaExpression expression)
        {
            throw new DatabaseOperationNotSupportedException();
        }

        /// <summary>
        /// Generates an SQL statement to move a table from one schema to another
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        /// <exception cref="DatabaseOperationNotSupportedException"></exception>
        public override string Generate(AlterSchemaExpression expression)
        {
            throw new DatabaseOperationNotSupportedException();
        }

        /// <summary>
        /// Generates a <c>ALTER TABLE DROP COLUMN</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        /// <exception cref="DatabaseOperationNotSupportedException"></exception>
        public override string Generate(DeleteColumnExpression expression)
        {
            if (expression.ColumnNames.Count != 1)
            {
                throw new DatabaseOperationNotSupportedException();
            }

            // Limited functionality in CE, for now will just drop the column.. no DECLARE support!
            const string sql = @"ALTER TABLE {0} DROP COLUMN {1};";
            return string.Format(sql, Quoter.QuoteTableName(expression.TableName), Quoter.QuoteColumnName(expression.ColumnNames.ElementAt(0)));
        }

        /// <summary>
        /// Generates an SQL statement to drop an index
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(DeleteIndexExpression expression)
        {
            return string.Format("DROP INDEX {0}.{1}", Quoter.QuoteTableName(expression.Index.TableName), Quoter.QuoteIndexName(expression.Index.Name));
        }

        /// <summary>
        /// Generates an SQL statement to alter a DEFAULT constraint
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        /// <exception cref="DatabaseOperationNotSupportedException"></exception>
        public override string Generate(AlterDefaultConstraintExpression expression)
        {
            throw new DatabaseOperationNotSupportedException();
        }

        /// <summary>
        /// Generates an SQL statement to drop a default constraint
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        /// <exception cref="DatabaseOperationNotSupportedException"></exception>
        public override string Generate(DeleteDefaultConstraintExpression expression)
        {
            throw new DatabaseOperationNotSupportedException();
        }

        /// <summary>
        /// Generates the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>System.String.</returns>
        public override string Generate(InsertDataExpression expression)
        {
            var errors = ValidateAdditionalFeatureCompatibility(expression.AdditionalFeatures);
            if (!string.IsNullOrEmpty(errors)) return errors;

            var columnNamesValues = GenerateColumnNamesAndValues(expression);
            var selectStrings = columnNamesValues.Select(kv => "SELECT " + kv.Value);

            var sql = string.Format(InsertData, Quoter.QuoteTableName(expression.TableName), columnNamesValues.FirstOrDefault().Key, string.Join(" UNION ALL ", selectStrings.ToArray()));

            if (IsUsingIdentityInsert(expression))
            {
                return string.Format("{0}; {1}; {2}",
                            string.Format(IdentityInsert, Quoter.QuoteTableName(expression.TableName), "ON"),
                            sql,
                            string.Format(IdentityInsert, Quoter.QuoteTableName(expression.TableName), "OFF"));
            }
            return sql;
        }
    }
}
