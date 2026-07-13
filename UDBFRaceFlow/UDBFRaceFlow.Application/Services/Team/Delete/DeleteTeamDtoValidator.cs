using FluentValidation;
using UDBFRaceFlow.Application.Dto.Request.Team;
using UDBFRaceFlow.Domain.Resources;

namespace UDBFRaceFlow.Application.Services.Team.Delete
{
    public class DeleteTeamDtoValidator : AbstractValidator<DeleteTeamDto>
    {
        public DeleteTeamDtoValidator()
        {
            RuleFor(x => x.TeamId)
                .NotEmpty()
                .WithMessage(Messages.Error_PropertyIsRequired);
        }
    }
}
