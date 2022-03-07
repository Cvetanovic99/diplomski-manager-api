using System;
using System.Linq;
using System.Linq.Expressions;
using Manager.Core.Entities;

namespace Manager.Application.Specifications
{
    public class WarehouseByProductIdSpecification : BaseSpecification<Warehouse>
    {
        public WarehouseByProductIdSpecification(int productId, Expression<Func<Warehouse, bool>> criteria, int? start = null, int? take = null,
            string orderBy = null, string direction = "ASC")
            : base(criteria, start, take, direction)
        {
            AddInclude(warehouse => warehouse.States.Where(state => state.Product.Id == productId && ((state.Product.HasSN && state.Quantity > 0) || !state.Product.HasSN)));
        }
    }
}