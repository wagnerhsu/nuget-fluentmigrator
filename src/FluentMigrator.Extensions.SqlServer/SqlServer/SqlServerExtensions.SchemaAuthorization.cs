// ***********************************************************************
// Assembly         : FluentMigrator.Extensions.SqlServer
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SqlServerExtensions.SchemaAuthorization.cs" company="FluentMigrator Project">
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

using System;

using FluentMigrator.Builders.Create.Schema;
using FluentMigrator.Infrastructure;

namespace FluentMigrator.SqlServer
{
    /// <summary>
    /// Class SqlServerExtensions.
    /// </summary>
    public static partial class SqlServerExtensions
    {
        /// <summary>
        /// Sets the schema owner during schema creation
        /// </summary>
        /// <param name="expression">The schema creation expression</param>
        /// <param name="owner">The schema owner</param>
        /// <returns>The next step</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static ICreateSchemaOptionsSyntax Authorization(this ICreateSchemaOptionsSyntax expression, string owner)
        {
            var additionalFeatures = expression as ISupportAdditionalFeatures ??
                throw new InvalidOperationException(UnsupportedMethodMessage(nameof(Authorization), nameof(ISupportAdditionalFeatures)));
            additionalFeatures.AdditionalFeatures[SchemaAuthorization] = owner;
            return expression;
        }
    }
}
