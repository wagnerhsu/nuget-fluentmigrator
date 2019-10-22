// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="JetTestTable.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
// Copyright (c) 2007-2018, Sean Chambers <schambers80@gmail.com>
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Text;
using FluentMigrator.Runner.Generators.Jet;
using FluentMigrator.Runner.Processors.Jet;

namespace FluentMigrator.Tests.Helpers
{
    /// <summary>
    /// Class JetTestTable.
    /// Implements the <see cref="System.IDisposable" />
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class JetTestTable : IDisposable
    {
        /// <summary>
        /// The quoter
        /// </summary>
        private readonly JetQuoter _quoter = new JetQuoter();
        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <value>The connection.</value>
        public OleDbConnection Connection { get; private set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets the transaction.
        /// </summary>
        /// <value>The transaction.</value>
        public OleDbTransaction Transaction { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JetTestTable"/> class.
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <param name="columnDefinitions">The column definitions.</param>
        public JetTestTable(JetProcessor processor, params string[] columnDefinitions)
        {
            Name = "Table" + Guid.NewGuid().ToString("N");
            Init(processor, columnDefinitions);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JetTestTable"/> class.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="processor">The processor.</param>
        /// <param name="columnDefinitions">The column definitions.</param>
        public JetTestTable(string tableName, JetProcessor processor, params string[] columnDefinitions)
        {
            Name = _quoter.QuoteTableName(tableName);
            Init(processor, columnDefinitions);
        }

        /// <summary>
        /// Initializes the specified processor.
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <param name="columnDefinitions">The column definitions.</param>
        private void Init(JetProcessor processor, IEnumerable<string> columnDefinitions)
        {
            Connection = processor.Connection;
            Transaction = processor.Transaction;

            var csb = new OleDbConnectionStringBuilder(Connection.ConnectionString);
            var dbFileName = HostUtilities.ReplaceDataDirectory(csb.DataSource);
            csb.DataSource = dbFileName;

            if (!File.Exists(dbFileName))
            {
                var connString = csb.ConnectionString;
                var type = Type.GetTypeFromProgID("ADOX.Catalog");
                if (type != null)
                {
                    dynamic cat = Activator.CreateInstance(type);
                    cat.Create(connString);
                }
            }

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
            if (Connection.State != ConnectionState.Open)
                Connection.Open();

            var sb = new StringBuilder();
            sb.Append("CREATE TABLE ");

            sb.Append(Name);

            foreach (string definition in columnDefinitions)
            {
                sb.Append("(");
                sb.Append(definition);
                sb.Append("), ");
            }

            sb.Remove(sb.Length - 2, 2);
            using (var command = new OleDbCommand(sb.ToString(), Connection, Transaction))
                command.ExecuteNonQuery();
        }

        /// <summary>
        /// Drops this instance.
        /// </summary>
        public void Drop()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("DROP TABLE {0}", Name);

            using (var command = new OleDbCommand(sb.ToString(), Connection, Transaction))
                command.ExecuteNonQuery();
        }
    }
}
