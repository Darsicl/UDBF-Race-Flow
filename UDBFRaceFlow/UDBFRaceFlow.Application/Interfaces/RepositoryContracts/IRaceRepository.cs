namespace UDBFRaceFlow.Application.Interfaces
{
    public interface IRaceRepository<T> where T : class
    {
        Task AddAsync(T entity);

        Task<T?> GetByIdAsync(Guid Id);

        Task UpdateAsync(T entity);

        Task DeleteAsync(T entity);

        Task SaveChangesAsync();
    }
}