// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Core
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="GenericGenerator.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using FluentMigrator.Model;
using FluentMigrator.Runner.Generators.Base;

using Microsoft.Extensions.Options;

namespace FluentMigrator.Runner.Generators.Generic
{
    /// <summary>
    /// Class GenericGenerator.
    /// Implements the <see cref="FluentMigrator.Runner.Generators.Base.GeneratorBase" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Generators.Base.GeneratorBase" />
    public abstract class GenericGenerator : GeneratorBase
    {
        /// <summary>
        /// The compatability mode
        /// </summary>
        [Obsolete("Use the CompatibilityMode property")]
        // ReSharper disable once InconsistentNaming
#pragma warning disable 618
        public CompatabilityMode compatabilityMode;
#pragma warning restore 618

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericGenerator"/> class.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="quoter">The quoter.</param>
        /// <param name="descriptionGenerator">The description generator.</param>
        /// <param name="generatorOptions">The generator options.</param>
        protected GenericGenerator(
            IColumn column,
            IQuoter quoter,
            IDescriptionGenerator descriptionGenerator,
            IOptions<GeneratorOptions> generatorOptions)
            : base(column, quoter, descriptionGenerator)
        {
            CompatibilityMode = generatorOptions.Value.CompatibilityMode ?? CompatibilityMode.LOOSE;
        }

#pragma warning disable 618, 3005
        /// <summary>
        /// Gets or sets the compatibility mode.
        /// </summary>
        /// <value>The compatibility mode.</value>
        public CompatibilityMode CompatibilityMode
        {
            get => (CompatibilityMode) compatabilityMode;
            set => compatabilityMode = (CompatabilityMode) value;
        }
#pragma warning restore 618, 3005

        /// <summary>
        /// Gets the create table.
        /// </summary>
        /// <value>The create table.</value>
        public virtual string CreateTable { get { return "CREATE TABLE {0} ({1})"; } }
        /// <summary>
        /// Gets the drop table.
        /// </summary>
        /// <value>The drop table.</value>
        public virtual string DropTable { get { return "DROP TABLE {0}"; } }

        /// <summary>
        /// Gets the add column.
        /// </summary>
        /// <value>The add column.</value>
        public virtual string AddColumn { get { return "ALTER TABLE {0} ADD COLUMN {1}"; } }
        /// <summary>
        /// Gets the drop column.
        /// </summary>
        /// <value>The drop column.</value>
        public virtual string DropColumn { get { return "ALTER TABLE {0} DROP COLUMN {1}"; } }
        /// <summary>
        /// Gets the alter column.
        /// </summary>
        /// <value>The alter column.</value>
        public virtual string AlterColumn { get { return "ALTER TABLE {0} ALTER COLUMN {1}"; } }
        /// <summary>
        /// Gets the rename column.
        /// </summary>
        /// <value>The rename column.</value>
        public virtual string RenameColumn { get { return "ALTER TABLE {0} RENAME COLUMN {1} TO {2}"; } }

        /// <summary>
        /// Gets the rename table.
        /// </summary>
        /// <value>The rename table.</value>
        public virtual string RenameTable { get { return "RENAME TABLE {0} TO {1}"; } }

        /// <summary>
        /// Gets the create schema.
        /// </summary>
        /// <value>The create schema.</value>
        public virtual string CreateSchema { get { return "CREATE SCHEMA {0}"; } }
        /// <summary>
        /// Gets the alter schema.
        /// </summary>
        /// <value>The alter schema.</value>
        public virtual string AlterSchema { get { return "ALTER SCHEMA {0} TRANSFER {1}"; } }
        /// <summary>
        /// Gets the drop schema.
        /// </summary>
        /// <value>The drop schema.</value>
        public virtual string DropSchema { get { return "DROP SCHEMA {0}"; } }

