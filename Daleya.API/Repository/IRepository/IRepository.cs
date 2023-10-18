using System.Linq.Expressions;

namespace Daleya.API.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null,
            Expression<Func<T, object>>? orderByAscending = null,
            Expression<Func<T, object>>? orderByDescending = null);
        Task<T> GetAsync(Expression<Func<T, bool>> filter = null, bool tracked = true, string? includeProperties = null);
        Task CreateAsync(T entity);
        Task RemoveAsync(T entity);
        Task SaveAsync();
    }
}
