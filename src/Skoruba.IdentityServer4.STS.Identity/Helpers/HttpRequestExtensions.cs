using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.STS.Identity.Helpers
{
    public static class HttpRequestExtensions
    {
        public static bool IsLocal(this HttpRequest req)
        {
            var connection = req.HttpContext.Connection;
            if (connection.RemoteIpAddress != null)
            {
                if (connection.RemoteIpAddress.ToString() == "::1" || connection.RemoteIpAddress.ToString() == "127.0.0.1")
                    return true;

                if (connection.LocalIpAddress != null)
                {
                    return connection.RemoteIpAddress.Equals(connection.LocalIpAddress);
                }
                else
                {
                    return IPAddress.IsLoopback(connection.RemoteIpAddress);
                }
            }

            // for in memory TestServer or when dealing with default connection info
            if (connection.RemoteIpAddress == null && connection.LocalIpAddress == null)
            {
                return true;
            }

            return false;
        }

        public static bool IsFromLocalSubnet(this HttpRequest req)
        {
            if (req.IsLocal())
                return true;

            var connection = req.HttpContext.Connection;
            if (connection.RemoteIpAddress != null)
            {
                var clientIPv4 = connection.RemoteIpAddress.MapToIPv4();

                foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (ni.OperationalStatus == OperationalStatus.Up)
                    {
                        foreach (UnicastIPAddressInformation uipi in ni.GetIPProperties().UnicastAddresses)
                        {
                            if (clientIPv4.IsInSameSubnet(uipi.Address.MapToIPv4(), uipi.IPv4Mask))
                                return true;
                        }
                    }
                }

                return false;
            }
            return false;
        }
    }
}
