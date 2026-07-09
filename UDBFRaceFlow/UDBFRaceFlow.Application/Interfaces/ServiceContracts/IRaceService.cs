using FluentResults;

namespace UDBFRaceFlow.Application.Interfaces.ServiceContracts
{
    public interface IRaceService
    {
        Task<Result> CheckFinishOfHeats(Guid categoryId, CancellationToken cancellationToken);
        Task<Result> CheckFinishOfSemis(Guid categoryId, CancellationToken cancellationToken);
    }
}
