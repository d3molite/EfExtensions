namespace EfExtensions.Core.Interfaces.Repository;

public interface IBaseKeyedRepository<T, in TKey>
{
    /// <summary>
    /// Tries to find an item with the matching Id.
    /// </summary>
    /// <param name="id">Id of the item to find.</param>
    /// <returns>The item if found, otherwise null.</returns>
    public T? Get(TKey id);
    
    /// <summary>
    /// Tries to find an item with the matching Id.
    /// </summary>
    /// <param name="id">Id of the item to find.</param>
    /// <returns>The item if found, otherwise null.</returns>
    public Task<T?> GetAsync(TKey id);
}