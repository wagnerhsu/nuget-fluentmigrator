// ***********************************************************************
// Assembly         : FluentMigrator.Runner
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="MissingMigrationsException.cs" company="FluentMigrator Project">
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

namespace FluentMigrator.Runner.Exceptions
{
    /// <summary>
    /// Class MissingMigrationsException.
    /// Implements the <see cref="FluentMigrator.Runner.Exceptions.RunnerException" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Exceptions.RunnerException" />
    public class MissingMigrationsException : RunnerException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingMigrationsException"/> class.
        /// </summary>
        public MissingMigrationsException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingMigrationsException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public MissingMigrationsException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingMigrationsException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public MissingMigrationsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingMigrationsException"/> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        public MissingMigrationsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
