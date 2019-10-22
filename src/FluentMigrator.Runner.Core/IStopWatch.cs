// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Core
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="IStopWatch.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;

namespace FluentMigrator.Runner
{
    /// <summary>
    /// Interface IStopWatch
    /// </summary>
    public interface IStopWatch
    {
        /// <summary>
        /// Starts this instance.
        /// </summary>
        void Start();
        /// <summary>
        /// Stops this instance.
        /// </summary>
        void Stop();
        /// <summary>
        /// Elapseds the time.
        /// </summary>
        /// <returns>TimeSpan.</returns>
        TimeSpan ElapsedTime();
        /// <summary>
        /// Times the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>TimeSpan.</returns>
        TimeSpan Time(Action action);
    }
}
