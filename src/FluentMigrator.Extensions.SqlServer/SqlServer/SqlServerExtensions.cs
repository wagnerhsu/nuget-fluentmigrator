// ***********************************************************************
// Assembly         : FluentMigrator.Extensions.SqlServer
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SqlServerExtensions.cs" company="FluentMigrator Project">
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

using FluentMigrator.Builders;
using FluentMigrator.Builders.Create.Constraint;
using FluentMigrator.Builders.Create.Table;
using FluentMigrator.Builders.Insert;
using FluentMigrator.Infrastructure;

namespace FluentMigrator.SqlServer
{
    /// <summary>
    /// Class SqlServerExtensions.
    /// </summary>
    public static partial class SqlServerExtensions
    {
        /// <summary>
        /// The identity insert
        /// </summary>
        public static readonly string IdentityInsert = "SqlServerIdentityInsert";
        /// <summary>
        /// The identity seed
        /// </summary>
        public static readonly string IdentitySeed = "SqlServerIdentitySeed";
        /// <summary>
        /// The identity increment
        /// </summary>
        public static readonly string IdentityIncrement = "SqlServerIdentityIncrement";
        /// <summary>
        /// The constraint type
        /// </summary>
        public static readonly string ConstraintType = "SqlServerConstraintType";
        /// <summary>
        /// The includes list
        /// </summary>
        public static readonly string IncludesList = "SqlServerIncludes";
        /// <summary>
        /// The online index
        /// </summary>
        public static readonly string OnlineIndex = "SqlServerOnlineIndex";
        /// <summary>
        /// The row unique identifier column
        /// </summary>
        public static readonly string RowGuidColumn = "SqlServerRowGuidColumn";
        /// <summary>
        /// The index column nulls distinct
        /// </summary>
        public static readonly string IndexColumnNullsDistinct = "SqlServerIndexColumnNullsDistinct";
        /// <summary>
        /// The schema authorization
        /// </summary>
        public static readonly string SchemaAuthorization = "SqlServerSchemaAuthorization";
        /// <summary>
        /// The sparse column
        /// </summary>
        public static readonly string SparseColumn = "SqlServerSparseColumn";

        /// <summary>
        /// Inserts data using Sql Server's IDENTITY INSERT feature.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>IInsertDataSyntax.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static IInsertDataSyntax WithIdentityInsert(this IInsertDataSyntax expression)
        {
            var castExpression = expression as ISupportAdditionalFeatures ??
                throw new InvalidOperationException(UnsupportedMethodMessage(nameof(WithIdentityInsert), nameof(ISupportAdditionalFeatures)));
            castExpression.AdditionalFeatures[IdentityInsert] = true;
            return expression;
        }

        /// <summary>
        /// Sets the type of the constraint.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="type">The type.</param>
        /// <exception cref="InvalidOperationException"></exception>
        private static void SetConstraintType(ICreateConstraintOptionsSyntax expression, SqlServerConstraintType type)
        {
            if (!(expression is ISupportAdditionalFeatures additionalFeatures))
                throw new InvalidOperationException(UnsupportedMethodMessage(type, nameof(ISupportAdditionalFeatures)));

            additionalFeatures.AdditionalFeatures[ConstraintType] = type;
        }

        /// <summary>
        /// Clustereds the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public static void Clustered(this ICreateConstraintOptionsSyntax expression)
        {
            SetConstraintType(expression, SqlServerConstraintType.Clustered);
        }

        /// <summary>
        /// Nons the clustered.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public static void NonClustered(this ICreateConstraintOptionsSyntax expression)
        {
            SetConstraintType(expression, SqlServerConstraintType.NonClustered);
        }

        /// <summary>
        /// Rows the unique identifier.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>ICreateTableColumnOptionOrWithColumnSyntax.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static ICreateTableColumnOptionOrWithColumnSyntax RowGuid(this ICreateTableColumnOptionOrWithColumnSyntax expression)
        {
            var columnExpression = expression as IColumnExpressionBuilder ??
                throw new InvalidOperationException(UnsupportedMethodMessage(nameof(RowGuid), nameof(IColumnExpressionBuilder)));
            columnExpression.Column.AdditionalFeatures[RowGuidColumn] = true;
            return expression;
        }

        /// <summary>
        /// Sparses the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>ICreateTableColumnOptionOrWithColumnSyntax.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static ICreateTableColumnOptionOrWithColumnSyntax Sparse(this ICreateTableColumnOptionOrWithColumnSyntax expression)
        {
            var columnExpression = expression as IColumnExpressionBuilder ??
                throw new InvalidOperationException(UnsupportedMethodMessage(nameof(Sparse), nameof(IColumnExpressionBuilder)));
            columnExpression.Column.AdditionalFeatures[SparseColumn] = true;
            return expression;
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
