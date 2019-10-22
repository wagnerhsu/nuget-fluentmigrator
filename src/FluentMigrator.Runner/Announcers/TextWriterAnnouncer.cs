// ***********************************************************************
// Assembly         : FluentMigrator.Runner
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="TextWriterAnnouncer.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License

// Copyright (c) 2007-2018, Sean Chambers <schambers80@gmail.com>
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System;
using System.IO;

using Microsoft.Extensions.Options;

namespace FluentMigrator.Runner.Announcers
{
    /// <summary>
    /// Class TextWriterAnnouncer.
    /// Implements the <see cref="FluentMigrator.Runner.Announcers.Announcer" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Announcers.Announcer" />
    [Obsolete]
    public class TextWriterAnnouncer : Announcer
    {
        /// <summary>
        /// The write
        /// </summary>
        private readonly Action<string> _write;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextWriterAnnouncer"/> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public TextWriterAnnouncer(TextWriter writer)
            : this(writer.Write)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextWriterAnnouncer"/> class.
        /// </summary>
        /// <param name="write">The write.</param>
        public TextWriterAnnouncer(Action<string> write)
        {
            _write = write;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextWriterAnnouncer"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public TextWriterAnnouncer(IOptions<TextWriterAnnouncerOptions> options)
            : base(options)
        {
            _write = options.Value.WriteDelegate;
        }

        /// <summary>
        /// Headings the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public override void Heading(string message)
        {
            base.Heading(string.Format("{0} ", message).PadRight(75, '='));
            _write(Environment.NewLine);
        }

        /// <summary>
        /// Elapseds the time.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        public override void ElapsedTime(TimeSpan timeSpan)
        {
            base.ElapsedTime(timeSpan);
            _write(Environment.NewLine);
        }

        /// <summary>
        /// Writes the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="isNotSql">if set to <c>true</c> [is not SQL].</param>
        public override void Write(string message, bool isNotSql = true)
        {
            _write(isNotSql ? string.Format("/* {0} */", message) : message);
            _write(Environment.NewLine);
        }
    }
}
