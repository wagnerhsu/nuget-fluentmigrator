// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="BaseConstraintsTests.cs" company="FluentMigrator Project">
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
    /// Class BaseConstraintsTests.
    /// </summary>
    [Category("Generator")]
    [Category("Constraint")]
    public abstract class BaseConstraintsTests
    {
        /// <summary>
        /// Determines whether this instance [can create foreign key with custom schema].
        /// </summary>
        public abstract void CanCreateForeignKeyWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can create foreign key with default schema].
        /// </summary>
        public abstract void CanCreateForeignKeyWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can create foreign key with different schemas].
        /// </summary>
        public abstract void CanCreateForeignKeyWithDifferentSchemas();
        /// <summary>
        /// Determines whether this instance [can create multi column foreign key with custom schema].
        /// </summary>
        public abstract void CanCreateMultiColumnForeignKeyWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can create multi column foreign key with default schema].
        /// </summary>
        public abstract void CanCreateMultiColumnForeignKeyWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can create multi column foreign key with different schemas].
        /// </summary>
        public abstract void CanCreateMultiColumnForeignKeyWithDifferentSchemas();
        /// <summary>
        /// Determines whether this instance [can create multi column primary key constraint with custom schema].
        /// </summary>
        public abstract void CanCreateMultiColumnPrimaryKeyConstraintWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can create multi column primary key constraint with default schema].
        /// </summary>
        public abstract void CanCreateMultiColumnPrimaryKeyConstraintWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can create multi column unique constraint with custom schema].
        /// </summary>
        public abstract void CanCreateMultiColumnUniqueConstraintWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can create multi column unique constraint with default schema].
        /// </summary>
        public abstract void CanCreateMultiColumnUniqueConstraintWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can create named foreign key with custom schema].
        /// </summary>
        public abstract void CanCreateNamedForeignKeyWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can create named foreign key with default schema].
        /// </summary>
        public abstract void CanCreateNamedForeignKeyWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can create named foreign key with different schemas].
        /// </summary>
        public abstract void CanCreateNamedForeignKeyWithDifferentSchemas();
        /// <summary>
        /// Determines whether this instance [can create named foreign key with on delete and on update options].
        /// </summary>
        public abstract void CanCreateNamedForeignKeyWithOnDeleteAndOnUpdateOptions();
        /// <summary>
        /// Determines whether this instance [can create named foreign key with on delete options] the specified rule.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <param name="output">The output.</param>
        public abstract void CanCreateNamedForeignKeyWithOnDeleteOptions(System.Data.Rule rule, string output);
        /// <summary>
        /// Determines whether this instance [can create named foreign key with on update options] the specified rule.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <param name="output">The output.</param>
        public abstract void CanCreateNamedForeignKeyWithOnUpdateOptions(System.Data.Rule rule, string output);
        /// <summary>
        /// Determines whether this instance [can create named multi column foreign key with custom schema].
        /// </summary>
        public abstract void CanCreateNamedMultiColumnForeignKeyWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can create named multi column foreign key with default schema].
        /// </summary>
        public abstract void CanCreateNamedMultiColumnForeignKeyWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can create named multi column foreign key with different schemas].
        /// </summary>
        public abstract void CanCreateNamedMultiColumnForeignKeyWithDifferentSchemas();
        /// <summary>
        /// Determines whether this instance [can create named multi column primary key constraint with custom schema].
        /// </summary>
        public abstract void CanCreateNamedMultiColumnPrimaryKeyConstraintWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can create named multi column primary key constraint with default schema].
        /// </summary>
        public abstract void CanCreateNamedMultiColumnPrimaryKeyConstraintWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can create named multi column unique constraint with custom schema].
        /// </summary>
        public abstract void CanCreateNamedMultiColumnUniqueConstraintWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can create named multi column unique constraint with default schema].
        /// </summary>
        public abstract void CanCreateNamedMultiColumnUniqueConstraintWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can create named primary key constraint with custom schema].
        /// </summary>
        public abstract void CanCreateNamedPrimaryKeyConstraintWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can create named primary key constraint with default schema].
        /// </summary>
        public abstract void CanCreateNamedPrimaryKeyConstraintWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can create named unique constraint with custom schema].
        /// </summary>
        public abstract void CanCreateNamedUniqueConstraintWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can create named unique constraint with default schema].
        /// </summary>
        public abstract void CanCreateNamedUniqueConstraintWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can create primary key constraint with custom schema].
        /// </summary>
        public abstract void CanCreatePrimaryKeyConstraintWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can create primary key constraint with default schema].
        /// </summary>
        public abstract void CanCreatePrimaryKeyConstraintWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can create unique constraint with custom schema].
        /// </summary>
        public abstract void CanCreateUniqueConstraintWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can create unique constraint with default schema].
        /// </summary>
        public abstract void CanCreateUniqueConstraintWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can drop foreign key with custom schema].
        /// </summary>
        public abstract void CanDropForeignKeyWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can drop foreign key with default schema].
        /// </summary>
        public abstract void CanDropForeignKeyWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can drop primary key constraint with custom schema].
        /// </summary>
        public abstract void CanDropPrimaryKeyConstraintWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can drop primary key constraint with default schema].
        /// </summary>
        public abstract void CanDropPrimaryKeyConstraintWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can drop unique constraint with custom schema].
        /// </summary>
        public abstract void CanDropUniqueConstraintWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can drop unique constraint with default schema].
        /// </summary>
        public abstract void CanDropUniqueConstraintWithDefaultSchema();
    }
}
