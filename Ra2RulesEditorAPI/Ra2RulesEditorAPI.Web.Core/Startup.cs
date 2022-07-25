using Furion;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Ra2RulesEditorAPI.Core.Utils;

namespace Ra2RulesEditorAPI.Web.Core;

public class Startup : AppStartup
{
  

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddJwt<JwtHandler>();
        // 注入 ini 操作类
        services.AddSingleton(new IniFileHelper(App.Configuration["IniFilePath"]));

        services.AddCorsAccessor();

        services.AddControllers()
                .AddInjectWithUnifyResult();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        //app.UseHttpsRedirection();

        app.UseRouting();

        app.UseCorsAccessor();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseInject(string.Empty);

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
