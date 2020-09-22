using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Skoruba.IdentityServer4.STS.Identity.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.STS.Identity.Helpers.ADServices
{
    public class ADUserSynchronizerHostedService<TUser, TKey> : BackgroundService
        where TUser : IdentityUser<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ADUserSynchronizerHostedService<TUser, TKey>> _logger;
        private readonly IOptionsMonitor<WindowsAuthConfiguration> _adOptions;

        public ADUserSynchronizerHostedService(
            IServiceProvider serviceProvider,
            ILogger<ADUserSynchronizerHostedService<TUser, TKey>> logger,
            IOptionsMonitor<WindowsAuthConfiguration> adOptions
        )
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _adOptions = adOptions ?? throw new ArgumentNullException(nameof(adOptions));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_adOptions.CurrentValue.BackgroundSynchronization)
            {
                _logger.LogInformation($"Background service: AD user synchronization started");
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        Stopwatch stopwatch = new Stopwatch();
                        try
                        {
                            _logger.LogInformation($"Background service: AD user synchronization begin of sync");
                            stopwatch.Start();
                            using (var scope = _serviceProvider.CreateScope())
                            {
                                var userSynchronizer = scope.ServiceProvider.GetRequiredService<ADUserSynchronizer<TUser, TKey>>();
                                await userSynchronizer.SynchronizeAll(stoppingToken);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"An exception occurred when synchronizing users from AD");
                        }
                        finally
                        {
                            stopwatch.Stop();
                            var elapsedTime = stopwatch.Elapsed;
                            string elapsedTimeText = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                                elapsedTime.Hours, elapsedTime.Minutes, elapsedTime.Seconds,
                                elapsedTime.Milliseconds / 10);
                            _logger.LogInformation($"Background service: AD user synchronization end of sync. Elapsed time {elapsedTimeText}");
                            await Task.Delay(_adOptions.CurrentValue.BackgroundSynchronizationSleep, stoppingToken);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"An unexpected exception occurred when obtaning value of {nameof(WindowsAuthConfiguration.BackgroundSynchronizationSleep)}");
                    }
                }
            }
        }
    }
}
