using Manager.Application.Interfaces.ThirdPartyContracts;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Infrastructure.FileManager.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFileManager(this IServiceCollection services)
        {
            services.AddScoped<IFileManagerService, FileManagerService>();

            return services;
        }
    }
}
