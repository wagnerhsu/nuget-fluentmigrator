// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Core
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="GeneratorBase.cs" company="FluentMigrator Project">
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

using FluentMigrator.Expressions;

namespace FluentMigrator.Runner.Generators.Base
{
    /// <summary>
    /// Class GeneratorBase.
    /// Implements the <see cref="FluentMigrator.IMigrationGenerator" />
    /// </summary>
    /// <seealso cref="FluentMigrator.IMigrationGenerator" />
    public abstract class GeneratorBase : IMigrationGenerator
    {
        /// <summary>
        /// The column
        /// </summary>
        private readonly IColumn _column;
        /// <summary>
        /// The quoter
        /// </summary>
        private readonly IQuoter _quoter;
        /// <summary>
        /// The description generator
        /// </summary>
        private readonly IDescriptionGenerator _descriptionGenerator;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratorBase"/> class.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="quoter">The quoter.</param>
        /// <param name="descriptionGenerator">The description generator.</param>
        public GeneratorBase(IColumn column, IQuoter quoter, IDescriptionGenerator descriptionGenerator)
        {
            _column = column;
            _quoter = quoter;
            _descriptionGenerator = descriptionGenerator;
        }

        /// <summary>
        /// Generates a <c>CREATE SCHEMA</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public abstract string Generate(CreateSchemaExpression expression);
        /// <summary>
        /// Generates a <c>DROP SCHEMA</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public abstract string Generate(DeleteSchemaExpression expression);
        /// <summary>
        /// Generates a <c>CREATE TABLE</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public abstract string Generate(CreateTableExpression expression);
        /// <summary>
        /// Generates a <c>ALTER TABLE ALTER COLUMN</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public abstract string Generate(AlterColumnExpression expression);
        /// <summary>
        /// Generates a <c>ALTER TABLE ADD COLUMN</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public abstract string Generate(CreateColumnExpression expression);
        /// <summary>
        /// Generates a <c>DROP TABLE</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public abstract string Generate(DeleteTableExpression expression);
        /// <summary>
        /// Generates a <c>ALTER TABLE DROP COLUMN</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public abstract string Generate(DeleteColumnExpression expression);
        /// <summary>
        /// Generates an SQL statement to create a foreign key
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public abstract string Generate(CreateForeignKeyExpression expression);
        /// <summary>
        /// Generates an SQL statement to delete a foreign key
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public abstract string Generate(DeleteForeignKeyExpression expression);
        /// <summary>
        /// Generates an SQL statement to create an index
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public abstract string Generate(CreateIndexExpression expression);
        /// <summary>
        /// Generates an SQL statement to drop an index
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public abstract string Generate(DeleteIndexExpression expression);
        /// <summary>
        /// Generates an SQL statement to rename a table
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public abstract string Generate(RenameTableExpression expression);
        /// <summary>
        /// Generates an SQL statement to rename a column
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public abstract string Generate(RenameColumnExpression expression);
        /// <summary>
        /// Generates an SQL statement to INSERT data
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public abstract string Generate(InsertDataExpression expression);
        /// <summary>
        /// Generates an SQL statement to alter a DEFAULT constraint
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public abstract string Generate(AlterDefaultConstraintExpression expression);
        /// <summary>
        /// Generates an SQL statement to DELETE data
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public abstract string Generate(DeleteDataExpression expression);
        /// <summary>
        /// Generates an SQL statement to UPDATE data
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public abstract string Generate(UpdateDataExpression expression);
        /// <summary>
        /// Generates an SQL statement to move a table from one schema to another
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public abstract string Generate(AlterSchemaExpression expression);
        /// <summary>
        /// Generates a <c>CREATE SEQUENCE</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public abstract string Generate(CreateSequenceExpression expression);
        /// <summary>
        /// Generates a <c>DROP SEQUENCE</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public abstract string Generate(DeleteSequenceExpression expression);
        /// <summary>
        /// Generates an SQL statement to create a constraint
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public abstract string Generate(CreateConstraintExpression expression);
        /// <summary>
        /// Generates an SQL statement to drop a constraint
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public abstract string Generate(DeleteConstraintExpression expression);
        /// <summary>
        /// Generates an SQL statement to drop a default constraint
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public abstract string Generate(DeleteDefaultConstraintExpression expression);

        /// <summary>
        /// Determines whether [is additional feature supported] [the specified feature].
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <returns><c>true</c> if [is additional feature supported] [the specified feature]; otherwise, <c>false</c>.</returns>
        public virtual bool IsAdditionalFeatureSupported(string feature)
        {
            return false;
        }

        /// <summary>
        /// Generates a <c>ALTER TABLE</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public virtual string Generate(AlterTableExpression expression)
        {
            // returns nothing because the individual AddColumn and AlterColumn calls
            //  create CreateColumnExpression and AlterColumnExpression respectively
            return string.Empty;
        }

        /// <summary>
        /// Gets the column.
        /// </summary>
        /// <value>The column.</value>
        protected IColumn Column
        {
            get { return _column; }
        }

        /// <summary>
        /// Gets the quoter.
        /// </summary>
        /// <value>The quoter.</value>
        public IQuoter Quoter
        {
            get { return _quoter; }
        }

        /// <summary>
        /// Gets the description generator.
        /// </summary>
        /// <value>The description generator.</value>
        protected IDescriptionGenerator DescriptionGenerator
        {
            get { return _descriptionGenerator; }
        }
    }
}
