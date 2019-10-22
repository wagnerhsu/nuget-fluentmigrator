// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Core
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="IAnnouncer.cs" company="FluentMigrator Project">
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

namespace FluentMigrator.Runner
{
    /// <summary>
    /// Interface IAnnouncer
    /// </summary>
    [Obsolete]
    public interface IAnnouncer
    {
        /// <summary>
        /// Headings the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        void Heading(string message);
        /// <summary>
        /// Says the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        void Say(string message);
        /// <summary>
        /// Emphasizes the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        void Emphasize(string message);
        /// <summary>
        /// SQLs the specified SQL.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        void Sql(string sql);
        /// <summary>
        /// Elapseds the time.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        void ElapsedTime(TimeSpan timeSpan);
        /// <summary>
        /// Errors the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        void Error(string message);
        /// <summary>
        /// Errors the specified exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        void Error(Exception exception);

        /// <summary>
        /// Writes the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="isNotSql">if set to <c>true</c> [is not SQL].</param>
        [Obsolete]
        void Write(string message, bool isNotSql = true);
    }
}
