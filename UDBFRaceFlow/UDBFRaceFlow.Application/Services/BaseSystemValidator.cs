using FluentValidation;
using UDBFRaceFlow.Application.Dto;
using UDBFRaceFlow.Domain.Resources;

namespace UDBFRaceFlow.Application.Services
{
    public class BaseSystemValidator : AbstractValidator<CreateFullGridDto>
    {
        public BaseSystemValidator()
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
                .WithMessage(Messages.Error_PropertyIsRequired);
        }
    }
}
