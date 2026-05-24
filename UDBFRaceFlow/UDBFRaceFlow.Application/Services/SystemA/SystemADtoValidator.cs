using FluentValidation;
using UDBFRaceFlow.Application.Dto;
using UDBFRaceFlow.Domain.Enums;
using UDBFRaceFlow.Domain.Resources;

namespace UDBFRaceFlow.Application.Services.SystemA
{
    public class SystemADtoValidator : AbstractValidator<CreateFullGridDto>
    {
        public SystemADtoValidator()
        {
            RuleFor(x => x.RaceSystem)
                .IsInEnum()
                .WithMessage(Messages.Error_PropertyIsRequired);

            RuleFor(x => x.Distance)
                .NotEmpty()
                .WithMessage(Messages.Error_PropertyIsRequired)
                .GreaterThanOrEqualTo(200)
                .WithMessage(Messages.Error_MinLength)
                .LessThanOrEqualTo(2000)
                .WithMessage(Messages.Error_MaxLenght);

            RuleFor(x => x.Gender)
                .IsInEnum()
                .WithMessage(Messages.Error_PropertyIsRequired);

            RuleFor(x => x.BoatSize)
                .IsInEnum()
                .WithMessage(Messages.Error_PropertyIsRequired);

            RuleFor(x => x.Races)
                .NotEmpty()
                .WithMessage(Messages.Error_PropertyIsRequired);

            When(x => x.Races != null && x.Races.Any(), () =>
            {
                RuleFor(x => x)
                    .Must(CountOfRacesShouldBeSingle)
                    .WithMessage(Messages.Error_CountOfRaces);

                RuleFor(x => x)
                    .Must(BeChronological)
                    .WithMessage(Messages.Error_Chronological);
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

            return sf == 1 && final == 1 && heats == 2;
        }

        private bool BeChronological(CreateFullGridDto dto)
        {
            if (dto.Races == null)
            {
                return false;
            }

            List<RaceCreationDto> heats = dto.Races
                .Where(r => r.RaceType == RaceType.Heat)
                .OrderBy(r => r.SequenceNumber)
                .ToList();

            if (heats.Count() < 2)
            {
                return false;
            }

            return heats[1].RaceTime > heats[0].RaceTime;
        }

    }
}
