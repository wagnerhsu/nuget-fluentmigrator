// ***********************************************************************
// Assembly         : FluentMigrator.Abstractions
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="CreateTableExpression.cs" company="FluentMigrator Project">
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

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using FluentMigrator.Infrastructure;
using FluentMigrator.Model;
using FluentMigrator.Validation;

namespace FluentMigrator.Expressions
{
    /// <summary>
    /// Expression to create a table
    /// </summary>
    public class CreateTableExpression : MigrationExpressionBase, ISchemaExpression, IColumnsExpression, IValidationChildren
    {
        /// <inheritdoc />
        public virtual string SchemaName { get; set; }

        /// <inheritdoc />
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = nameof(ErrorMessages.TableNameCannotBeNullOrEmpty))]
        public virtual string TableName { get; set; }

        /// <summary>
        /// Gets or sets the column definitions
        /// </summary>
        /// <value>The columns.</value>
        public virtual IList<ColumnDefinition> Columns { get; set; } = new List<ColumnDefinition>();

        /// <summary>
        /// Gets or sets the table description
        /// </summary>
        /// <value>The table description.</value>
        public virtual string TableDescription { get; set; }

        /// <inheritdoc />
        IEnumerable<ColumnDefinition> IColumnsExpression.Columns => Columns;

        /// <inheritdoc />
        public override void ExecuteWith(IMigrationProcessor processor)
        {
            processor.Process(this);
        }

        /// <inheritdoc />
        public override IMigrationExpression Reverse()
        {
            return new DeleteTableExpression
                    {
                        TableName = TableName,
                        SchemaName = SchemaName
                    };
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return base.ToString() + TableName;
        }

        /// <inheritdoc />
        IEnumerable<object> IValidationChildren.Children => Columns;
    }
}
