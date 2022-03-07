using Manager.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Application.Specifications
{
    public abstract class BaseSpecification<T> : ISpecification<T>
    {
        protected Expression<Func<T, bool>> _criteria;
        protected List<Expression<Func<T, object>>> _includes;
        protected List<string> _includeStrings;
        protected List<string> _orderByStrings;
        protected List<Expression<Func<T, object>>> _orderBy;

        protected string _direction;
        protected int? _skip;
        protected int? _take;
        protected BaseSpecification()
        {
            _includes = new List<Expression<Func<T, object>>>();
            _includeStrings = new List<string>();
            _orderByStrings = new List<string>();
            _orderBy = new List<Expression<Func<T, object>>>();
        }

        protected BaseSpecification(Expression<Func<T, bool>> criteria, int? skip = null, int? take = null, string direction = "ASC")
            : this()
        {
            _criteria = criteria;
            _direction = direction;

            if (skip.HasValue && take.HasValue)
            {
                _skip = skip.Value;
                _take = take.Value;
            }
        }

        public Expression<Func<T, bool>> Criteria => _criteria;
        public IReadOnlyList<Expression<Func<T, object>>> Includes => _includes;
        public IReadOnlyList<string> IncludeStrings => _includeStrings;
        public IReadOnlyList<string> OrderByStrings => _orderByStrings;
        public IReadOnlyList<Expression<Func<T, object>>> OrderBy => _orderBy;
        public int? Skip => _skip;
        public int? Take => _take;
        public string Direction => _direction;

        protected virtual void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            _includes.Add(includeExpression);
        }

        protected virtual void AddInclude(string includeString)
        {
            _includeStrings.Add(includeString);
        }

        protected virtual void AddOrderBy(string orderByString)
        {
            _orderByStrings.Add(orderByString);
        }

        protected virtual void AddOrderBy(Expression<Func<T, object>> includeExpression)
        {
            _orderBy.Add(includeExpression);
        }
    }
}
