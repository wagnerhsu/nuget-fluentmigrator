// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="AssemblyLoaderTests.cs" company="FluentMigrator Project">
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

using FluentMigrator.Runner.Initialization.AssemblyLoader;
using NUnit.Framework;

namespace FluentMigrator.Tests.Unit.AssemblyLoader
{
    /// <summary>
    /// Defines test class AssemblyLoaderTests.
    /// </summary>
    [TestFixture]
    public class AssemblyLoaderTests
    {
        /// <summary>
        /// The assembly loader factory
        /// </summary>
        private AssemblyLoaderFactory _assemblyLoaderFactory;

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _assemblyLoaderFactory = new AssemblyLoaderFactory();
        }

        /// <summary>
        /// Defines the test method ShouldBeAbleToLoadAssemblyByFileName.
        /// </summary>
        [Test]
        public void ShouldBeAbleToLoadAssemblyByFileName()
        {
            var assemblyLoader = _assemblyLoaderFactory.GetAssemblyLoader(GetType().Assembly.Location);
            Assert.IsInstanceOf(typeof(AssemblyLoaderFromFile), assemblyLoader);
            Assert.AreEqual(GetType().Assembly, assemblyLoader.Load());
        }

        /// <summary>
        /// Defines the test method ShouldBeAbleToLoadAssemblyAssemblyName.
        /// </summary>
        [Test]
        public void ShouldBeAbleToLoadAssemblyAssemblyName()
        {
            var assemblyLoader = _assemblyLoaderFactory.GetAssemblyLoader(GetType().Assembly.GetName().Name);
            Assert.IsInstanceOf(typeof(AssemblyLoaderFromName), assemblyLoader);
            Assert.AreEqual(GetType().Assembly, assemblyLoader.Load());
        }
    }
}
