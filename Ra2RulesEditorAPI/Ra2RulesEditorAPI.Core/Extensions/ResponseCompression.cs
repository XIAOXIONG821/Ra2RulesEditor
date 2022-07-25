using System.IO.Compression;
using System.Linq;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.ResponseCompression;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ResponseCompression
    {
        /// <summary>
        /// 响应压缩
        ///
        /// services.AddMyResponseCompression(); 需要配合 app.UseResponseCompression();
        /// </summary>
        /// <param name="services"></param>
        public static void AddMyResponseCompression(this IServiceCollection services)
        {
            // 第一步: 配置gzip与br的压缩等级为最优
            services.Configure<BrotliCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            });

            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            });

            // 第二步: 添加中间件
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                // 添加br与gzip的Provider
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
                // 扩展一些类型 (MimeTypes中有一些基本的类型,可以打断点看看)
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
                {
                    "text/html; charset=utf-8",
                    "application/xhtml+xml",
                    "application/atom+xml",
                    "image/svg+xml"
                });
            });
        }
    }
}