// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="BaseTableTests.cs" company="FluentMigrator Project">
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
    /// Class BaseTableTests.
    /// </summary>
    [Category("Generator")]
    [Category("Table")]
    public abstract class BaseTableTests
    {
        /// <summary>
        /// Determines whether this instance [can create table with custom column type with custom schema].
        /// </summary>
        public abstract void CanCreateTableWithCustomColumnTypeWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can create table with custom column type with default schema].
        /// </summary>
        public abstract void CanCreateTableWithCustomColumnTypeWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can create table with custom schema].
        /// </summary>
        public abstract void CanCreateTableWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can create table with default schema].
        /// </summary>
        public abstract void CanCreateTableWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can create table with default value explicitly set to null with custom schema].
        /// </summary>
        public abstract void CanCreateTableWithDefaultValueExplicitlySetToNullWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can create table with default value explicitly set to null with default schema].
        /// </summary>
        public abstract void CanCreateTableWithDefaultValueExplicitlySetToNullWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can create table with default value with custom schema].
        /// </summary>
        public abstract void CanCreateTableWithDefaultValueWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can create table with default value with default schema].
        /// </summary>
        public abstract void CanCreateTableWithDefaultValueWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can create table with identity with custom schema].
        /// </summary>
        public abstract void CanCreateTableWithIdentityWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can create table with identity with default schema].
        /// </summary>
        public abstract void CanCreateTableWithIdentityWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can create table with multi column primary key with custom schema].
        /// </summary>
        public abstract void CanCreateTableWithMultiColumnPrimaryKeyWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can create table with multi column primary key with default schema].
        /// </summary>
        public abstract void CanCreateTableWithMultiColumnPrimaryKeyWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can create table with named multi column primary key with custom schema].
        /// </summary>
        public abstract void CanCreateTableWithNamedMultiColumnPrimaryKeyWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can create table with named multi column primary key with default schema].
        /// </summary>
        public abstract void CanCreateTableWithNamedMultiColumnPrimaryKeyWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can create table with named primary key with custom schema].
        /// </summary>
        public abstract void CanCreateTableWithNamedPrimaryKeyWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can create table with named primary key with default schema].
        /// </summary>
        public abstract void CanCreateTableWithNamedPrimaryKeyWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can create table with nullable field with custom schema].
        /// </summary>
        public abstract void CanCreateTableWithNullableFieldWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can create table with nullable field with default schema].
        /// </summary>
        public abstract void CanCreateTableWithNullableFieldWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can create table with primary key with custom schema].
        /// </summary>
        public abstract void CanCreateTableWithPrimaryKeyWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can create table with primary key with default schema].
        /// </summary>
        public abstract void CanCreateTableWithPrimaryKeyWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can drop table with custom schema].
        /// </summary>
        public abstract void CanDropTableWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can drop table with default schema].
        /// </summary>
        public abstract void CanDropTableWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can rename table with custom schema].
        /// </summary>
        public abstract void CanRenameTableWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can rename table with default schema].
        /// </summary>
        public abstract void CanRenameTableWithDefaultSchema();
    }
}
