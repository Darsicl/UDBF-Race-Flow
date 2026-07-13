using FluentResults;
using UDBFRaceFlow.Application.Dto.Request.Race.Delete;

namespace UDBFRaceFlow.Application.Interfaces.ServiceContracts.Race.Delete
{
    public interface IDeleteRaceCategoryService
    {
        Task<Result> DeleteRaceCategory(DeleteRaceCategoryDto raceCategoryDto, CancellationToken cancellationToken);
    }
}
