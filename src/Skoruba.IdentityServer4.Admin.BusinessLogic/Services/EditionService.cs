using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration.Tenants;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Mappers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.ExceptionHandling;
using Skoruba.IdentityServer4.Admin.EntityFramework.Entities;
using Skoruba.IdentityServer4.Admin.EntityFramework.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Services
{
    public class EditionService : IEditionService
    {
        protected readonly ITenantRepository TenantRepository;

        public EditionService(
            ITenantRepository tenantRepository)
        {
            TenantRepository = tenantRepository;
        }

        public virtual async Task<EditionDto> GetAsync(string id)
        {
            Guid.TryParse(id, out Guid result);

            var edition = await TenantRepository.FindEditionByIdAsync(result);
            if (edition == null) throw new UserFriendlyErrorPageException($"There is no tenant by id: {id}");

            var editionDto = edition.ToModel();
            return editionDto;
        }

        public virtual async Task<EditionsDto> GetListAsync(string search, int page = 1, int pageSize = 10)
        {
            var pagedList = await TenantRepository.GetEditionListAsync(search, page, pageSize);
            var editionsDto = pagedList.ToModel();
            return editionsDto;
        }

        public virtual async Task<List<EditionDto>> GetAllListAsync()
        {
            var editionList = await TenantRepository.GetEditionListAsync();
            return editionList.ToModel();
        }

        public virtual async Task<EditionDto> CreateAsync(CreateEditionDto input)
        {
            if (string.IsNullOrWhiteSpace(input.Name))
            {
                throw new ArgumentException(nameof(input.Name));
            }
            await ValidateNameAsync(input.Name);
            var edition = new Edition(Guid.NewGuid(), input.Name);
            var entity = await TenantRepository.AddEditionAsync(edition);
            return entity.ToModel();
        }

        public virtual async Task<EditionDto> UpdateAsync(UpdateEditionDto input)
        {
            if (string.IsNullOrWhiteSpace(input.Name))
            {
                throw new ArgumentException(nameof(input.Name));
            }
            var edition = await TenantRepository.FindEditionByIdAsync(input.Id);
            edition.SetName(input.Name);
            var entity = await TenantRepository.UpdateEditionAsync(edition);
            return entity.ToModel();
        }

        public virtual async Task DeleteAsync(Guid id)
        {
            var edition = await TenantRepository.FindEditionByIdAsync(id);
            if (edition == null)
            {
                return;
            }
            await TenantRepository.DeleteEditionAsync(edition);
        }

        protected virtual async Task ValidateNameAsync(string name, Guid? expectedId = null)
        {
            var edition = await TenantRepository.FindEditionByNameAsync(name);
            if (edition != null && edition.Id != expectedId)
            {
                throw new Exception("Duplicate edition name: " + name);
            }
        }

    }
}
