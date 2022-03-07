using Manager.Core.Entities;

namespace Manager.Application.Specifications
{
    public class ProductStateByIdSpecification : BaseSpecification<ProductState>
    {
        public ProductStateByIdSpecification(int id)
            : base(ps => ps.Id == id)
        {
            AddInclude(ps => ps.Product);
            AddInclude(ps => ps.Warehouse);
        }
    }
}