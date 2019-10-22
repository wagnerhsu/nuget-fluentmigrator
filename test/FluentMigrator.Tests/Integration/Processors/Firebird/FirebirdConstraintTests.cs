// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="FirebirdConstraintTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors.Firebird;
using FluentMigrator.Tests.Helpers;

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Integration.Processors.Firebird
{
    /// <summary>
    /// Defines test class FirebirdConstraintTests.
    /// Implements the <see cref="FluentMigrator.Tests.Integration.Processors.BaseConstraintTests" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Integration.Processors.BaseConstraintTests" />
    [TestFixture]
    [Category("Integration")]
    [Category("Firebird")]
    public class FirebirdConstraintTests : BaseConstraintTests
    {
        /// <summary>
        /// The prober
        /// </summary>
        private readonly FirebirdLibraryProber _prober = new FirebirdLibraryProber();
        /// <summary>
        /// The temporary database
        /// </summary>
        private TemporaryDatabase _temporaryDatabase;

        /// <summary>
        /// Gets or sets the service provider.
        /// </summary>
        /// <value>The service provider.</value>
        private ServiceProvider ServiceProvider { get; set; }
        /// <summary>
        /// Gets or sets the service scope.
        /// </summary>
        /// <value>The service scope.</value>
        private IServiceScope ServiceScope { get; set; }
        /// <summary>
        /// Gets or sets the processor.
        /// </summary>
        /// <value>The processor.</value>
        private FirebirdProcessor Processor { get; set; }

        /// <summary>
        /// Defines the test method CallingConstraintExistsCanAcceptConstraintNameWithSingleQuote.
        /// </summary>
        [Test]
        public override void CallingConstraintExistsCanAcceptConstraintNameWithSingleQuote()
        {
            using (var table = new FirebirdTestTable(Processor, "id int", string.Format("wibble int CONSTRAINT {0} CHECK(wibble > 0)", "\"c'1\"")))
                Processor.ConstraintExists(null, table.Name, "\"c'1\"").ShouldBeTrue();
        }

        /// <summary>
        /// Defines the test method CallingConstraintExistsCanAcceptTableNameWithSingleQuote.
        /// </summary>
        [Test]
        public override void CallingConstraintExistsCanAcceptTableNameWithSingleQuote()
        {
            using (var table = new FirebirdTestTable("\"Test'Table\"", Processor, "id int", "wibble int CONSTRAINT c1 CHECK(wibble > 0)"))
                Processor.ConstraintExists(null, table.Name, "C1").ShouldBeTrue();
        }

        /// <summary>
        /// Defines the test method CallingConstraintExistsReturnsFalseIfConstraintDoesNotExist.
        /// </summary>
        [Test]
        public override void CallingConstraintExistsReturnsFalseIfConstraintDoesNotExist()
        {
            using (var table = new FirebirdTestTable(Processor, "id int"))
                Processor.ConstraintExists(null, table.Name, "DoesNotExist").ShouldBeFalse();
        }

        /// <summary>
        /// Defines the test method CallingConstraintExistsReturnsFalseIfConstraintDoesNotExistWithSchema.
        /// </summary>
        [Test]
        public override void CallingConstraintExistsReturnsFalseIfConstraintDoesNotExistWithSchema()
        {
            using (var table = new FirebirdTestTable(Processor, "id int"))
                Processor.ConstraintExists("TestSchema", table.Name, "DoesNotExist").ShouldBeFalse();
        }

        /// <summary>
        /// Defines the test method CallingConstraintExistsReturnsFalseIfTableDoesNotExist.
        /// </summary>
        [Test]
        public override void CallingConstraintExistsReturnsFalseIfTableDoesNotExist()
        {
            Processor.ConstraintExists(null, "DoesNotExist", "DoesNotExist").ShouldBeFalse();
        }

        /// <summary>
        /// Defines the test method CallingConstraintExistsReturnsFalseIfTableDoesNotExistWithSchema.
        /// </summary>
        [Test]
        public override void CallingConstraintExistsReturnsFalseIfTableDoesNotExistWithSchema()
        {
            Processor.ConstraintExists("TestSchema", "DoesNotExist", "DoesNotExist").ShouldBeFalse();
        }

        /// <summary>
        /// Defines the test method CallingConstraintExistsReturnsTrueIfConstraintExists.
        /// </summary>
        [Test]
        public override void CallingConstraintExistsReturnsTrueIfConstraintExists()
        {
            using (var table = new FirebirdTestTable(Processor, "id int", "wibble int CONSTRAINT c1 CHECK(wibble > 0)"))
                Processor.ConstraintExists(null, table.Name, "C1").ShouldBeTrue();
        }

        /// <summary>
        /// Defines the test method CallingConstraintExistsReturnsTrueIfConstraintExistsWithSchema.
        /// </summary>
        [Test]
        public override void CallingConstraintExistsReturnsTrueIfConstraintExistsWithSchema()
        {
            using (var table = new FirebirdTestTable(Processor, "id int", "wibble int CONSTRAINT C1 CHECK(wibble > 0)"))
                Processor.ConstraintExists("TestSchema", table.Name, "C1").ShouldBeTrue();
        }

        /// <summary>
        /// Sets up.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            if (!IntegrationTestOptions.Firebird.IsEnabled)
                Assert.Ignore();

            _temporaryDatabase = new TemporaryDatabase(
                IntegrationTestOptions.Firebird,
                _prober);

            var serivces = ServiceCollectionExtensions.CreateServices()
                .ConfigureRunner(builder => builder.AddFirebird())
                .AddScoped<IConnectionStringReader>(
                    _ => new PassThroughConnectionStringReader(_temporaryDatabase.ConnectionString));

            ServiceProvider = serivces.BuildServiceProvider();
            ServiceScope = ServiceProvider.CreateScope();
            Processor = ServiceScope.ServiceProvider.GetRequiredService<FirebirdProcessor>();
        }

        /// <summary>
        /// Tears down.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            ServiceScope?.Dispose();
            ServiceProvider?.Dispose();
            if (_temporaryDatabase != null)
            {
                var connString = _temporaryDatabase.ConnectionString;
                _temporaryDatabase = null;
                FbDatabase.DropDatabase(connString);
            }
        }
    }
}
