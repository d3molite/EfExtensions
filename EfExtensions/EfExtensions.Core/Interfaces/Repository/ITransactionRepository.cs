namespace EfExtensions.Core.Interfaces.Repository;

public interface ITransactionRepository<T> : IBaseRepository<T>
{
    public void StartTransaction();

    public Task StartTransactionAsync();

    public void CommitTransaction();

    public Task CommitTransactionAsync();

    public void RollbackTransaction();

    public Task RollbackTransactionAsync();
}