using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Filters;
using Skoruba.IdentityServer4.STS.Identity.Configuration;

namespace Skoruba.IdentityServer4.STS.Identity
{
    public class Program
    {
        private static IConfiguration _bootstrapperConfig;

        public static void Main(string[] args)
        {
            _bootstrapperConfig = GetBootstrapperConfig(args);
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(_bootstrapperConfig)
                .CreateLogger();
            try
            {
                // EZYC-2851-modification: we're not using default way of dockerizing.
                //DockerHelpers.ApplyDockerConfiguration(configuration);

                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IConfiguration GetBootstrapperConfig(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            // EZYC-2851-modification: allow sub-environment config
            var subEnvironment = Environment.GetEnvironmentVariable("SUB_ENVIRONMENT");
            var templatesFolder = Environment.GetEnvironmentVariable("ENV_TEMPLATES_FOLDER") ?? "CustomSettings";
            var isDevelopment = environment == Environments.Development;
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("{templatesFolder}/appsettings._Shared.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"{templatesFolder}/appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"{templatesFolder}/appsettings.{environment}.{subEnvironment}.json", optional: true, reloadOnChange: true)
                .AddJsonFile("serilog.json", optional: true, reloadOnChange: true)
                .AddJsonFile("{templatesFolder}/serilog._Shared.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"{templatesFolder}/serilog.{environment}.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"{templatesFolder}/serilog.{environment}.{subEnvironment}.json", optional: true, reloadOnChange: true);

            if (isDevelopment)
            {
                configurationBuilder.AddUserSecrets<Startup>();
            }

            // EZY-modification (EZYC-3029) - disabled Azure key Vault for consistency with the Admin project. Here it however looks to be correctly applied
            // var configuration = configurationBuilder.Build();
            //
            // configuration.AddAzureKeyVaultConfiguration(configurationBuilder);

            configurationBuilder.AddCommandLine(args);
            configurationBuilder.AddEnvironmentVariables();

            return configurationBuilder.Build();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                 .ConfigureAppConfiguration((hostContext, configApp) =>
                 {
                     // EZYC-2851-modification: allow more robust configuration
                     var bootstrapperAdminConfig = _bootstrapperConfig.GetSection(nameof(AdminConfiguration)).Get<AdminConfiguration>();
                     var env = hostContext.HostingEnvironment;
                     var subEnvironment = Environment.GetEnvironmentVariable("SUB_ENVIRONMENT");
                     var templatesFolder = Environment.GetEnvironmentVariable("ENV_TEMPLATES_FOLDER") ?? "CustomSettings";
                     configApp.AddJsonFile($"{templatesFolder}/appsettings._Shared.json", optional: true, reloadOnChange: true);
                     configApp.AddJsonFile($"{templatesFolder}/appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                     configApp.AddJsonFile($"{templatesFolder}/appsettings.{env.EnvironmentName}.{subEnvironment}.json", optional: true,
                         reloadOnChange: true);
                     configApp.AddJsonFile("serilog.json", optional: true, reloadOnChange: true);
                     configApp.AddJsonFile("{templatesFolder}/serilog._Shared.json", optional: true, reloadOnChange: true);
                     configApp.AddJsonFile($"{templatesFolder}/serilog.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                     configApp.AddJsonFile($"{templatesFolder}/serilog.{env.EnvironmentName}.{subEnvironment}.json", optional: true, reloadOnChange: true);

                     if (!hostContext.HostingEnvironment.IsDevelopment())
                     {
                         configApp.AddSecretsManager(configurator: options =>
                         {
                             var prefix = $"{bootstrapperAdminConfig.ApplicationName}/{env.EnvironmentName}/";
                             options.SecretFilter = entry => entry.Name.StartsWith(prefix);
                             options.KeyGenerator = (entry, key) =>
                             {
                                 var transformedKey = key.Substring(prefix.Length).Replace("__", ":");
                                 Console.WriteLine($"Reading secret key {key} transformed as {transformedKey}");

                                 return transformedKey;
                             };
                         });
                     }

                     if (env.IsDevelopment())
                     {
                         configApp.AddUserSecrets<Startup>();
                     }

                     // EZY-modification (EZYC-3029): disabling Azure key vault - as per above comments
                     // configurationRoot.AddAzureKeyVaultConfiguration(configApp);

                     configApp.AddEnvironmentVariables();
                     configApp.AddCommandLine(args);
                 })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(options => options.AddServerHeader = false);
                    webBuilder.UseStartup<Startup>();
                })
                .UseSerilog((hostContext, loggerConfig) =>
                {
                    loggerConfig
                        .ReadFrom.Configuration(hostContext.Configuration)
                        .Enrich.WithProperty("ApplicationName", hostContext.HostingEnvironment.ApplicationName);

                    // EZYC-2851-modification below
                    loggerConfig.WriteTo.Logger(lc =>
                    {
                        // Serilog doesn't allow to override certain logger levels on per-logger basis. Also, it would be good to have at least some
                        // decent console logging on non-dev environments, like few initial lifetime (startup) events. That's why, let's include console
                        // logger for every environment and only in Development mode, let's not restrict it to Microsoft.Hosting.Lifetime
                        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                        if (environment != "Development")
                            lc.Filter.ByIncludingOnly(Matching.FromSource("Microsoft.Hosting.Lifetime"));

                        lc.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3} {SourceContext}] {Message:lj}{NewLine}{Exception}");
                    });
                });
    }
}
