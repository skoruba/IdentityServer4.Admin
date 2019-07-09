using IdentityServer4.Configuration;
using IdentityServer4.Events;
using IdentityServer4.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Skoruba.IdentityServer4.Audit.Sink.Recorders;
using Skoruba.IdentityServer4.Audit.Sink.Sinks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Skoruba.IdentityServer4.Audit.Sink.DependencyInjection
{
    public static class IdentityServer4AuditSinkServiceCollectionExtensions
    {
        public static IIdentityServer4AuditSinkBuilder AddIdentityServer4Auditing(this IServiceCollection services)
        {
            var builder = new IdentityServer4AuditSinkBuilder(services);
            services.AddTransient<IEventService, DefaultEventService>();
            services.AddTransient<IEventSink, EventSinkAggregator>();
            return builder;
        }

        public static IIdentityServer4AuditSinkBuilder AddIdentityServerOptions(this IIdentityServer4AuditSinkBuilder builder)
        {
            builder.AddIdentityServerOptions(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            });
            return builder;
        }

        public static IIdentityServer4AuditSinkBuilder AddIdentityServerOptions(this IIdentityServer4AuditSinkBuilder builder, Action<IdentityServerOptions> options)
        {
            builder.Services.Configure(options);
            builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<IdentityServerOptions>>().Value);
            return builder;
        }

        public static IIdentityServer4AuditSinkBuilder AddDefaultIdentityServer4Sink(this IIdentityServer4AuditSinkBuilder builder)
        {
            builder.Services.AddTransient<IAuditSink, IdentityServerEventSink>();
            return builder;
        }

        public static IIdentityServer4AuditSinkBuilder AddConsoleSink(this IIdentityServer4AuditSinkBuilder builder)
        {
            builder.Services.AddSingleton<ConsoleRecorder>();
            builder.Services.AddTransient<IAuditSink, ConsoleSink>();
            return builder;
        }
    }
}