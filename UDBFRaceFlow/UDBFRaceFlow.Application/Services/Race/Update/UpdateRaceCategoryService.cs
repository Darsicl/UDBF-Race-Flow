using FluentResults;
using Mapster;
using Microsoft.Extensions.Logging;
using UDBFRaceFlow.Application.Dto.Request.Race.Update;
using UDBFRaceFlow.Application.Interfaces.RepositoryContracts;
using UDBFRaceFlow.Application.Interfaces.ServiceContracts.Race.Update;
using UDBFRaceFlow.Domain.Entities.Race;
using UDBFRaceFlow.Domain.Resources;

namespace UDBFRaceFlow.Application.Services.Race.Update
{
    public class UpdateRaceCategoryService : IUpdateRaceCategoryService
    {
        private readonly IRaceCategoryRepository _raceCategoryRepository;
        private readonly ILogger<UpdateRaceCategoryService> _logger;

        public UpdateRaceCategoryService(IRaceCategoryRepository raceCategoryRepository, ILogger<UpdateRaceCategoryService> logger)
        {
            _raceCategoryRepository = raceCategoryRepository;
            _logger = logger;
        }

        public async Task<Result> UpdateCategoryDetails(UpdateCategoryDetailsDto categoryDetailsDto, CancellationToken cancellationToken = default)
        {
            var category = await _raceCategoryRepository.GetByIdAsync(categoryDetailsDto.CategoryId, cancellationToken);

            if (category is null)
            {
                string errorMsg = string.Format(Messages.Error_EntityWithIdNotFound, nameof(RaceCategory), categoryDetailsDto.CategoryId);
                _logger.LogError(errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            categoryDetailsDto.Adapt(category);

            var checkCategory = await _raceCategoryRepository.IsCategoryUnique(category, cancellationToken);

            if (checkCategory)
            {
                string errorMsg = string.Format(Messages.Error_PropertyNotUnique, category.CategoryName);
                _logger.LogError(errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            await _raceCategoryRepository.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }

    }
}
