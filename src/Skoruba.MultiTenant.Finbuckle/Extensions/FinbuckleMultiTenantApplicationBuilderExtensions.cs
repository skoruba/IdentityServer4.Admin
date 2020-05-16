using Skoruba.MultiTenant.Finbuckle.Middleware;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Extension methods for using Finbuckle.MultiTenant.AspNetCore.
    /// </summary>
    public static class FinbuckleMultiTenantApplicationBuilderExtensions
    {
       
        /// <summary>
        /// Middleware to read claims to establish tenant when processing the request.  This middleware must
        /// be implemented after authorization middleware.
        /// </summary>
        /// <param name="builder">The IApplicationBuilder<c/> instance the extension method applies to.</param>
        /// <returns>The same IApplicationBuilder passed into the method.</returns>
        public static IApplicationBuilder UseMultiTenantFromClaims(this IApplicationBuilder builder)
            => builder.UseMiddleware<MultiTenantClaimMiddleware>();

        /// <summary>
        /// Middleware to read cookie value to establish tenant when processing the request.
        /// </summary>
        /// <param name="builder">The IApplicationBuilder<c/> instance the extension method applies to.</param>
        /// <returns>The same IApplicationBuilder passed into the method.</returns>
        public static IApplicationBuilder UseMultiTenantFromCookie(this IApplicationBuilder builder)
            => builder.UseMiddleware<MultiTenantCookieMiddleware>();
    }
}