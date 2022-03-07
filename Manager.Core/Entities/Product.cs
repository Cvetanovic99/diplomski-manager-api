using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Core.Entities
{
    public class Product : BaseEntity
    {
        public string Manufacturer { get; set; }
     
        public string Supplier { get; set; }
        
        public bool HasSN { get; set; }
        
        public string Name { get; set; }
        
        public string Model { get; set; }
        
        public string Unit { get; set; }
        
        public ICollection<ProductState> States { get; set; }
    }
}
