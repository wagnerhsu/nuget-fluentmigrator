// ***********************************************************************
// Assembly         : FluentMigrator.Abstractions
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="IMigrationContext.cs" company="FluentMigrator Project">
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
using System.Collections.Generic;

using FluentMigrator.Expressions;

using JetBrains.Annotations;

namespace FluentMigrator.Infrastructure
{
    /// <summary>
    /// The context of a migration while collecting up/down expressions
    /// </summary>
    public interface IMigrationContext
    {
        /// <summary>
        /// Gets the service provider used to create this migration context
        /// </summary>
        /// <value>The service provider.</value>
        [NotNull]
        IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Gets or sets the collection of expressions
        /// </summary>
        /// <value>The expressions.</value>
        ICollection<IMigrationExpression> Expressions { get; set; }

        /// <summary>
        /// Gets the <see cref="IQuerySchema" /> to access the database
        /// </summary>
        /// <value>The query schema.</value>
        IQuerySchema QuerySchema { get; }

        /// <summary>
        /// Gets or sets the collection of migration assemblies
        /// </summary>
        /// <value>The migration assemblies.</value>
        [Obsolete]
        [CanBeNull]
        IAssemblyCollection MigrationAssemblies { get; set; }

        /// <summary>
        /// Gets or sets the arbitrary application context passed to the task runner
        /// </summary>
        /// <value>The application context.</value>
        [Obsolete]
        object ApplicationContext { get; set; }

        /// <summary>
        /// Gets or sets the connection string
        /// </summary>
        /// <value>The connection.</value>
        string Connection { get; set; }
    }
}
