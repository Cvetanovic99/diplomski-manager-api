using Manager.Application.Interfaces;
using Manager.Application.Interfaces.ThirdPartyContracts;
using Manager.Core.Entities;
using Manager.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Manager.Infrastructure.Persistence.Repositories
{
    public class AsyncRepository<T> : IAsyncRepository<T>
            where T : BaseEntity
    {

        private AppDbContext _dbContext;

        public AsyncRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }
        
        public async Task AddRangeAsync(List<T> entities)
        {
            await _dbContext.Set<T>().AddRangeAsync(entities);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
        
        public async Task DeleteRangeAsync(IList<T> entities)
        {
            _dbContext.Set<T>().RemoveRange(entities);
            await _dbContext.SaveChangesAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsync(true);
        }

        public async ValueTask DisposeAsync(bool disposing)
        {
            if (disposing && _dbContext != null)
            {
                await _dbContext.DisposeAsync()
                    .ConfigureAwait(false);

                _dbContext = null;
            }
        }

        public async Task<IList<T>> GetAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public async Task<IList<T>> GetBySpecAsync(ISpecification<T> specification)
        {
            return await GetQueryable(specification).ToListAsync();
        }

        public async Task<float> GetSumBySpecAsync(ISpecification<T> specification, Expression<Func<T, float>> property)
        {
            return  GetQueryable(specification).Select(property).Sum();
        }

        public async Task<int> GetCountBySpecAsync(ISpecification<T> specification)
        {
            return await GetQueryable(specification).CountAsync();
        }
        
        public Task<T> GetSingleBySpecAsync(ISpecification<T> specification)
        {
            return Task.FromResult(GetQueryable(specification).FirstOrDefault());
        }

        public async Task UpdateAsync(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        private IQueryable<T> GetQueryable(ISpecification<T> specification)
        {
            var queryable = specification.Includes
                .Aggregate(_dbContext.Set<T>().AsQueryable(),
                    (current, include) => current.Include(include));

            queryable = specification.IncludeStrings
                .Aggregate(queryable, (current, include) => current.Include(include));


            queryable = specification.OrderByStrings
                .Aggregate(queryable, (current, orderBy) => specification.Direction == "ASC"
                    ? current.OrderBy(orderBy => orderBy)
                    : current.OrderByDescending(orderBy => orderBy));

            queryable = queryable.Where(specification.Criteria);

            if (specification.Skip.HasValue)
            {
                queryable = queryable.Skip(specification.Skip.Value);
            }

            if (specification.Take.HasValue)
            {
                queryable = queryable.Take(specification.Take.Value);
            }

            return queryable;
        }
    }
}
