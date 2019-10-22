// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="Db2TestTable.cs" company="FluentMigrator Project">
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
using System.Data;
using System.Text;

using FluentMigrator.Runner.Generators;
using FluentMigrator.Runner.Generators.DB2;
using FluentMigrator.Runner.Processors.DB2;

namespace FluentMigrator.Tests.Helpers
{
    /// <summary>
    /// Class Db2TestTable.
    /// Implements the <see cref="System.IDisposable" />
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class Db2TestTable : IDisposable
    {
        /// <summary>
        /// The quoter
        /// </summary>
        private readonly IQuoter _quoter = new Db2Quoter();

        /// <summary>
        /// The schema
        /// </summary>
        private readonly string _schema;

        /// <summary>
        /// Initializes a new instance of the <see cref="Db2TestTable"/> class.
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <param name="schema">The schema.</param>
        /// <param name="columnDefinitions">The column definitions.</param>
        public Db2TestTable(Db2Processor processor, string schema, params string[] columnDefinitions)
        {
            Processor = processor;
            _schema = schema;

            if (Connection.State != ConnectionState.Open)
                Connection.Open();

            Name = "TestTable";
            NameWithSchema = _quoter.QuoteTableName(Name, _schema);
            Create(columnDefinitions);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Db2TestTable"/> class.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="processor">The processor.</param>
        /// <param name="schema">The schema.</param>
        /// <param name="columnDefinitions">The column definitions.</param>
        public Db2TestTable(string table, Db2Processor processor, string schema, params string[] columnDefinitions)
        {
            Processor = processor;
            _schema = schema;

            if (Connection.State != ConnectionState.Open)
                Connection.Open();

            Name = _quoter.UnQuote(table);
            NameWithSchema = _quoter.QuoteTableName(Name, _schema);
            Create(columnDefinitions);
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get;
        }

        /// <summary>
        /// Gets the name with schema.
        /// </summary>
        /// <value>The name with schema.</value>
        public string NameWithSchema
        {
            get;
        }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <value>The connection.</value>
        private IDbConnection Connection => Processor.Connection;

        /// <summary>
        /// Creates the specified column definitions.
        /// </summary>
        /// <param name="columnDefinitions">The column definitions.</param>
        public void Create(string[] columnDefinitions)
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(_schema))
            {
                sb.AppendFormat("CREATE SCHEMA {0};", _quoter.QuoteSchemaName(_schema));
            }

            var columns = string.Join(", ", columnDefinitions);
            sb.AppendFormat("CREATE TABLE {0} ({1})", NameWithSchema, columns);

            Processor.Execute(sb.ToString());
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Drop();
        }

        /// <summary>
        /// Drops this instance.
        /// </summary>
        public void Drop()
        {
            var tableCommand = string.Format("DROP TABLE {0}", NameWithSchema);
            Processor.Execute(tableCommand);

            if (!string.IsNullOrEmpty(_schema))
            {
                var schemaCommand = string.Format("DROP SCHEMA {0} RESTRICT", _quoter.QuoteSchemaName(_schema));
                Processor.Execute(schemaCommand);
            }
        }

        /// <summary>
        /// Withes the index on.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="name">The name.</param>
        public void WithIndexOn(string column, string name)
        {
            var query = string.Format("CREATE UNIQUE INDEX {0} ON {1} ({2})",
                _quoter.QuoteIndexName(name, _schema),
                NameWithSchema,
                _quoter.QuoteColumnName(column)
                );

            Processor.Execute(query);
        }

        /// <summary>
        /// Withes the unique constraint on.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="name">The name.</param>
        public void WithUniqueConstraintOn(string column, string name)
        {
            var constraintName = _quoter.QuoteConstraintName(name, _schema);

            var query = string.Format("ALTER TABLE {0} ADD CONSTRAINT {1} UNIQUE ({2})",
                NameWithSchema,
                constraintName,
                _quoter.QuoteColumnName(column)
            );

            Processor.Execute(query);
        }

        /// <summary>
        /// Gets the processor.
        /// </summary>
        /// <value>The processor.</value>
        public Db2Processor Processor { get; }
    }
}
