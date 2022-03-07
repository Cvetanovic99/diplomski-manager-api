using System;
using System.Linq.Expressions;
using Manager.Core.Entities;

namespace Manager.Application.Specifications
{
    public class ProductSpecification : BaseSpecification<Product>
    {
        public ProductSpecification(Expression<Func<Product, bool>> criteria, int? start = null, int? take = null,
            string orderBy = null, string direction = "ASC")
            : base(criteria, start, take, direction)
        {
            
        }
    }
}