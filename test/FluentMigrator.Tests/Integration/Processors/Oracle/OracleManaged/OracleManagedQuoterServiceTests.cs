// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="OracleManagedQuoterServiceTests.cs" company="FluentMigrator Project">
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

using FluentMigrator.Runner;

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

namespace FluentMigrator.Tests.Integration.Processors.Oracle.OracleManaged
{
    /// <summary>
    /// Defines test class OracleManagedQuoterServiceTests.
    /// Implements the <see cref="FluentMigrator.Tests.Integration.Processors.Oracle.OracleQuoterServiceTestsBase" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Integration.Processors.Oracle.OracleQuoterServiceTestsBase" />
    [TestFixture]
    [Category("OracleManaged")]
    public class OracleManagedQuoterServiceTests : OracleQuoterServiceTestsBase
    {
        /// <inheritdoc />
        protected override IServiceCollection AddOracleServices(IServiceCollection services)
        {
            return services.ConfigureRunner(r => r.AddOracleManaged());
        }
    }
}
