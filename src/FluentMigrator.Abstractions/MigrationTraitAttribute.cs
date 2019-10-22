// ***********************************************************************
// Assembly         : FluentMigrator.Abstractions
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="MigrationTraitAttribute.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
//
// Copyright (c) 2007-2018, Sean Chambers <schambers80@gmail.com>
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

using System;

namespace FluentMigrator
{
    /// <summary>
    /// A trait for a migration
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class MigrationTraitAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationTraitAttribute" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public MigrationTraitAttribute(string name)
            : this(name, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationTraitAttribute" /> class.
        /// </summary>
        /// <param name="name">The trait name</param>
        /// <param name="value">The trait value</param>
        public MigrationTraitAttribute(string name, object value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Gets the trait name
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; }

        /// <summary>
        /// Gets the trait value
        /// </summary>
        /// <value>The value.</value>
        public object Value { get; }
    }
}
