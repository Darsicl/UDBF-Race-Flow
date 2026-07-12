using UDBFRaceFlow.Application.Interfaces.ServiceContracts.Create;
using UDBFRaceFlow.Domain.Entities.Race;
using UDBFRaceFlow.Domain.Enums;
using UDBFRaceFlow.Domain.Resources;

namespace UDBFRaceFlow.Application.Services.RaceSystems.SystemRound.RoundTypes
{
    public class RoundForFiveTeams : IRoundGenerator
    {
        public bool ApplyParametrs(int CountOfTeams)
        {
            return CountOfTeams == 5;
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

            var teamThree = heatLanes.FirstOrDefault(l => l.StartLane == 3);
            ArgumentNullException.ThrowIfNull(teamThree, string.Format(Messages.Error_LaneIsNull, teamThree));

            var teamFour = heatLanes.FirstOrDefault(l => l.StartLane == 4);
            ArgumentNullException.ThrowIfNull(teamFour, string.Format(Messages.Error_LaneIsNull, teamFour));

            var teamFive = heatLanes.FirstOrDefault(l => l.StartLane == 5);
            ArgumentNullException.ThrowIfNull(teamFive, string.Format(Messages.Error_LaneIsNull, teamFive));


            semi.Lanes.Clear();
            semi.Lanes.Add(new LaneData { StartLane = 1, TeamId = teamThree.TeamId, RaceId = semi.Id });
            semi.Lanes.Add(new LaneData { StartLane = 2, TeamId = teamFour.TeamId, RaceId = semi.Id });
            semi.Lanes.Add(new LaneData { StartLane = 3, TeamId = teamOne.TeamId, RaceId = semi.Id });
            semi.Lanes.Add(new LaneData { StartLane = 4, TeamId = teamFive.TeamId, RaceId = semi.Id });
            semi.Lanes.Add(new LaneData { StartLane = 5, TeamId = teamTwo.TeamId, RaceId = semi.Id });

            final.Lanes.Clear();
            final.Lanes.Add(new LaneData { StartLane = 1, TeamId = teamFour.TeamId, RaceId = final.Id });
            final.Lanes.Add(new LaneData { StartLane = 2, TeamId = teamThree.TeamId, RaceId = final.Id });
            final.Lanes.Add(new LaneData { StartLane = 3, TeamId = teamFive.TeamId, RaceId = final.Id });
            final.Lanes.Add(new LaneData { StartLane = 4, TeamId = teamOne.TeamId, RaceId = final.Id });
            final.Lanes.Add(new LaneData { StartLane = 5, TeamId = teamTwo.TeamId, RaceId = final.Id });

            return category.Races;
        }
    }
}
