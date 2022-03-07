using Manager.Application.Interfaces.ThirdPartyContracts;
using Manager.Infrastructure.Identity.Contexts;
using Manager.Infrastructure.Identity.Models;
using Manager.Infrastructure.Identity.Options;
using Manager.Infrastructure.Identity.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Infrastructure.Identity.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddIdentity(this IServiceCollection services)
        {
           
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
          
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .Build();

          
            services.AddDbContext<IdentityAppDbContext>(options =>
            {
                options.UseMySQL(
                    configuration.GetConnectionString("DefaultConnection"),
                    x => x.MigrationsAssembly("Diplomski.Infrastructure"));
            });

           
            services.AddIdentity<IdentityAppUser, IdentityRole>()
                    .AddEntityFrameworkStores<IdentityAppDbContext>()
                    .AddDefaultTokenProviders();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();

            
            services.AddSingleton(typeof(JwtSecurityTokenHandler));

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SigningKey"]));

            services.Configure<JwtOption>(option =>
            {
                option.Audience = configuration["JWT:Audience"];
                option.Issuer = configuration["JWT:Issuer"];
                option.SigningKey = configuration["JWT:SigningKey"];
                option.SigninCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            });

            services.AddAuthentication(config =>
            {
                config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(config =>
            {
                config.RequireHttpsMetadata = false;
                config.SaveToken = true;
                config.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ClockSkew = TimeSpan.Zero,

                    ValidAudience = configuration["JWT:Audience"],
                    ValidIssuer = configuration["JWT:Issuer"],
                    IssuerSigningKey = signingKey,

                };
            });

            return services;
        }
    }
}
