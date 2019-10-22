// ***********************************************************************
// Assembly         : FluentMigrator.Runner
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="MigrationScopeHandler.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
//
// Copyright (c) 2018, Fluent Migrator Project
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

using FluentMigrator.Runner.Processors;

namespace FluentMigrator.Runner
{
    /// <summary>
    /// Class MigrationScopeHandler.
    /// Implements the <see cref="FluentMigrator.Runner.IMigrationScopeManager" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.IMigrationScopeManager" />
    public class MigrationScopeHandler : IMigrationScopeManager
    {
        /// <summary>
        /// The processor
        /// </summary>
        private readonly IMigrationProcessor _processor;
        /// <summary>
        /// The preview only
        /// </summary>
        private readonly bool _previewOnly;

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationScopeHandler"/> class.
        /// </summary>
        /// <param name="processor">The processor.</param>
        [Obsolete]
        public MigrationScopeHandler(IMigrationProcessor processor)
        {
            _processor = processor;
            _previewOnly = processor.Options?.PreviewOnly ?? false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationScopeHandler"/> class.
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <param name="processorOptions">The processor options.</param>
        public MigrationScopeHandler(IMigrationProcessor processor, ProcessorOptions processorOptions)
        {
            _processor = processor;
            _previewOnly = processorOptions.PreviewOnly;
        }

        /// <summary>
        /// Gets or sets the current scope.
        /// </summary>
        /// <value>The current scope.</value>
        public IMigrationScope CurrentScope { get; set; }

        /// <summary>
        /// Creates new migration scope
        /// </summary>
        /// <returns>Newly created scope</returns>
        public IMigrationScope BeginScope()
        {
            GuardAgainstActiveMigrationScope();
            CurrentScope = new TransactionalMigrationScope(_processor, () => CurrentScope = null);
            return CurrentScope;
        }

        /// <summary>
        /// Creates new migrations scope or reuses existing one
        /// </summary>
        /// <param name="transactional">Defines if transactions should be used</param>
        /// <returns>Migration scope</returns>
        public IMigrationScope CreateOrWrapMigrationScope(bool transactional = true)
        {
            // Prevent connection from being opened when --no-connection is specified in preview mode
            if (_previewOnly)
            {
                return new NoOpMigrationScope();
            }

            if (HasActiveMigrationScope) return new NoOpMigrationScope();
            if (transactional) return BeginScope();
            return new NoOpMigrationScope();
        }

        /// <summary>
        /// Guards the against active migration scope.
        /// </summary>
        /// <exception cref="InvalidOperationException">The runner is already in an active migration scope.</exception>
        private void GuardAgainstActiveMigrationScope()
        {
            if (HasActiveMigrationScope) throw new InvalidOperationException("The runner is already in an active migration scope.");
        }

        /// <summary>
        /// Gets a value indicating whether this instance has active migration scope.
        /// </summary>
        /// <value><c>true</c> if this instance has active migration scope; otherwise, <c>false</c>.</value>
        private bool HasActiveMigrationScope
        {
            get { return CurrentScope != null && CurrentScope.IsActive; }
        }
    }
}
