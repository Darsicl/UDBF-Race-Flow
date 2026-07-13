using FluentValidation;
using UDBFRaceFlow.Application.Dto.Request.Race.Update;
using UDBFRaceFlow.Domain.Resources;

namespace UDBFRaceFlow.Application.Services.Race.Update.RaceCategory
{
    public class UpdateRaceCategoryServiceDtoValidator : AbstractValidator<UpdateCategoryDetailsDto>
    {
        public UpdateRaceCategoryServiceDtoValidator()
        {
            RuleFor(x => x.CategoryId)
                .NotEmpty()
                .WithMessage(Messages.Error_PropertyIsRequired);

            RuleFor(x => x.RaceAge)
                .IsInEnum()
                .WithMessage(Messages.Error_PropertyIsRequired);

            RuleFor(x => x.Distance)
                .NotEmpty()
                .WithMessage(Messages.Error_PropertyIsRequired)
                .GreaterThanOrEqualTo(200)
                .WithMessage(Messages.Error_MinLength)
                .LessThanOrEqualTo(2000)
                .WithMessage(Messages.Error_MaxLength);

            RuleFor(x => x.BoatSize)
                .IsInEnum()
                .WithMessage(Messages.Error_PropertyIsRequired);

            RuleFor(x => x.GenderCategory)
                .IsInEnum()
                .WithMessage(Messages.Error_PropertyIsRequired);
        }
    }
}
