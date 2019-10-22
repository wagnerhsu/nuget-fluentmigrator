// ***********************************************************************
// Assembly         : FluentMigrator.Abstractions
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="InsertDataExpression.cs" company="FluentMigrator Project">
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

using FluentMigrator.Infrastructure;
using FluentMigrator.Model;

namespace FluentMigrator.Expressions
{
    /// <summary>
    /// Expression to insert data
    /// </summary>
    public class InsertDataExpression : IMigrationExpression, ISupportAdditionalFeatures, ISchemaExpression
    {
        /// <inheritdoc />
        public string SchemaName { get; set; }

        /// <summary>
        /// Gets or sets the table name
        /// </summary>
        /// <value>The name of the table.</value>
        [Required]
        public string TableName { get; set; }

        /// <inheritdoc />
        public IDictionary<string, object> AdditionalFeatures { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Gets the rows to be inserted
        /// </summary>
        /// <value>The rows.</value>
        public List<InsertionDataDefinition> Rows { get; } = new List<InsertionDataDefinition>();

        /// <inheritdoc />
        [Obsolete("Use the System.ComponentModel.DataAnnotations.Validator instead")]
        public void CollectValidationErrors(ICollection<string> errors)
        {
            this.CollectErrors(errors);
        }

        /// <inheritdoc />
        public void ExecuteWith(IMigrationProcessor processor)
        {
            processor.Process(this);
        }

        /// <inheritdoc />
        public IMigrationExpression Reverse()
        {
            var expression = new DeleteDataExpression
                                {
                                    SchemaName = SchemaName,
                                    TableName = TableName
                                };

            foreach (var row in Rows)
            {
                var dataDefinition = new DeletionDataDefinition();
                dataDefinition.AddRange(row);

                expression.Rows.Add(dataDefinition);
            }

            return expression;
        }
    }
}
