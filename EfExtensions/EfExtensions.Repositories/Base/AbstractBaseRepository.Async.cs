using System.Linq.Expressions;
using System.Reflection;
using EfExtensions.Core.Attributes;
using EfExtensions.Core.Enum;
using EfExtensions.Core.Interfaces.Model;
using EfExtensions.Core.Interfaces.Result;
using EfExtensions.Items.Result;
using Microsoft.EntityFrameworkCore;

namespace EfExtensions.Repositories.Base;

public abstract partial class AbstractBaseRepository<T, TContext>
    where T: class, IHasOperation
    where TContext : DbContext
{
    protected abstract Task<TContext> GetContextAsync();

    private static async Task<IDbResult<T>> TrySaveAsync(TContext db, T item)
    {
        try
        {
            await db.SaveChangesAsync();
            return DbResult<T>.Ok(item);
        }
        catch (Exception ex)
        {
            return DbResult<T>.Failed(item, ex.Message);
        }
    }

    ///<inheritdoc/>
    public async Task<T?> GetCustomAsync(Func<T, bool> match)
    {
        await using var db = await GetContextAsync();
        return db.Set<T>().AsEnumerable().FirstOrDefault(match.Invoke);
    }

    ///<inheritdoc/>
    public async Task<T[]> GetAllAsync()
    {
        await using var db = await GetContextAsync();
        return await db.Set<T>().ToArrayAsync();
    }

    ///<inheritdoc/>
    public async Task<T[]> GetAllCustomAsync(Expression<Func<T, bool>> match)
    {
        await using var db = await GetContextAsync();
        return await db.Set<T>().Where(match).ToArrayAsync();
    }
    
    ///<inheritdoc/>
    public async Task<IDbResult<T>[]> CrudManyAsync(IEnumerable<T> items)
    {
        var results = new List<IDbResult<T>>();
        
        foreach (var item in items)
        {
            results.Add(await CrudAsync(item));
        }

        return results.ToArray();
    }
    
    ///<inheritdoc/>
    public async Task<IDbResult<T>> CrudAsync(T item)
    {
        return item.OperationType switch
        {
            Operation.Created => await CreateAsync(item),
            Operation.Updated => await UpdateAsync(item),
            Operation.Removed => await DeleteAsync(item),
            Operation.None => DbResult<T>.Ok(item),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    ///<inheritdoc/>
    public async Task<IDbResult<T>> CreateAsync(T item)
    {
        await using var db = await GetContextAsync();
        db.Set<T>().Add(item);
        return await TrySaveAsync(db, item);
    }
    
    ///<inheritdoc/>
    public async Task<IDbResult<T>> UpdateAsync(T item)
    {
        if (typeof(T).GetCustomAttribute(typeof(ComplexCollectionAttribute)) != null)
        {
            return await UpdateComplexAsync(item);
        }
        
        await using var db = await GetContextAsync();
        db.Set<T>().Update(item);
        return await TrySaveAsync(db, item);
    }
    
    ///<inheritdoc/>
    public async Task<IDbResult<T>> DeleteAsync(T item)
    {
        await using var db = await GetContextAsync();
        db.Set<T>().Remove(item);
        return await TrySaveAsync(db, item);
    }
    
    ///<inheritdoc/>
    public async Task<IDbResult<T>> UpdateComplexAsync(T item)
    {
        await using var db = await GetContextAsync();
        var currentItem = await FindAsync(item, db);

        if (currentItem is null) return DbResult<T>.Failed(item, "Item does not exist.");
		
        UpdateCollections(item, currentItem, db);
        UpdateEfItem(item, ref currentItem);

        return await TrySaveAsync(db, item);
    }

    /// <summary>
    /// Method to find an item on a provided db context.
    /// </summary>
    /// <param name="item">Item to find.</param>
    /// <param name="db">Db context to query.</param>
    /// <returns></returns>
    protected abstract Task<T?> FindAsync(T item, TContext db);
}