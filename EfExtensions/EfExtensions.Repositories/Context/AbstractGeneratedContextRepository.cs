using EfExtensions.Core.Interfaces.Model;
using EfExtensions.Repositories.Extended;
using Microsoft.EntityFrameworkCore;

namespace EfExtensions.Repositories.Context;

public abstract class AbstractGeneratedContextRepository<T, TKey, TContext> : AbstractBaseBaseKeyedRepository<T, TKey, TContext>
    where T : class, IDbItem<TKey>
    where TContext : DbContext 
{
    private readonly IDbContextFactory<TContext> _dbContextFactory;

    protected AbstractGeneratedContextRepository(IDbContextFactory<TContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    protected override TContext GetContext()
    {
        return _dbContextFactory.CreateDbContext();
    }

    protected override async Task<TContext> GetContextAsync()
    {
        return await _dbContextFactory.CreateDbContextAsync();
    }
}