        /// <summary>
        /// Gets the index of the create.
        /// </summary>
        /// <value>The index of the create.</value>
        public virtual string CreateIndex { get { return "CREATE {0}{1}INDEX {2} ON {3} ({4})"; } }
        /// <summary>
        /// Gets the index of the drop.
        /// </summary>
        /// <value>The index of the drop.</value>
        public virtual string DropIndex { get { return "DROP INDEX {0}"; } }

        /// <summary>
        /// Gets the insert data.
        /// </summary>
        /// <value>The insert data.</value>
        public virtual string InsertData { get { return "INSERT INTO {0} ({1}) VALUES ({2})"; } }
        /// <summary>
        /// Gets the update data.
        /// </summary>
        /// <value>The update data.</value>
        public virtual string UpdateData { get { return "UPDATE {0} SET {1} WHERE {2}"; } }
        /// <summary>
        /// Gets the delete data.
        /// </summary>
        /// <value>The delete data.</value>
        public virtual string DeleteData { get { return "DELETE FROM {0} WHERE {1}"; } }

        /// <summary>
        /// Gets the create constraint.
        /// </summary>
        /// <value>The create constraint.</value>
        public virtual string CreateConstraint { get { return "ALTER TABLE {0} ADD CONSTRAINT {1} {2} ({3})"; } }
        /// <summary>
        /// Gets the delete constraint.
        /// </summary>
        /// <value>The delete constraint.</value>
        public virtual string DeleteConstraint { get { return "ALTER TABLE {0} DROP CONSTRAINT {1}"; } }
        /// <summary>
        /// Gets the create foreign key constraint.
        /// </summary>
        /// <value>The create foreign key constraint.</value>
        public virtual string CreateForeignKeyConstraint { get { return "ALTER TABLE {0} ADD {1}"; } }

        /// <summary>
        /// Gets the unique string.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns>System.String.</returns>
        public virtual string GetUniqueString(CreateIndexExpression column)
        {
            return column.Index.IsUnique ? "UNIQUE " : string.Empty;
        }

        /// <summary>
        /// Gets the cluster type string.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns>System.String.</returns>
        public virtual string GetClusterTypeString(CreateIndexExpression column)
        {
            return string.Empty;
        }

        /// <summary>
        /// Outputs a create table string
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        /// <exception cref="ArgumentNullException">expression</exception>
        /// <exception cref="ArgumentException">You must specifiy at least one column</exception>
        public override string Generate(CreateTableExpression expression)
        {
            if (string.IsNullOrEmpty(expression.TableName))
            {
                throw new ArgumentNullException(nameof(expression), ErrorMessages.ExpressionTableNameMissing);
            }

            if (expression.Columns.Count == 0)
            {
                throw new ArgumentException("You must specifiy at least one column");
            }

            var quotedTableName = Quoter.QuoteTableName(expression.TableName, expression.SchemaName);

            var errors = ValidateAdditionalFeatureCompatibility(expression.Columns.SelectMany(x => x.AdditionalFeatures));
            if (!string.IsNullOrEmpty(errors))
            {
                return errors;
            }

            return string.Format(CreateTable, quotedTableName, Column.Generate(expression.Columns, quotedTableName));
        }

        /// <summary>
        /// Generates a <c>DROP TABLE</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(DeleteTableExpression expression)
        {
            return string.Format(DropTable, Quoter.QuoteTableName(expression.TableName, expression.SchemaName));
        }

        /// <summary>
        /// Generates an SQL statement to rename a table
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(RenameTableExpression expression)
        {
            return string.Format(RenameTable, Quoter.QuoteTableName(expression.OldName, expression.SchemaName), Quoter.Quote(expression.NewName));
        }

        /// <summary>
        /// Generates a <c>ALTER TABLE ADD COLUMN</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(CreateColumnExpression expression)
        {
            var errors = ValidateAdditionalFeatureCompatibility(expression.Column.AdditionalFeatures);
            if (!string.IsNullOrEmpty(errors))
            {
                return errors;
            }

            return string.Format(AddColumn, Quoter.QuoteTableName(expression.TableName, expression.SchemaName), Column.Generate(expression.Column));
        }

