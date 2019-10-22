// ***********************************************************************
// Assembly         : FluentMigrator.Runner.SqlServer
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SqlServer2012Generator.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
//
// Copyright (c) 2007-2018, Sean Chambers <schambers80@gmail.com>
// Copyright (c) 2012, Daniel Lee
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

using System.Text;

using JetBrains.Annotations;

using Microsoft.Extensions.Options;

namespace FluentMigrator.Runner.Generators.SqlServer
{
    /// <summary>
    /// Class SqlServer2012Generator.
    /// Implements the <see cref="FluentMigrator.Runner.Generators.SqlServer.SqlServer2008Generator" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Generators.SqlServer.SqlServer2008Generator" />
    public class SqlServer2012Generator : SqlServer2008Generator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServer2012Generator"/> class.
        /// </summary>
        public SqlServer2012Generator()
            : this(new SqlServer2008Quoter())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServer2012Generator"/> class.
        /// </summary>
        /// <param name="quoter">The quoter.</param>
        public SqlServer2012Generator(
            [NotNull] SqlServer2008Quoter quoter)
            : base(quoter, new OptionsWrapper<GeneratorOptions>(new GeneratorOptions()))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServer2012Generator"/> class.
        /// </summary>
        /// <param name="quoter">The quoter.</param>
        /// <param name="generatorOptions">The generator options.</param>
        public SqlServer2012Generator(
            [NotNull] SqlServer2008Quoter quoter,
            [NotNull] IOptions<GeneratorOptions> generatorOptions)
            : base(quoter, generatorOptions)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServer2012Generator"/> class.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="quoter">The quoter.</param>
        /// <param name="descriptionGenerator">The description generator.</param>
        /// <param name="generatorOptions">The generator options.</param>
        protected SqlServer2012Generator(
            [NotNull] IColumn column,
            [NotNull] IQuoter quoter,
            [NotNull] IDescriptionGenerator descriptionGenerator,
            [NotNull] IOptions<GeneratorOptions> generatorOptions)
            : base(column, quoter, descriptionGenerator, generatorOptions)
        {
        }

        /// <summary>
        /// Generates a <c>CREATE SEQUENCE</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(Expressions.CreateSequenceExpression expression)
        {
            var result = new StringBuilder("CREATE SEQUENCE ");
            var seq = expression.Sequence;
            result.Append(Quoter.QuoteSequenceName(seq.Name, seq.SchemaName));

            if (seq.Increment.HasValue)
            {
                result.AppendFormat(" INCREMENT BY {0}", seq.Increment);
            }

            if (seq.MinValue.HasValue)
            {
                result.AppendFormat(" MINVALUE {0}", seq.MinValue);
            }

            if (seq.MaxValue.HasValue)
            {
                result.AppendFormat(" MAXVALUE {0}", seq.MaxValue);
            }

            if (seq.StartWith.HasValue)
            {
                result.AppendFormat(" START WITH {0}", seq.StartWith);
            }

            const long MINIMUM_CACHE_VALUE = 2;
            if (seq.Cache.HasValue)
            {
                if (seq.Cache.Value < MINIMUM_CACHE_VALUE)
                {
                    return CompatibilityMode.HandleCompatibilty("Cache size must be greater than 1; if you intended to disable caching, set Cache to null.");
                }
                result.AppendFormat(" CACHE {0}", seq.Cache);
            }
            else
            {
                result.Append(" NO CACHE");
            }

            if (seq.Cycle)
            {
                result.Append(" CYCLE");
            }

            return result.ToString();
        }

        /// <summary>
        /// Generates the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>System.String.</returns>
        public override string Generate(Expressions.DeleteSequenceExpression expression)
        {
            return $"DROP SEQUENCE {Quoter.QuoteSequenceName(expression.SequenceName, expression.SchemaName)}";
        }
    }
}
