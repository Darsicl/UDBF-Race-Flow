using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;
using UDBFRaceFlow.Application.Dto.Request.Race.Delete;
using UDBFRaceFlow.Application.Extensions.ValidatorExtensions;
using UDBFRaceFlow.Application.Interfaces.RepositoryContracts;
using UDBFRaceFlow.Application.Interfaces.ServiceContracts.Race.Delete;
using UDBFRaceFlow.Domain.Entities.Race;
using UDBFRaceFlow.Domain.Resources;

namespace UDBFRaceFlow.Application.Services.Race.Delete
{
    public class DeleteRaceCategoryService : IDeleteRaceCategoryService
    {
        private readonly IRaceCategoryRepository _raceCategoryRepository;
        private readonly ILogger<DeleteRaceCategoryService> _logger;
        private readonly IValidator<DeleteRaceCategoryDto> _validator;

        public DeleteRaceCategoryService(IRaceCategoryRepository raceCategoryRepository, ILogger<DeleteRaceCategoryService> logger, IValidator<DeleteRaceCategoryDto> validator)
        {
            _raceCategoryRepository = raceCategoryRepository;
            _logger = logger;
            _validator = validator;
        }

        public async Task<Result> DeleteRaceCategory(DeleteRaceCategoryDto raceCategoryDto, CancellationToken cancellationToken = default)
        {
            var validate = await _validator.ValidateDtoAsync(raceCategoryDto, cancellationToken);

            if (validate.IsFailed)
            {
                return validate;
            }

            var category = await _raceCategoryRepository.GetByIdAsync(raceCategoryDto.Id, cancellationToken);

            if (category is null)
            {
                string errorMsg = string.Format(Messages.Error_EntityWithIdNotFound, nameof(RaceCategory), raceCategoryDto.Id);
                _logger.LogError(errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var isCategoryActive = await _raceCategoryRepository.IsCategoryActiveAsync(category.Id, cancellationToken);

            if (isCategoryActive)
            {
                string errorMsg = string.Format(Messages.Error_DeleteIsForbidden, category.CategoryName);
                _logger.LogError(errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            await _raceCategoryRepository.DeleteAsync(category);
            await _raceCategoryRepository.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}
