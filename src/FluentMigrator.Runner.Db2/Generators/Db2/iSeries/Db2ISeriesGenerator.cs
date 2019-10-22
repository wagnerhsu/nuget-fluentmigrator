// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Db2
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="Db2ISeriesGenerator.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
// Copyright (c) 2018, FluentMigrator Project
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

using Microsoft.Extensions.Options;

namespace FluentMigrator.Runner.Generators.DB2.iSeries
{
    /// <summary>
    /// Class Db2ISeriesGenerator.
    /// Implements the <see cref="FluentMigrator.Runner.Generators.DB2.Db2Generator" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Generators.DB2.Db2Generator" />
    public class Db2ISeriesGenerator : Db2Generator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Db2ISeriesGenerator"/> class.
        /// </summary>
        public Db2ISeriesGenerator()
            : this(new Db2ISeriesQuoter())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Db2ISeriesGenerator"/> class.
        /// </summary>
        /// <param name="quoter">The quoter.</param>
        public Db2ISeriesGenerator(
            Db2ISeriesQuoter quoter)
            : this(quoter, new OptionsWrapper<GeneratorOptions>(new GeneratorOptions()))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Db2ISeriesGenerator"/> class.
        /// </summary>
        /// <param name="quoter">The quoter.</param>
        /// <param name="generatorOptions">The generator options.</param>
        public Db2ISeriesGenerator(
            Db2ISeriesQuoter quoter,
            IOptions<GeneratorOptions> generatorOptions)
            : base(quoter, generatorOptions)
        {
        }
    }
}
