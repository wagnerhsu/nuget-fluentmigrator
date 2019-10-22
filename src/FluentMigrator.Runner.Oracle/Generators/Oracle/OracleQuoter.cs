// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Oracle
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="OracleQuoter.cs" company="FluentMigrator Project">
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

namespace FluentMigrator.Runner.Generators.Oracle
{
    /// <summary>
    /// Class OracleQuoter.
    /// Implements the <see cref="FluentMigrator.Runner.Generators.Oracle.OracleQuoterQuotedIdentifier" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Generators.Oracle.OracleQuoterQuotedIdentifier" />
    public class OracleQuoter : OracleQuoterQuotedIdentifier
    {
        /// <summary>
        /// Quotes the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        public override string Quote(string name)
        {
            return UnQuote(name);
        }

        /// <summary>
        /// Quotes the name of the constraint.
        /// </summary>
        /// <param name="constraintName">Name of the constraint.</param>
        /// <param name="schemaName">Name of the schema.</param>
        /// <returns>System.String.</returns>
        public override string QuoteConstraintName(string constraintName, string schemaName = null)
        {
            return base.QuoteConstraintName(UnQuote(constraintName), UnQuote(schemaName));
        }

        /// <summary>
        /// Quotes the name of the index.
        /// </summary>
        /// <param name="indexName">Name of the index.</param>
        /// <param name="schemaName">Name of the schema.</param>
        /// <returns>System.String.</returns>
        public override string QuoteIndexName(string indexName, string schemaName)
        {
            return base.QuoteIndexName(UnQuote(indexName), UnQuote(schemaName));
        }

        /// <summary>
        /// Quotes the name of the table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="schemaName">Name of the schema.</param>
        /// <returns>System.String.</returns>
        public override string QuoteTableName(string tableName, string schemaName = null)
        {
            return base.QuoteTableName(UnQuote(tableName), UnQuote(schemaName));
        }

        /// <summary>
        /// Quotes the name of the sequence.
        /// </summary>
        /// <param name="sequenceName">Name of the sequence.</param>
        /// <param name="schemaName">Name of the schema.</param>
        /// <returns>System.String.</returns>
        public override string QuoteSequenceName(string sequenceName, string schemaName)
        {
            return base.QuoteTableName(UnQuote(sequenceName), UnQuote(schemaName));
        }
    }
}
