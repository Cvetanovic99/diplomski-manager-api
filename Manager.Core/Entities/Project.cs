using System.Collections.Generic;

namespace Manager.Core.Entities
{
    public class Project : BaseEntity
    {
        public string Title { get; set; }

        public string State { get; set; }

        public Client Client { get; set; }

        public Warehouse Warehouse { get; set; }

        public ICollection<ProjectTask> Tasks { get; set; }
        
        public ICollection<PdfFile> PdfFiles { get; set; }
    }
}