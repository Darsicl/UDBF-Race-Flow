using FluentValidation;
using UDBFRaceFlow.Application.Dto;
using UDBFRaceFlow.Domain.Resources;

namespace UDBFRaceFlow.Application.Services
{
    public class BaseSystemValidator : AbstractValidator<CreateFullGridDto>
    {
        public BaseSystemValidator(IValidator<RaceCreationDto> baseValidator)
        {
            RuleFor(x => x.RaceSystem)
                .IsInEnum()
                .WithMessage(Messages.Error_PropertyIsRequired);

            RuleFor(x => x.Gender)
                .IsInEnum()
                .WithMessage(Messages.Error_PropertyIsRequired);

            RuleFor(x => x.RaceAge)
                .IsInEnum()
                .WithMessage(Messages.Error_PropertyIsRequired);

            RuleFor(x => x.BoatSize)
                .IsInEnum()
                .WithMessage(Messages.Error_PropertyIsRequired);

            RuleFor(x => x.Races)
                .NotEmpty()
                .WithMessage(Messages.Error_PropertyIsRequired)
                .Must(race => race == null || race.Select(r => r.RaceNumber).Distinct().Count() == race.Count)
                .Must(race => race == null || race.Select(r => r.RaceTime).Distinct().Count() == race.Count)
                .WithMessage(Messages.Error_PropertyNotUnique);

            RuleForEach(x => x.Races)
                .SetValidator(baseValidator);
        }
    }
}
