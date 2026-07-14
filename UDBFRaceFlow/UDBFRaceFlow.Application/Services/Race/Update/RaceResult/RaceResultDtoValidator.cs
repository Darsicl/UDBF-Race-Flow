using FluentValidation;
using UDBFRaceFlow.Application.Dto.Request.Race.Update;
using UDBFRaceFlow.Domain.Enums;
using UDBFRaceFlow.Domain.Resources;

namespace UDBFRaceFlow.Application.Services.Race.Update.RaceResult
{
    public class RaceResultDtoValidator : AbstractValidator<RaceResultDto>
    {
        public RaceResultDtoValidator()
        {
            RuleFor(x => x.LaneId)
                .NotEmpty()
                .WithMessage(Messages.Error_PropertyIsRequired);

            RuleFor(x => x.FinishStatus)
                .IsInEnum()
                .WithMessage(Messages.Error_PropertyIsRequired);

            RuleFor(x => x.FinishTime)
                .NotEmpty()
                .WithMessage(Messages.Error_PropertyIsRequired)
                .When(x => x.FinishStatus == FinishStatus.Confirmed);
        }
    }
}
