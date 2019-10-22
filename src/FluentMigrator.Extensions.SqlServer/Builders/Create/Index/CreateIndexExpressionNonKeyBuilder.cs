// ***********************************************************************
// Assembly         : FluentMigrator.Extensions.SqlServer
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="CreateIndexExpressionNonKeyBuilder.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
// Copyright (c) 2007-2018, Sean Chambers and the FluentMigrator Project
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

using FluentMigrator.Infrastructure;
using FluentMigrator.SqlServer;

namespace FluentMigrator.Builders.Create.Index
{
    /// <summary>
    /// Class CreateIndexExpressionNonKeyBuilder.
    /// Implements the <see cref="FluentMigrator.Builders.Create.Index.ICreateIndexNonKeyColumnSyntax" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Builders.Create.Index.ICreateIndexNonKeyColumnSyntax" />
    internal class CreateIndexExpressionNonKeyBuilder : ICreateIndexNonKeyColumnSyntax
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateIndexExpressionNonKeyBuilder"/> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="supportAdditionalFeatures">The support additional features.</param>
        public CreateIndexExpressionNonKeyBuilder(ICreateIndexOnColumnSyntax expression, ISupportAdditionalFeatures supportAdditionalFeatures)
        {
            Expression = expression;
            SupportAdditionalFeatures = supportAdditionalFeatures;
        }

        /// <summary>
        /// Gets the expression.
        /// </summary>
        /// <value>The expression.</value>
        public ICreateIndexOnColumnSyntax Expression { get; }

        /// <summary>
        /// Gets the support additional features.
        /// </summary>
        /// <value>The support additional features.</value>
        public ISupportAdditionalFeatures SupportAdditionalFeatures { get; }

        /// <summary>
        /// Includes the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>ICreateIndexNonKeyColumnSyntax.</returns>
        public ICreateIndexNonKeyColumnSyntax Include(string columnName)
        {
            SupportAdditionalFeatures.Include(columnName);
            return this;
        }
    }
}
