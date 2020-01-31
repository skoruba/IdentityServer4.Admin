using System;
using System.Linq;

namespace Skoruba.DbMigrator.Abstractions.Extensions
{
    public static class AppDomainExtensions
    {
        ///// <summary>
        ///// Gets a all Type instances matching the specified class name with just non-namespace qualified class name.
        ///// </summary>
        ///// <param name="className">Name of the class sought.</param>
        ///// <remarks>https://stackoverflow.com/questions/9273629/avoid-giving-namespace-name-in-type-gettype</remarks>
        ///// <returns>Types that have the class name specified. They may not be in the same namespace.</returns>
        //public static Type GetTypeByFullName(string className)
        //{
        //    return AppDomain.CurrentDomain
        //        .GetAssemblies()
        //        .Where(a => className.StartsWith(a.GetName().Name))
        //        .SelectMany(x => x.GetTypes())
        //        .FirstOrDefault(t => t.FullName == className);
        //}

        /// <summary>
        /// Gets the first type that matches the fullname.
        /// </summary>
        /// <param name="appDomain"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public static Type GetTypeByFullName(this AppDomain appDomain, string className)
        {
            return appDomain
                .GetAssemblies()
                .Where(a => className.StartsWith(a.GetName().Name))
                .SelectMany(x => x.GetTypes())
                .FirstOrDefault(t => t.FullName == className);
        }
    }
}
