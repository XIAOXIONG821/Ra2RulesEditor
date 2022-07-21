using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;

using MyDbContext;

using MyUtils;

using Ra2RulesWebApi.Services;

namespace Ra2RulesWebApi
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var services = builder.Services;
            var conf = builder.Configuration;
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddDbContext<SqliteDbContext>(options =>
            {
                // 数据库地址
                options.UseSqlite(conf.GetConnectionString("Sqlite"),
                    (optionsBuilder) =>
                    {
                        // 迁移文件放到 DbMigrations 类库中
                        optionsBuilder.MigrationsAssembly("DbMigrations");
                    });
            });

            services.AddSingleton(new IniFileHelper(conf.GetValue<string>("IniFilePath")));
            services.AddScoped<EditorServices>();
            services.AddCors(options =>
            {
                options.AddPolicy("all", builder =>
                {
                    builder
                    .WithMethods("GET", "POST", "HEAD", "PUT", "DELETE", "OPTIONS")
                    .AllowAnyHeader()
                    .AllowAnyOrigin(); //允许任何来源的主机访问

                    //.AllowCredentials()//(不能加,加上就报错.不能共存)
                });
            });

            var app = builder.Build();

            //if (app.Environment.IsDevelopment())
            //{
            app.UseSwagger();
            app.UseSwaggerUI();
            //}
            app.UseStaticFiles();
            app.Use(async (ctx, next) =>
            {
                var req = ctx.Request;

                if (req.Method != "OPTIONS")
                {
                    Console.WriteLine($"    {req.Method}        {req.Path}{req.QueryString.Value}");
                }

                await next.Invoke();
            });
            app.UseAuthorization();
            app.UseCors("all"); // 跨域放在验证后面
            app.MapControllers();

            app.Run();
        }
    }
}