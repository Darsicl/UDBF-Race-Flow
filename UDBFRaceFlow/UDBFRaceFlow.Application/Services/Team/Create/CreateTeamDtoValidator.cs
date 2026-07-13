using FluentValidation;
using UDBFRaceFlow.Application.Dto.Request.Team;
using UDBFRaceFlow.Domain.Resources;

namespace UDBFRaceFlow.Application.Services.Team.Create
{
    public class CreateTeamDtoValidator : AbstractValidator<CreateTeamDto>
    {
        public CreateTeamDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(Messages.Error_PropertyIsRequired)
                .MaximumLength(50)
                .WithMessage(Messages.Error_MaxLength);
        }
    }
}
