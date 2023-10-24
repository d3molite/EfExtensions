using EfExtensions.Core.Interfaces.Model;
using EfExtensions.Core.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace EfExtensions.Repositories.Context;

public abstract partial class AbstractInjectedContextRepository<T, TContext> : ITransactionRepository<T>
    where T : class, IHasOperation
    where TContext : DbContext
{
    public void StartTransaction()
    {
        _dbContext.Database.BeginTransaction();
    }
    
    public async Task StartTransactionAsync()
    {
        await _dbContext.Database.BeginTransactionAsync();
    }
    
    public void CommitTransaction()
    {
        _dbContext.Database.CommitTransaction();
    }
    
    public async Task CommitTransactionAsync()
    {
        await _dbContext.Database.CommitTransactionAsync();
    }
    
    public void RollbackTransaction()
    {
        _dbContext.Database.RollbackTransaction();
    }
    
    public async Task RollbackTransactionAsync()
    {
        await _dbContext.Database.RollbackTransactionAsync();
    }
}