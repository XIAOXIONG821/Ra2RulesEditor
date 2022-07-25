using Furion;

using Microsoft.Extensions.DependencyInjection;

namespace Ra2RulesEditorAPI.EntityFramework.Core;

public class Startup : AppStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDatabaseAccessor(options =>
        {
            options.AddDbPool<DefaultDbContext>();
        }, "Ra2RulesEditorAPI.Database.Migrations");
    }
}