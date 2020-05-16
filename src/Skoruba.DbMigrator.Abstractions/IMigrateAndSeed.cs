using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Skoruba.DbMigrator.Abstractions
{
    public interface IMigrateAndSeed
    {
        Task Migrate(IServiceCollection services);
        Task Seed(IServiceCollection services, IConfigurationRoot configurationRoot);
    }

}
