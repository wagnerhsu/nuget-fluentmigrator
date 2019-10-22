// ***********************************************************************
// Assembly         : FluentMigrator.Runner
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="ConsoleAnnouncer.cs" company="FluentMigrator Project">
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

using Microsoft.Extensions.Options;

namespace FluentMigrator.Runner.Announcers
{
    /// <summary>
    /// Class ConsoleAnnouncer.
    /// Implements the <see cref="FluentMigrator.Runner.Announcers.Announcer" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Announcers.Announcer" />
    [Obsolete]
    public class ConsoleAnnouncer : Announcer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleAnnouncer"/> class.
        /// </summary>
        public ConsoleAnnouncer()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleAnnouncer"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public ConsoleAnnouncer(IOptions<AnnouncerOptions> options)
            : base(options)
        {
        }

        /// <summary>
        /// Headers this instance.
        /// </summary>
        public void Header()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            HorizontalRule();
            Write("=============================== FluentMigrator ================================");
            HorizontalRule();
            Write("Source Code:");
            Write("  https://github.com/fluentmigrator/fluentmigrator");
            Write("Ask For Help:");
            Write("  https://gitter.im/FluentMigrator/fluentmigrator");
            HorizontalRule();
            Console.ResetColor();
        }

        /// <summary>
        /// Horizontals the rule.
        /// </summary>
        public void HorizontalRule()
        {
            Write("".PadRight(79, '-'));
        }

        /// <summary>
        /// Headings the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public override void Heading(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            HorizontalRule();
            base.Heading(message);
            HorizontalRule();
            Console.ResetColor();
        }

        /// <summary>
        /// Says the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public override void Say(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            base.Say(string.Format("[+] {0}", message));
            Console.ResetColor();
        }

        /// <summary>
        /// Emphasizes the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public override void Emphasize(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            base.Say(string.Format("[+] {0}", message));
            Console.ResetColor();
        }

        /// <summary>
        /// Elapseds the time.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        public override void ElapsedTime(TimeSpan timeSpan)
        {
            Console.ResetColor();
            base.ElapsedTime(timeSpan);
        }

        /// <summary>
        /// Errors the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public override void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine("!!! {0}", message);
            Console.ResetColor();
        }

        /// <summary>
        /// Writes the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="isNotSql">if set to <c>true</c> [is not SQL].</param>
        public override void Write(string message, bool isNotSql = true)
        {
            Console.Out.WriteLine(message);
        }
    }
}
