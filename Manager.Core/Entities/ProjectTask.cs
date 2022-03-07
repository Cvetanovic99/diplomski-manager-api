using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Core.Entities
{
    public class ProjectTask : BaseEntity
    {
        public float QuantityUsed { get; set; }
        public string Description { get; set; }
        
        public string SN { get; set; }
        
        public int? ProductStateId { get; set; }
        public ProductState ProductState { get; set; }
        public Project Project { get; set; }
        public User Employed1 { get; set; }
        public User Employed2 { get; set; }
        



    }
}
