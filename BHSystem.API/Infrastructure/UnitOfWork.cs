using BHSystem.API.Repositories;

namespace BHSystem.API.Infrastructure
{
    public interface IUnitOfWork
    {
        Task CompleteAsync();
    }
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly ApplicationDbContext _context;
        public UnitOfWork(ApplicationDbContext context) => _context = context;
        public async Task CompleteAsync() => await _context.SaveChangesAsync();
        public void Dispose() => _context.Dispose();

    }
}
