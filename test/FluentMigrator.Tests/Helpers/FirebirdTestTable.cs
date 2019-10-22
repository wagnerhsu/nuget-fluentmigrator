// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="FirebirdTestTable.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using FirebirdSql.Data.FirebirdClient;
using FluentMigrator.Runner.Generators.Firebird;
using FluentMigrator.Runner.Processors.Firebird;

namespace FluentMigrator.Tests.Helpers
{
    /// <summary>
    /// Class FirebirdTestTable.
    /// Implements the <see cref="System.IDisposable" />
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class FirebirdTestTable : IDisposable
    {
        /// <summary>
        /// The quoter
        /// </summary>
        private readonly FirebirdQuoter _quoter = new FirebirdQuoter(false);
        /// <summary>
        /// The processor
        /// </summary>
        private readonly FirebirdProcessor _processor;

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <value>The connection.</value>
        public FbConnection Connection => (FbConnection)_processor.Connection;

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; }

        /// <summary>
        /// Gets the transaction.
        /// </summary>
        /// <value>The transaction.</value>
        public FbTransaction Transaction => (FbTransaction)_processor.Transaction;

        /// <summary>
        /// Initializes a new instance of the <see cref="FirebirdTestTable"/> class.
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <param name="columnDefinitions">The column definitions.</param>
        public FirebirdTestTable(FirebirdProcessor processor, params string[] columnDefinitions)
        {
            _processor = processor;
            if (_processor.Connection.State != ConnectionState.Open)
                _processor.Connection.Open();
            string guid = Guid.NewGuid().ToString("N");
            Name = "Table" + guid.Substring(0, Math.Min(guid.Length, 16));
            Init(columnDefinitions);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FirebirdTestTable"/> class.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="processor">The processor.</param>
        /// <param name="columnDefinitions">The column definitions.</param>
        public FirebirdTestTable(string tableName, FirebirdProcessor processor, params string[] columnDefinitions)
        {
            _processor = processor;
            if (_processor.Connection.State != ConnectionState.Open)
                _processor.Connection.Open();
            Name = tableName;
            Init(columnDefinitions);
        }

        /// <summary>
        /// Initializes the specified column definitions.
        /// </summary>
        /// <param name="columnDefinitions">The column definitions.</param>
        private void Init(IEnumerable<string> columnDefinitions)
        {
            Create(columnDefinitions);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (Connection.State == ConnectionState.Open && !_processor.WasCommitted)
                Drop();
        }

        /// <summary>
        /// Creates the specified column definitions.
        /// </summary>
        /// <param name="columnDefinitions">The column definitions.</param>
        private void Create(IEnumerable<string> columnDefinitions)
        {
            var sb = new StringBuilder();

            sb.Append("CREATE TABLE ");

            sb.Append(_quoter.QuoteTableName(Name, string.Empty));
            sb.Append(" (");

            foreach (string definition in columnDefinitions)
            {
                sb.Append(definition);
                sb.Append(", ");
            }

            sb.Remove(sb.Length - 2, 2);
            sb.Append(")");

            var s = sb.ToString();

            using (var command = new FbCommand(s, Connection, Transaction))
                command.ExecuteNonQuery();

            _processor.AutoCommit();

            _processor.LockTable(Name);

        }

        /// <summary>
        /// Drops this instance.
        /// </summary>
        private void Drop()
        {
            _processor.CheckTable(Name);
            var sb = new StringBuilder();
            sb.AppendFormat("DROP TABLE {0}", _quoter.QuoteTableName(Name));

            using (var command = new FbCommand(sb.ToString(), Connection, Transaction))
                command.ExecuteNonQuery();

            _processor.AutoCommit();

        }
    }
}
