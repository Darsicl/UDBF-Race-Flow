using UDBFRaceFlow.Application.Interfaces;

namespace UDBFRaceFlow.Infrastructure.Persistence.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T>
        where T : class
    {
        private readonly RaceDbContext _context;

        public BaseRepository(RaceDbContext context)
        {
            _context = context;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _context.Set<T>().AddAsync(entity, cancellationToken);
        }

        public async Task<T?> GetByIdAsync(Guid Id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<T>().FindAsync(Id, cancellationToken);
        }

        public Task UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(T entity)
        {
            _context.Remove(entity);
            return Task.CompletedTask;
        }
    }
}
