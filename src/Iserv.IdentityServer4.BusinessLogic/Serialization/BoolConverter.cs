using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Iserv.IdentityServer4.BusinessLogic.Serialization
{
    public class BoolToLowerConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(bool);
        }
        
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var jObject = JObject.FromObject(value, serializer);
            jObject.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return JsonConvert.DeserializeObject<bool>((string)reader.Value);
        }
    }
}