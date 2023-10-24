using EfExtensions.Core.Interfaces.Model;
using EfExtensions.Core.Interfaces.Repository;
using EfExtensions.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace EfExtensions.Repositories.Extended;

/// <summary>
/// A base, abstract Repository which provides all necessary behavior to get EF Core up and running quickly.
/// This is the repository for items which use an int id as their primary key.
/// </summary>
/// <typeparam name="T">Item type.</typeparam>
/// <typeparam name="TKey">Item key type.</typeparam>
/// <typeparam name="TContext">Db Context type.</typeparam>
public abstract class AbstractBaseBaseKeyedRepository<T, TKey, TContext> : AbstractBaseRepository<T, TContext>, IBaseKeyedRepository<T, TKey>
    where T : class, IDbItem<TKey>
    where TContext : DbContext
{
    
    public T? Get(TKey id)
    {
        using var db = GetContext();
        return db.Set<T>().Find(id);
    }

    public async Task<T?> GetAsync(TKey id)
    {
        await using var db = await GetContextAsync();
        return await db.Set<T>().FindAsync(id);
    }
    
    protected override T? Find(T item, TContext db)
    {
        return db.Set<T>().Find(item.Id);
    }

    protected override async Task<T?> FindAsync(T item, TContext db)
    {
        return await db.Set<T>().FindAsync(item.Id);
    }
}