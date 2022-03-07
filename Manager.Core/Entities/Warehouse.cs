using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Core.Entities
{
    public class Warehouse : BaseEntity
    {
        public string Name { get; set; }
        public string City { get; set; }
        
        public ICollection<ProductState> States { get; set; }
    }
}
