namespace Skoruba.IdentityServer4.Admin.EntityFramework.Extensions.Common
{
	public class SelectItem
	{
		public SelectItem(string id, string text)
		{
			Id = id;
			Text = text;
		}

		public string Id { get; set; }

		public string Text { get; set; }
	}
}