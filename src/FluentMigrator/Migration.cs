// ***********************************************************************
// Assembly         : FluentMigrator
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="Migration.cs" company="FluentMigrator Project">
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

using FluentMigrator.Builders.Delete;
using FluentMigrator.Builders.Execute;
using FluentMigrator.Builders.Update;

namespace FluentMigrator
{
    /// <summary>
    /// The base migration class for custom SQL queries and data updates/deletions
    /// </summary>
    public abstract class Migration : MigrationBase
    {
        /// <summary>
        /// Gets the starting point for data deletions
        /// </summary>
        /// <value>The delete.</value>
        public IDeleteExpressionRoot Delete
        {
            get { return new DeleteExpressionRoot(Context); }
        }

        /// <summary>
        /// Gets the starting point for SQL execution
        /// </summary>
        /// <value>The execute.</value>
        public IExecuteExpressionRoot Execute
        {
            get { return new ExecuteExpressionRoot(Context); }
        }

        /// <summary>
        /// Gets the starting point for data updates
        /// </summary>
        /// <value>The update.</value>
        public IUpdateExpressionRoot Update
        {
            get { return new UpdateExpressionRoot(Context); }
        }
    }
}
