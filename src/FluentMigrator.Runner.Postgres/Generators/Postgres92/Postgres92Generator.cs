// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Postgres
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="Postgres92Generator.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
// Copyright (c) 2019, FluentMigrator Project
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

using FluentMigrator.Runner.Generators.Postgres;

using JetBrains.Annotations;

using Microsoft.Extensions.Options;

namespace FluentMigrator.Runner.Generators.Postgres92
{
    /// <summary>
    /// Class Postgres92Generator.
    /// Implements the <see cref="FluentMigrator.Runner.Generators.Postgres.PostgresGenerator" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Generators.Postgres.PostgresGenerator" />
    public class Postgres92Generator : PostgresGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Postgres92Generator"/> class.
        /// </summary>
        /// <param name="quoter">The quoter.</param>
        /// <param name="generatorOptions">The generator options.</param>
        public Postgres92Generator(
            [NotNull] PostgresQuoter quoter,
            [NotNull] IOptions<GeneratorOptions> generatorOptions)
            : base(quoter, generatorOptions, new Postgres92TypeMap())
        {
        }
    }
}
