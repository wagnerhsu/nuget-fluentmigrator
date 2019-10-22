// ***********************************************************************
// Assembly         : FluentMigrator.Runner
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="InvalidMigrationException.cs" company="FluentMigrator Project">
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

namespace FluentMigrator.Runner.Exceptions
{
    /// <summary>
    /// Class InvalidMigrationException.
    /// Implements the <see cref="FluentMigrator.Runner.Exceptions.RunnerException" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Exceptions.RunnerException" />
    public class InvalidMigrationException : RunnerException
    {
        /// <summary>
        /// The migration
        /// </summary>
        private readonly IMigration _migration;
        /// <summary>
        /// The errors
        /// </summary>
        private readonly string _errors;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidMigrationException"/> class.
        /// </summary>
        /// <param name="migration">The migration.</param>
        /// <param name="errors">The errors.</param>
        public InvalidMigrationException(IMigration migration, string errors)
        {
            _migration = migration;
            _errors = errors;
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>The message.</value>
        public override string Message
        {
            get
            {
                return string.Format("The migration {0} contained the following Validation Error(s): {1}", _migration.GetType().Name, _errors);
            }
        }
    }
}