        /// <summary>
        /// Generates a <c>ALTER TABLE ALTER COLUMN</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(AlterColumnExpression expression)
        {
            var errors = ValidateAdditionalFeatureCompatibility(expression.Column.AdditionalFeatures);
            if (!string.IsNullOrEmpty(errors))
            {
                return errors;
            }

            return string.Format(AlterColumn, Quoter.QuoteTableName(expression.TableName, expression.SchemaName), Column.Generate(expression.Column));
        }

        /// <summary>
        /// Generates a <c>ALTER TABLE DROP COLUMN</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(DeleteColumnExpression expression)
        {
            var builder = new StringBuilder();
            foreach (var columnName in expression.ColumnNames)
            {
                if (expression.ColumnNames.First() != columnName)
                {
                    AppendSqlStatementEndToken(builder);
                }

                builder.AppendFormat(DropColumn, Quoter.QuoteTableName(expression.TableName, expression.SchemaName), Quoter.QuoteColumnName(columnName));
            }
            return builder.ToString();
        }

        /// <summary>
        /// Generates an SQL statement to rename a column
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(RenameColumnExpression expression)
        {
            return string.Format(RenameColumn,
                Quoter.QuoteTableName(expression.TableName, expression.SchemaName),
                Quoter.QuoteColumnName(expression.OldName),
                Quoter.QuoteColumnName(expression.NewName)
                );
        }

        /// <inheritdoc />
        public override string Generate(CreateIndexExpression expression)
        {
            var indexColumns = new string[expression.Index.Columns.Count];
            IndexColumnDefinition columnDef;

            for (var i = 0; i < expression.Index.Columns.Count; i++)
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

            return string.Format(CreateIndex
                , GetUniqueString(expression)
                , GetClusterTypeString(expression)
                , Quoter.QuoteIndexName(expression.Index.Name)
                , Quoter.QuoteTableName(expression.Index.TableName, expression.Index.SchemaName)
                , string.Join(", ", indexColumns));
        }

        /// <summary>
        /// Generates an SQL statement to drop an index
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(DeleteIndexExpression expression)
        {
            return string.Format(DropIndex, Quoter.QuoteIndexName(expression.Index.Name), Quoter.QuoteTableName(expression.Index.TableName, expression.Index.SchemaName));
        }

        /// <summary>
        /// Generates an SQL statement to create a foreign key
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(CreateForeignKeyExpression expression)
        {
            return string.Format(
                CreateForeignKeyConstraint,
                Quoter.QuoteTableName(expression.ForeignKey.ForeignTable, expression.ForeignKey.ForeignTableSchema),
                Column.FormatForeignKey(expression.ForeignKey, GenerateForeignKeyName));
        }

        /// <summary>
        /// Generates an SQL statement to create a constraint
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(CreateConstraintExpression expression)
        {
            var constraintType = (expression.Constraint.IsPrimaryKeyConstraint) ? "PRIMARY KEY" : "UNIQUE";

            var columns = new string[expression.Constraint.Columns.Count];

            for (var i = 0; i < expression.Constraint.Columns.Count; i++)
            {
                columns[i] = Quoter.QuoteColumnName(expression.Constraint.Columns.ElementAt(i));
            }

            return string.Format(CreateConstraint, Quoter.QuoteTableName(expression.Constraint.TableName, expression.Constraint.SchemaName),
                Quoter.QuoteConstraintName(expression.Constraint.ConstraintName),
                constraintType,
                string.Join(", ", columns));
        }

        /// <summary>
        /// Generates an SQL statement to drop a constraint
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(DeleteConstraintExpression expression)
        {
            return string.Format(DeleteConstraint, Quoter.QuoteTableName(expression.Constraint.TableName, expression.Constraint.SchemaName), Quoter.QuoteConstraintName(expression.Constraint.ConstraintName));
        }

