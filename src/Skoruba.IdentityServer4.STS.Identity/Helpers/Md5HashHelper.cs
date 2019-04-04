using System.Security.Cryptography;
using System.Text;

namespace Skoruba.IdentityServer4.STS.Identity.Helpers
{

    /// <summary>
    /// Helper-class to create Md5hashes from strings
    /// </summary>
    public static class Md5HashHelper
    {
        /// <summary>
        /// Computes a Md5-hash of the submitted string and returns the corresponding hash
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetHash(string input)
        {
            using (var md5 = MD5.Create())
            {
                var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));

                var sBuilder = new StringBuilder();

                foreach (var dataByte in bytes)
                {
                    sBuilder.Append(dataByte.ToString("x2"));
                }

                return sBuilder.ToString();
            }
        }
    }
}
