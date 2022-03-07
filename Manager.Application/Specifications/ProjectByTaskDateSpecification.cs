using System;
using System.Linq;
using System.Linq.Expressions;
using Manager.Core.Entities;

namespace Manager.Application.Specifications
{
    public class ProjectByTaskDateSpecification : BaseSpecification<Project>
    {
        public ProjectByTaskDateSpecification(User employee, Expression<Func<Project, bool>> criteria, 
                                                DateTime startDate, DateTime endDate,
                                                int? start = null, int? take = null, 
                                                string orderBy = null, string direction = "ASC")
            : base(criteria, start, take, direction)
        {
            AddInclude(p => p.Warehouse);
            AddInclude(p => p.Client);
            AddInclude(p => p.Tasks.Where(task => task.CreatedAt >= startDate && task.CreatedAt <= endDate && (task.Employed1 == employee || task.Employed2 == employee)));
            // AddInclude("Tasks.Product");
        }
    }
}