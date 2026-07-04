using FluentValidation;
using UDBFRaceFlow.Application.Dto;
using UDBFRaceFlow.Application.Interfaces.RepositoryContracts;
using UDBFRaceFlow.Domain.Resources;

namespace UDBFRaceFlow.Application.Services
{
    public class BaseRaceValidator : AbstractValidator<RaceCreationDto>
    {
        private readonly IRaceDataRepository _raceRepository;
        public BaseRaceValidator(IRaceDataRepository raceRepository)
        {
            _raceRepository = raceRepository;

            RuleFor(x => x.RaceNumber)
                .NotEmpty()
                .
                .WithMessage(Messages.Error_PropertyIsRequired);

            RuleFor(x => x.RaceTime)
                .NotEmpty
                .
        }

        private async Task<bool> IsUniqueRaceNumberAsync(int raceNumber, CancellationToken cancellationToken)
        {
            return await
        }
    }
}
