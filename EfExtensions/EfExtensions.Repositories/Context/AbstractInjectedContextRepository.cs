using EfExtensions.Core.Interfaces.Model;
using Microsoft.EntityFrameworkCore;

namespace EfExtensions.Repositories.Context;

public abstract partial class AbstractInjectedContextRepository<T, TContext> : Base.AbstractBaseRepository<T, TContext>
    where T : class, IHasOperation
    where TContext : DbContext
{
    private readonly TContext _dbContext;
    
    protected AbstractInjectedContextRepository(TContext dbContext)
    {
        _dbContext = dbContext;
    }

    protected override TContext GetContext()
    {
        return _dbContext;
    }

    protected override async Task<TContext> GetContextAsync()
    {
        return await Task.Run(GetContext);
    }
}