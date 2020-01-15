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

using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Linq;
using System.Linq.Expressions;
using Finbuckle.MultiTenant.Core;

#if NETSTANDARD2_0
using Microsoft.EntityFrameworkCore.Metadata;
using System.Collections.Generic;
#endif

namespace Skoruba.MultiTenant.EntityFrameworkCore
{
    public static class FinbuckleEntityTypeBuilderExtensions
    {
        private class ExpressionVariableScope
        {
            public IMultiTenantDbContext Context { get; }
        }

        internal static LambdaExpression GetQueryFilter(this EntityTypeBuilder builder)
        {
#if NETSTANDARD2_1
            return builder.Metadata.GetQueryFilter();
#elif NETSTANDARD2_0
            return builder.Metadata.QueryFilter;
#else
#error No valid path!
#endif
        }

        /// <summary>
        /// Adds MultiTenant support for an entity. Call <see cref="IsMultiTenant" /> after 
        /// <see cref="EntityTypeBuilder.HasQueryFilter" /> to merge query filters.
        /// </summary>
        /// <typeparam name="T">The specific type of <see cref="EntityTypeBuilder"/></typeparam>
        /// <param name="builder">The entity's type builder</param>
        /// <returns>The original type builder reference for chaining</returns>
        public static T IsMultiTenant<T>(this T builder) where T : EntityTypeBuilder
        {
            builder.HasAnnotation(Constants.MultiTenantAnnotationName, true);

            try
            {
                builder.Property<string>("TenantId").IsRequired().HasMaxLength(Finbuckle.MultiTenant.Core.Constants.TenantIdMaxLength);
            }
            catch (Exception ex)
            {
                throw new MultiTenantException($"{builder.Metadata.ClrType} unable to add TenantId property", ex);
            }

            // build expression tree for e => EF.Property<string>(e, "TenantId") == TenantInfo.Id

            // where e is one of our entity types
            // will need this ParameterExpression for next step and for final step
            var entityParamExp = Expression.Parameter(builder.Metadata.ClrType, "e");

            var existingQueryFilter = builder.GetQueryFilter();

            // override to match existing query paraameter if applicable
            if (existingQueryFilter != null)
            {
                entityParamExp = existingQueryFilter.Parameters.First();
            }

            // build up expression tree for EF.Property<string>(e, "TenantId")
            var tenantIdExp = Expression.Constant("TenantId", typeof(string));
            var efPropertyExp = Expression.Call(typeof(EF), "Property", new[] { typeof(string) }, entityParamExp, tenantIdExp);
            var leftExp = efPropertyExp;

            var scope = new ExpressionVariableScope();
            var scopeConstantExp = Expression.Constant(scope);

            // EF will rewrite the IMultiTenantDbContext reference to the correct context type
            var contextVariableExp = typeof(ExpressionVariableScope).GetMember(nameof(ExpressionVariableScope.Context))[0];
            var contextMemberAccessExp = Expression.MakeMemberAccess(scopeConstantExp, contextVariableExp);

            // build expression tree for EF.Property<string>(e, "TenantId") == TenantInfo.Id'
            var contextTenantInfoExp = Expression.Property(contextMemberAccessExp, nameof(IMultiTenantDbContext.TenantInfo));
            var rightExp = Expression.Property(contextTenantInfoExp, nameof(TenantInfo.Id));
            var predicate = Expression.Equal(leftExp, rightExp);

            // combine with existing filter
            if (existingQueryFilter != null)
            {
                predicate = Expression.AndAlso(existingQueryFilter.Body, predicate);
            }

            // build the final expression tree
            var delegateType = Expression.GetDelegateType(builder.Metadata.ClrType, typeof(bool));
            var lambdaExp = Expression.Lambda(delegateType, predicate, entityParamExp);

            // set the filter
            builder.HasQueryFilter(lambdaExp);

            Type clrType = builder.Metadata.ClrType;

            if (clrType != null)
            {
                if (clrType.ImplementsOrInheritsUnboundGeneric(typeof(IdentityUser<>)))
                {
                    UpdateIdentityUserIndex(builder);
                }

                if (clrType.ImplementsOrInheritsUnboundGeneric(typeof(IdentityRole<>)))
                {
                    UpdateIdentityRoleIndex(builder);
                }

                if (clrType.ImplementsOrInheritsUnboundGeneric(typeof(IdentityUserLogin<>)))
                {
                    UpdateIdentityUserLoginPrimaryKey(builder);
                    AddIdentityUserLoginIndex(builder);
                }
            }

            return builder;
        }        

        private static void UpdateIdentityUserIndex(this EntityTypeBuilder builder)
        {
            builder.RemoveIndex("NormalizedUserName");
            builder.HasIndex("NormalizedUserName", "TenantId").HasName("UserNameIndex").IsUnique();
        }

        private static void UpdateIdentityRoleIndex(this EntityTypeBuilder builder)
        {
            builder.RemoveIndex("NormalizedName");
            builder.HasIndex("NormalizedName", "TenantId").HasName("RoleNameIndex").IsUnique();
        }

        private static void UpdateIdentityUserLoginPrimaryKey(this EntityTypeBuilder builder)
        {
            var pk = builder.Metadata.FindPrimaryKey();
            builder.Metadata.RemoveKey(pk.Properties);

            // Create a new ID and a unique index to replace the old pk.
            builder.Property<string>("Id").ValueGeneratedOnAdd();
        }

        private static void AddIdentityUserLoginIndex(this EntityTypeBuilder builder) 
        { 
            builder.HasIndex("LoginProvider", "ProviderKey", "TenantId").IsUnique();
        }

        private static void RemoveIndex(this EntityTypeBuilder builder, string propName)
        {
#if NETSTANDARD2_1
            var prop = builder.Metadata.FindProperty(propName);
            var index = builder.Metadata.FindIndex(prop);
            builder.Metadata.RemoveIndex(index);
#elif NETSTANDARD2_0
            var props = new List<IProperty>(new[] { builder.Metadata.FindProperty(propName) });
            builder.Metadata.RemoveIndex(props);
#else
#error No valid path!
#endif
        }
    }
}