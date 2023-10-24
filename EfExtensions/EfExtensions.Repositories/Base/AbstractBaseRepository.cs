using System.Linq.Expressions;
using System.Reflection;
using EfExtensions.Core.Attributes;
using EfExtensions.Core.Enum;
using EfExtensions.Core.Interfaces.Model;
using EfExtensions.Core.Interfaces.Repository;
using EfExtensions.Core.Interfaces.Result;
using EfExtensions.Items.Result;
using Microsoft.EntityFrameworkCore;

namespace EfExtensions.Repositories.Base;

/// <summary>
/// A base, abstract Repository which provides all necessary behavior to get EF Core up and running quickly.
/// </summary>
/// <typeparam name="T">Item type.</typeparam>
/// <typeparam name="TContext">Db Context type.</typeparam>
public abstract partial class AbstractBaseRepository<T, TContext> : IBaseRepository<T>
    where T: class, IHasOperation
    where TContext : DbContext
{
    protected abstract TContext GetContext();

    private static IDbResult<T> TrySave(TContext db, T item)
    {
        try
        {
            db.SaveChanges();
            return DbResult<T>.Ok(item);
        }
        catch (Exception ex)
        {
            return DbResult<T>.Failed(item, ex.Message);
        }
    }

    ///<inheritdoc/>
    public T? GetCustom(Func<T, bool> match)
    {
        using var db = GetContext();
        return db.Set<T>().AsEnumerable().FirstOrDefault(match.Invoke);
    }

    ///<inheritdoc/>
    public T[] GetAll()
    {
        using var db = GetContext();
        return db.Set<T>().ToArray();
    }

    ///<inheritdoc/>
    public T[] GetAllCustom(Expression<Func<T, bool>> match)
    {
        using var db = GetContext();
        return db.Set<T>().Where(match).ToArray();
    }
    
    ///<inheritdoc/>
    public IDbResult<T>[] CrudMany(IEnumerable<T> items)
    {
        return items.Select(Crud).ToArray();
    }
    
    ///<inheritdoc/>
    public IDbResult<T> Crud(T item)
    {
        return item.OperationType switch
        {
            Operation.Created => Create(item),
            Operation.Updated => Update(item),
            Operation.Removed => Delete(item),
            Operation.None => DbResult<T>.Ok(item),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    ///<inheritdoc/>
    public IDbResult<T> Update(T item)
    {
        if (typeof(T).GetCustomAttribute(typeof(ComplexCollectionAttribute)) != null)
        {
            return UpdateComplex(item);
        }
        
        using var db = GetContext();
        db.Set<T>().Update(item);
        return TrySave(db, item);
    }
    
    ///<inheritdoc/>
    public IDbResult<T> Create(T item)
    {
        using var db = GetContext();
        db.Set<T>().Add(item);
        return TrySave(db, item);
    }
    
    ///<inheritdoc/>
    public IDbResult<T> Delete(T item)
    {
        using var db = GetContext();
        db.Set<T>().Remove(item);
        return TrySave(db, item);
    }
    
    ///<inheritdoc/>
    public IDbResult<T> UpdateComplex(T item)
    {
        using var db = GetContext();
        
        var currentItem = Find(item, db);

        if (currentItem is null) return DbResult<T>.Failed(item, "Item does not exist.");
		
        UpdateCollections(item, currentItem, db);
        UpdateEfItem(item, ref currentItem);

        return TrySave(db, item);
    }
    
    /// <summary>
    /// Method to find an item on a provided db context.
    /// </summary>
    /// <param name="item">Item to find.</param>
    /// <param name="db">Db context to query.</param>
    /// <returns></returns>
    protected abstract T? Find(T item, TContext db);
    
    /// <summary>
    /// Method to update collections on an existing database item with a provided db context.
    /// which removes orphaned items which no longer exist on an incoming item.
    /// </summary>
    /// <param name="incoming">Incoming item.</param>
    /// <param name="existing">Item in the database.</param>
    /// <param name="db">Db context.</param>
    protected abstract void UpdateCollections(T incoming, T existing, TContext db);

    /// <summary>
    /// Method to update an existing database item with values from an incoming item.
    /// Necessary when complex collection updates are made.
    /// </summary>
    /// <param name="incoming">Incoming item.</param>
    /// <param name="existing">Item in the database.</param>
    protected abstract void UpdateEfItem(T incoming, ref T existing);
}