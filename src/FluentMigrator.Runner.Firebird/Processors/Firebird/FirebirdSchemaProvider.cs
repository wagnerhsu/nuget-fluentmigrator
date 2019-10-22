// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Firebird
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="FirebirdSchemaProvider.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
//
// Copyright (c) 2007-2018, Fluent Migrator Project
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
using System.Linq;

using FluentMigrator.Model;
using FluentMigrator.Runner.Generators.Firebird;
using FluentMigrator.Runner.Models;

namespace FluentMigrator.Runner.Processors.Firebird
{
    /// <summary>
    /// Class FirebirdSchemaProvider.
    /// </summary>
    public class FirebirdSchemaProvider
    {
        /// <summary>
        /// The quoter
        /// </summary>
        private readonly FirebirdQuoter _quoter;
        /// <summary>
        /// The table schemas
        /// </summary>
        internal Dictionary<string, FirebirdTableSchema> TableSchemas = new Dictionary<string, FirebirdTableSchema>();
        /// <summary>
        /// Gets or sets the processor.
        /// </summary>
        /// <value>The processor.</value>
        public FirebirdProcessor Processor { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FirebirdSchemaProvider"/> class.
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <param name="quoter">The quoter.</param>
        public FirebirdSchemaProvider(FirebirdProcessor processor, FirebirdQuoter quoter)
        {
            _quoter = quoter;
            Processor = processor;
        }

        /// <summary>
        /// Gets the column definition.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>ColumnDefinition.</returns>
        public ColumnDefinition GetColumnDefinition(string tableName, string columnName)
        {
            FirebirdTableDefinition firebirdTableDef = GetTableDefinition(tableName);
            return firebirdTableDef.Columns.First(x => x.Name == columnName);
        }

        /// <summary>
        /// Gets the table definition.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>FirebirdTableDefinition.</returns>
        internal FirebirdTableDefinition GetTableDefinition(string tableName)
        {
            return GetTableSchema(tableName).Definition;
        }

        /// <summary>
        /// Gets the table schema.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>FirebirdTableSchema.</returns>
        internal FirebirdTableSchema GetTableSchema(string tableName)
        {
            if (TableSchemas.ContainsKey(tableName))
                return TableSchemas[tableName];
            return LoadTableSchema(tableName);
        }

        /// <summary>
        /// Loads the table schema.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>FirebirdTableSchema.</returns>
        internal FirebirdTableSchema LoadTableSchema(string tableName)
        {
            FirebirdTableSchema schema = new FirebirdTableSchema(tableName, Processor, _quoter);
            TableSchemas.Add(tableName, schema);
            return schema;
        }

        /// <summary>
        /// Gets the index.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="indexName">Name of the index.</param>
        /// <returns>IndexDefinition.</returns>
        public IndexDefinition GetIndex(string tableName, string indexName)
        {
            FirebirdTableDefinition firebirdTableDef = GetTableDefinition(tableName);
            if (firebirdTableDef.Indexes.Any(x => x.Name == indexName))
                return firebirdTableDef.Indexes.First(x => x.Name == indexName);
            return null;
        }

        /// <summary>
        /// Gets the sequence.
        /// </summary>
        /// <param name="sequenceName">Name of the sequence.</param>
        /// <returns>SequenceInfo.</returns>
        public SequenceInfo GetSequence(string sequenceName)
        {
            return SequenceInfo.Read(Processor, sequenceName, _quoter);
        }
    }
}
