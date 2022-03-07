using System.IO;
using Manager.Application.Interfaces.ThirdPartyContracts;
using Manager.Infrastructure.Storage.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace Manager.Infrastructure.Storage.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFileStorage(this IServiceCollection services)
        {
            services.AddTransient<IStorageService, StorageService>();
            return services;
        }

        public static IApplicationBuilder UseFileStorage(this IApplicationBuilder app, IWebHostEnvironment env) {
            
            // var path = Path.Combine("var/www/html/crm/manager-api/storage/pdffiles").ToLower();
            //
            // if (!Directory.Exists(path))
            // {
            //     Directory.CreateDirectory(path);
            // }
            //
            // app.UseStaticFiles(new StaticFileOptions
            // {
            //     FileProvider = new PhysicalFileProvider(
            //         Path.Combine("var/www/html/crm/manager-api/storage/pdffiles")),
            //     RequestPath = "/pdffiles"
            // });

            return app;
        }
    }
}