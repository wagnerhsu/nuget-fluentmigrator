// ***********************************************************************
// Assembly         : FluentMigrator.Runner.SqlServer
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SqlServer2008Generator.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
//
// Copyright (c) 2010, Nathan Brown
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

using System.Collections.Generic;
using System.Linq;

using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure.Extensions;
using FluentMigrator.Model;
using FluentMigrator.SqlServer;

using JetBrains.Annotations;

using Microsoft.Extensions.Options;

namespace FluentMigrator.Runner.Generators.SqlServer
{
    /// <summary>
    /// Class SqlServer2008Generator.
    /// Implements the <see cref="FluentMigrator.Runner.Generators.SqlServer.SqlServer2005Generator" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Generators.SqlServer.SqlServer2005Generator" />
    public class SqlServer2008Generator : SqlServer2005Generator
    {
        /// <summary>
        /// The supported additional features
        /// </summary>
        private static readonly HashSet<string> _supportedAdditionalFeatures = new HashSet<string>
        {
            SqlServerExtensions.IndexColumnNullsDistinct,
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServer2008Generator"/> class.
        /// </summary>
        public SqlServer2008Generator()
            : this(new SqlServer2008Quoter())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServer2008Generator"/> class.
        /// </summary>
        /// <param name="quoter">The quoter.</param>
        public SqlServer2008Generator(
            [NotNull] SqlServer2008Quoter quoter)
            : this(quoter, new OptionsWrapper<GeneratorOptions>(new GeneratorOptions()))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServer2008Generator"/> class.
        /// </summary>
        /// <param name="quoter">The quoter.</param>
        /// <param name="generatorOptions">The generator options.</param>
        public SqlServer2008Generator(
            [NotNull] SqlServer2008Quoter quoter,
            [NotNull] IOptions<GeneratorOptions> generatorOptions)
            : this(
                new SqlServer2008Column(new SqlServer2008TypeMap(), quoter),
                quoter,
                new SqlServer2005DescriptionGenerator(),
                generatorOptions)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServer2008Generator"/> class.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="quoter">The quoter.</param>
        /// <param name="descriptionGenerator">The description generator.</param>
        /// <param name="generatorOptions">The generator options.</param>
        protected SqlServer2008Generator(
            [NotNull] IColumn column,
            [NotNull] IQuoter quoter,
            [NotNull] IDescriptionGenerator descriptionGenerator,
            [NotNull] IOptions<GeneratorOptions> generatorOptions)
            : base(column, quoter, descriptionGenerator, generatorOptions)
        {
        }

        /// <summary>
        /// Determines whether [is additional feature supported] [the specified feature].
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <returns><c>true</c> if [is additional feature supported] [the specified feature]; otherwise, <c>false</c>.</returns>
        public override bool IsAdditionalFeatureSupported(string feature)
        {
            return _supportedAdditionalFeatures.Contains(feature)
             || base.IsAdditionalFeatureSupported(feature);
        }

        /// <summary>
        /// Gets the with nulls distinct string.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>System.String.</returns>
        public virtual string GetWithNullsDistinctString(IndexDefinition index)
        {
            bool? GetNullsDistinct(IndexColumnDefinition column)
            {
                return column.GetAdditionalFeature(SqlServerExtensions.IndexColumnNullsDistinct, (bool?) null);
            }

            var indexNullsDistinct = index.GetAdditionalFeature(SqlServerExtensions.IndexColumnNullsDistinct, (bool?) null);

            var nullDistinctColumns = index.Columns.Where(c => indexNullsDistinct != null || GetNullsDistinct(c) != null).ToList();
            if (nullDistinctColumns.Count != 0 && !index.IsUnique)
            {
                // Should never occur
                CompatibilityMode.HandleCompatibilty("With nulls distinct can only be used for unique indexes");
                return string.Empty;
            }

            // The "Nulls (not) distinct" value of the column
            // takes higher precedence than the value of the index
            // itself.
            var conditions = nullDistinctColumns
                .Where(x => (GetNullsDistinct(x) ?? indexNullsDistinct ?? true) == false)
                .Select(c => $"{Quoter.QuoteColumnName(c.Name)} IS NOT NULL");

            var condition = string.Join(" AND ", conditions);
            if (condition.Length == 0)
                return string.Empty;

            return $" WHERE {condition}";
        }

        /// <summary>
        /// Generates the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>System.String.</returns>
        public override string Generate(CreateIndexExpression expression)
        {
            var sql = base.Generate(expression);
            sql += GetWithNullsDistinctString(expression.Index);
            return sql;
        }
    }
}
