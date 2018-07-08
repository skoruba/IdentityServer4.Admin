using System.Collections.Generic;
using Newtonsoft.Json;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Common;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Helpers
{
	public static class ComboBoxHelpers
	{
		public static void PopulateValuesToList(string values, List<string> list)
		{
			if (string.IsNullOrEmpty(values)) return;

			var listValues = JsonConvert.DeserializeObject<List<string>>(values);
			if (listValues == null) return;

			list.AddRange(listValues);
		}

	    public static void PopulateValue(string value)
	    {
	        if (string.IsNullOrEmpty(value)) return;

	        var selectedValue = JsonConvert.DeserializeObject<string>(value);
	        if (selectedValue == null) return;

	        value = selectedValue;
	    }
    }
}