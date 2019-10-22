// ***********************************************************************
// Assembly         : FluentMigrator.Abstractions
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="CreateConstraintExpression.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;

using FluentMigrator.Infrastructure;
using FluentMigrator.Model;
using FluentMigrator.Validation;

namespace FluentMigrator.Expressions
{
    /// <summary>
    /// The expression to create a constraint
    /// </summary>
    public class CreateConstraintExpression : MigrationExpressionBase, ISupportAdditionalFeatures, IConstraintExpression, IValidationChildren
    {
        /// <inheritdoc />
        public virtual ConstraintDefinition Constraint { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateConstraintExpression" /> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public CreateConstraintExpression(ConstraintType type)
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            Constraint = new ConstraintDefinition(type);
        }

        /// <inheritdoc />
        public IDictionary<string, object> AdditionalFeatures => Constraint.AdditionalFeatures;

        /// <inheritdoc />
        public override void ExecuteWith(IMigrationProcessor processor)
        {
            processor.Process(this);
        }

        /// <inheritdoc />
        public override IMigrationExpression Reverse()
        {
            //constraint type is private in ConstraintDefinition
            return new DeleteConstraintExpression(Constraint.IsPrimaryKeyConstraint ? ConstraintType.PrimaryKey : ConstraintType.Unique)
            {
                Constraint = Constraint
            };
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return base.ToString() + Constraint.ConstraintName;
        }

        /// <inheritdoc />
        IEnumerable<object> IValidationChildren.Children
        {
            get { yield return Constraint; }
        }
    }
}
