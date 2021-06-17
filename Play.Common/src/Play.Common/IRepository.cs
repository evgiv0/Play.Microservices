using System.Linq.Expressions;
using Play.Common.Service.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Play.Common.Service.Repositories
{
    public interface IRepository<T> where T : IEntity
    {
        Task CreateAsync(T entity);
        Task DeleteAsync(Guid id);
        Task<IReadOnlyCollection<T>> GetAllAsync();
        Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T, bool>> filter);
        Task<T> GetByIdAsync(Guid id);
        Task<T> GetByIdAsync(Expression<Func<T, bool>> filter);
        Task UpdateAsync(T entity);
    }
}