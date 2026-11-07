using System.Linq.Expressions;
using BuckleApp.Core.IRepositories;
using BuckleApp.Data.Data;
using Microsoft.EntityFrameworkCore;

namespace BuckleApp.Data.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationContext Context;

    protected Repository(ApplicationContext context)
    {
        Context = context;
    }

    public virtual async Task<T> GetAsync(Guid id)
    {
        return await Context.Set<T>().FindAsync(id);
    }

    public async Task<T> GetAsync(string id)
    {
        return await Context.Set<T>().FindAsync(id);
    }

    public Task<List<T>> GetAllAsync()
    {
        return Context.Set<T>().ToListAsync();
    }

    public IQueryable<T> Query(Expression<Func<T, bool>> predicate = null,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = Context.Set<T>();

        if (predicate != null)
            query = query.Where(predicate);

        return includes == null ? query : includes.Aggregate(query, (current, include) => current.Include(include));
    }

    public IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
    {
        return Context.Set<T>().Where(predicate);
    }

    public async Task<T> SingleOrDefault(Expression<Func<T, bool>> predicate)
    {
        return await Context.Set<T>().SingleOrDefaultAsync(predicate);
    }

    public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        return await Context.Set<T>().FirstOrDefaultAsync(predicate);
    }

    public async Task AddAsync(T entity)
    {
        await Context.Set<T>().AddAsync(entity);
    }

    public async Task AddRange(IEnumerable<T> entities)
    {
        await Context.Set<T>().AddRangeAsync(entities);
    }

    public void Remove(T entity)
    {
        Context.Set<T>().Remove(entity);
    }

    public void RemoveRange(IEnumerable<T> entities)
    {
        Context.Set<T>().RemoveRange(entities);
    }
}