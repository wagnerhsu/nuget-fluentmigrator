// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SQLiteTestTable.cs" company="FluentMigrator Project">
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

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using FluentMigrator.Runner.Processors.SQLite;

namespace FluentMigrator.Tests.Helpers {
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Class SQLiteTestTable.
    /// Implements the <see cref="System.IDisposable" />
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class SQLiteTestTable : IDisposable {
        /// <summary>
        /// The schema name
        /// </summary>
        private readonly string _schemaName;
        /// <summary>
        /// Gets or sets the connection.
        /// </summary>
        /// <value>The connection.</value>
        private IDbConnection Connection { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the transaction.
        /// </summary>
        /// <value>The transaction.</value>
        private IDbTransaction Transaction { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteTestTable"/> class.
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <param name="schemaName">Name of the schema.</param>
        /// <param name="columnDefinitions">The column definitions.</param>
        public SQLiteTestTable( SQLiteProcessor processor, string schemaName, params string[] columnDefinitions ) {
            _schemaName = schemaName;
            Connection = processor.Connection;
            Transaction = processor.Transaction;

            Name = "Table" + Guid.NewGuid().ToString( "N" );
            Create( columnDefinitions );
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {
            Drop();
        }

        /// <summary>
        /// Creates the specified column definitions.
        /// </summary>
        /// <param name="columnDefinitions">The column definitions.</param>
        public void Create( IEnumerable<string> columnDefinitions ) {
            if ( !string.IsNullOrEmpty( _schemaName ) ) {
                using ( var command = Connection.CreateCommand() ) {
                    //new DbCommand(string.Format("CREATE SCHEMA [{0}]", _schemaName), Connection, Transaction)
                    command.CommandText = string.Format( "CREATE SCHEMA [{0}]", _schemaName );
                    command.Transaction = Transaction;
                    command.ExecuteNonQuery();
                }
            }

            var sb = new StringBuilder();
            sb.Append( "CREATE TABLE " );
            if ( !string.IsNullOrEmpty( _schemaName ) )
                sb.AppendFormat( "[{0}].", _schemaName );
            sb.Append( Name );

            foreach ( string definition in columnDefinitions ) {
                sb.Append( "(" );
                sb.Append( definition );
                sb.Append( "), " );
            }

            sb.Remove( sb.Length - 2, 2 );
            using ( var command = Connection.CreateCommand() ) {
                //var command = new SqlCommand(sb.ToString(), Connection, Transaction)
                command.CommandText = sb.ToString();
                command.Transaction = Transaction;
                command.ExecuteNonQuery();
            }
            //using (var command = new SqlCommand(sb.ToString(), Connection, Transaction))
            //    command.ExecuteNonQuery();
        }

        /// <summary>
        /// Drops this instance.
        /// </summary>
        public void Drop() {
            if ( string.IsNullOrEmpty( _schemaName ) ) {
                using ( var command = Connection.CreateCommand() ) {
                    //var command = new SqlCommand(sb.ToString(), Connection, Transaction)
                    command.CommandText = "DROP TABLE " + Name;
                    command.Transaction = Transaction;
                    command.ExecuteNonQuery();
                }
                //using (var command = new SqlCommand("DROP TABLE " + Name, Connection, Transaction))
                //    command.ExecuteNonQuery();
            } else {
                using ( var command = Connection.CreateCommand() ) {
                    //var command = new SqlCommand(sb.ToString(), Connection, Transaction)
                    command.CommandText = string.Format( "DROP TABLE [{0}].{1}", _schemaName, Name );
                    command.Transaction = Transaction;
                    command.ExecuteNonQuery();
                }
                //using (var command = new SqlCommand(string.Format("DROP TABLE [{0}].{1}", _schemaName, Name), Connection, Transaction))
                //    command.ExecuteNonQuery();

                using ( var command = Connection.CreateCommand() ) {
                    //var command = new SqlCommand(sb.ToString(), Connection, Transaction)
                    command.CommandText = string.Format( "DROP SCHEMA [{0}]", _schemaName );
                    command.Transaction = Transaction;
                    command.ExecuteNonQuery();
                }
                //using (var command = new SqlCommand(string.Format("DROP SCHEMA [{0}]", _schemaName), Connection, Transaction))
                //    command.ExecuteNonQuery();
            }
        }
    }
}
