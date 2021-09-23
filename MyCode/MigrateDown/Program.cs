using FluentMigrator.Runner;

using Microsoft.Extensions.DependencyInjection;

using MigrateCommon;

namespace MigrateDown
{
    internal class Program
    {
        static void Main()
        {
            var serviceProvider = CreateServices();

            // Put the database update into a scope to ensure
            // that all resources will be disposed.
            using (var scope = serviceProvider.CreateScope())
            {
                UpdateDatabase(scope.ServiceProvider);
            }
            Console.WriteLine("Press any key to continue...");
            Console.Read();
        }
        private static void UpdateDatabase(IServiceProvider serviceProvider)
        {
            // Instantiate the runner
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

            // Execute the migrations
            runner.MigrateDown(100);
        }

        private static IServiceProvider CreateServices()
        {
            var services = new ServiceCollection();
            var serviceProvider = services.AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                .AddSqlServer()
                .WithGlobalConnectionString("Data Source=.;Initial Catalog=FluentMigrator;Integrated Security=True")
                .ScanIn(typeof(AddLogTable).Assembly).For.Migrations())
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                .BuildServiceProvider();
            return serviceProvider;
        }
    }
}
