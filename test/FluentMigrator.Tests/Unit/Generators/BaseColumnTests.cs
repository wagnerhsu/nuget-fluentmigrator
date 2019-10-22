// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="BaseColumnTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
//
// Copyright (c) 2018, Fluent Migrator Project
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

using NUnit.Framework;

namespace FluentMigrator.Tests.Unit.Generators
{
    /// <summary>
    /// Class BaseColumnTests.
    /// </summary>
    [Category("Generator")]
    [Category("Column")]
    public abstract class BaseColumnTests
    {
        /// <summary>
        /// Determines whether this instance [can alter column with custom schema].
        /// </summary>
        public abstract void CanAlterColumnWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can alter column with default schema].
        /// </summary>
        public abstract void CanAlterColumnWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can create automatic increment column with custom schema].
        /// </summary>
        public abstract void CanCreateAutoIncrementColumnWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can create automatic increment column with default schema].
        /// </summary>
        public abstract void CanCreateAutoIncrementColumnWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can create column with custom schema].
        /// </summary>
        public abstract void CanCreateColumnWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can create column with default schema].
        /// </summary>
        public abstract void CanCreateColumnWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can create column with system method and custom schema].
        /// </summary>
        public abstract void CanCreateColumnWithSystemMethodAndCustomSchema();
        /// <summary>
        /// Determines whether this instance [can create column with system method and default schema].
        /// </summary>
        public abstract void CanCreateColumnWithSystemMethodAndDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can create decimal column with custom schema].
        /// </summary>
        public abstract void CanCreateDecimalColumnWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can create decimal column with default schema].
        /// </summary>
        public abstract void CanCreateDecimalColumnWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can drop column with custom schema].
        /// </summary>
        public abstract void CanDropColumnWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can drop column with default schema].
        /// </summary>
        public abstract void CanDropColumnWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can drop multiple columns with custom schema].
        /// </summary>
        public abstract void CanDropMultipleColumnsWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can drop multiple columns with default schema].
        /// </summary>
        public abstract void CanDropMultipleColumnsWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can rename column with custom schema].
        /// </summary>
        public abstract void CanRenameColumnWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can rename column with default schema].
        /// </summary>
        public abstract void CanRenameColumnWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can create nullable column with custom domain type and custom schema].
        /// </summary>
        public abstract void CanCreateNullableColumnWithCustomDomainTypeAndCustomSchema();
        /// <summary>
        /// Determines whether this instance [can create nullable column with custom domain type and default schema].
        /// </summary>
        public abstract void CanCreateNullableColumnWithCustomDomainTypeAndDefaultSchema();
    }
}
