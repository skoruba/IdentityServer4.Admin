//    Copyright 2018 Andrew White
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
//    limitations under the License.

using Microsoft.EntityFrameworkCore.Metadata;
using System.Collections.Generic;
using System.Linq;

namespace Skoruba.MultiTenant.EntityFrameworkCore
{
    public static class ModelExtensions
    {
        /// <summary>
        /// Gets all MultiTenant entity types defined in the model.
        /// </summary>
        /// <param name="model">the model from which to list entities.</param>
        /// <returns>MultiTenant entity types.</returns>
        public static IEnumerable<IEntityType> GetMultiTenantEntityTypes(this IModel model)
        {
            return model.GetEntityTypes().Where(et => et.IsMultiTenant());
        }
    }
}
