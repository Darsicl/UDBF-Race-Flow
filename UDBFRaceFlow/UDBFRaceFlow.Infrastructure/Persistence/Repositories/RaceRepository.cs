using UDBFRaceFlow.Application.Interfaces;

namespace UDBFRaceFlow.Infrastructure.Persistence.Repositories
{
    public class RaceRepository<T> : IRaceRepository<T>
        where T : class
    {
        private readonly RaceDbContext _context;

        public RaceRepository(RaceDbContext context)
        {
            _context = context;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }

        public async Task<T?> GetByIdAsync(Guid Id)
        {
            return await _context.Set<T>().FindAsync(Id);
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