        /// <summary>
        /// Generates the name of the foreign key.
        /// </summary>
        /// <param name="foreignKey">The foreign key.</param>
        /// <returns>System.String.</returns>
        public virtual string GenerateForeignKeyName(ForeignKeyDefinition foreignKey)
        {
            return Column.GenerateForeignKeyName(foreignKey);
        }

        /// <summary>
        /// Generates an SQL statement to delete a foreign key
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        /// <exception cref="ArgumentNullException">expression</exception>
        public override string Generate(DeleteForeignKeyExpression expression)
        {
            if (expression.ForeignKey.ForeignTable == null)
            {
                throw new ArgumentNullException(nameof(expression), ErrorMessages.ExpressionTableNameMissingWithHints);
            }

            return string.Format(DeleteConstraint, Quoter.QuoteTableName(expression.ForeignKey.ForeignTable, expression.ForeignKey.ForeignTableSchema), Quoter.QuoteColumnName(expression.ForeignKey.Name));
        }

        /// <summary>
        /// Generates an SQL statement to INSERT data
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(InsertDataExpression expression)
        {
            var errors = ValidateAdditionalFeatureCompatibility(expression.AdditionalFeatures);
            if (!string.IsNullOrEmpty(errors))
            {
                return errors;
            }

            var output = new StringBuilder();
            foreach (var pair in GenerateColumnNamesAndValues(expression))
            {
                if (output.Length != 0)
                {
                    AppendSqlStatementEndToken(output);
                }

                output.AppendFormat(
                    InsertData,
                    Quoter.QuoteTableName(expression.TableName, expression.SchemaName),
                    pair.Key,
                    pair.Value);
            }

            return output.ToString();
        }

        /// <summary>
        /// Appends the SQL statement end token.
        /// </summary>
        /// <param name="stringBuilder">The string builder.</param>
        /// <returns>StringBuilder.</returns>
        protected virtual StringBuilder AppendSqlStatementEndToken(StringBuilder stringBuilder)
        {
            return stringBuilder.Append("; ");
        }

        /// <summary>
        /// Generates the column names and values.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>List&lt;KeyValuePair&lt;System.String, System.String&gt;&gt;.</returns>
        protected List<KeyValuePair<string,string>> GenerateColumnNamesAndValues(InsertDataExpression expression)
        {
            var insertStrings = new List<KeyValuePair<string, string>>();

            foreach (InsertionDataDefinition row in expression.Rows)
            {
                var columnNames = new List<string>();
                var columnValues = new List<string>();
                foreach (KeyValuePair<string, object> item in row)
                {
                    columnNames.Add(Quoter.QuoteColumnName(item.Key));
                    columnValues.Add(Quoter.QuoteValue(item.Value));
                }

                var columns = string.Join(", ", columnNames.ToArray());
                var values = string.Join(", ", columnValues.ToArray());
                insertStrings.Add(new KeyValuePair<string, string>(columns, values));
            }

            return insertStrings;
        }

        /// <summary>
        /// Validates the additional feature compatibility.
        /// </summary>
        /// <param name="features">The features.</param>
        /// <returns>System.String.</returns>
        protected string ValidateAdditionalFeatureCompatibility(IEnumerable<KeyValuePair<string, object>> features)
        {
            if (CompatibilityMode == CompatibilityMode.STRICT) {
                var unsupportedFeatures =
                    features.Where(x => !IsAdditionalFeatureSupported(x.Key)).Select(x => x.Key).ToList();

                if (unsupportedFeatures.Count > 0) {
                    var errorMessage =
                        string.Format(
                            "The following database specific additional features are not supported in strict mode [{0}]",
                            unsupportedFeatures.Aggregate((x, y) => x + ", " + y));
                    {
                        return CompatibilityMode.HandleCompatibilty(errorMessage);
                    }
                }
            }
            return string.Empty;
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

            if(expression.IsAllRows)
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
            return string.Format(UpdateData, Quoter.QuoteTableName(expression.TableName, expression.SchemaName), string.Join(", ", updateItems.ToArray()), string.Join(" AND ", whereClauses.ToArray()));
        }

