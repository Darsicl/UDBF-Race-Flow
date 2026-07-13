using UDBFRaceFlow.Domain.Entities.Team;

namespace UDBFRaceFlow.Application.Interfaces.RepositoryContracts
{
    public interface ITeamDataRepository : IBaseRepository<TeamData>
    {
        Task<bool> IsTeamExistsAsync(string TeamName, CancellationToken cancellationToken);
        Task<bool> IsTeamExistsAsync(string TeamName, Guid teamId, CancellationToken cancellationToken);
    }
}
