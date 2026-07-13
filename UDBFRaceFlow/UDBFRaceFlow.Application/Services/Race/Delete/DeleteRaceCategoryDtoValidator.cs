using FluentValidation;
using UDBFRaceFlow.Application.Dto.Request.Race.Delete;
using UDBFRaceFlow.Domain.Resources;

namespace UDBFRaceFlow.Application.Services.Race.Delete
{
    public class DeleteRaceCategoryDtoValidator : AbstractValidator<DeleteRaceCategoryDto>
    {
        public DeleteRaceCategoryDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage(Messages.Error_PropertyIsRequired);
        }
    }
}