        /// <summary>
        /// Generates an SQL statement to DELETE data
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(DeleteDataExpression expression)
        {
            var deleteItems = new List<string>();

            if (expression.IsAllRows)
            {
                deleteItems.Add(string.Format(DeleteData, Quoter.QuoteTableName(expression.TableName, expression.SchemaName), "1 = 1"));
            }
            else
            {
                foreach (var row in expression.Rows)
                {
                    var whereClauses = new List<string>();
                    foreach (KeyValuePair<string, object> item in row)
                    {
                        var op = item.Value == null || item.Value == DBNull.Value ? "IS" : "=";
                        whereClauses.Add(
                            string.Format(
                                "{0} {1} {2}",
                                Quoter.QuoteColumnName(item.Key),
                                op,
                                Quoter.QuoteValue(item.Value)));
                    }

                    deleteItems.Add(string.Format(DeleteData, Quoter.QuoteTableName(expression.TableName, expression.SchemaName), string.Join(" AND ", whereClauses.ToArray())));
                }
            }

            var output = new StringBuilder();
            foreach (var deleteItem in deleteItems)
            {
                if (output.Length != 0)
                {
                    AppendSqlStatementEndToken(output);
                }

                output.Append(deleteItem);
            }

            return output.ToString();
        }

        //All Schema method throw by default as only Sql server 2005 and up supports them.
        /// <summary>
        /// Generates a <c>CREATE SCHEMA</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(CreateSchemaExpression expression)
        {
            return CompatibilityMode.HandleCompatibilty("Schemas are not supported");
        }

        /// <summary>
        /// Generates a <c>DROP SCHEMA</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(DeleteSchemaExpression expression)
        {
            return CompatibilityMode.HandleCompatibilty("Schemas are not supported");
        }

        /// <summary>
        /// Generates an SQL statement to move a table from one schema to another
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(AlterSchemaExpression expression)
        {
            return CompatibilityMode.HandleCompatibilty("Schemas are not supported");
        }

        /// <summary>
        /// Generates a <c>CREATE SEQUENCE</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(CreateSequenceExpression expression)
        {
            var result = new StringBuilder("CREATE SEQUENCE ");
            var seq = expression.Sequence;
            result.AppendFormat(Quoter.QuoteSequenceName(seq.Name, seq.SchemaName));

            if (seq.Increment.HasValue)
            {
                result.AppendFormat(" INCREMENT {0}", seq.Increment);
            }

            if (seq.MinValue.HasValue)
            {
                result.AppendFormat(" MINVALUE {0}", seq.MinValue);
            }

            if (seq.MaxValue.HasValue)
            {
                result.AppendFormat(" MAXVALUE {0}", seq.MaxValue);
            }

            if (seq.StartWith.HasValue)
            {
                result.AppendFormat(" START WITH {0}", seq.StartWith);
            }

            const long MINIMUM_CACHE_VALUE = 2;
            if (seq.Cache.HasValue)
            {
                if (seq.Cache.Value < MINIMUM_CACHE_VALUE)
                {
                    return CompatibilityMode.HandleCompatibilty("Cache size must be greater than 1; if you intended to disable caching, set Cache to null.");
                }
                result.AppendFormat(" CACHE {0}", seq.Cache);
            }
            else
            {
                result.Append(" NO CACHE");
            }

            if (seq.Cycle)
            {
                result.Append(" CYCLE");
            }

            return result.ToString();
        }

        /// <summary>
        /// Generates the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>System.String.</returns>
        public override string Generate(DeleteSequenceExpression expression)
        {
            var result = new StringBuilder("DROP SEQUENCE ");
            result.AppendFormat(Quoter.QuoteSequenceName(expression.SequenceName, expression.SchemaName));
            return result.ToString();
        }
    }
}
