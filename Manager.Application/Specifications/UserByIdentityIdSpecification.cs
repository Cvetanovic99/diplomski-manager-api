using Manager.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Application.Specifications
{
    public class UserByIdentityIdSpecification : BaseSpecification<User>
    {
        public UserByIdentityIdSpecification(string identityId)
            : base(user => user.IdentityId == identityId)
        {
        }
    }
}
