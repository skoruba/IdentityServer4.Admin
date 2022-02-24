using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Skoruba.IdentityServer4.Admin.UI.Configuration;

namespace SkorubaIdentityServer4Admin.Admin.Helpers;

public static class StartupHelpers
{
    public static void AddAdminUIRazorRuntimeCompilation(this IServiceCollection services, IWebHostEnvironment hostingEnvironment)
    {
        if (hostingEnvironment.IsDevelopment())
        {
            var builder = services.AddControllersWithViews();

            var adminAssembly = typeof(AdminConfiguration).GetTypeInfo().Assembly.GetName().Name;

            builder.AddRazorRuntimeCompilation(options =>
            {
                if (adminAssembly == null) return;

                var libraryPath = Path.GetFullPath(Path.Combine(hostingEnvironment.ContentRootPath, "..", adminAssembly));

                if (Directory.Exists(libraryPath))
                {
                    options.FileProviders.Add(new PhysicalFileProvider(libraryPath));
                }
            });
        }
    }
}







