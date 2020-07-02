using AutoMapper;
using Skoruba.AuditLogging.EntityFramework.Helpers.Common;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration.Tenants;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Mappers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.ExceptionHandling;
using Skoruba.IdentityServer4.Admin.EntityFramework.Entities;
using Skoruba.IdentityServer4.Admin.EntityFramework.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Mappers;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Services
{
    public class TenantService : ITenantService
    {
        protected readonly ITenantRepository TenantRepository;

        public TenantService(
            ITenantRepository tenantRepository)
        {
            TenantRepository = tenantRepository;
        }

        public virtual async Task<TenantDto> GetAsync(string id)
        {
            Guid.TryParse(id, out Guid result);

            var tenant = await TenantRepository.FindByIdAsync(result);
            if (tenant == null) throw new UserFriendlyErrorPageException($"There is no tenant by id: {id}");

            var tenantDto = tenant.ToModel();
            return tenantDto;
        }

        public virtual async Task<TenantsDto> GetListAsync(string search, int page = 1, int pageSize = 10)
        {
            var pagedList = await TenantRepository.GetListAsync(search, page, pageSize);
            var tenantsDto = pagedList.ToModel();
            return tenantsDto;
        }

        public virtual async Task<TenantDto> CreateAsync(CreateTenantDto input)
        {
            if (string.IsNullOrWhiteSpace(input.Name))
            {
                throw new ArgumentException(nameof(input.Name));
            }
            await ValidateNameAsync(input.Name);
            var tenant = new Tenant(Guid.NewGuid(), input.Name);
            var entity = await TenantRepository.AddAsync(tenant);
            return entity.ToModel();
        }

        public virtual async Task<TenantDto> UpdateAsync(UpdateTenantDto input)
        {
            if (string.IsNullOrWhiteSpace(input.Name))
            {
                throw new ArgumentException(nameof(input.Name));
            }
            var tenant = await TenantRepository.FindByIdAsync(input.Id);
            tenant.SetName(input.Name);
            var entity = await TenantRepository.UpdateAsync(tenant);
            return entity.ToModel();
        }

        public virtual async Task DeleteAsync(Guid id)
        {
            var tenant = await TenantRepository.FindByIdAsync(id);
            if (tenant == null)
            {
                return;
            }
            await TenantRepository.DeleteAsync(tenant);
        }

        protected virtual async Task ValidateNameAsync(string name, Guid? expectedId = null)
        {
            var tenant = await TenantRepository.FindByNameAsync(name);
            if (tenant != null && tenant.Id != expectedId)
            {
                throw new Exception("Duplicate tenancy name: " + name);
            }
        }

    }
}
