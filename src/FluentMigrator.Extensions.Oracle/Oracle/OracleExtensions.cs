// ***********************************************************************
// Assembly         : FluentMigrator.Extensions.Oracle
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="OracleExtensions.cs" company="FluentMigrator Project">
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

namespace FluentMigrator.Oracle
{
    /// <summary>
    /// Class OracleExtensions.
    /// </summary>
    public static class OracleExtensions
    {
        /// <summary>
        /// Gets the identity generation.
        /// </summary>
        /// <value>The identity generation.</value>
        public static string IdentityGeneration => "OracleIdentityGeneration";
        /// <summary>
        /// Gets the identity start with.
        /// </summary>
        /// <value>The identity start with.</value>
        public static string IdentityStartWith => "OracleIdentityStartWith";
        /// <summary>
        /// Gets the identity increment by.
        /// </summary>
        /// <value>The identity increment by.</value>
        public static string IdentityIncrementBy => "OracleIdentityIncrementBy";
        /// <summary>
        /// Gets the identity minimum value.
        /// </summary>
        /// <value>The identity minimum value.</value>
        public static string IdentityMinValue => "OracleIdentityMinValue";
        /// <summary>
        /// Gets the identity maximum value.
        /// </summary>
        /// <value>The identity maximum value.</value>
        public static string IdentityMaxValue => "OracleIdentityMaxValue";

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

        /// <summary>
        /// Makes a column an Identity column using the specified generation type.
        /// </summary>
        /// <typeparam name="TNext">The type of the t next.</typeparam>
        /// <typeparam name="TNextFk">The type of the t next fk.</typeparam>
        /// <param name="expression">Column on which to apply the identity.</param>
        /// <param name="generation">The generation type</param>
        /// <returns>TNext.</returns>
        public static TNext Identity<TNext, TNextFk>(
            this IColumnOptionSyntax<TNext, TNextFk> expression,
            OracleGenerationType generation)
            where TNext : IFluentSyntax where TNextFk : IFluentSyntax
        {
            var castColumn = GetColumn(expression);
            return SetIdentity(expression, generation, startWith: null, incrementBy: null, minValue: null, maxValue: null, castColumn);
        }

        /// <summary>
        /// Makes a column an Identity column using the specified generation type, seed and increment values.
        /// </summary>
        /// <typeparam name="TNext">The type of the t next.</typeparam>
        /// <typeparam name="TNextFk">The type of the t next fk.</typeparam>
        /// <param name="expression">Column on which to apply the identity.</param>
        /// <param name="generation">The generation type</param>
        /// <param name="startWith">Starting value of the identity.</param>
        /// <param name="incrementBy">Increment value of the identity.</param>
        /// <returns>TNext.</returns>
        public static TNext Identity<TNext, TNextFk>(
            this IColumnOptionSyntax<TNext, TNextFk> expression,
            OracleGenerationType generation,
            int startWith,
            int incrementBy)
            where TNext : IFluentSyntax where TNextFk : IFluentSyntax
        {
            var castColumn = GetColumn(expression);
            return SetIdentity(expression, generation, startWith, incrementBy, minValue: null, maxValue: null, castColumn);
        }

        /// <summary>
        /// Makes a column an Identity column using the specified generation type, seed and increment values with bigint support.
        /// </summary>
        /// <typeparam name="TNext">The type of the t next.</typeparam>
        /// <typeparam name="TNextFk">The type of the t next fk.</typeparam>
        /// <param name="expression">Column on which to apply the identity.</param>
        /// <param name="generation">The generation type</param>
        /// <param name="startWith">Starting value of the identity.</param>
        /// <param name="incrementBy">Increment value of the identity.</param>
        /// <returns>TNext.</returns>
        public static TNext Identity<TNext, TNextFk>(
            this IColumnOptionSyntax<TNext, TNextFk> expression,
            OracleGenerationType generation,
            long startWith,
            int incrementBy)
            where TNext : IFluentSyntax where TNextFk : IFluentSyntax
        {
            var castColumn = GetColumn(expression);
            return SetIdentity(expression, generation, startWith, incrementBy, minValue: null, maxValue: null, castColumn);
        }

        /// <summary>
        /// Makes a column an Identity column using the specified generation type, startWith, increment, minValue and maxValue with bigint support.
        /// </summary>
        /// <typeparam name="TNext">The type of the t next.</typeparam>
        /// <typeparam name="TNextFk">The type of the t next fk.</typeparam>
        /// <param name="expression">Column on which to apply the identity.</param>
        /// <param name="generation">The generation type</param>
        /// <param name="startWith">Starting value of the identity.</param>
        /// <param name="incrementBy">Increment value of the identity.</param>
        /// <param name="minValue">Min value of the identity.</param>
        /// <param name="maxValue">Max value of the identity.</param>
        /// <returns>TNext.</returns>
        public static TNext Identity<TNext, TNextFk>(
            this IColumnOptionSyntax<TNext, TNextFk> expression,
            OracleGenerationType generation,
            long startWith,
            int incrementBy,
            long minValue,
            long maxValue)
            where TNext : IFluentSyntax where TNextFk : IFluentSyntax
        {
            var castColumn = GetColumn(expression);
            return SetIdentity(expression, generation, startWith, incrementBy, minValue, maxValue, castColumn);
        }

        /// <summary>
        /// Sets the identity.
        /// </summary>
        /// <typeparam name="TNext">The type of the t next.</typeparam>
        /// <typeparam name="TNextFk">The type of the t next fk.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="generation">The generation.</param>
        /// <param name="startWith">The start with.</param>
        /// <param name="incrementBy">The increment by.</param>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="maxValue">The maximum value.</param>
        /// <param name="castColumn">The cast column.</param>
        /// <returns>TNext.</returns>
        private static TNext SetIdentity<TNext, TNextFk>(
            IColumnOptionSyntax<TNext, TNextFk> expression,
            OracleGenerationType generation,
            long? startWith,
            int? incrementBy,
            long? minValue,
            long? maxValue,
            ISupportAdditionalFeatures castColumn)
            where TNext : IFluentSyntax where TNextFk : IFluentSyntax
        {
            castColumn.AdditionalFeatures[IdentityGeneration] = generation;
            castColumn.AdditionalFeatures[IdentityStartWith] = startWith;
            castColumn.AdditionalFeatures[IdentityIncrementBy] = incrementBy;
            castColumn.AdditionalFeatures[IdentityMinValue] = minValue;
            castColumn.AdditionalFeatures[IdentityMaxValue] = maxValue;
            return expression.Identity();
        }

        /// <summary>
        /// Gets the column.
        /// </summary>
        /// <typeparam name="TNext">The type of the t next.</typeparam>
        /// <typeparam name="TNextFk">The type of the t next fk.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>ISupportAdditionalFeatures.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        private static ISupportAdditionalFeatures GetColumn<TNext, TNextFk>(IColumnOptionSyntax<TNext, TNextFk> expression) where TNext : IFluentSyntax where TNextFk : IFluentSyntax
        {
            if (expression is IColumnExpressionBuilder cast1)
            {
                return cast1.Column;
            }

            throw new InvalidOperationException(UnsupportedMethodMessage(nameof(IdentityGeneration), nameof(IColumnExpressionBuilder)));
        }
    }
}
