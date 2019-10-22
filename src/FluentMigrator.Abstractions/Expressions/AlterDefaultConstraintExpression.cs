// ***********************************************************************
// Assembly         : FluentMigrator.Abstractions
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="AlterDefaultConstraintExpression.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.ComponentModel.DataAnnotations;

using FluentMigrator.Infrastructure;

namespace FluentMigrator.Expressions
{
    /// <summary>
    /// Expression to alter default constraints
    /// </summary>
    public class AlterDefaultConstraintExpression
        : MigrationExpressionBase,
          ISchemaExpression
    {
        /// <inheritdoc />
        public virtual string SchemaName { get; set; }

        /// <summary>
        /// Gets or sets the table name
        /// </summary>
        /// <value>The name of the table.</value>
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = nameof(ErrorMessages.TableNameCannotBeNullOrEmpty))]
        public virtual string TableName { get; set; }

        /// <summary>
        /// Gets or sets the column name
        /// </summary>
        /// <value>The name of the column.</value>
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = nameof(ErrorMessages.ColumnNameCannotBeNullOrEmpty))]
        public virtual string ColumnName { get; set; }

        /// <summary>
        /// Gets or sets the default value
        /// </summary>
        /// <value>The default value.</value>
        [Required(AllowEmptyStrings = true, ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = nameof(ErrorMessages.DefaultValueCannotBeNull))]
        public virtual object DefaultValue { get; set; }

        /// <inheritdoc />
        public override void ExecuteWith(IMigrationProcessor processor)
        {
            processor.Process(this);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return base.ToString() +
                    string.Format("{0}.{1} {2} {3}",
                                SchemaName,
                                TableName,
                                ColumnName,
                                DefaultValue);
        }
    }
}
