namespace LMS.Application.Interfaces.Repositories;

public interface IUnitOfWork
{
    Task SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}