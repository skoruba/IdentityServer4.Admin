using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;

namespace Skoruba.IdentityServer4.Admin.Helpers
{
    public static class TempDataExtensions
    {
        public static void Set<T>(this ITempDataDictionary tempData, string key, T value)
        {
            tempData.Remove(key);
            string json = JsonConvert.SerializeObject(value);
            tempData.Add(key, json);
        }

        public static T Get<T>(this ITempDataDictionary tempData, string key)
        {
            if (!tempData.ContainsKey(key)) return default(T);

            var value = tempData[key] as string;

            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}