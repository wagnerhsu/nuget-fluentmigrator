// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SqlServerTestSequence.cs" company="FluentMigrator Project">
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
using System.Data.SqlClient;
using FluentMigrator.Runner.Processors.SqlServer;

namespace FluentMigrator.Tests.Helpers
{
    /// <summary>
    /// Class SqlServerTestSequence.
    /// Implements the <see cref="System.IDisposable" />
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class SqlServerTestSequence : IDisposable
    {
        /// <summary>
        /// The schema name
        /// </summary>
        private readonly string _schemaName;

        /// <summary>
        /// Gets or sets the connection.
        /// </summary>
        /// <value>The connection.</value>
        private SqlConnection Connection { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the transaction.
        /// </summary>
        /// <value>The transaction.</value>
        private SqlTransaction Transaction { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerTestSequence"/> class.
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <param name="schemaName">Name of the schema.</param>
        /// <param name="sequenceName">Name of the sequence.</param>
        public SqlServerTestSequence(SqlServerProcessor processor, string schemaName, string sequenceName)
        {
            _schemaName = schemaName;
            Name = sequenceName;

            Connection = (SqlConnection)processor.Connection;
            Transaction = (SqlTransaction)processor.Transaction;
            Create();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Drop();
        }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        public void Create()
        {
            if (!string.IsNullOrEmpty(_schemaName))
            {
                using (var command = new SqlCommand(string.Format("CREATE SCHEMA [{0}]", _schemaName), Connection, Transaction))
                    command.ExecuteNonQuery();
            }

            var schema = string.IsNullOrEmpty(_schemaName) ? "dbo" : _schemaName;

            string createCommand = string.Format("CREATE SEQUENCE [{0}].[{1}] INCREMENT BY 2 MINVALUE 0 MAXVALUE 100 START WITH 2 CACHE 10 CYCLE", schema, Name);
            using (var command = new SqlCommand(createCommand, Connection, Transaction))
                command.ExecuteNonQuery();
        }

        /// <summary>
        /// Drops this instance.
        /// </summary>
        public void Drop()
        {
            if (string.IsNullOrEmpty(_schemaName))
            {
                using (var command = new SqlCommand("DROP SEQUENCE " + Name, Connection, Transaction))
                    command.ExecuteNonQuery();
            }
            else
            {
                using (var command = new SqlCommand(string.Format("DROP SEQUENCE [{0}].{1}", _schemaName, Name), Connection, Transaction))
                    command.ExecuteNonQuery();

                using (var command = new SqlCommand(string.Format("DROP SCHEMA [{0}]", _schemaName), Connection, Transaction))
                    command.ExecuteNonQuery();
            }
        }
    }
}
