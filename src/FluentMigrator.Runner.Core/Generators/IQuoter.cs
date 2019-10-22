// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Core
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="IQuoter.cs" company="FluentMigrator Project">
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

using JetBrains.Annotations;

namespace FluentMigrator.Runner.Generators
{
    /// <summary>
    /// The interface to be implemented for handling quotes
    /// </summary>
    public interface IQuoter
    {
        /// <summary>
        /// Returns a quoted string that has been correctly escaped
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        string Quote([CanBeNull] string name);

        /// <summary>
        /// Provides an unquoted, unescaped string
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        string UnQuote(string value);

        /// <summary>
        /// Quotes a value to be embedded into an SQL script/statement
        /// </summary>
        /// <param name="value">The value to be quoted</param>
        /// <returns>The quoted value</returns>
        string QuoteValue(object value);

        /// <summary>
        /// Returns true is the value starts and ends with a close quote
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the specified value is quoted; otherwise, <c>false</c>.</returns>
        bool IsQuoted(string value);

        /// <summary>
        /// Quotes a column name
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>System.String.</returns>
        string QuoteColumnName(string columnName);

        /// <summary>
        /// Quotes a Table name
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="schemaName">Name of the schema.</param>
        /// <returns>System.String.</returns>
        string QuoteTableName(string tableName, string schemaName = null);

        /// <summary>
        /// Quote an index name
        /// </summary>
        /// <param name="indexName">Name of the index.</param>
        /// <param name="schemaName">Name of the schema.</param>
        /// <returns>System.String.</returns>
        string QuoteIndexName(string indexName, string schemaName = null);

        /// <summary>
        /// Quotes a constraint name
        /// </summary>
        /// <param name="contraintName">Name of the contraint.</param>
        /// <param name="schemaName">Name of the schema.</param>
        /// <returns>System.String.</returns>
        string QuoteConstraintName(string contraintName, string schemaName = null);

        /// <summary>
        /// Quotes a Sequence name
        /// </summary>
        /// <param name="sequenceName">Name of the sequence.</param>
        /// <param name="schemaName">Name of the schema.</param>
        /// <returns>System.String.</returns>
        string QuoteSequenceName(string sequenceName, string schemaName = null);

        /// <summary>
        /// Quotes a schema name
        /// </summary>
        /// <param name="schemaName">The schema name to quote</param>
        /// <returns>The quoted schema name</returns>
        string QuoteSchemaName(string schemaName);
    }
}
