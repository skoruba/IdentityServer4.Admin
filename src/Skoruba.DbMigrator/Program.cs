using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Scrutor;
using Serilog;
using Serilog.Events;
using Skoruba.DbMigrator.Abstractions;
using Skoruba.DbMigrator.Abstractions.Dependency;
using Skoruba.DbMigrator.Abstractions.Extensions;
using Skoruba.DbMigrator.MigrateAndSeed;
using Skoruba.MultiTenant.Finbuckle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Skoruba.DbMigrator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // To close the console window when debugging is finished:
            // Debug > Options > General > Automatically close the console when debugging stops
            //https://developercommunity.visualstudio.com/content/problem/321978/how-to-remove-exited-with-code-0-from-cmd-console.html

            IConfigurationRoot config = GetConfiguration();
            
            ConfigureLogging();
            IServiceCollection services = new ServiceCollection();
            services.AddLogging(s => s.AddSerilog());

            var startup = new IdentityServer4.Admin.Startup(null, config);

            startup.ConfigureServices(services);
            
            AddMigrateAndSeedServices(services);

            var migrators = services
                .BuildServiceProvider()
                .GetServices<IMigrateAndSeed>()
                .Select(m => new DependencyItem(m.GetType(), m))
                .ToList()
                .SetDependencies()
                .SortByDependencies(m => m.Dependencies)
                .Select(m => m.Instance as IMigrateAndSeed);


            foreach (var migrator in migrators)
            {
                await migrator.Migrate(services);

                await migrator.Seed(services, config);

                Console.WriteLine("-------------------------------------------");
                Console.WriteLine();
            }

            Console.WriteLine("\npress any key to exit the process...");

            Console.ReadKey();
        }

        static void ConfigureLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
#if DEBUG
                .MinimumLevel.Override("Skoruba", LogEventLevel.Debug)
#else
                .MinimumLevel.Override("Skoruba", LogEventLevel.Information)
#endif
                .Enrich.FromLogContext()
                .WriteTo.File(Path.Combine(Directory.GetCurrentDirectory(), "Logs/logs.txt"))
                .WriteTo.Console()
                .CreateLogger();
        }

        static IConfigurationRoot GetConfiguration()
        {
            var migratorDirectory = AppContext.BaseDirectory;

            const string relativePathToSolutionSrc = @"../../../../../src";

            var contentRootPath = Path.Combine(migratorDirectory, relativePathToSolutionSrc);

            return new ConfigurationBuilder()
                .SetBasePath(contentRootPath)
                .AddJsonFile("Skoruba.IdentityServer4.Admin/appsettings.json", optional: false)
                .AddJsonFile("Skoruba.IdentityServer4.Admin/connectionstrings.json", optional: true)
                .AddJsonFile("Skoruba.DbMigrator/identityserverdata.json", optional: false)
                .AddJsonFile("Skoruba.DbMigrator/identitydata.json", optional: false)
                .AddJsonFile("Skoruba.DbMigrator/tenantdata.json", optional: false)
                .AddUserSecrets<Program>()
                .Build();

        }

        static void AddMigrateAndSeedServices(IServiceCollection services)
        {
            services.Scan(scan => scan
                .FromApplicationDependencies(dep => dep.FullName.Contains("Skoruba"))
                .AddClasses(c => c.AssignableTo<IMigrateAndSeed>())
                .UsingRegistrationStrategy(RegistrationStrategy.Append)
                .AsImplementedInterfaces()
                .WithScopedLifetime());
        }
    }
}
