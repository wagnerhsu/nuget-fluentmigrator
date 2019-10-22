// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="BaseDataTests.cs" company="FluentMigrator Project">
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

using System.ComponentModel;

namespace FluentMigrator.Tests.Unit.Generators
{
    /// <summary>
    /// Class BaseDataTests.
    /// </summary>
    [Category("Generator")]
    public abstract class BaseDataTests
    {
        /// <summary>
        /// Determines whether this instance [can delete data for all rows with custom schema].
        /// </summary>
        public abstract void CanDeleteDataForAllRowsWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can delete data for all rows with default schema].
        /// </summary>
        public abstract void CanDeleteDataForAllRowsWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can delete data for multiple rows with custom schema].
        /// </summary>
        public abstract void CanDeleteDataForMultipleRowsWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can delete data for multiple rows with default schema].
        /// </summary>
        public abstract void CanDeleteDataForMultipleRowsWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can delete data with custom schema].
        /// </summary>
        public abstract void CanDeleteDataWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can delete data with default schema].
        /// </summary>
        public abstract void CanDeleteDataWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can delete data with database null criteria].
        /// </summary>
        public abstract void CanDeleteDataWithDbNullCriteria();
        /// <summary>
        /// Determines whether this instance [can insert data with custom schema].
        /// </summary>
        public abstract void CanInsertDataWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can insert data with default schema].
        /// </summary>
        public abstract void CanInsertDataWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can insert unique identifier data with custom schema].
        /// </summary>
        public abstract void CanInsertGuidDataWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can insert unique identifier data with default schema].
        /// </summary>
        public abstract void CanInsertGuidDataWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can update data for all data with custom schema].
        /// </summary>
        public abstract void CanUpdateDataForAllDataWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can update data for all data with default schema].
        /// </summary>
        public abstract void CanUpdateDataForAllDataWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can update data with custom schema].
        /// </summary>
        public abstract void CanUpdateDataWithCustomSchema();
        /// <summary>
        /// Determines whether this instance [can update data with default schema].
        /// </summary>
        public abstract void CanUpdateDataWithDefaultSchema();
        /// <summary>
        /// Determines whether this instance [can update data with database null criteria].
        /// </summary>
        public abstract void CanUpdateDataWithDbNullCriteria();
    }
}
