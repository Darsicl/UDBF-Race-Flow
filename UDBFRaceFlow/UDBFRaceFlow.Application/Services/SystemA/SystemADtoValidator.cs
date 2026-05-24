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
            RuleFor(x => x.Distance)
                .NotEmpty()
                .WithMessage(Messages.Error_PropertyIsRequired)
                .GreaterThanOrEqualTo(200)
                .WithMessage(Messages.Error_MinLength)
                .LessThanOrEqualTo(2000)
                .WithMessage(Messages.Error_MaxLenght);

            RuleFor(x => x.Races)
                .NotEmpty()
                .WithMessage(Messages.Error_PropertyIsRequired);

            RuleFor(x => x.Gender)
                .NotEmpty()
                .WithMessage(Messages.Error_PropertyIsRequired);

            RuleFor(x => x.BoatSize)
                .NotEmpty()
                .WithMessage(Messages.Error_PropertyIsRequired);

            RuleFor(x => x.RaceSystem)
                .NotEmpty()
                .WithMessage(Messages.Error_PropertyIsRequired);

            RuleFor(x => x)
                .Must(BeChronological)
                .WithMessage(Messages.Error_Chronological);
        }

        private bool BeChronological(CreateFullGridDto dto)
        {
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
