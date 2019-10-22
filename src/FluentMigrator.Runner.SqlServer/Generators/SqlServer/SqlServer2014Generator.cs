// ***********************************************************************
// Assembly         : FluentMigrator.Runner.SqlServer
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SqlServer2014Generator.cs" company="FluentMigrator Project">
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

namespace FluentMigrator.Runner.Generators.SqlServer
{
    /// <summary>
    /// Class SqlServer2014Generator.
    /// Implements the <see cref="FluentMigrator.Runner.Generators.SqlServer.SqlServer2012Generator" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Generators.SqlServer.SqlServer2012Generator" />
    public class SqlServer2014Generator : SqlServer2012Generator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServer2014Generator"/> class.
        /// </summary>
        public SqlServer2014Generator()
            : this(new SqlServer2008Quoter())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServer2014Generator"/> class.
        /// </summary>
        /// <param name="quoter">The quoter.</param>
        public SqlServer2014Generator(
            [NotNull] SqlServer2008Quoter quoter)
            : base(quoter, new OptionsWrapper<GeneratorOptions>(new GeneratorOptions()))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServer2014Generator"/> class.
        /// </summary>
        /// <param name="quoter">The quoter.</param>
        /// <param name="generatorOptions">The generator options.</param>
        public SqlServer2014Generator(
            [NotNull] SqlServer2008Quoter quoter,
            [NotNull] IOptions<GeneratorOptions> generatorOptions)
            : base(quoter, generatorOptions)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServer2014Generator"/> class.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="quoter">The quoter.</param>
        /// <param name="descriptionGenerator">The description generator.</param>
        /// <param name="generatorOptions">The generator options.</param>
        protected SqlServer2014Generator(
            [NotNull] IColumn column,
            [NotNull] IQuoter quoter,
            [NotNull] IDescriptionGenerator descriptionGenerator,
            [NotNull] IOptions<GeneratorOptions> generatorOptions)
            : base(column, quoter, descriptionGenerator, generatorOptions)
        {
        }
    }
}
