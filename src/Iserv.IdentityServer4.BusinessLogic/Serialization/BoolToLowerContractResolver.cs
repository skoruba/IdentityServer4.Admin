using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Iserv.IdentityServer4.BusinessLogic.Serialization
{
    public class BoolToLowerContractResolver : DefaultContractResolver
    {
        
        
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            if (property.PropertyType == typeof(bool))
            {
                property.ValueProvider = new BoolToLowerValueProvider(property.ValueProvider);
            }
            return property;
        }
    }
}