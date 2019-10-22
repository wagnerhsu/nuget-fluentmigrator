// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="TestVersionTableMetaData.cs" company="FluentMigrator Project">
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

using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.VersionTableInfo;

using Microsoft.Extensions.Options;

#pragma warning disable 3005
namespace FluentMigrator.Tests.Unit
{
    /// <summary>
    /// Class TestVersionTableMetaData.
    /// Implements the <see cref="FluentMigrator.Runner.VersionTableInfo.IVersionTableMetaData" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.VersionTableInfo.IVersionTableMetaData" />
    [VersionTableMetaData]
    public class TestVersionTableMetaData : IVersionTableMetaData
    {
        /// <summary>
        /// The table name
        /// </summary>
        public const string TABLE_NAME = "testVersionTableName";
        /// <summary>
        /// The column name
        /// </summary>
        public const string COLUMN_NAME = "testColumnName";
        /// <summary>
        /// The unique index name
        /// </summary>
        public const string UNIQUE_INDEX_NAME = "testUniqueIndexName";
        /// <summary>
        /// The description column name
        /// </summary>
        public const string DESCRIPTION_COLUMN_NAME = "testDescriptionColumnName";
        /// <summary>
        /// The applied on column name
        /// </summary>
        public const string APPLIED_ON_COLUMN_NAME = "testAppliedOnColumnName";

        /// <summary>
        /// Initializes a new instance of the <see cref="TestVersionTableMetaData" /> class.
        /// </summary>
        /// <param name="options">The runner options</param>
        /// <remarks>This constructor must come first due to a bug in aspnet/DependencyInjection. An issue is already filed.</remarks>
        public TestVersionTableMetaData(IOptions<RunnerOptions> options)
            : this()
        {
#pragma warning disable 612
            ApplicationContext = options.Value.ApplicationContext;
#pragma warning restore 612
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestVersionTableMetaData"/> class.
        /// </summary>
        public TestVersionTableMetaData()
        {
            SchemaName = "testSchemaName";
            OwnsSchema = true;
        }

        /// <summary>
        /// Provides access to <code>ApplicationContext</code> object.
        /// </summary>
        /// <value>The application context.</value>
        /// <remarks>ApplicationContext value is set by FluentMigrator immediately after instantiation of a class
        /// implementing <code>IVersionTableMetaData</code> and before any of properties of <code>IVersionTableMetaData</code>
        /// is called. Properties can use <code>ApplicationContext</code> value to implement context-depending logic.</remarks>
        public object ApplicationContext { get; set; }

        /// <summary>
        /// Gets the name of the schema.
        /// </summary>
        /// <value>The name of the schema.</value>
        public string SchemaName { get; set; }

        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        /// <value>The name of the table.</value>
        public string TableName => TABLE_NAME;

        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        /// <value>The name of the column.</value>
        public string ColumnName => COLUMN_NAME;

        /// <summary>
        /// Gets the name of the unique index.
        /// </summary>
        /// <value>The name of the unique index.</value>
        public string UniqueIndexName => UNIQUE_INDEX_NAME;

        /// <summary>
        /// Gets the name of the applied on column.
        /// </summary>
        /// <value>The name of the applied on column.</value>
        public string AppliedOnColumnName => APPLIED_ON_COLUMN_NAME;

        /// <summary>
        /// Gets the name of the description column.
        /// </summary>
        /// <value>The name of the description column.</value>
        public string DescriptionColumnName => DESCRIPTION_COLUMN_NAME;

        /// <summary>
        /// Gets or sets a value indicating whether [owns schema].
        /// </summary>
        /// <value><c>true</c> if [owns schema]; otherwise, <c>false</c>.</value>
        public bool OwnsSchema { get; set; }
    }
}
