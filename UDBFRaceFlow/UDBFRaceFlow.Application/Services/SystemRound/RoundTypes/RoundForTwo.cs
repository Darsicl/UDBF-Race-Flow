using UDBFRaceFlow.Application.Interfaces.ServiceContracts;
using UDBFRaceFlow.Domain.Entities.Race;
using UDBFRaceFlow.Domain.Enums;
using UDBFRaceFlow.Domain.Resources;

namespace UDBFRaceFlow.Application.Services.SystemRound.RoundTypes
{
    public class RoundForTwo : IRoundGenerator
    {
        public bool ApplyParametrs(int CountOfTeams)
        {
            return CountOfTeams == 2;
        }

        public List<RaceData> CreateRestRound(RaceCategory category)
        {
            var semi = category.Races.FirstOrDefault(s => s.RaceType == RaceType.Semifinal);

            var final = category.Races.FirstOrDefault(f => f.RaceType == RaceType.Final);

            var laneOneTeam = category.Races
                .Where(l => l.RaceType == RaceType.Heat)
                .SelectMany(l => l.Lanes)
                .FirstOrDefault(l => l.StartLane == 1);

            var laneTwoTeam = category.Races
                .Where(l => l.RaceType == RaceType.Heat)
                .SelectMany(l => l.Lanes)
                .FirstOrDefault(l => l.StartLane == 2);

            if (semi is null || final is null || laneOneTeam is null || laneTwoTeam is null)
            {
                throw new Exception(Messages.Error_PropertyIsRequired);
            }

            semi.Lanes.Clear();

            semi.Lanes.Add(new LaneData { StartLane = 1, TeamId = laneTwoTeam.TeamId, RaceId = semi.Id });
            semi.Lanes.Add(new LaneData { StartLane = 2, TeamId = laneOneTeam.TeamId, RaceId = semi.Id });

            category.Races.Add(semi);

            final.Lanes.Clear();

            final.Lanes.Add(new LaneData { StartLane = 1, TeamId = laneOneTeam.TeamId, RaceId = semi.Id });
            final.Lanes.Add(new LaneData { StartLane = 2, TeamId = laneTwoTeam.TeamId, RaceId = semi.Id });

            category.Races.Add(semi);

            return category.Races;
        }
    }
}
