using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Iserv.IdentityServer4.BusinessLogic.Extension
{
    public static class TracingExtensions
    {
        public static void InitTraceIdRenderer()
        {
            AccountService.Infrastucture.Tracing.TracingExtensions.InitTraceIdRenderer();
        }

        public static void AddTracing(this IServiceCollection services, IConfiguration configuration)
        {
            AccountService.Infrastucture.Tracing.TracingExtensions.AddTracing(services, configuration);
        }

        public static void AddTracingFilters(this MvcOptions options)
        {
            options.Filters.Add(typeof(TracingRequestFilter));
            options.Filters.Add(typeof(AccountService.Infrastucture.Tracing.TracingExceptionFilter));
        }
    }
}