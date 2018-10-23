using System.Collections.Generic;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.Dtos.Common
{
    public class PagedList<T> where T : class 
    {
        public PagedList()
        {
            Data = new List<T>();
        }

        public List<T> Data { get; }

        public int TotalCount { get; set; }

        public int PageSize { get; set; }
    }
}
