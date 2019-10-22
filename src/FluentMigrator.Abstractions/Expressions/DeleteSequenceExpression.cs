// ***********************************************************************
// Assembly         : FluentMigrator.Abstractions
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="DeleteSequenceExpression.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.ComponentModel.DataAnnotations;

using FluentMigrator.Infrastructure;

namespace FluentMigrator.Expressions
{
    /// <summary>
    /// Expression to delete a sequence
    /// </summary>
    public class DeleteSequenceExpression : MigrationExpressionBase, ISchemaExpression
    {
        /// <inheritdoc />
        public virtual string SchemaName { get; set; }

        /// <summary>
        /// Gets or sets the sequence name
        /// </summary>
        /// <value>The name of the sequence.</value>
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = nameof(ErrorMessages.SequenceNameCannotBeNullOrEmpty))]
        public virtual string SequenceName { get; set; }

        /// <inheritdoc />
        public override void ExecuteWith(IMigrationProcessor processor)
        {
            processor.Process(this);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return base.ToString() + SequenceName;
        }
    }
}
