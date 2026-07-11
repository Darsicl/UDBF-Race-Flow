using FluentValidation;
using UDBFRaceFlow.Application.Dto.Create;
using UDBFRaceFlow.Domain.Enums;
using UDBFRaceFlow.Domain.Resources;

namespace UDBFRaceFlow.Application.Services.RaceSystems.SystemLong
{
    public class SystemLongDtoValidator : BaseSystemValidator
    {
        public SystemLongDtoValidator(IValidator<CreateRaceDto> validator) : base(validator)
        {
            RuleFor(x => x.Distance)
                .Equal(2000)
                .WithMessage(Messages.Error_PropertyShouldBeEqual);

            When(x => x.Races != null && x.Races.Any(), () =>
            {
                RuleFor(x => x)
                    .Must(CountOfRacesShouldBeSingle)
                    .WithMessage(Messages.Error_CountOfRaces);

                RuleFor(x => x)
                    .Must(BeChronological)
                    .WithMessage(Messages.Error_Chronological);

                RuleFor(x => x)
                    .Must(MinCountOfTeams)
                    .WithMessage(Messages.Error_NotEnoughTeam);
            });
        }

        private bool MinCountOfTeams(CreateCategoryDto dto)
        {
            if (dto.Races is null)
            {
                return false;
            }

            int countOfTeams = dto.Races
                .SelectMany(c => c.Lanes)
                .GroupBy(c => c.TeamId)
                .Count();

            return countOfTeams > 1;
        }

        private bool CountOfRacesShouldBeSingle(CreateCategoryDto dto)
        {
            if (dto.Races == null)
            {
                return false;
            }

            int heats = dto.Races
                .Count(r => r.RaceType == RaceType.Heat);

            int sf = dto.Races
                .Count(r => r.RaceType == RaceType.Semifinal);

            int final = dto.Races
                .Count(r => r.RaceType == RaceType.Final);

            return sf == 0 && final >= 1 && heats == 0;
        }

        private bool BeChronological(CreateCategoryDto dto)
        {
            if (dto.Races == null)
            {
                return false;
            }

            List<CreateRaceDto> finals = dto.Races
                .Where(r => r.RaceType == RaceType.Final)
                .OrderBy(r => r.SequenceNumber)
                .ToList();

            if (finals.Count == 1)
            {
                return true;
            }

            for (int i = 0; i <= finals.Count - 1; i++)
            {
                if (finals[i + 1].RaceTime < finals[i].RaceTime)
                {
                    return false;
                }
            }

            return true;
        }

    }
}
