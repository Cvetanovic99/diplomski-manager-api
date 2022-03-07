using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Manager.Infrastructure.Identity.Models
{
    public class IdentityAppUser : IdentityUser
    {
        public ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
