using FluentResults;
using UDBFRaceFlow.Application.Dto.Request.Update;

namespace UDBFRaceFlow.Application.Interfaces.ServiceContracts.Update
{
    public interface IUpdateRaceCategoryService
    {
        Task<Result> UpdateCategoryDetails(UpdateCategoryDetailsDto categoryDetailsDto, CancellationToken cancellationToken);
    }
}