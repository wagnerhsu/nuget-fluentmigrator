// ***********************************************************************
// Assembly         : FluentMigrator.Runner
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="Announcer.cs" company="FluentMigrator Project">
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
    /// Class Announcer.
    /// Implements the <see cref="FluentMigrator.Runner.IAnnouncer" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.IAnnouncer" />
    [Obsolete]
    public abstract class Announcer : IAnnouncer
    {
        /// <summary>
        /// Gets or sets a value indicating whether [show SQL].
        /// </summary>
        /// <value><c>true</c> if [show SQL]; otherwise, <c>false</c>.</value>
        public virtual bool ShowSql { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [show elapsed time].
        /// </summary>
        /// <value><c>true</c> if [show elapsed time]; otherwise, <c>false</c>.</value>
        public virtual bool ShowElapsedTime { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Announcer"/> class.
        /// </summary>
        protected Announcer()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Announcer"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        protected Announcer(IOptions<AnnouncerOptions> options)
        {
            // ReSharper disable VirtualMemberCallInConstructor
            ShowSql = options.Value.ShowSql;
            ShowElapsedTime = options.Value.ShowElapsedTime;
            // ReSharper restore VirtualMemberCallInConstructor
        }

        /// <summary>
        /// Headings the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public virtual void Heading(string message)
        {
            Write(message);
        }

        /// <summary>
        /// Says the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public virtual void Say(string message)
        {
            Write(message);
        }

        /// <summary>
        /// Emphasizes the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public virtual void Emphasize(string message)
        {
            Write(message);
        }

        /// <summary>
        /// SQLs the specified SQL.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        public virtual void Sql(string sql)
        {
            if (!ShowSql) return;

            if (string.IsNullOrEmpty(sql))
                Write("No SQL statement executed.");
            else Write(sql, false);
        }

        /// <summary>
        /// Elapseds the time.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        public virtual void ElapsedTime(TimeSpan timeSpan)
        {
            if (!ShowElapsedTime) return;

            Write(string.Format("=> {0}s", timeSpan.TotalSeconds));
        }

        /// <summary>
        /// Errors the specified exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public virtual void Error(Exception exception)
        {
            while (exception != null)
            {
                Error(exception.Message);
                exception = exception.InnerException;
            }
        }

        /// <summary>
        /// Errors the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public virtual void Error(string message)
        {
            Write(string.Format("!!! {0}", message));
        }

        /// <summary>
        /// Writes the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="isNotSql">if set to <c>true</c> [is not SQL].</param>
        public abstract void Write(string message, bool isNotSql = true);
    }
}
