using Manager.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Application.Specifications
{
    public class ProjectTaskByIdspecification : BaseSpecification<ProjectTask>
    {
          public ProjectTaskByIdspecification(int id)
            : base(p => p.Id == id)
        {
            AddInclude(p => p.ProductState);
            AddInclude(p => p.Project);
            AddInclude(p => p.Employed1);
            AddInclude(p => p.Employed2);
        }
    }
}
