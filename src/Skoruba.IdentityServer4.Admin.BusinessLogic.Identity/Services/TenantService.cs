using System;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Tenant;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Helpers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Resources;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.ExceptionHandling;
using Skoruba.IdentityServer4.Admin.EntityFramework.Extensions.Common;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.DbContexts;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Tenants;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Services
{
    public class TenantService<TbContext> : ITenantService
        where TbContext : DbContext, IMultiTenantDbContext
    {
        public TenantService(TbContext dbContext, IMapper mapper, ITenantServiceResources tenantServiceResources)
        {
            DbContext = dbContext;
            Mapper = mapper;
            TenantServiceResources = tenantServiceResources;
        }

        public TbContext DbContext { get; }
        public IMapper Mapper { get; }
        public ITenantServiceResources TenantServiceResources { get; }

        public async Task<TenantDto> AddTenantAsync(TenantDto tenantDto)
        {
            tenantDto.Id = Guid.NewGuid().ToString();

            //set tenant properties
            var newTenant = new Tenant()
            {
                Id = tenantDto.Id,
                Name = tenantDto.Name,
                Code = tenantDto.Code
            };

            DbContext.Tenants.Add(newTenant);
            await DbContext.SaveChangesAsync();
            return Mapper.Map<TenantDto>(newTenant);
        }

        public async Task<TenantDto> GetTenantAsync(string id)
        {
            var tenant = await DbContext.Tenants.ProjectTo<TenantDto>(Mapper.ConfigurationProvider).FirstOrDefaultAsync(a => a.Id == id);

            if (tenant == null) throw new UserFriendlyErrorPageException(string.Format(TenantServiceResources.TenantDoesNotExist().Description, id), TenantServiceResources.TenantDoesNotExist().Description);

            return tenant;
        }

        public async Task<TenantsDto> GetTenantsAsync(int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<TenantDto>();
            var tenants = await DbContext.Tenants.PageBy(x => x.Id, page, pageSize).ProjectTo<TenantDto>(Mapper.ConfigurationProvider).ToListAsync();
            pagedList.Data.AddRange(tenants);
            pagedList.TotalCount = await DbContext.Tenants.CountAsync();
            pagedList.PageSize = pageSize;
            var tenantsDto = Mapper.Map<TenantsDto>(pagedList);
            return tenantsDto;
        }

        //public Task<Action> GetTenantUsersAsync(string tenantId)
        //{
        //    var pagedList = new PagedList<IUserDto>();
        //    var users =
        //}

        public async Task<IdentityResult> DisableTenantAsync(string id)
        {
            var tenant = await DbContext.Tenants.FirstOrDefaultAsync(a => a.Id == id);

            //how to remove?
            tenant.IsActive = false;
            //DbContext.Tenants.Remove(tenant);

            await DbContext.SaveChangesAsync();
            return IdentityResult.Success;
        }

        public Task<IdentityResult> UpdateTenantAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}