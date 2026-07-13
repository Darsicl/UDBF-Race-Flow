using FluentValidation;
using UDBFRaceFlow.Application.Dto.Request.Race.Update;
using UDBFRaceFlow.Domain.Resources;

namespace UDBFRaceFlow.Application.Services.Race.Update.RaceStatus
{
    public class UpdateRaceStatusServiceDtoValidator : AbstractValidator<UpdateStatusDto>
    {
        public UpdateRaceStatusServiceDtoValidator()
        {
            RuleFor(x => x.RaceId)
                .NotEmpty()
                .WithMessage(Messages.Error_PropertyIsRequired);

            RuleFor(x => x.RaceStatus)
                .IsInEnum()
                .WithMessage(Messages.Error_PropertyIsRequired);
        }
    }
}
