// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Redshift
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="RedshiftDescriptionGenerator.cs" company="FluentMigrator Project">
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

using FluentMigrator.Runner.Generators.Generic;

namespace FluentMigrator.Runner.Generators.Redshift
{
    /// <summary>
    /// almost copied from OracleDescriptionGenerator,
    /// modified for escaping table description
    /// </summary>
    public class RedshiftDescriptionGenerator : GenericDescriptionGenerator
    {
        /// <summary>
        /// The quoter
        /// </summary>
        private readonly IQuoter _quoter;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedshiftDescriptionGenerator"/> class.
        /// </summary>
        public RedshiftDescriptionGenerator()
        {
            _quoter = new RedshiftQuoter();
        }

        /// <summary>
        /// Gets the quoter.
        /// </summary>
        /// <value>The quoter.</value>
        protected IQuoter Quoter
        {
            get { return _quoter; }
        }

        #region Constants

        /// <summary>
        /// The table description template
        /// </summary>
        private const string TableDescriptionTemplate = "COMMENT ON TABLE {0} IS '{1}';";
        /// <summary>
        /// The column description template
        /// </summary>
        private const string ColumnDescriptionTemplate = "COMMENT ON COLUMN {0}.{1} IS '{2}';";

        #endregion

        /// <summary>
        /// Gets the full name of the table.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>System.String.</returns>
        private string GetFullTableName(string schemaName, string tableName)
        {
            return string.IsNullOrEmpty(schemaName)
               ? Quoter.QuoteTableName(tableName)
               : string.Format("{0}.{1}", Quoter.QuoteSchemaName(schemaName), Quoter.QuoteTableName(tableName));
        }

        /// <summary>
        /// Generates the table description.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="tableDescription">The table description.</param>
        /// <returns>System.String.</returns>
        protected override string GenerateTableDescription(
            string schemaName, string tableName, string tableDescription)
        {
            if (string.IsNullOrEmpty(tableDescription))
                return string.Empty;

            return string.Format(TableDescriptionTemplate, GetFullTableName(schemaName, tableName), tableDescription.Replace("'", "''"));
        }

        /// <summary>
        /// Generates the column description.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="columnDescription">The column description.</param>
        /// <returns>System.String.</returns>
        protected override string GenerateColumnDescription(
            string schemaName, string tableName, string columnName, string columnDescription)
        {
            if (string.IsNullOrEmpty(columnDescription))
                return string.Empty;

            return string.Format(
                ColumnDescriptionTemplate,
                GetFullTableName(schemaName, tableName),
                Quoter.QuoteColumnName(columnName),
                columnDescription.Replace("'", "''"));
        }
    }
}
