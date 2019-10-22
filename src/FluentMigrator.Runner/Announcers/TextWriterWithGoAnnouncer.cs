// ***********************************************************************
// Assembly         : FluentMigrator.Runner
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="TextWriterWithGoAnnouncer.cs" company="FluentMigrator Project">
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
using System.IO;

using Microsoft.Extensions.Options;

namespace FluentMigrator.Runner.Announcers
{
    /// <summary>
    /// Class TextWriterWithGoAnnouncer.
    /// Implements the <see cref="FluentMigrator.Runner.Announcers.TextWriterAnnouncer" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Announcers.TextWriterAnnouncer" />
    [Obsolete]
    public class TextWriterWithGoAnnouncer : TextWriterAnnouncer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextWriterWithGoAnnouncer"/> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public TextWriterWithGoAnnouncer(TextWriter writer)
            : base(writer)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextWriterWithGoAnnouncer"/> class.
        /// </summary>
        /// <param name="write">The write.</param>
        public TextWriterWithGoAnnouncer(Action<string> write)
            : base(write)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextWriterWithGoAnnouncer"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public TextWriterWithGoAnnouncer(IOptions<TextWriterAnnouncerOptions> options)
            : base(options)
        { }

        /// <summary>
        /// SQLs the specified SQL.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        public override void Sql(string sql)
        {
            if (!ShowSql) return;

            base.Sql(sql);

            if (!string.IsNullOrEmpty(sql))
                Write("GO", false);
        }
    }
}
