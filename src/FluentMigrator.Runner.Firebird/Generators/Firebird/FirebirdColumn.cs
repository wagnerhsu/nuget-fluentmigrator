// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Firebird
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="FirebirdColumn.cs" company="FluentMigrator Project">
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

using System;
using System.Collections.Generic;
using System.Linq;

using FluentMigrator.Model;
using FluentMigrator.Runner.Generators.Base;
using FluentMigrator.Runner.Processors.Firebird;

using JetBrains.Annotations;

namespace FluentMigrator.Runner.Generators.Firebird
{
    /// <summary>
    /// Class FirebirdColumn.
    /// Implements the <see cref="FluentMigrator.Runner.Generators.Base.ColumnBase" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Generators.Base.ColumnBase" />
    internal class FirebirdColumn : ColumnBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FirebirdColumn"/> class.
        /// </summary>
        /// <param name="fbOptions">The fb options.</param>
        public FirebirdColumn([NotNull] FirebirdOptions fbOptions) : base(new FirebirdTypeMap(), new FirebirdQuoter(fbOptions.ForceQuote))
        {
            FBOptions = fbOptions;

            //In firebird DEFAULT clause precedes NULLABLE clause
            ClauseOrder = new List<Func<ColumnDefinition, string>> { FormatString, FormatType, FormatDefaultValue, FormatNullable, FormatPrimaryKey, FormatIdentity };
        }

        /// <summary>
        /// Gets the fb options.
        /// </summary>
        /// <value>The fb options.</value>
        protected FirebirdOptions FBOptions { get; }

        /// <summary>
        /// Formats the identity SQL fragment
        /// </summary>
        /// <param name="column">The column definition</param>
        /// <returns>The formatted identity SQL fragment</returns>
        protected override string FormatIdentity(ColumnDefinition column)
        {
            //Identity not supported
           return string.Empty;
        }

        /// <summary>
        /// Gets the name of the primary key constraint. Some Generators may need to override if the constraint name is limited
        /// </summary>
        /// <param name="primaryKeyColumns">The primary key columns</param>
        /// <param name="tableName">The table name</param>
        /// <returns>The constraint clause</returns>
        /// <exception cref="ArgumentException">Name too long: {primaryKeyName}</exception>
        protected override string GetPrimaryKeyConstraintName(IEnumerable<ColumnDefinition> primaryKeyColumns, string tableName)
        {
            string primaryKeyName = primaryKeyColumns.Select(x => x.PrimaryKeyName).FirstOrDefault();

            if (string.IsNullOrEmpty(primaryKeyName))
            {
                return string.Empty;
            }
            else if (primaryKeyName.Length > FirebirdOptions.MaxNameLength)
            {
                if (!FBOptions.TruncateLongNames)
                    throw new ArgumentException($"Name too long: {primaryKeyName}");
                primaryKeyName = primaryKeyName.Substring(0, Math.Min(FirebirdOptions.MaxNameLength, primaryKeyName.Length));
            }

            return $"CONSTRAINT {Quoter.QuoteIndexName(primaryKeyName)} ";
        }

        /// <summary>
        /// Generates for type alter.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns>System.String.</returns>
        public virtual string GenerateForTypeAlter(ColumnDefinition column)
        {
            return FormatType(column);
        }

        /// <summary>
        /// Generates for default alter.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns>System.String.</returns>
        public virtual string GenerateForDefaultAlter(ColumnDefinition column)
        {
            return FormatDefaultValue(column);
        }
    }
}
