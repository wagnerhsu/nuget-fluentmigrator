// ***********************************************************************
// Assembly         : FluentMigrator.Abstractions
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="DeleteConstraintExpression.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using FluentMigrator.Infrastructure;
using FluentMigrator.Model;

namespace FluentMigrator.Expressions
{
    /// <summary>
    /// Expression to delete a constraint
    /// </summary>
    public class DeleteConstraintExpression : MigrationExpressionBase, ISupportAdditionalFeatures, IConstraintExpression, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteConstraintExpression" /> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public DeleteConstraintExpression(ConstraintType type)
        {
            Constraint = new ConstraintDefinition(type);
        }

        /// <summary>
        /// Gets or sets the constraint definition
        /// </summary>
        /// <value>The constraint.</value>
        public ConstraintDefinition Constraint { get; set; }

        /// <inheritdoc />
        public IDictionary<string, object> AdditionalFeatures => Constraint.AdditionalFeatures;

        /// <inheritdoc />
        public override void ExecuteWith(IMigrationProcessor processor)
        {
            processor.Process(this);
        }

        /// <inheritdoc />
        public override string ToString()
        {

            return base.ToString() + Constraint.ConstraintName;
        }

        /// <inheritdoc />
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(Constraint.TableName))
            {
                yield return new ValidationResult(ErrorMessages.TableNameCannotBeNullOrEmpty);
            }
        }
    }
}
