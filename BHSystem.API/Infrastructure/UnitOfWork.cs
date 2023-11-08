using BHSystem.API.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace BHSystem.API.Infrastructure
{
    public interface IUnitOfWork
    {
        Task CompleteAsync();
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;
        public UnitOfWork(ApplicationDbContext context) => _context = context;

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            if (_transaction != null) await _transaction.CommitAsync();
        }
        public async Task RollbackAsync()
        {
            if (_transaction != null) await _transaction.RollbackAsync();
        }

        public async Task CompleteAsync() => await _context.SaveChangesAsync();
        public void Dispose() => _context.Dispose();

        
    }
}
