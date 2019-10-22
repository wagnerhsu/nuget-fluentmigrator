// ***********************************************************************
// Assembly         : FluentMigrator.Runner
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="DefaultVersionTableMetaData.cs" company="FluentMigrator Project">
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

using FluentMigrator.Expressions;
using FluentMigrator.Runner.Conventions;
using FluentMigrator.Runner.Initialization;

using Microsoft.Extensions.Options;

namespace FluentMigrator.Runner.VersionTableInfo
{
    /// <summary>
    /// Class DefaultVersionTableMetaData.
    /// Implements the <see cref="FluentMigrator.Runner.VersionTableInfo.IVersionTableMetaData" />
    /// Implements the <see cref="FluentMigrator.Expressions.ISchemaExpression" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.VersionTableInfo.IVersionTableMetaData" />
    /// <seealso cref="FluentMigrator.Expressions.ISchemaExpression" />
    public class DefaultVersionTableMetaData : IVersionTableMetaData, ISchemaExpression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultVersionTableMetaData"/> class.
        /// </summary>
        /// <param name="conventionSet">The convention set.</param>
        /// <param name="runnerOptions">The runner options.</param>
        public DefaultVersionTableMetaData(IConventionSet conventionSet, IOptions<RunnerOptions> runnerOptions)
        {
#pragma warning disable 618
#pragma warning disable 612
            ApplicationContext = runnerOptions.Value.ApplicationContext;
#pragma warning restore 612
#pragma warning restore 618
            conventionSet.SchemaConvention?.Apply(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultVersionTableMetaData"/> class.
        /// </summary>
        [Obsolete("Use dependency injection")]
        public DefaultVersionTableMetaData()
            : this(string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultVersionTableMetaData"/> class.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        [Obsolete("Use dependency injection")]
        public DefaultVersionTableMetaData(string schemaName)
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            SchemaName = schemaName ?? string.Empty;
        }

        /// <summary>
        /// Provides access to <code>ApplicationContext</code> object.
        /// </summary>
        /// <value>The application context.</value>
        /// <remarks>ApplicationContext value is set by FluentMigrator immediately after instantiation of a class
        /// implementing <code>IVersionTableMetaData</code> and before any of properties of <code>IVersionTableMetaData</code>
        /// is called. Properties can use <code>ApplicationContext</code> value to implement context-depending logic.</remarks>
        [Obsolete("Use dependency injection to get data using your own services")]
        public object ApplicationContext { get; set; }

        /// <summary>
        /// Gets or sets the name of the schema.
        /// </summary>
        /// <value>The name of the schema.</value>
        public virtual string SchemaName { get; set; }

        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        /// <value>The name of the table.</value>
        public virtual string TableName => "VersionInfo";

        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        /// <value>The name of the column.</value>
        public virtual string ColumnName => "Version";

        /// <summary>
        /// Gets the name of the unique index.
        /// </summary>
        /// <value>The name of the unique index.</value>
        public virtual string UniqueIndexName => "UC_Version";

        /// <summary>
        /// Gets the name of the applied on column.
        /// </summary>
        /// <value>The name of the applied on column.</value>
        public virtual string AppliedOnColumnName => "AppliedOn";

        /// <summary>
        /// Gets the name of the description column.
        /// </summary>
        /// <value>The name of the description column.</value>
        public virtual string DescriptionColumnName => "Description";

        /// <summary>
        /// Gets a value indicating whether [owns schema].
        /// </summary>
        /// <value><c>true</c> if [owns schema]; otherwise, <c>false</c>.</value>
        public virtual bool OwnsSchema => true;
    }
}
