// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="HanaTestTable.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using FluentMigrator.Runner.Generators;
using FluentMigrator.Runner.Generators.Hana;
using FluentMigrator.Runner.Processors.Hana;
using Sap.Data.Hana;

namespace FluentMigrator.Tests.Helpers
{
    /// <summary>
    /// Class HanaTestTable.
    /// Implements the <see cref="System.IDisposable" />
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class HanaTestTable : IDisposable
    {
        /// <summary>
        /// The quoter
        /// </summary>
        private readonly IQuoter _quoter = new HanaQuoter();
        /// <summary>
        /// The schema name
        /// </summary>
        private readonly string _schemaName;
        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <value>The connection.</value>
        public HanaConnection Connection { get; private set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the name with schema.
        /// </summary>
        /// <value>The name with schema.</value>
        public string NameWithSchema { get; set; }
        /// <summary>
        /// Gets the transaction.
        /// </summary>
        /// <value>The transaction.</value>
        public HanaTransaction Transaction { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HanaTestTable"/> class.
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <param name="schemaName">Name of the schema.</param>
        /// <param name="columnDefinitions">The column definitions.</param>
        public HanaTestTable(HanaProcessor processor, string schemaName, params string[] columnDefinitions)
        {
            _schemaName = schemaName;
            Name = "Table" + Guid.NewGuid().ToString("N");
            Init(processor, columnDefinitions);

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HanaTestTable"/> class.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="processor">The processor.</param>
        /// <param name="schemaName">Name of the schema.</param>
        /// <param name="columnDefinitions">The column definitions.</param>
        public HanaTestTable(string tableName, HanaProcessor processor, string schemaName, params string[] columnDefinitions)
        {
            _schemaName = schemaName;

            Name = _quoter.UnQuote(tableName);
            Init(processor, columnDefinitions);
        }

        /// <summary>
        /// Initializes the specified processor.
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <param name="columnDefinitions">The column definitions.</param>
        private void Init(HanaProcessor processor, IEnumerable<string> columnDefinitions)
        {
            NameWithSchema = _quoter.QuoteTableName(Name, _schemaName);

            Connection = (HanaConnection)processor.Connection;
            Transaction = (HanaTransaction)processor.Transaction;

            if (Connection.State != ConnectionState.Open)
                Connection.Open();

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
            var sb = new StringBuilder();

            var quotedSchema = _quoter.QuoteSchemaName(_schemaName);
            if (!string.IsNullOrEmpty(quotedSchema))
                sb.AppendFormat("CREATE SCHEMA {0};", quotedSchema);

            sb.Append("CREATE TABLE ");

            sb.Append(NameWithSchema);
            sb.Append(" (");

            foreach (string definition in columnDefinitions)
            {
                sb.Append(definition);
                sb.Append(", ");
            }

            sb.Remove(sb.Length - 2, 2);
            sb.Append(")");

            var s = sb.ToString();
            using (var command = new HanaCommand(s, Connection, Transaction))
                command.ExecuteNonQuery();
        }

        /// <summary>
        /// Drops this instance.
        /// </summary>
        public void Drop()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("DROP TABLE {0}", NameWithSchema);
            var quotedSchema = _quoter.QuoteSchemaName(_schemaName);
            if (!string.IsNullOrEmpty(quotedSchema))
                sb.AppendFormat(";DROP SCHEMA {0}", quotedSchema);

            using (var command = new HanaCommand(sb.ToString(), Connection, Transaction))
                command.ExecuteNonQuery();
        }

        /// <summary>
        /// Withes the default value on.
        /// </summary>
        /// <param name="column">The column.</param>
        public void WithDefaultValueOn(string column)
        {
            const int defaultValue = 1;
            using (var command = new HanaCommand(string.Format(" ALTER TABLE {0} ALTER {1} SET DEFAULT {2}", _quoter.QuoteTableName(Name, _schemaName), _quoter.QuoteColumnName(column), defaultValue), Connection, Transaction))
                command.ExecuteNonQuery();
        }

        /// <summary>
        /// Withes the index on.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns>System.String.</returns>
        public string WithIndexOn(string column)
        {
            var indexName = string.Format("idx_{0}", column);

            var quotedObjectName = _quoter.QuoteTableName(Name);

            var quotedIndexName = _quoter.QuoteIndexName(indexName);

            using (var command = new HanaCommand(string.Format("CREATE INDEX {0} ON {1} ({2})", quotedIndexName, quotedObjectName, _quoter.QuoteColumnName(column)), Connection, Transaction))
                command.ExecuteNonQuery();

            return indexName;
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
            using (var command = new HanaCommand(sb.ToString(), Connection, Transaction))
                command.ExecuteNonQuery();
        }
    }
}
