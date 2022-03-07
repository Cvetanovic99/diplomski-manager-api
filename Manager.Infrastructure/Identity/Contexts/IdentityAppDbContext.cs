using Manager.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Manager.Infrastructure.Identity.Constants;

namespace Manager.Infrastructure.Identity.Contexts
{
    public class IdentityAppDbContext : IdentityDbContext<IdentityAppUser>
    {
        public IdentityAppDbContext(DbContextOptions<IdentityAppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<RefreshToken>().ToTable("RefreshTokens");
            builder.Entity<IdentityAppUser>(entity => entity.Property(m => m.Id).HasMaxLength(85));
            builder.Entity<IdentityRole>(entity => entity.Property(m => m.Id).HasMaxLength(85));

            builder.Entity<IdentityUserClaim<string>>(entity => entity.Property(m => m.Id).HasMaxLength(85));
            builder.Entity<IdentityRoleClaim<string>>(entity => entity.Property(m => m.Id).HasMaxLength(85));

            builder.Entity<IdentityUserLogin<string>>(entity => entity.Property(m => m.LoginProvider).HasMaxLength(85));
            builder.Entity<IdentityUserLogin<string>>(entity => entity.Property(m => m.ProviderKey).HasMaxLength(85));

            builder.Entity<IdentityUserToken<string>>(entity => entity.Property(m => m.LoginProvider).HasMaxLength(85));
            builder.Entity<IdentityUserToken<string>>(entity => entity.Property(m => m.Name).HasMaxLength(85));
         
            List<IdentityRole> roles = new List<IdentityRole>();
            foreach (KeyValuePair<string, string> item in IdentityRoles.Roles)
            {
                var role = new IdentityRole(item.Value);
                role.NormalizedName = item.Value.ToUpper();

                roles.Add(role);
            }

            builder.Entity<IdentityRole>().HasData(roles);
            base.OnModelCreating(builder);
        }
    }
}
