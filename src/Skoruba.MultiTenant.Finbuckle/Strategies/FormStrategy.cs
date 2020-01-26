using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Skoruba.MultiTenant.Finbuckle.Strategies
{
    public class FormStrategy : IMultiTenantStrategy
    {
        private readonly ILogger<FormStrategy> _logger;
        private readonly Configuration.Configuration _multiTenantConfiguration;

        public FormStrategy(ILogger<FormStrategy> logger, Configuration.Configuration multiTenantConfiguration)
        {
            _logger = logger;
            _multiTenantConfiguration = multiTenantConfiguration;
        }

        public async Task<string> GetIdentifierAsync(object context)
        {
            if (!(context is HttpContext))
                throw new MultiTenantException(null,
                    new ArgumentException($"\"{nameof(context)}\" type must be of type HttpContext", nameof(context)));

            var httpContext = context as HttpContext;

            if (string.Equals(httpContext.Request.Method, "POST", StringComparison.OrdinalIgnoreCase)
                && httpContext.Request.HasFormContentType
                && httpContext.Request.Body.CanRead)
            {
                var store = httpContext.RequestServices.GetRequiredService<IMultiTenantStore>();
                var routeData = httpContext.GetRouteData();
                var controller = (string)routeData.Values["Controller"];
                var action = (string)routeData.Values["action"];

                var requestParameters = _multiTenantConfiguration.RequestParameters.Where(a => a.Controller == controller && a.Action == action);
                TenantInfo tenantInfo = null;

                foreach (var r in requestParameters)
                {
                    var formOptions = new FormOptions { BufferBody = true };
                    var form = await httpContext.Request.ReadFormAsync(formOptions);
                    string value = form[r.Name];
                    if (r.Type == 1)
                    {
                        tenantInfo = await store.TryGetByIdentifierAsync(value);
                    }
                    else
                    {
                        _logger.LogDebug($"Returning tenant id for form value: {value}, controller: {controller}, and action: {action}.");
                        return value;
                    }

                    if (tenantInfo != null)
                    {
                        _logger.LogDebug($"Returning tenant id for form value: {value}, controller: {controller}, and action: {action}.");
                        return tenantInfo.Identifier;
                    }
                    _logger.LogDebug($"Tenant could not be found for form value: {value}, controller: {controller}, and action: {action}.");
                }
            }

            return null;
        }
    }

}