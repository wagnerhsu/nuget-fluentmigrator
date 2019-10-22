// ***********************************************************************
// Assembly         : FluentMigrator.Runner
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="RunnerException.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
// Copyright (c) 2018, FluentMigrator Project
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
using System.Runtime.Serialization;

using FluentMigrator.Exceptions;

namespace FluentMigrator.Runner.Exceptions
{
    /// <summary>
    /// Class RunnerException.
    /// Implements the <see cref="FluentMigrator.Exceptions.FluentMigratorException" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Exceptions.FluentMigratorException" />
    public class RunnerException : FluentMigratorException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RunnerException"/> class.
        /// </summary>
        protected RunnerException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RunnerException"/> class.
        /// </summary>
        /// <param name="message">The exception message</param>
        protected RunnerException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RunnerException"/> class.
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="innerException">The inner exception</param>
        protected RunnerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RunnerException"/> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        protected RunnerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
