// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="OracleTestTable.cs" company="FluentMigrator Project">
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
using System.Data;
using System.Text;

using FluentMigrator.Runner.Generators;
using FluentMigrator.Runner.Generators.Oracle;
using FluentMigrator.Runner.Processors;

namespace FluentMigrator.Tests.Helpers
{
    /// <summary>
    /// Class OracleTestTable.
    /// Implements the <see cref="System.IDisposable" />
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class OracleTestTable : IDisposable
    {
        /// <summary>
        /// The quoter
        /// </summary>
        private readonly IQuoter _quoter = new OracleQuoterQuotedIdentifier();
        /// <summary>
        /// The processor
        /// </summary>
        private readonly GenericProcessorBase _processor;
        /// <summary>
        /// The schema
        /// </summary>
        private readonly string _schema;
        /// <summary>
        /// The constraints
        /// </summary>
        private readonly List<string> _constraints = new List<string>();
        /// <summary>
        /// The indexies
        /// </summary>
        private readonly List<string> _indexies = new List<string>();

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <value>The connection.</value>
        private IDbConnection Connection => _processor.Connection;
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="OracleTestTable"/> class.
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <param name="schema">The schema.</param>
        /// <param name="columnDefinitions">The column definitions.</param>
        public OracleTestTable(GenericProcessorBase processor, string schema, params string[] columnDefinitions)
        {
            _processor = processor;
            _schema = schema;

            if (Connection.State != ConnectionState.Open)
                Connection.Open();

            Name = "TestTable";
            Create(columnDefinitions);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OracleTestTable"/> class.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="processor">The processor.</param>
        /// <param name="schema">The schema.</param>
        /// <param name="columnDefinitions">The column definitions.</param>
        public OracleTestTable(string table, GenericProcessorBase processor, string schema, params string[] columnDefinitions)
        {
            _processor = processor;
            _schema = schema;

            if (Connection.State != ConnectionState.Open)
                Connection.Open();

            Name = table;
            Create(columnDefinitions);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Drop();
        }

        /// <summary>
        /// Creates the specified column definitions.
        /// </summary>
        /// <param name="columnDefinitions">The column definitions.</param>
        public void Create(IEnumerable<string> columnDefinitions)
        {
            var sb = CreateSchemaQuery();

            sb.Append("CREATE TABLE ");
            sb.Append(_quoter.QuoteTableName(Name));

            foreach (string definition in columnDefinitions)
            {
                sb.Append("(");
                sb.Append(definition);
                sb.Append("), ");
            }

            sb.Remove(sb.Length - 2, 2);

            _processor.Execute(sb.ToString());
        }

        /// <summary>
        /// Creates the schema query.
        /// </summary>
        /// <returns>StringBuilder.</returns>
        private StringBuilder CreateSchemaQuery()
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(_schema))
            {
                sb.Append(string.Format("CREATE SCHEMA AUTHORIZATION {0} ", _schema));
            }
            return sb;
        }

        /// <summary>
        /// Withes the unique constraint on.
        /// </summary>
        /// <param name="column">The column.</param>
        public void WithUniqueConstraintOn(string column)
        {
            WithUniqueConstraintOn(column, "UC_" + column);
        }

        /// <summary>
        /// Withes the unique constraint on.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="name">The name.</param>
        public void WithUniqueConstraintOn(string column, string name)
        {
            var sb = new StringBuilder();
            sb.Append(string.Format("ALTER TABLE {0} ADD CONSTRAINT {1} UNIQUE ({2})", _quoter.QuoteTableName(Name), _quoter.QuoteConstraintName(name), _quoter.QuoteColumnName(column)));
            _processor.Execute(sb.ToString());
            _constraints.Add(name);
       }

        /// <summary>
        /// Withes the index on.
        /// </summary>
        /// <param name="column">The column.</param>
        public void WithIndexOn(string column)
        {
            WithIndexOn(column, "UI_" + column);
        }

        /// <summary>
        /// Withes the index on.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="name">The name.</param>
        public void WithIndexOn(string column, string name)
        {
            var sb = new StringBuilder();
            sb.Append(string.Format("CREATE UNIQUE INDEX {0} ON {1} ({2})", _quoter.QuoteIndexName(name), _quoter.QuoteTableName(Name), _quoter.QuoteColumnName(column)));
            _processor.Execute(sb.ToString());
            _indexies.Add(name);
        }

        /// <summary>
        /// Drops this instance.
        /// </summary>
        public void Drop()
        {
            foreach(var constraint in _constraints)
            {
                var cmd = string.Format(
                    "ALTER TABLE {0} DROP CONSTRAINT {1}",
                    _quoter.QuoteTableName(Name),
                    _quoter.QuoteConstraintName(constraint));
                _processor.Execute(cmd);
            }

            foreach (var index in _indexies)
            {
                var cmd = string.Format("DROP INDEX {0}", _quoter.QuoteIndexName(index));
                _processor.Execute(cmd);
            }

            var dropSql = "DROP TABLE " + _quoter.QuoteTableName(Name);
            _processor.Execute(dropSql);
        }
    }
}
