// ***********************************************************************
// Assembly         : FluentMigrator.Runner
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="MigrationValidator.cs" company="FluentMigrator Project">
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using FluentMigrator.Expressions;
using FluentMigrator.Runner.Conventions;
using FluentMigrator.Runner.Exceptions;
using FluentMigrator.Runner.Logging;
using FluentMigrator.Validation;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;

namespace FluentMigrator.Runner
{
    /// <summary>
    /// Class MigrationValidator.
    /// </summary>
    public class MigrationValidator
    {
        /// <summary>
        /// The logger
        /// </summary>
        [CanBeNull]
        private readonly ILogger _logger;

        /// <summary>
        /// The conventions
        /// </summary>
        [CanBeNull]
        private readonly IConventionSet _conventions;

        /// <summary>
        /// The validator
        /// </summary>
        [NotNull]
        private readonly IMigrationExpressionValidator _validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationValidator"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="conventions">The conventions.</param>
        /// <param name="validator">The validator.</param>
        internal MigrationValidator(
            [NotNull] ILogger logger,
            [NotNull] IConventionSet conventions,
            [CanBeNull] IMigrationExpressionValidator validator = null)
        {
            _logger = logger;
            _conventions = conventions;
            _validator = validator ?? new DefaultMigrationExpressionValidator(serviceProvider: null);
        }

        // ReSharper disable once UnusedMember.Global
        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationValidator"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="conventions">The conventions.</param>
        /// <param name="validator">The validator.</param>
        public MigrationValidator(
            [NotNull] ILogger<MigrationValidator> logger,
            [NotNull] IConventionSet conventions,
            [CanBeNull] IMigrationExpressionValidator validator = null)
        {
            _logger = logger;
            _conventions = conventions;
            _validator = validator ?? new DefaultMigrationExpressionValidator(serviceProvider: null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationValidator"/> class.
        /// </summary>
        [Obsolete]
        public MigrationValidator()
        {
            _validator = new DefaultMigrationExpressionValidator(null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationValidator"/> class.
        /// </summary>
        /// <param name="announcer">The announcer.</param>
        /// <param name="conventions">The conventions.</param>
        [Obsolete]
        public MigrationValidator(IAnnouncer announcer, IConventionSet conventions)
        {
            _validator = new DefaultMigrationExpressionValidator(null);
            _logger = new AnnouncerFluentMigratorLogger(announcer);
            _conventions = conventions;
        }

        /// <summary>
        /// Validates each migration expression that has implemented the ICanBeValidated interface.
        /// It throws an InvalidMigrationException exception if validation fails.
        /// </summary>
        /// <param name="migration">The current migration being run</param>
        /// <param name="expressions">All the expressions contained in the up or down action</param>
        /// <exception cref="FluentMigrator.Runner.Exceptions.InvalidMigrationException"></exception>
        public void ApplyConventionsToAndValidateExpressions(IMigration migration, IEnumerable<IMigrationExpression> expressions)
        {
            var errorMessageBuilder = new StringBuilder();

            foreach (var expression in expressions.Apply(_conventions))
            {
                var errors = new Collection<string>();
                foreach (var result in _validator.Validate(expression))
                {
                    errors.Add(result.ErrorMessage);
                }

                if (errors.Count > 0)
                {
                    AppendError(errorMessageBuilder, expression.GetType().Name, string.Join(" ", errors.ToArray()));
                }
            }

            if (errorMessageBuilder.Length > 0)
            {
                var errorMessage = errorMessageBuilder.ToString();
                _logger?.LogError("The migration {0} contained the following Validation Error(s): {1}", migration.GetType().Name, errorMessage);
                throw new InvalidMigrationException(migration, errorMessage);
            }
        }

        /// <summary>
        /// Appends the error.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="expressionType">Type of the expression.</param>
        /// <param name="errors">The errors.</param>
        private void AppendError(StringBuilder builder, string expressionType, string errors)
        {
            builder.AppendFormat("{0}: {1}{2}", expressionType, errors, Environment.NewLine);
        }
    }
}
