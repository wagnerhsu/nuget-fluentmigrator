// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Core
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="AnnouncerOptions.cs" company="FluentMigrator Project">
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

using System;

namespace FluentMigrator.Runner.Announcers
{
    /// <summary>
    /// Options for the <see cref="IAnnouncer" />
    /// </summary>
    [Obsolete]
    public class AnnouncerOptions
    {
        /// <summary>
        /// All enabled
        /// </summary>
        [Obsolete]
        public static readonly AnnouncerOptions AllEnabled = new AnnouncerOptions()
        {
            ShowElapsedTime = true,
            ShowSql = true,
        };

        /// <summary>
        /// Gets or sets a value indicating whether SQL statements should be shown
        /// </summary>
        /// <value><c>true</c> if [show SQL]; otherwise, <c>false</c>.</value>
        public bool ShowSql { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the elapsed time should be shown
        /// </summary>
        /// <value><c>true</c> if [show elapsed time]; otherwise, <c>false</c>.</value>
        public bool ShowElapsedTime { get; set; }
    }
}
