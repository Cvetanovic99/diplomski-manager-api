using Manager.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Application.Specifications
{
    public class ProjectTaskSpecification : BaseSpecification<ProjectTask>
    {
        public ProjectTaskSpecification(Expression<Func<ProjectTask, bool>> criteria, int? start = null, int? take = null,
           string orderBy = null, string direction = "ASC")
           : base(criteria, start, take, direction)
        {
            AddInclude(pt => pt.ProductState);
            AddOrderBy(orderBy);
            AddInclude(pt => pt.Employed1);
            AddInclude(pt => pt.Employed2);
        }
    }
}
