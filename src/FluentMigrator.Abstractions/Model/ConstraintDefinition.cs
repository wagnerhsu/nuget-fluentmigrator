// ***********************************************************************
// Assembly         : FluentMigrator.Abstractions
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="ConstraintDefinition.cs" company="FluentMigrator Project">
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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using FluentMigrator.Infrastructure;
using FluentMigrator.Infrastructure.Extensions;

namespace FluentMigrator.Model
{
    /// <summary>
    /// The constraint definition
    /// </summary>
    public class ConstraintDefinition
        : ICloneable,
#pragma warning disable 618
          ICanBeValidated,
#pragma warning restore 618
          ISupportAdditionalFeatures,
          IValidatableObject
    {
        /// <summary>
        /// The constraint type
        /// </summary>
        private readonly ConstraintType _constraintType;

        /// <summary>
        /// Gets a value indicating whether the constraint is a primary key constraint
        /// </summary>
        /// <value><c>true</c> if this instance is primary key constraint; otherwise, <c>false</c>.</value>
        public bool IsPrimaryKeyConstraint => ConstraintType.PrimaryKey == _constraintType;

        /// <summary>
        /// Gets a value indicating whether the constraint is a unique constraint
        /// </summary>
        /// <value><c>true</c> if this instance is unique constraint; otherwise, <c>false</c>.</value>
        public bool IsUniqueConstraint => ConstraintType.Unique == _constraintType;

        /// <summary>
        /// Gets or sets the schema name
        /// </summary>
        /// <value>The name of the schema.</value>
        public virtual string SchemaName { get; set; }

        /// <summary>
        /// Gets or sets the constraint name
        /// </summary>
        /// <value>The name of the constraint.</value>
        public virtual string ConstraintName { get; set; }

        /// <summary>
        /// Gets or sets the table name
        /// </summary>
        /// <value>The name of the table.</value>
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = nameof(ErrorMessages.TableNameCannotBeNullOrEmpty))]
        public virtual string TableName { get; set; }

        /// <summary>
        /// Gets or sets the column names
        /// </summary>
        /// <value>The columns.</value>
        public virtual ICollection<string> Columns { get; set; } = new HashSet<string>();


        /// <summary>
        /// Initializes a new instance of the <see cref="ConstraintDefinition" /> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public ConstraintDefinition(ConstraintType type)
        {
            _constraintType = type;
        }

        /// <inheritdoc />
        public IDictionary<string, object> AdditionalFeatures { get; } = new Dictionary<string, object>();

        /// <inheritdoc />
        public object Clone()
        {
            var result = new ConstraintDefinition(_constraintType)
            {
                Columns = Columns,
                ConstraintName = ConstraintName,
                TableName = TableName
            };

            AdditionalFeatures.CloneTo(result.AdditionalFeatures);

            return result;
        }

        /// <inheritdoc />
        [Obsolete("Use the System.ComponentModel.DataAnnotations.Validator instead")]
        public void CollectValidationErrors(ICollection<string> errors)
        {
            this.CollectErrors(errors);
        }

        /// <inheritdoc />
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (0 == Columns.Count)
            {
                yield return new ValidationResult(ErrorMessages.ConstraintMustHaveAtLeastOneColumn);
            }
        }
    }
}
