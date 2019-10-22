// ***********************************************************************
// Assembly         : FluentMigrator.Abstractions
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="ColumnDefinition.cs" company="FluentMigrator Project">
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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;

using FluentMigrator.Infrastructure;

namespace FluentMigrator.Model
{
    /// <summary>
    /// The column definition
    /// </summary>
    public class ColumnDefinition
        : ICloneable,
#pragma warning disable 618
          ICanBeValidated,
#pragma warning restore 618
          ISupportAdditionalFeatures,
          IValidatableObject
    {
        /// <summary>
        /// Gets or sets the column definition name
        /// </summary>
        /// <value>The name.</value>
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = nameof(ErrorMessages.ColumnNameCannotBeNullOrEmpty))]
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the column type
        /// </summary>
        /// <value>The type.</value>
        public virtual DbType? Type { get; set; }

        /// <summary>
        /// Gets or sets the column type size (read: precision or length)
        /// </summary>
        /// <value>The size.</value>
        public virtual int? Size { get; set; }

        /// <summary>
        /// Gets or sets the column type precision (read: scale)
        /// </summary>
        /// <value>The precision.</value>
        public virtual int? Precision { get; set; }

        /// <summary>
        /// Gets or sets a database specific custom column type
        /// </summary>
        /// <value>The type of the custom.</value>
        public virtual string CustomType { get; set; }

        /// <summary>
        /// Gets or sets the columns default value
        /// </summary>
        /// <value>The default value.</value>
        public virtual object DefaultValue { get; set; } = new UndefinedDefaultValue();

        /// <summary>
        /// Gets or sets a value indicating whether this column is a foreign key
        /// </summary>
        /// <value><c>true</c> if this instance is foreign key; otherwise, <c>false</c>.</value>
        public virtual bool IsForeignKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this column gets its value using an identity definition
        /// </summary>
        /// <value><c>true</c> if this instance is identity; otherwise, <c>false</c>.</value>
        public virtual bool IsIdentity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating that this column is indexed
        /// </summary>
        /// <value><c>true</c> if this instance is indexed; otherwise, <c>false</c>.</value>
        public virtual bool IsIndexed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this column is a primary key
        /// </summary>
        /// <value><c>true</c> if this instance is primary key; otherwise, <c>false</c>.</value>
        public virtual bool IsPrimaryKey { get; set; }

        /// <summary>
        /// Gets or sets the primary key constraint name
        /// </summary>
        /// <value>The name of the primary key.</value>
        public virtual string PrimaryKeyName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this column is nullable
        /// </summary>
        /// <value><c>null</c> if [is nullable] contains no value, <c>true</c> if [is nullable]; otherwise, <c>false</c>.</value>
        public virtual bool? IsNullable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this column must be unique
        /// </summary>
        /// <value><c>true</c> if this instance is unique; otherwise, <c>false</c>.</value>
        public virtual bool IsUnique { get; set; }

        /// <summary>
        /// Gets or sets the columns table name
        /// </summary>
        /// <value>The name of the table.</value>
        public virtual string TableName { get; set; }

        /// <summary>
        /// Gets or sets if the column definition results in a CREATE or an ALTER SQL statement
        /// </summary>
        /// <value>The type of the modification.</value>
        public virtual ColumnModificationType ModificationType { get; set; }

        /// <summary>
        /// Gets or sets the column description
        /// </summary>
        /// <value>The column description.</value>
        public virtual string ColumnDescription { get; set; }

        /// <summary>
        /// Gets or sets the collation name if the column has a string or ansi string type
        /// </summary>
        /// <value>The name of the collation.</value>
        public virtual string CollationName { get; set; }

        /// <summary>
        /// Gets or sets the foreign key definition
        /// </summary>
        /// <value>The foreign key.</value>
        /// <remarks>A column might be marked as <see cref="IsForeignKey" />, but
        /// <see cref="ForeignKey" /> might still be <c>null</c>. This
        /// happens when <c>ForeignKey()</c> without arguments gets
        /// called on a column.</remarks>
        public virtual ForeignKeyDefinition ForeignKey { get; set; }

        /// <inheritdoc />
        [Obsolete("Use the System.ComponentModel.DataAnnotations.Validator instead")]
        public virtual void CollectValidationErrors(ICollection<string> errors)
        {
            this.CollectErrors(errors);
        }

        /// <inheritdoc />
        public virtual object Clone()
        {
            return MemberwiseClone();
        }

        /// <summary>
        /// Instances of this class are used to specify an undefined default value
        /// </summary>
        public sealed class UndefinedDefaultValue
        {
        }

        /// <inheritdoc />
        public IDictionary<string, object> AdditionalFeatures { get; } = new Dictionary<string, object>();

        /// <inheritdoc />
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Type == null && CustomType == null)
            {
                yield return new ValidationResult(ErrorMessages.ColumnTypeMustBeDefined);
            }
        }
    }
}
