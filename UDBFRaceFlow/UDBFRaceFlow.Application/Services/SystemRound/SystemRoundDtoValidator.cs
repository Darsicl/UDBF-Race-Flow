using FluentValidation;
using UDBFRaceFlow.Application.Dto;
using UDBFRaceFlow.Domain.Enums;
using UDBFRaceFlow.Domain.Resources;

namespace UDBFRaceFlow.Application.Services.SystemRound
{
    public class SystemRoundDtoValidator : BaseSystemValidator
    {
        public SystemRoundDtoValidator(IValidator<RaceCreationDto> baseValidator) : base(baseValidator)
        {
            RuleFor(x => x.Distance)
                .NotEmpty()
                .WithMessage(Messages.Error_PropertyIsRequired)
                .GreaterThanOrEqualTo(200)
                .WithMessage(Messages.Error_MinLength)
                .LessThanOrEqualTo(500)
                .WithMessage(Messages.Error_MaxLength);

            When(x => x.Races != null && x.Races.Any(), () =>
            {
                RuleFor(x => x)
                    .Must(CountOfRacesShouldBeSingle)
                    .WithMessage(Messages.Error_CountOfRaces);
            });

        }

        private bool CountOfRacesShouldBeSingle(CreateFullGridDto dto)
        {
            if (dto.Races == null)
            {
                return false;
            }

            int heats = dto.Races
                .Count(r => r.RaceType == RaceType.Heat);

            int sf = dto.Races
                .Count(r => r.RaceType == RaceType.Semifinal);

            int final = dto.Races
                .Count(r => r.RaceType == RaceType.Final);

            return sf == 1 && final == 1 && heats == 1;
        }


    }

}
