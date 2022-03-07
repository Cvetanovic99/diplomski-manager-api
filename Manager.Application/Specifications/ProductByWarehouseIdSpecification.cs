using Manager.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Application.Specifications
{
    public class ProductByWarehouseIdSpecification : BaseSpecification<Product>
    {
        public ProductByWarehouseIdSpecification(int warehouseId, Expression<Func<Product, bool>> criteria, int? start = null, int? take = null,
            string orderBy = null, string direction = "ASC")
            : base(criteria, start, take, direction)
        {
            AddInclude(product => product.States.Where(state => state.Warehouse.Id == warehouseId && ((state.Product.HasSN && state.Quantity > 0) || !state.Product.HasSN)));
        }
    }
}
