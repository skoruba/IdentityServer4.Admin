using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Skoruba.IdentityServer4.Audit.Core.Mappers;
using Skoruba.IdentityServer4.Audit.EntityFramework.Recorders;
using Skoruba.IdentityServer4.Audit.EntityFramework.Sink;
using Skoruba.IdentityServer4.Audit.Sink.DependencyInjection;
using Skoruba.IdentityServer4.Audit.Sink.Sinks;
using System;

namespace Skoruba.IdentityServer4.Audit.EntityFramework.DependencyInjection
{
    public static class IdentityServer4AuditSinkServiceCollectionExtensions
    {
        /// <summary>
        /// Adds an Entity Framework sink for writing audit logs and injects the <see cref="AuditDbContext"/> and <see cref="EntityFrameworkRecorder"/>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static IIdentityServer4AuditSinkBuilder AddEntityFrameworkSink(this IIdentityServer4AuditSinkBuilder builder, string connectionString, string environmentName)
        {
            builder.AddDbContext(connectionString, environmentName);
            builder.Services.AddTransient<EntityFrameworkRecorder>();
            builder.Services.AddTransient<IAuditSink, EntityFrameworkSink>();
            return builder;
        }

        /// <summary>
        /// Adds a Serilog sink for writing audit logs to a database.  This does not include <see cref="AuditDbContext"/>.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static IIdentityServer4AuditSinkBuilder AddSerilogSink(this IIdentityServer4AuditSinkBuilder builder, string connectionString, string environmentName)
        {
            if (environmentName != EnvironmentName.Staging)
            {
                builder.Services.AddSingleton<SerilogRecorder>(p => new SerilogRecorder(connectionString));
                builder.Services.AddTransient<IAuditSink, SerilogSink>();
            }
            return builder;
        }

        /// <summary>
        /// Adds a Serilog sink for writing audit logs to a database and the <see cref="AuditDbContext"/>.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static IIdentityServer4AuditSinkBuilder AddSerilogSinkWithDbContext(this IIdentityServer4AuditSinkBuilder builder, string connectionString, string environmentName)
        {
            builder.AddDbContext(connectionString, environmentName);
            if (environmentName != EnvironmentName.Staging)
            {
                builder.AddSerilogSink(connectionString, environmentName);
            }
            return builder;
        }

        /// <summary>
        /// Adds the <see cref="AuditDbContext"/>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static IIdentityServer4AuditSinkBuilder AddDbContext(this IIdentityServer4AuditSinkBuilder builder, string connectionString, string environmentName)
        {
            if (environmentName != EnvironmentName.Staging)
            {
                builder.Services.AddDbContext<AuditDbContext>(options => options.UseSqlServer(connectionString));
            }
            else
            {
                var databaseName = Guid.NewGuid().ToString();
                builder.Services.AddDbContext<AuditDbContext>(optionsBuilder => optionsBuilder.UseInMemoryDatabase(databaseName));
            }
            return builder;
        }
    }
}