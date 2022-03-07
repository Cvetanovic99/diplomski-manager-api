using Manager.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Infrastructure.Identity.Contexts
{
    public class AppDbContextFactory : DesignTimeDbContextFactoryBase<IdentityAppDbContext>
    {
        protected override IdentityAppDbContext CreateNewInstance(DbContextOptions<IdentityAppDbContext> options)
        {
            return new IdentityAppDbContext(options);
        }

    }
}
