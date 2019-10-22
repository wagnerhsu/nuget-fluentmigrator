// ***********************************************************************
// Assembly         : FluentMigrator.Abstractions
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="CreateSchemaExpression.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using FluentMigrator.Infrastructure;

namespace FluentMigrator.Expressions
{
    /// <summary>
    /// Expression to create a schema
    /// </summary>
    public class CreateSchemaExpression : MigrationExpressionBase, ISupportAdditionalFeatures
    {
        /// <summary>
        /// Gets or sets the schema name
        /// </summary>
        /// <value>The name of the schema.</value>
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = nameof(ErrorMessages.SchemaNameCannotBeNullOrEmpty))]
        public virtual string SchemaName { get; set; }

        /// <inheritdoc />
        public IDictionary<string, object> AdditionalFeatures { get; } = new Dictionary<string, object>();

        /// <inheritdoc />
        public override void ExecuteWith(IMigrationProcessor processor)
        {
            processor.Process(this);
        }

        /// <inheritdoc />
        public override IMigrationExpression Reverse()
        {
            return new DeleteSchemaExpression { SchemaName = SchemaName };
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return base.ToString() + SchemaName;
        }
    }
}
