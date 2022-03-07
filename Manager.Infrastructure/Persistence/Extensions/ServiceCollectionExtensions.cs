using Manager.Application.Interfaces.ThirdPartyContracts;
using Manager.Infrastructure.Persistence.Contexts;
using Manager.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Infrastructure.Persistence.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .Build();

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseMySQL(
                    configuration.GetConnectionString("DefaultConnection"),
                    x => x.MigrationsAssembly("Diplomski.Infrastructure"));
            });

            services.AddScoped(typeof(IAsyncRepository<>), typeof(AsyncRepository<>));
            //services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }
    }
}
