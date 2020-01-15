using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Skoruba.MultiTenant.Identity
{
    public static class IdentityDbContextExtensions
    {
        public static void RemoveIndex(this EntityTypeBuilder builder, string propName)
        {
            var prop = builder.Metadata.FindProperty(propName);
            var index = builder.Metadata.FindIndex(prop);
            builder.Metadata.RemoveIndex(index);
        }
    }
}
