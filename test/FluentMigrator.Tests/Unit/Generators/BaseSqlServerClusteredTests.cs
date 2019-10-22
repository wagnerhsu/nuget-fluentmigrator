// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="BaseSqlServerClusteredTests.cs" company="FluentMigrator Project">
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

namespace FluentMigrator.Tests.Unit.Generators
{
    /// <summary>
    /// Class BaseSqlServerClusteredTests.
    /// </summary>
    public abstract class BaseSqlServerClusteredTests
    {
        /// <summary>
        /// Determines whether this instance [can create clustered index with custom schema].
        /// </summary>
        public abstract void CanCreateClusteredIndexWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can create clustered index with default schema].
        /// </summary>
        public abstract void CanCreateClusteredIndexWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can create multi column clustered index with custom schema].
        /// </summary>
        public abstract void CanCreateMultiColumnClusteredIndexWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can create multi column clustered index with default schema].
        /// </summary>
        public abstract void CanCreateMultiColumnClusteredIndexWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can create named clustered primary key constraint with custom schema].
        /// </summary>
        public abstract void CanCreateNamedClusteredPrimaryKeyConstraintWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can create named clustered primary key constraint with default schema].
        /// </summary>
        public abstract void CanCreateNamedClusteredPrimaryKeyConstraintWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can create named clustered unique constraint with custom schema].
        /// </summary>
        public abstract void CanCreateNamedClusteredUniqueConstraintWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can create named clustered unique constraint with default schema].
        /// </summary>
        public abstract void CanCreateNamedClusteredUniqueConstraintWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can create named multi column clustered primary key constraint with custom schema].
        /// </summary>
        public abstract void CanCreateNamedMultiColumnClusteredPrimaryKeyConstraintWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can create named multi column clustered primary key constraint with default schema].
        /// </summary>
        public abstract void CanCreateNamedMultiColumnClusteredPrimaryKeyConstraintWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can create named multi column clustered unique constraint with custom schema].
        /// </summary>
        public abstract void CanCreateNamedMultiColumnClusteredUniqueConstraintWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can create named multi column clustered unique constraint with default schema].
        /// </summary>
        public abstract void CanCreateNamedMultiColumnClusteredUniqueConstraintWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can create named multi column non clustered primary key constraint with custom schema].
        /// </summary>
        public abstract void CanCreateNamedMultiColumnNonClusteredPrimaryKeyConstraintWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can create named multi column non clustered primary key constraint with default schema].
        /// </summary>
        public abstract void CanCreateNamedMultiColumnNonClusteredPrimaryKeyConstraintWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can create named multi column non clustered unique constraint with custom schema].
        /// </summary>
        public abstract void CanCreateNamedMultiColumnNonClusteredUniqueConstraintWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can create named multi column non clustered unique constraint with default schema].
        /// </summary>
        public abstract void CanCreateNamedMultiColumnNonClusteredUniqueConstraintWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can create named non clustered primary key constraint with custom schema].
        /// </summary>
        public abstract void CanCreateNamedNonClusteredPrimaryKeyConstraintWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can create named non clustered primary key constraint with default schema].
        /// </summary>
        public abstract void CanCreateNamedNonClusteredPrimaryKeyConstraintWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can create named non clustered unique constraint with custom schema].
        /// </summary>
        public abstract void CanCreateNamedNonClusteredUniqueConstraintWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can create named non clustered unique constraint with default schema].
        /// </summary>
        public abstract void CanCreateNamedNonClusteredUniqueConstraintWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can create unique clustered index with custom schema].
        /// </summary>
        public abstract void CanCreateUniqueClusteredIndexWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can create unique clustered index with default schema].
        /// </summary>
        public abstract void CanCreateUniqueClusteredIndexWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can create unique clustered multi column index with custom schema].
        /// </summary>
        public abstract void CanCreateUniqueClusteredMultiColumnIndexWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can create unique clustered multi column index with default schema].
        /// </summary>
        public abstract void CanCreateUniqueClusteredMultiColumnIndexWithDefaultSchema();
    }
}
