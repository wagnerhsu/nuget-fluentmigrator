// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="HanaTestSequence.cs" company="FluentMigrator Project">
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
using System.Data;

using FluentMigrator.Runner.Generators.Hana;
using FluentMigrator.Runner.Processors.Hana;

using Sap.Data.Hana;

namespace FluentMigrator.Tests.Helpers
{
    /// <summary>
    /// Class HanaTestSequence.
    /// Implements the <see cref="System.IDisposable" />
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class HanaTestSequence: IDisposable
    {
        /// <summary>
        /// The quoter
        /// </summary>
        private readonly HanaQuoter _quoter = new HanaQuoter();
        /// <summary>
        /// The schema name
        /// </summary>
        private readonly string _schemaName;
        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <value>The connection.</value>
        private HanaConnection Connection { get; }
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
        private HanaTransaction Transaction { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HanaTestSequence"/> class.
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <param name="schemaName">Name of the schema.</param>
        /// <param name="sequenceName">Name of the sequence.</param>
        public HanaTestSequence(HanaProcessor processor, string schemaName, string sequenceName)
        {
            _schemaName = schemaName;
            Name = _quoter.QuoteSequenceName(sequenceName, null);

            Connection = (HanaConnection)processor.Connection;
            Transaction = (HanaTransaction)processor.Transaction;
            NameWithSchema = _quoter.QuoteSequenceName(sequenceName, schemaName);
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
            if (Connection.State != ConnectionState.Open)
                Connection.Open();

            if (!string.IsNullOrEmpty(_schemaName))
            {
                using (var command = new HanaCommand($"CREATE SCHEMA \"{_schemaName}\";", Connection, Transaction))
                    command.ExecuteNonQuery();
            }

            string createCommand = $"CREATE SEQUENCE {NameWithSchema} INCREMENT BY 2 MINVALUE 0 MAXVALUE 100 START WITH 2 CACHE 10 CYCLE";
            using (var command = new HanaCommand(createCommand, Connection, Transaction))
                command.ExecuteNonQuery();
        }

        /// <summary>
        /// Drops this instance.
        /// </summary>
        public void Drop()
        {
            using (var command = new HanaCommand("DROP SEQUENCE " + NameWithSchema, Connection, Transaction))
                command.ExecuteNonQuery();

            if (!string.IsNullOrEmpty(_schemaName))
            {
                using (var command = new HanaCommand($"DROP SCHEMA \"{_schemaName}\"", Connection, Transaction))
                    command.ExecuteNonQuery();
            }
        }
    }
}
