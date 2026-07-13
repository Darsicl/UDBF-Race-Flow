using FluentValidation;
using UDBFRaceFlow.Application.Dto.Request.Team;
using UDBFRaceFlow.Domain.Resources;

namespace UDBFRaceFlow.Application.Services.Team.Update
{
    public class UpdateTeamDtoValidator : AbstractValidator<UpdateTeamDto>
    {
        public UpdateTeamDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage(Messages.Error_PropertyIsRequired);

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(Messages.Error_PropertyIsRequired)
                .MaximumLength(50)
                .WithMessage(Messages.Error_MaxLength);
        }
    }
}
