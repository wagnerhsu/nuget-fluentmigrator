// ***********************************************************************
// Assembly         : FluentMigrator.Runner
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="StopWatch.cs" company="FluentMigrator Project">
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

namespace FluentMigrator.Runner
{
    /// <summary>
    /// Class StopWatch.
    /// Implements the <see cref="FluentMigrator.Runner.IStopWatch" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.IStopWatch" />
    public class StopWatch : IStopWatch
    {
        /// <summary>
        /// The time now
        /// </summary>
        public static Func<DateTime> TimeNow = () => DateTime.Now;

        /// <summary>
        /// The start time
        /// </summary>
        private DateTime _startTime;
        /// <summary>
        /// The end time
        /// </summary>
        private DateTime _endTime;

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            _startTime = TimeNow();
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            _endTime = TimeNow();
        }

        /// <summary>
        /// Elapseds the time.
        /// </summary>
        /// <returns>TimeSpan.</returns>
        public TimeSpan ElapsedTime()
        {
            return _endTime - _startTime;
        }

        /// <summary>
        /// Times the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>TimeSpan.</returns>
        public TimeSpan Time(Action action)
        {
            Start();

            action();

            Stop();

            return ElapsedTime();
        }
    }
}
