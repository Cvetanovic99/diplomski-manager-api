using System;
using System.Linq.Expressions;
using Manager.Core.Entities;

namespace Manager.Application.Specifications
{
    public class ProjectTaskWithProjectSpecification : BaseSpecification<ProjectTask>
    {
        public ProjectTaskWithProjectSpecification(Expression<Func<ProjectTask, bool>> criteria, int? start = null, int? take = null,
            string orderBy = null, string direction = "ASC")
            : base(criteria, start, take, direction)
        {
            AddInclude(pt => pt.ProductState);
            AddInclude("Project.Client");
            AddInclude(pt => pt.Employed1);
            AddInclude(pt => pt.Employed2);
        }
    }
}