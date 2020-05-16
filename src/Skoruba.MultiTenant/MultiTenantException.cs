using Skoruba.MultiTenant.Configuration;
using System;

namespace Skoruba.MultiTenant
{
    /// <summary>
    /// A derived Exception class for any multi-tenant exception generated.
    /// </summary>
    public class MultiTenantException : Exception
    {
        public static MultiTenantException MissingTenant => new MultiTenantException(MultiTenantConstants.MissingTenantExceptionMessage);
        public MultiTenantException(string message, Exception innerException = null)
            : base(message, innerException) { }
    }
}