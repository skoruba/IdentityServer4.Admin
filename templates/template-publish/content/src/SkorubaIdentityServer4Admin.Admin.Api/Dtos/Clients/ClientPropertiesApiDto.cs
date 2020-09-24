using System.Collections.Generic;

namespace SkorubaIdentityServer4Admin.Admin.Api.Dtos.Clients
{
    public class ClientPropertiesApiDto
    {
        public ClientPropertiesApiDto()
        {
            ClientProperties = new List<ClientPropertyApiDto>();
        }

        public List<ClientPropertyApiDto> ClientProperties { get; set; }

        public int TotalCount { get; set; }

        public int PageSize { get; set; }
    }
}





