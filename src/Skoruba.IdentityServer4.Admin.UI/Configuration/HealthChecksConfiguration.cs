using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Skoruba.IdentityServer4.Admin.UI.Configuration
{
	public class HealthChecksConfiguration
	{
		public bool UseHealthChecks { get; set; } = true;

		public Func<IServiceCollection, IHealthChecksBuilder> BuilderFactory { get; set; }

		public Action<HealthCheckOptions> ConfigureAction { get; set; }

		public string RoutePattern { get; set; } = "/health";
	}
}
