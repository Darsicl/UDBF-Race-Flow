using FluentValidation;
using UDBFRaceFlow.Application.Dto.Request.Race.Update;
using UDBFRaceFlow.Domain.Resources;

namespace UDBFRaceFlow.Application.Services.Race.Update.RaceDelay
{
    public class UpdateRaceDelayServiceDtoValidator : AbstractValidator<RaceDelayDto>
    {
        public UpdateRaceDelayServiceDtoValidator()
        {
            RuleFor(x => x.Delay)
                .NotEmpty()
                .WithMessage(Messages.Error_PropertyIsRequired);

            RuleFor(x => x.DelayDay)
                .NotEmpty()
                .WithMessage(Messages.Error_PropertyIsRequired);
        }
    }
}
