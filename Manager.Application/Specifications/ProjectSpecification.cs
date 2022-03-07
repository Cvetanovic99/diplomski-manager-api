using System;
using System.Linq.Expressions;
using Manager.Core.Entities;

namespace Manager.Application.Specifications
{
    public class ProjectSpecification : BaseSpecification<Project>
    {
        public ProjectSpecification(Expression<Func<Project, bool>> criteria, int? start = null, int? take = null,
            string orderBy = null, string direction = "ASC")
            : base(criteria, start, take, direction)
        {
            AddInclude(p => p.Warehouse);
            AddInclude(p => p.Client);
        }
    }
}