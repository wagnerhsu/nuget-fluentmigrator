// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="ConstrainedMigrations.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
//
// Copyright (c) 2019, Fluent Migrator Project
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
using FluentMigrator.Runner.Constraints;

namespace FluentMigrator.Tests.Integration.Migrations.Constrained
{
    namespace Constraints
    {
        /// <summary>
        /// Class Step1Migration.
        /// Implements the <see cref="FluentMigrator.Migration" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Migration" />
        [Migration(1)]
        public class Step1Migration : Migration
        {
            /// <summary>
            /// Collect the UP migration expressions
            /// </summary>
            public override void Up() { }

            /// <summary>
            /// Collects the DOWN migration expressions
            /// </summary>
            public override void Down() { }
        }

        /// <summary>
        /// Class Step2Migration.
        /// Implements the <see cref="FluentMigrator.Migration" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Migration" />
        [Migration(2)]
        [CurrentVersionMigrationConstraint(1)]
        public class Step2Migration : Migration
        {
            /// <summary>
            /// Collect the UP migration expressions
            /// </summary>
            public override void Up() { }

            /// <summary>
            /// Collects the DOWN migration expressions
            /// </summary>
            public override void Down() { }
        }

        /// <summary>
        /// Class Step2Migration2.
        /// Implements the <see cref="FluentMigrator.Migration" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Migration" />
        [Migration(3)]
        [CurrentVersionMigrationConstraint(1)]
        public class Step2Migration2 : Migration
        {
            /// <summary>
            /// Collect the UP migration expressions
            /// </summary>
            public override void Up() { }

            /// <summary>
            /// Collects the DOWN migration expressions
            /// </summary>
            public override void Down() { }
        }
    }
    namespace ConstraintsMultiple
    {
        /// <summary>
        /// Class AlwaysFalseConstraint.
        /// Implements the <see cref="FluentMigrator.Runner.Constraints.MigrationConstraintAttribute" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Runner.Constraints.MigrationConstraintAttribute" />
        public class AlwaysFalseConstraint : MigrationConstraintAttribute
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="AlwaysFalseConstraint"/> class.
            /// </summary>
            public AlwaysFalseConstraint() : base(opts => false)
            {

            }
        }
        /// <summary>
        /// Class AlwaysTrueConstraint.
        /// Implements the <see cref="FluentMigrator.Runner.Constraints.MigrationConstraintAttribute" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Runner.Constraints.MigrationConstraintAttribute" />
        public class AlwaysTrueConstraint : MigrationConstraintAttribute
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="AlwaysTrueConstraint"/> class.
            /// </summary>
            public AlwaysTrueConstraint() : base(opts => true)
            {

            }
        }
        /// <summary>
        /// Class MultipleConstraintsMigration.
        /// Implements the <see cref="FluentMigrator.Migration" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Migration" />
        [Migration(1)]
        [AlwaysTrueConstraint]
        [AlwaysFalseConstraint]
        public class MultipleConstraintsMigration : Migration
        {
            /// <summary>
            /// Collect the UP migration expressions
            /// </summary>
            public override void Up() { }

            /// <summary>
            /// Collects the DOWN migration expressions
            /// </summary>
            public override void Down() { }
        }
    }

    namespace ConstraintsSuccess
    {
        /// <summary>
        /// Class ConstrainedMigrationSuccess.
        /// Implements the <see cref="FluentMigrator.Migration" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Migration" />
        [Migration(1)]
        [ConstraintsMultiple.AlwaysTrueConstraint]
        public class ConstrainedMigrationSuccess : Migration
        {
            /// <summary>
            /// Collect the UP migration expressions
            /// </summary>
            public override void Up() { }

            /// <summary>
            /// Downs this instance.
            /// </summary>
            public override void Down() { }
        }
    }
}
