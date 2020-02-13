using Iserv.IdentityServer4.BusinessLogic.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Iserv.IdentityServer4.BusinessLogic.Services
{
    public class AuthPortalHostedService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AuthPortalHostedService> _logger;

        public AuthPortalHostedService(IServiceScopeFactory scopeFactory, ILogger<AuthPortalHostedService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var options = scope.ServiceProvider.GetRequiredService<AuthPortalOptions>();
                _logger.LogInformation("Запуск планировщика аутентификации на портале.");
                if (!TimeSpan.TryParse(options.Interval, out TimeSpan interval))
                    interval = new TimeSpan(0, 30, 0);
                _timer = new Timer(DoWork, null, TimeSpan.Zero, interval);
                return Task.CompletedTask;
            }
        }

        private void DoWork(object state)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var authPortal = scope.ServiceProvider.GetRequiredService<IPortalService>();
                _logger.LogInformation("Аутентификация");
                try
                {
                    Task.WaitAll(authPortal.UpdateSessionAsync());
                    _logger.LogInformation("Аутентификация выполнена");
                }
                catch (Exception exc)
                {
                    _logger.LogError(exc, exc.Message);
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                _logger.LogInformation("Остановка планировщика аутентификации на портале.");
                _timer?.Change(Timeout.Infinite, 0);
                return Task.CompletedTask;
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
