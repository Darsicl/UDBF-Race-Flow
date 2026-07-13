using FluentValidation;
using UDBFRaceFlow.Application.Dto.Request.Race.Update;
using UDBFRaceFlow.Domain.Resources;

namespace UDBFRaceFlow.Application.Services.Race.Update
{
    public class UpdateRaceDetailsServiceDtoValidator : AbstractValidator<UpdateRaceDetailsDto>
    {
        public UpdateRaceDetailsServiceDtoValidator()
        {
            RuleFor(x => x.RaceId)
                .NotEmpty()
                .WithMessage(Messages.Error_PropertyIsRequired);

            RuleFor(x => x.RaceNumber)
                .NotEmpty()
                .WithMessage(Messages.Error_PropertyIsRequired)
                .GreaterThan(0)
                .WithMessage(Messages.Error_MinLength);

            RuleFor(x => x.OriginalDateTime)
                .NotEmpty()
                .WithMessage(Messages.Error_PropertyIsRequired);
        }
    }
}
