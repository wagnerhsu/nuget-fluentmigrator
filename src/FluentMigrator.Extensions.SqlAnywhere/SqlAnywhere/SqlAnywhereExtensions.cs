// ***********************************************************************
// Assembly         : FluentMigrator.Extensions.SqlAnywhere
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SqlAnywhereExtensions.cs" company="FluentMigrator Project">
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

using FluentMigrator.Builders.Create.Constraint;
using FluentMigrator.Infrastructure;

namespace FluentMigrator.SqlAnywhere
{
    /// <summary>
    /// Extension methods for SQL Anywhere
    /// </summary>
    public static partial class SqlAnywhereExtensions
    {
        /// <summary>
        /// The constraint type
        /// </summary>
        public const string ConstraintType = "SqlAnywhereConstraintType";
        /// <summary>
        /// The schema password
        /// </summary>
        public const string SchemaPassword = "SqlAnywhereSchemaPassword";
        /// <summary>
        /// The with nulls distinct
        /// </summary>
        public const string WithNullsDistinct = "SqlAnywhereNullsDistinct";

        /// <summary>
        /// Sets the index/unique constraint type
        /// </summary>
        /// <param name="expression">The expression</param>
        /// <param name="type">The constraint type</param>
        /// <exception cref="InvalidOperationException"></exception>
        private static void SetConstraintType(ICreateConstraintOptionsSyntax expression, SqlAnywhereConstraintType type)
        {
            if (!(expression is ISupportAdditionalFeatures additionalFeatures))
                throw new InvalidOperationException(UnsupportedMethodMessage(type, nameof(ISupportAdditionalFeatures)));

            additionalFeatures.AdditionalFeatures[ConstraintType] = type;
        }

        /// <summary>
        /// Set the unique/index constraint type to <see cref="SqlAnywhereConstraintType.Clustered" />
        /// </summary>
        /// <param name="expression">The expression</param>
        public static void Clustered(this ICreateConstraintOptionsSyntax expression)
        {
            SetConstraintType(expression, SqlAnywhereConstraintType.Clustered);
        }

        /// <summary>
        /// Set the unique/index constraint type to <see cref="SqlAnywhereConstraintType.NonClustered" />
        /// </summary>
        /// <param name="expression">The expression</param>
        public static void NonClustered(this ICreateConstraintOptionsSyntax expression)
        {
            SetConstraintType(expression, SqlAnywhereConstraintType.NonClustered);
        }

        /// <summary>
        /// Unsupporteds the method message.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="interfaceName">Name of the interface.</param>
        /// <returns>System.String.</returns>
        private static string UnsupportedMethodMessage(object methodName, string interfaceName)
        {
            var msg = string.Format(ErrorMessages.MethodXMustBeCalledOnObjectImplementingY, methodName, interfaceName);
            return msg;
        }
    }
}
