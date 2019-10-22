// ***********************************************************************
// Assembly         : FluentMigrator.Runner
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="TypeExtensions.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License

// Copyright (c) 2007-2018, Sean Chambers <schambers80@gmail.com>
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

namespace FluentMigrator.Runner.Extensions
{
    /// <summary>
    /// Class TypeExtensions.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Determines whether [is] [the specified type].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if [is] [the specified type]; otherwise, <c>false</c>.</returns>
        public static bool Is<T>(this Type type)
        {
            return typeof (T).IsAssignableFrom(type);
        }

        /// <summary>
        /// Determines whether the specified type is concrete.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the specified type is concrete; otherwise, <c>false</c>.</returns>
        public static bool IsConcrete(this Type type)
        {
            return type.IsClass && !type.IsAbstract;
        }
    }
}
