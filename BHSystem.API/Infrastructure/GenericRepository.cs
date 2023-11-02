using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BHSystem.API.Infrastructure
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<T?> GetById(int id);
        Task<bool> Add(T entity);
        Task<bool> Delete();
        void Update(T entity);
        Task<bool> CheckContainsAsync(Expression<Func<T, bool>> predicate);
        Task<T?> GetSingleByCondition(Expression<Func<T, bool>> expression);
        Task<bool> DeleteMulti(IEnumerable<T> objects);

    }    
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected ApplicationDbContext _context;
        protected DbSet<T> _dbSet;
        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAll() => await _dbSet.ToListAsync();

        public async Task<T?> GetById(int id) => await _dbSet.FindAsync(id);

        public async Task<bool> Add(T entity)
        {
            await _dbSet.AddAsync(entity);
            return true;
        }

        public Task<bool> Delete()
        {
            throw new NotImplementedException();
        }

        public void Update(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public async Task<bool> CheckContainsAsync(Expression<Func<T, bool>> predicate) => await _dbSet.CountAsync<T>(predicate) > 0;

        public async Task<T?> GetSingleByCondition(Expression<Func<T, bool>> expression) => await _dbSet.FirstOrDefaultAsync(expression);

        public async Task<bool> DeleteMulti(IEnumerable<T> objects)
        {
            foreach (T oEntity in objects)
                _dbSet.Remove(oEntity);
            return true;
        }
    }
}
