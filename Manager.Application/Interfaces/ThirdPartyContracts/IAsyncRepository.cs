using Manager.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Manager.Application.Interfaces.ThirdPartyContracts
{
    public interface IAsyncRepository<T> : IAsyncDisposable
        where T : BaseEntity
    {
        Task<T> GetByIdAsync(int id);
        Task<IList<T>> GetAsync();
        Task<T> GetSingleBySpecAsync(ISpecification<T> specification);
        Task<IList<T>> GetBySpecAsync(ISpecification<T> specification);
        Task<int> GetCountBySpecAsync(ISpecification<T> specification);
        Task AddAsync(T entity);
        Task AddRangeAsync(List<T> entities);
        Task DeleteAsync(T entity);
        Task DeleteRangeAsync(IList<T> entities);
        Task UpdateAsync(T entity);
        Task<float> GetSumBySpecAsync(ISpecification<T> specification, Expression<Func<T, float>> property);


    }
}
