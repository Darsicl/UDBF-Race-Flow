namespace UDBFRaceFlow.Application.Interfaces
{
    public interface IRaceRepository<T> where T : class
    {
        Task AddAsync(T entity, CancellationToken cancellationToken);

        Task<T?> GetByIdAsync(Guid Id, CancellationToken cancellationToken);

        Task UpdateAsync(T entity, CancellationToken cancellationToken);

        Task DeleteAsync(T entity, CancellationToken cancellationToken);

        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}