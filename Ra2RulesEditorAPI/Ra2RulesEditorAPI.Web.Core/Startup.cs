using Furion;
using Furion.Templates;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;

using Ra2RulesEditorAPI.Core.Utils;

namespace Ra2RulesEditorAPI.Web.Core;

public class Startup : AppStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddJwt<JwtHandler>();

        services.AddMyResponseCompression();

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

        // https 重定向 (会影响跨域,因为开发环境前端是http)
        //app.UseHttpsRedirection();

        app.UseResponseCompression();

        // 开启 wwwroot 静态文件的映射
        app.UseStaticFiles();

        app.UseRouting();

        app.UseCorsAccessor();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseInject(string.Empty);

        var lifetime = App.GetService<IHostApplicationLifetime>();

        lifetime.ApplicationStarted.Register(() =>
        {
            var conf = App.GetService<IConfiguration>();
            var log = App.GetService<ILogger<Startup>>();

            log.LogInformation(TP.Wrapper(
                 "地址",
                 "",
                 $"## db 路径 ## {conf["ConnectionStrings:Sqlite"]}",
                 $"## ini 路径 ## {conf["IniFilePath"]}"
                 ));

        });

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}