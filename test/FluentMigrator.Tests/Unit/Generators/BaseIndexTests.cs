// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="BaseIndexTests.cs" company="FluentMigrator Project">
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
    /// Class BaseIndexTests.
    /// </summary>
    [Category("Generator")]
    [Category("Index")]
    public abstract class BaseIndexTests
    {
        /// <summary>
        /// Determines whether this instance [can create index with custom schema].
        /// </summary>
        public abstract void CanCreateIndexWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can create index with default schema].
        /// </summary>
        public abstract void CanCreateIndexWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can create multi column index with custom schema].
        /// </summary>
        public abstract void CanCreateMultiColumnIndexWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can create multi column index with default schema].
        /// </summary>
        public abstract void CanCreateMultiColumnIndexWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can create multi column unique index with custom schema].
        /// </summary>
        public abstract void CanCreateMultiColumnUniqueIndexWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can create multi column unique index with default schema].
        /// </summary>
        public abstract void CanCreateMultiColumnUniqueIndexWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can create unique index with custom schema].
        /// </summary>
        public abstract void CanCreateUniqueIndexWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can create unique index with default schema].
        /// </summary>
        public abstract void CanCreateUniqueIndexWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can drop index with custom schema].
        /// </summary>
        public abstract void CanDropIndexWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can drop index with default schema].
        /// </summary>
        public abstract void CanDropIndexWithDefaultSchema();
    }
}
