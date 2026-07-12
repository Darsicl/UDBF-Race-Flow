using FluentResults;

namespace UDBFRaceFlow.Application.Interfaces.ServiceContracts
{
    public interface ICheckRaceService
    {
        Task<Result> CheckFinishOfHeats(Guid categoryId, CancellationToken cancellationToken);
        Task<Result> CheckFinishOfSemis(Guid categoryId, CancellationToken cancellationToken);
    }
}
