// ***********************************************************************
// Assembly         : FluentMigrator.Runner
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="CompositeAnnouncer.cs" company="FluentMigrator Project">
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
using System.Collections.Generic;

namespace FluentMigrator.Runner.Announcers
{
    /// <summary>
    /// Class CompositeAnnouncer.
    /// Implements the <see cref="FluentMigrator.Runner.IAnnouncer" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.IAnnouncer" />
    [Obsolete]
    public class CompositeAnnouncer : IAnnouncer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeAnnouncer"/> class.
        /// </summary>
        /// <param name="announcers">The announcers.</param>
        public CompositeAnnouncer(params IAnnouncer[] announcers)
        {
            Announcers = announcers ?? new IAnnouncer[0];
        }

        /// <summary>
        /// Gets the announcers.
        /// </summary>
        /// <value>The announcers.</value>
        public IEnumerable<IAnnouncer> Announcers { get; }

        /// <summary>
        /// Headings the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Heading(string message)
        {
            Each(a => a.Heading(message));
        }

        /// <summary>
        /// Says the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Say(string message)
        {
            Each(a => a.Say(message));
        }

        /// <summary>
        /// Emphasizes the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Emphasize(string message)
        {
            Each(a => a.Emphasize(message));
        }

        /// <summary>
        /// SQLs the specified SQL.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        public void Sql(string sql)
        {
            Each(a => a.Sql(sql));
        }

        /// <summary>
        /// Elapseds the time.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        public void ElapsedTime(TimeSpan timeSpan)
        {
            Each(a => a.ElapsedTime(timeSpan));
        }

        /// <summary>
        /// Errors the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Error(string message)
        {
            Each(a => a.Error(message));
        }

        /// <summary>
        /// Errors the specified exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public void Error(Exception exception)
        {
            while (exception != null)
            {
                Error(exception.Message);
                exception = exception.InnerException;
            }
        }

        /// <summary>
        /// Writes the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="isNotSql">if set to <c>true</c> [is not SQL].</param>
        [Obsolete]
        public void Write(string message, bool isNotSql)
        {
            Each(a => a.Write(message, isNotSql));
        }

        /// <summary>
        /// Eaches the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        private void Each(Action<IAnnouncer> action)
        {
            foreach (var announcer in Announcers)
                action(announcer);
        }
    }
}
