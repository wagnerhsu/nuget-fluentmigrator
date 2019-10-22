// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="PostgresTestTable.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using FluentMigrator.Runner.Generators.Postgres;
using FluentMigrator.Runner.Processors.Postgres;
using Npgsql;

namespace FluentMigrator.Tests.Helpers
{
    /// <summary>
    /// Class PostgresTestTable.
    /// Implements the <see cref="System.IDisposable" />
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class PostgresTestTable : IDisposable
    {
        /// <summary>
        /// The quoter
        /// </summary>
        private readonly PostgresQuoter _quoter = new PostgresQuoter(new PostgresOptions());
        /// <summary>
        /// The schema name
        /// </summary>
        private readonly string _schemaName;
        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <value>The connection.</value>
        public NpgsqlConnection Connection { get; private set; }
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
        public NpgsqlTransaction Transaction { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgresTestTable"/> class.
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <param name="schemaName">Name of the schema.</param>
        /// <param name="columnDefinitions">The column definitions.</param>
        public PostgresTestTable(PostgresProcessor processor, string schemaName, params string[] columnDefinitions)
        {
            _schemaName = schemaName;
            Name = "Table" + Guid.NewGuid().ToString("N");
            Init(processor, columnDefinitions);

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgresTestTable"/> class.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="processor">The processor.</param>
        /// <param name="schemaName">Name of the schema.</param>
        /// <param name="columnDefinitions">The column definitions.</param>
        public PostgresTestTable(string tableName, PostgresProcessor processor, string schemaName, params string[] columnDefinitions)
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
        private void Init(PostgresProcessor processor, IEnumerable<string> columnDefinitions)
        {
            Connection = (NpgsqlConnection)processor.Connection;
            Transaction = (NpgsqlTransaction)processor.Transaction;

            NameWithSchema = _quoter.QuoteTableName(Name, _schemaName);
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

            if (!string.IsNullOrEmpty(_schemaName))
                sb.AppendFormat("CREATE SCHEMA \"{0}\";", _schemaName);

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
            using (var command = new NpgsqlCommand(s, Connection, Transaction))
                command.ExecuteNonQuery();
        }

        /// <summary>
        /// Drops this instance.
        /// </summary>
        public void Drop()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("DROP TABLE {0}", NameWithSchema);
            if (!string.IsNullOrEmpty(_schemaName))
                sb.AppendFormat(";DROP SCHEMA \"{0}\"", _schemaName);

            using (var command = new NpgsqlCommand(sb.ToString(), Connection, Transaction))
                command.ExecuteNonQuery();
        }

        /// <summary>
        /// Withes the default value on.
        /// </summary>
        /// <param name="column">The column.</param>
        public void WithDefaultValueOn(string column)
        {
            const int defaultValue = 1;
            using (var command = new NpgsqlCommand(string.Format(" ALTER TABLE {0} ALTER {1} SET DEFAULT {2}", _quoter.QuoteTableName(Name, _schemaName), _quoter.QuoteColumnName(column), defaultValue), Connection, Transaction))
                command.ExecuteNonQuery();
        }
    }
}
