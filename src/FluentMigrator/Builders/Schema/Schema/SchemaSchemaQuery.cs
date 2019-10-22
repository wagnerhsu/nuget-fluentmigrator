// ***********************************************************************
// Assembly         : FluentMigrator
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SchemaSchemaQuery.cs" company="FluentMigrator Project">
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

using FluentMigrator.Builders.Schema.Table;
using FluentMigrator.Infrastructure;

namespace FluentMigrator.Builders.Schema.Schema
{
    /// <summary>
    /// The implementation of the <see cref="ISchemaSchemaSyntax" /> interface.
    /// </summary>
    public class SchemaSchemaQuery : ISchemaSchemaSyntax
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly IMigrationContext _context;
        /// <summary>
        /// The schema name
        /// </summary>
        private readonly string _schemaName;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaSchemaQuery" /> class.
        /// </summary>
        /// <param name="context">The migration context</param>
        /// <param name="schemaName">The schema name</param>
        public SchemaSchemaQuery(IMigrationContext context, string schemaName)
        {
            _context = context;
            _schemaName = schemaName;
        }

        /// <inheritdoc />
        public bool Exists()
        {
            return _context.QuerySchema.SchemaExists(_schemaName);
        }

        /// <inheritdoc />
        public ISchemaTableSyntax Table(string tableName)
        {
            return new SchemaTableQuery(_context, _schemaName, tableName);
        }
    }
}
