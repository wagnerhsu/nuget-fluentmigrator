// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Oracle
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="Oracle12CGenerator.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
// Copyright (c) 2007-2018, FluentMigrator Project
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

using JetBrains.Annotations;

using Microsoft.Extensions.Options;

namespace FluentMigrator.Runner.Generators.Oracle
{
    /// <summary>
    /// Class Oracle12CGenerator.
    /// Implements the <see cref="FluentMigrator.Runner.Generators.Oracle.OracleGenerator" />
    /// Implements the <see cref="FluentMigrator.Runner.Generators.Oracle.IOracle12CGenerator" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Generators.Oracle.OracleGenerator" />
    /// <seealso cref="FluentMigrator.Runner.Generators.Oracle.IOracle12CGenerator" />
    public class Oracle12CGenerator : OracleGenerator, IOracle12CGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Oracle12CGenerator"/> class.
        /// </summary>
        public Oracle12CGenerator()
            : this(false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Oracle12CGenerator"/> class.
        /// </summary>
        /// <param name="useQuotedIdentifiers">if set to <c>true</c> [use quoted identifiers].</param>
        public Oracle12CGenerator(bool useQuotedIdentifiers)
            : this(GetQuoter(useQuotedIdentifiers))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Oracle12CGenerator"/> class.
        /// </summary>
        /// <param name="quoter">The quoter.</param>
        public Oracle12CGenerator(
            [NotNull] OracleQuoterBase quoter)
            : this(quoter, new OptionsWrapper<GeneratorOptions>(new GeneratorOptions()))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Oracle12CGenerator"/> class.
        /// </summary>
        /// <param name="quoter">The quoter.</param>
        /// <param name="generatorOptions">The generator options.</param>
        public Oracle12CGenerator(
            [NotNull] OracleQuoterBase quoter,
            [NotNull] IOptions<GeneratorOptions> generatorOptions)
            : base(new Oracle12CColumn(quoter), quoter, new OracleDescriptionGenerator(), generatorOptions)
        {
        }
    }
}
