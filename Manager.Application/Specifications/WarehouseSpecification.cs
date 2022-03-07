using Manager.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Application.Specifications
{
    public class WarehouseSpecification : BaseSpecification<Warehouse>
    {
        public WarehouseSpecification(Expression<Func<Warehouse, bool>> criteria, int? start = null, int? take = null,
            string orderBy = null, string direction = "ASC")
            : base(criteria, start, take, direction)
        {
            AddOrderBy(orderBy);
        }
    }
}
