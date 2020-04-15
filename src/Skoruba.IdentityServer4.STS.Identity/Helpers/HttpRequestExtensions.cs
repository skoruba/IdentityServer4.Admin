using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Skoruba.IdentityServer4.STS.Identity.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.STS.Identity.Helpers
{
    public static class HttpRequestExtensions
    {
        const string _PrivateAddressRegex = @"(^192\.168\.([0-9]|[0-9][0-9]|[0-2][0-9][0-9])\.([0-9]|[0-9][0-9]|[0-2][0-9][0-9])$)|(^172\.([1][6-9]|[2][0-9]|[3][0-1])\.([0-9]|[0-9][0-9]|[0-2][0-9][0-9])\.([0-9]|[0-9][0-9]|[0-2][0-9][0-9])$)|(^10\.([0-9]|[0-9][0-9]|[0-2][0-9][0-9])\.([0-9]|[0-9][0-9]|[0-2][0-9][0-9])\.([0-9]|[0-9][0-9]|[0-2][0-9][0-9])$)";
        public static bool IsLocal(this HttpRequest req, ILogger logger = null)
        {
            bool ret = false;
            var connection = req.HttpContext.Connection;

            if (connection.RemoteIpAddress != null)
            {
                if (connection.RemoteIpAddress.ToString() == "::1" || connection.RemoteIpAddress.ToString() == "127.0.0.1")
                {
                    if (logger != null)
                        logger.LogDebug("Remote IP address is local: {0}", connection.RemoteIpAddress.ToString());
                    ret = true;
                }
                else
                {
                    if (connection.LocalIpAddress != null)
                    {
                        ret = connection.RemoteIpAddress.Equals(connection.LocalIpAddress);

                        if (logger != null)
                            logger.LogDebug("RemoteIpAddress = LocalIpAddress?: {0}", ret.ToString());
                    }
                    else
                    {
                        ret = IPAddress.IsLoopback(connection.RemoteIpAddress);
                        if (logger != null)
                            logger.LogDebug("RemoteIpAddress is loopback?: {0}", ret.ToString());
                    }
                }
            }
            // for in memory TestServer or when dealing with default connection info
            else if (connection.RemoteIpAddress == null && connection.LocalIpAddress == null)
            {
                ret = true;
                logger.LogDebug("No RemoteIpAddress nor LocalIpAddress present in the connection, assuming local test server");
            }

            return ret;
        }

        public static bool IsFromLocalSubnet(this HttpRequest req, IEnumerable<Subnet> excludedLocalSubnets = null, ILogger logger = null)
        {
            if (req.IsLocal(logger))
                return true;

            var connection = req.HttpContext.Connection;
            var remoteIpAddress = connection.RemoteIpAddress;
            if (req.Headers.ContainsKey("X-Forwarded-For"))
            {
                bool correctHeaderFormat = IPAddress.TryParse(req.Headers["X-Forwarded-For"][0], out remoteIpAddress);
                logger.LogDebug("Found X-Forwarded-For header: {0}; will it be used? {1}", req.Headers["X-Forwarded-For"][0], correctHeaderFormat);
            }

            if (remoteIpAddress != null)
            {
                var clientIPv4 = remoteIpAddress.MapToIPv4();
                if (excludedLocalSubnets != null)
                {
                    foreach (var localSubnet in excludedLocalSubnets)
                    {
                        if (clientIPv4.IsInSameSubnet(localSubnet.SubnetIPAddress, localSubnet.SubnetIPMask))
                        {
                            if (logger != null)
                                logger.LogDebug("Remote IP address is one of the excluded subnet addresses, so it will be considered as NOT LOCAL.");
                            return false;
                        }
                    }
                }

                if (Regex.IsMatch(clientIPv4.ToString(), _PrivateAddressRegex))
                {
                    if (logger != null)
                        logger.LogDebug("Remote IP address {0} matches a private address, it will be considered as LOCAL", clientIPv4.ToString());

                    return true;
                }

                if (logger != null)
                    logger.LogDebug("Remote IP address {0} will be considered as NOT LOCAL", clientIPv4.ToString());
                return false;
            }

            if (logger != null)
                logger.LogDebug("Remote IP address is null, so it will be considered as NOT LOCAL");
            return false;
        }
    }
}
