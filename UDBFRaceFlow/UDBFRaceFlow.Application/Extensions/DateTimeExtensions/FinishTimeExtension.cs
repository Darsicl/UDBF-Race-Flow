using UDBFRaceFlow.Application.DTO;
using UDBFRaceFlow.Domain.Enums;

namespace UDBFRaceFlow.Domain.Entities.Race;

public static class FinishTimeExtension
{
    public static List<TeamRoundResultDto> CanculateRoundSystemLeaderBoard(this RaceCategory category)
    {
        var finalLeaderBoard = category.Races
                .SelectMany(f => f.Lanes)
                .Where(f => f.FinishTime > TimeSpan.Zero && f.Race.RaceStatus == RaceStatus.Finished)
                .GroupBy(f => f.TeamId)
                .Select(g => new TeamRoundResultDto
                (
                    g.Key,
                    TimeSpan.FromMilliseconds(g.Sum(f => f.FinishTime.TotalMilliseconds)),
                    g.Count()
                ))
                .OrderByDescending(f => f.RacesCount)
                .ThenBy(f => f.TotalTime)
                .ToList();

        return finalLeaderBoard;
    }
}


