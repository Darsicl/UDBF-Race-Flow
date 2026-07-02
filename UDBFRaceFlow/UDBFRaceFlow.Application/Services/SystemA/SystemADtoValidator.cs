using FluentValidation;
using UDBFRaceFlow.Application.Dto;
using UDBFRaceFlow.Domain.Enums;
using UDBFRaceFlow.Domain.Resources;

namespace UDBFRaceFlow.Application.Services.SystemA
{
    public class SystemADtoValidator : AbstractValidator<CreateFullGridDto>
    {
        public SystemADtoValidator()
        {
            RuleFor(x => x.Distance)
                .NotEmpty()
                .WithMessage(Messages.Error_PropertyIsRequired)
                .GreaterThanOrEqualTo(200)
                .WithMessage(Messages.Error_MinLength)
                .LessThanOrEqualTo(500)
                .WithMessage(Messages.Error_MaxLength);

            RuleFor(x => x.SystemType)
                .NotEmpty()
                .GreaterThan(0);

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

        private bool MinCountOfTeams(CreateFullGridDto dto)
        {
            if (dto.Races is null)
            {
                return false;
            }

            int countOfTeams = dto.Races
                .SelectMany(c => c.Lanes)
                .GroupBy(c => c.TeamId)
                .Count();

            return countOfTeams > 6;
        }

        private bool CountOfRacesShouldBeSingle(CreateFullGridDto dto)
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

            return sf == 1 && final == 1 && heats == 2;
        }

        private bool BeChronological(CreateFullGridDto dto)
        {
            if (dto.Races == null)
            {
                return false;
            }

            List<RaceCreationDto> heats = dto.Races
                .Where(r => r.RaceType == RaceType.Heat)
                .OrderBy(r => r.SequenceNumber)
                .ToList();

            if (heats.Count < 2)
            {
                return false;
            }

            return heats[1].RaceTime > heats[0].RaceTime;
        }

    }
}
