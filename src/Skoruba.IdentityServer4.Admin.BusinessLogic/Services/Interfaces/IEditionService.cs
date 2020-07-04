using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration.Tenants;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces
{
    public interface IEditionService
    {
        Task<EditionDto> GetAsync(string id);
        Task<EditionsDto> GetListAsync(string search, int page = 1, int pageSize = 10);
        Task<List<EditionDto>> GetAllListAsync();
        Task<EditionDto> CreateAsync(CreateEditionDto input);
        Task<EditionDto> UpdateAsync(UpdateEditionDto input);
        Task DeleteAsync(Guid id);
    }
}
