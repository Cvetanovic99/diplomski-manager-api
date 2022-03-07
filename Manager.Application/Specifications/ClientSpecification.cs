using System;
using System.Linq.Expressions;
using Manager.Core.Entities;

namespace Manager.Application.Specifications
{
    public class ClientSpecification : BaseSpecification<Client>
    {
        public ClientSpecification(Expression<Func<Client, bool>> criteria, int? start = null, int? take = null,
            string orderBy = null, string direction = "ASC")
            : base(criteria, start, take, direction)
        {
            
        }
    }
}