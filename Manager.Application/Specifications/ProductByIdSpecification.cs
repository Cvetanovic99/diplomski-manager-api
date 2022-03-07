using System.Linq;
using Manager.Core.Entities;

namespace Manager.Application.Specifications
{
    public class ProductByIdSpecification : BaseSpecification<Product>
    {
        public ProductByIdSpecification(int id)
            : base(ad => ad.Id == id)
        {
            AddInclude("States.Warehouse");
        }
    }
}