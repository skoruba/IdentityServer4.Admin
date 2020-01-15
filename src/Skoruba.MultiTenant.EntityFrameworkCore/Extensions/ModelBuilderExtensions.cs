// Copyright 2019 Andrew White
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Finbuckle.MultiTenant.Core;

namespace Skoruba.MultiTenant.EntityFrameworkCore
{
    static class FinbuckleModelBuilderExtensions
    {
        /// <summary>
        /// Configures any entity's with the [MultiTenant] attribute.
        /// </summary>
        public static ModelBuilder ConfigureMultiTenant(this ModelBuilder modelBuilder)
        {
            // Annotate the types marked with the MultiTenant Data Attribute
            foreach (var t in modelBuilder.Model.GetEntityTypes().Where(t => t.ClrType.HasMultiTenantAttribute()))
            {
                modelBuilder.Entity(t.ClrType).IsMultiTenant();
            }

            return modelBuilder;
        }
    }
}