using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Skoruba.MultiTenant.Finbuckle.Middleware
{
    public static class UsePerTenantApplicationBuilderExtensions
    {
        public static IApplicationBuilder UsePerTenant(this IApplicationBuilder app,
            Action<TenantPipelineBuilderContext, IApplicationBuilder> configuration)
        {
            //Ensure.Argument.NotNull(app, nameof(app));
            //Ensure.Argument.NotNull(configuration, nameof(configuration));

            //app.Use(next => new TenantPipelineMiddleware(next, app, configuration).Invoke);
            app.UseMiddleware<TenantPipelineMiddleware>(app, configuration);
            return app;
        }
    }
    public class TenantPipelineBuilderContext
    {
        public string Tenant { get; set; }
    }
    public class TenantPipelineMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IApplicationBuilder rootApp;
        private readonly Action<TenantPipelineBuilderContext, IApplicationBuilder> configuration;

        private readonly ConcurrentDictionary<string, Lazy<RequestDelegate>> pipelines
            = new ConcurrentDictionary<string, Lazy<RequestDelegate>>();

        public TenantPipelineMiddleware(
            RequestDelegate next,
            IApplicationBuilder rootApp,
            Action<TenantPipelineBuilderContext, IApplicationBuilder> configuration)
        {
            //Ensure.Argument.NotNull(next, nameof(next));
            //Ensure.Argument.NotNull(rootApp, nameof(rootApp));
            //Ensure.Argument.NotNull(configuration, nameof(configuration));

            this.next = next;
            this.rootApp = rootApp;
            this.configuration = configuration;
        }

        public async Task Invoke(HttpContext context, IMultiTenantStore store)
        {
            var multiTenantContext = context.GetMultiTenantContext();

            TenantInfo tenantInfo = multiTenantContext?.TenantInfo;

            if (tenantInfo != null && context.Request.GetDisplayUrl().Contains(tenantInfo?.Id))
            {
                //if (segments.Length > 1)
                //{
                //var tenant = segments[2].Replace("/", "");

                //TenantInfo tenantInfo = await store.TryGetByIdentifierAsync(tenant);

                //if (tenantInfo != null)
                //{
                var tenantPipeline = pipelines.GetOrAdd(
                    tenantInfo.Id,
                    new Lazy<RequestDelegate>(() => BuildTenantPipeline(tenantInfo.Id)));

                await tenantPipeline.Value(context);
                //}
                //}
            }
        }

        private RequestDelegate BuildTenantPipeline(string tenantContext)
        {
            var branchBuilder = rootApp.New();

            var builderContext = new TenantPipelineBuilderContext
            {
                Tenant = tenantContext
            };

            configuration(builderContext, branchBuilder);

            // register root pipeline at the end of the tenant branch
            branchBuilder.Run(next);

            return branchBuilder.Build();
        }
    }
}