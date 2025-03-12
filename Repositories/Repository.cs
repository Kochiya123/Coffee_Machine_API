// Repositories/Repository.cs
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebApplication2.Models;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly CoffeeShop01Context _context;
    private readonly DbSet<T> _dbSet;

    public Repository(CoffeeShop01Context context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<T> GetByIdAsync(long id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<T> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.CountAsync(predicate);
    }
    // New method for saving changes
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public IQueryable<T> Query()
    {
        return _dbSet.AsQueryable();
    }
}