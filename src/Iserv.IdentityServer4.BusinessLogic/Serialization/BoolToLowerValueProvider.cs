using System;
using Newtonsoft.Json.Serialization;

namespace Iserv.IdentityServer4.BusinessLogic.Serialization
{
    public class BoolToLowerValueProvider : IValueProvider
    {
        private readonly IValueProvider _baseProvider;

        public BoolToLowerValueProvider(IValueProvider baseProvider)
        {
            _baseProvider = baseProvider ?? throw new ArgumentNullException();
        }
            
        public void SetValue(object target, object value)
        {
            _baseProvider.SetValue(target, value);
        }

        public object GetValue(object target)
        {
            return _baseProvider.GetValue(target).ToString().ToLower();
        }
    }
}