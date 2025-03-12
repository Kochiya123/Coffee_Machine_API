using System.Linq.Expressions;

public interface IRepository<T> where T : class
{
    // Existing methods
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> GetByIdAsync(long id);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(long id);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<int> CountAsync(Expression<Func<T, bool>> predicate);

    // New method for saving changes
    Task SaveChangesAsync();

    IQueryable<T> Query();
}