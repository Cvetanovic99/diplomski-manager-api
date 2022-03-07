using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Core.Entities
{
    public class ProductState : BaseEntity
    {
        public float Quantity { get; set; }
        
        public string SN { get; set; }
        public Warehouse Warehouse { get; set; }
        
        public Product Product { get; set; } 

        public int ProductId { get; set; }

    }
}
