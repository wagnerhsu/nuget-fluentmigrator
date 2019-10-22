// ***********************************************************************
// Assembly         : FluentMigrator
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SchemaIndexQuery.cs" company="FluentMigrator Project">
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

using FluentMigrator.Infrastructure;

namespace FluentMigrator.Builders.Schema.Index
{
    /// <summary>
    /// The implementation of the <see cref="ISchemaIndexSyntax" /> interface.
    /// </summary>
    public class SchemaIndexQuery : ISchemaIndexSyntax
    {
        /// <summary>
        /// The schema name
        /// </summary>
        private readonly string _schemaName;
        /// <summary>
        /// The table name
        /// </summary>
        private readonly string _tableName;
        /// <summary>
        /// The index name
        /// </summary>
        private readonly string _indexName;
        /// <summary>
        /// The context
        /// </summary>
        private readonly IMigrationContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaIndexQuery" /> class.
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <param name="tableName">The table name</param>
        /// <param name="indexName">The index name</param>
        /// <param name="context">The migration context</param>
        public SchemaIndexQuery(string schemaName, string tableName, string indexName, IMigrationContext context)
        {
            _schemaName = schemaName;
            _tableName = tableName;
            _indexName = indexName;
            _context = context;
        }

        /// <inheritdoc />
        public bool Exists()
        {
            return _context.QuerySchema.IndexExists(_schemaName, _tableName, _indexName);
        }
    }
}
