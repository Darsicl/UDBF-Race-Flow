using UDBFRaceFlow.Application.Interfaces.ServiceContracts;
using UDBFRaceFlow.Domain.Entities.Race;
using UDBFRaceFlow.Domain.Enums;
using UDBFRaceFlow.Domain.Resources;

namespace UDBFRaceFlow.Application.Services.RaceSystems.SystemRound.RoundTypes
{
    public class RoundForTwoTeams : IRoundGenerator
    {
        public bool ApplyParametrs(int CountOfTeams)
        {
            return CountOfTeams == 2;
        }

        public List<RaceData> CreateRestRound(RaceCategory category)
        {
            var semi = category.Races.FirstOrDefault(s => s.RaceType == RaceType.Semifinal);
            ArgumentNullException.ThrowIfNull(semi, string.Format(Messages.Error_RaceIsNull, semi));

            var final = category.Races.FirstOrDefault(f => f.RaceType == RaceType.Final);
            ArgumentNullException.ThrowIfNull(final, string.Format(Messages.Error_RaceIsNull, final));

            var heatLanes = category.Races
                .Where(h => h.RaceType == RaceType.Heat)
                .SelectMany(h => h.Lanes)
                .ToList();

            var teamOne = heatLanes.FirstOrDefault(l => l.StartLane == 1);
            ArgumentNullException.ThrowIfNull(teamOne, string.Format(Messages.Error_LaneIsNull, teamOne));

            var teamTwo = heatLanes.FirstOrDefault(l => l.StartLane == 2);
            ArgumentNullException.ThrowIfNull(teamTwo, string.Format(Messages.Error_LaneIsNull, teamTwo));

            semi.Lanes.Clear();
            semi.Lanes.Add(new LaneData { StartLane = 1, TeamId = teamTwo.TeamId, RaceId = semi.Id });
            semi.Lanes.Add(new LaneData { StartLane = 2, TeamId = teamOne.TeamId, RaceId = semi.Id });

            final.Lanes.Clear();
            final.Lanes.Add(new LaneData { StartLane = 1, TeamId = teamOne.TeamId, RaceId = final.Id });
            final.Lanes.Add(new LaneData { StartLane = 2, TeamId = teamTwo.TeamId, RaceId = final.Id });

            return category.Races;
        }
    }
}
