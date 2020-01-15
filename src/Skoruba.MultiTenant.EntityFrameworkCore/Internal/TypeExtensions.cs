//    Copyright 2019 Andrew White
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the 

using Finbuckle.MultiTenant;
using System;
using System.Linq;
using System.Reflection;

namespace Skoruba.MultiTenant.EntityFrameworkCore
{
    internal static class TypeExtensions
    {
        public static bool ImplementsOrInheritsUnboundGeneric(this Type source, Type unboundGeneric)
        {
            if (unboundGeneric.IsInterface)
            {
                return source.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == unboundGeneric);
            }

            Type toCheck = source;

            if (unboundGeneric != toCheck)
            {
                while (toCheck != null && toCheck != typeof(object))
                {
                    var current = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;

                    if (unboundGeneric == current)
                    {
                        return true;
                    }

                    toCheck = toCheck.BaseType;
                }
            }

            return false;
        }

        public static bool HasMultiTenantAttribute(this Type type)
        {
            return type.GetCustomAttribute<MultiTenantAttribute>() != null;
        }
    }
}