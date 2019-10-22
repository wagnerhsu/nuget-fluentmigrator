// ***********************************************************************
// Assembly         : FluentMigrator.Abstractions
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="MigrationAttribute.cs" company="FluentMigrator Project">
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
    /// Attribute for a migration
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class MigrationAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationAttribute" /> class.
        /// </summary>
        /// <param name="version">The migration version</param>
        /// <param name="description">The migration description</param>
        public MigrationAttribute(long version, string description)
            : this(version, TransactionBehavior.Default, description)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationAttribute" /> class.
        /// </summary>
        /// <param name="version">The migration version</param>
        /// <param name="transactionBehavior">The desired transaction behavior</param>
        /// <param name="description">The migration description</param>
        public MigrationAttribute(long version, TransactionBehavior transactionBehavior = TransactionBehavior.Default, string description = null)
        {
            Version = version;
            TransactionBehavior = transactionBehavior;
            Description = description;
        }

        /// <summary>
        /// Gets the migration version
        /// </summary>
        /// <value>The version.</value>
        public long Version { get; }

        /// <summary>
        /// Gets the desired transaction behavior
        /// </summary>
        /// <value>The transaction behavior.</value>
        public TransactionBehavior TransactionBehavior { get; }

        /// <summary>
        /// Gets the description
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the transaction is a breaking change
        /// </summary>
        /// <value><c>true</c> if [breaking change]; otherwise, <c>false</c>.</value>
        public bool BreakingChange { get; set; }
    }
}
