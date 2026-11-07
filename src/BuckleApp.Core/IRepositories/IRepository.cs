using System.Linq.Expressions;

namespace BuckleApp.Core.IRepositories;

public interface IRepository<T>
{
    Task<T> GetAsync(Guid id);
    Task<T> GetAsync(string id);

    Task<List<T>> GetAllAsync();

    IQueryable<T> Query(Expression<Func<T, bool>> filter = null,
        params Expression<Func<T, object>>[] includes);

    IEnumerable<T> Find(Expression<Func<T, bool>> predicate);

    Task<T> SingleOrDefault(Expression<Func<T, bool>> predicate);
    Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

    Task AddAsync(T entity);
    Task AddRange(IEnumerable<T> entities);

    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
}