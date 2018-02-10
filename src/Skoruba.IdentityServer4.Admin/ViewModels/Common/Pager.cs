namespace Skoruba.IdentityServer4.Admin.ViewModels.Common
{
	public class Pager
	{
		public int TotalCount { get; set; }

		public int PageSize { get; set; }

		public string Action { get; set; }

        public string Search { get; set; }

	    public bool EnableSearch { get; set; } = false;
	}
}
