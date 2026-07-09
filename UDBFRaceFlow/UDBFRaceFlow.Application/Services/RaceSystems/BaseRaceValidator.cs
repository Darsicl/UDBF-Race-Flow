using FluentValidation;
using UDBFRaceFlow.Application.Dto;
using UDBFRaceFlow.Application.Interfaces.RepositoryContracts;
using UDBFRaceFlow.Domain.Resources;

namespace UDBFRaceFlow.Application.Services.RaceSystems
{
    public class BaseRaceValidator : AbstractValidator<CreateRaceDto>
    {
        private readonly IRaceDataRepository _raceRepository;
        public BaseRaceValidator(IRaceDataRepository raceRepository)
        {
            _raceRepository = raceRepository;

            RuleFor(x => x.RaceNumber)
                .NotEmpty()
                .WithMessage(Messages.Error_PropertyIsRequired)
                .MustAsync(IsUniqueRaceNumberAsync)
                .WithMessage(Messages.Error_PropertyNotUnique);

            RuleFor(x => x.RaceTime)
                .NotEmpty()
                .WithMessage(Messages.Error_PropertyIsRequired)
                .MustAsync(IsUniqueRaceTimeAsync)
                .WithMessage(Messages.Error_PropertyNotUnique);
        }

        private async Task<bool> IsUniqueRaceNumberAsync(int raceNumber, CancellationToken cancellationToken)
        {
            return await _raceRepository.IsRaceNumberUnique(raceNumber, cancellationToken);
        }

        private async Task<bool> IsUniqueRaceTimeAsync(DateTime raceTime, CancellationToken cancellationToken)
        {
            return await _raceRepository.IsRaceDateUnique(raceTime, cancellationToken);
        }
    }
}
