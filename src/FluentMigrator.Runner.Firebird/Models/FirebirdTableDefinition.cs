// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Firebird
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="FirebirdTableDefinition.cs" company="FluentMigrator Project">
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

using FluentMigrator.Model;

namespace FluentMigrator.Runner.Models
{
    /// <summary>
    /// Class FirebirdTableDefinition.
    /// </summary>
    internal class FirebirdTableDefinition
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the name of the schema.
        /// </summary>
        /// <value>The name of the schema.</value>
        public string SchemaName { get; set; }
        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        /// <value>The columns.</value>
        public ICollection<ColumnDefinition> Columns { get; set; } = new List<ColumnDefinition>();
        /// <summary>
        /// Gets or sets the foreign keys.
        /// </summary>
        /// <value>The foreign keys.</value>
        public ICollection<ForeignKeyDefinition> ForeignKeys { get; set; } = new List<ForeignKeyDefinition>();
        /// <summary>
        /// Gets or sets the indexes.
        /// </summary>
        /// <value>The indexes.</value>
        public ICollection<IndexDefinition> Indexes { get; set; } = new List<IndexDefinition>();
    }
}
