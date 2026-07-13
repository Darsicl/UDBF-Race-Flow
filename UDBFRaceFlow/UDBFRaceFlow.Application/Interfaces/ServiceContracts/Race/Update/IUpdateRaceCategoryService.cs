using FluentResults;
using UDBFRaceFlow.Application.Dto.Request.Race.Update;

namespace UDBFRaceFlow.Application.Interfaces.ServiceContracts.Race.Update
{
    public interface IUpdateRaceCategoryService
    {
        Task<Result> UpdateCategoryDetails(UpdateCategoryDetailsDto categoryDetailsDto, CancellationToken cancellationToken);
    }
}