using FluentValidation;
using UDBFRaceFlow.Application.Dto.Request.Race.Update;
using UDBFRaceFlow.Domain.Resources;

namespace UDBFRaceFlow.Application.Services.Race.Update
{
    public class RaceResultServiceDtoValidator : AbstractValidator<UpdateLaneResultDto>
    {
        public RaceResultServiceDtoValidator()
        {
            RuleFor(x => x.RaceId)
                .NotEmpty()
                .WithMessage(Messages.Error_PropertyIsRequired);

            RuleFor(x => x.RaceStatus)
                .IsInEnum()
                .WithMessage(Messages.Error_PropertyIsRequired);

            RuleForEach(x => x.LaneResults)
                .SetValidator(new RaceResultDtoValidator());
        }
    }
}
