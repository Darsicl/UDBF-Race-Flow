using FluentValidation;
using UDBFRaceFlow.Application.Dto.Request.Race.Update;
using UDBFRaceFlow.Domain.Resources;

namespace UDBFRaceFlow.Application.Services.Race.Update
{
    public class UpdateStartLaneNumberServiceDtoValidator : AbstractValidator<UpdateStartLaneDto>
    {
        public UpdateStartLaneNumberServiceDtoValidator()
        {
            RuleFor(x => x.raceId)
                .NotEmpty()
                .WithMessage(Messages.Error_PropertyIsRequired);

            RuleFor(x => x.draggetLaneId)
                .NotEmpty()
                .WithMessage(Messages.Error_PropertyIsRequired);

            RuleFor(x => x.targetTeamId)
                .NotEmpty()
                .WithMessage(Messages.Error_PropertyIsRequired);
        }
    }
}
