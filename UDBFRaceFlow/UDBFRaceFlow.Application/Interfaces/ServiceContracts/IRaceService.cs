using FluentResults;

namespace UDBFRaceFlow.Application.Interfaces.ServiceContracts
{
    public interface IRaceService
    {
        Task<Result> CheckFinishOfHeats(Guid categoryId);
        Task<Result> CheckFinishOfSemis(Guid categoryId);
    }
}
