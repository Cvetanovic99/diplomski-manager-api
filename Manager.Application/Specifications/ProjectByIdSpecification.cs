using System.Linq;
using Manager.Core.Entities;

namespace Manager.Application.Specifications
{
    public class ProjectByIdSpecification : BaseSpecification<Project>
    {
        public ProjectByIdSpecification(int id)
            : base(ad => ad.Id == id)
        {
            AddInclude(project => project.Client);
            AddInclude(project => project.Warehouse);
            AddInclude(project => project.PdfFiles);
        }
    }
}