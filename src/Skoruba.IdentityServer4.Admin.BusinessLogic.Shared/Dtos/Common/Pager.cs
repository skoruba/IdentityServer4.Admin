namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.Dtos.Common
{
	public class Pager
	{
		public int TotalCount { get; set; }

		public int PageSize { get; set; }

		public string Action { get; set; }

        public string Search { get; set; }

	    public bool EnableSearch { get; set; } = false;

	    public int MaxPages { get; set; } = 10;
	}
}
