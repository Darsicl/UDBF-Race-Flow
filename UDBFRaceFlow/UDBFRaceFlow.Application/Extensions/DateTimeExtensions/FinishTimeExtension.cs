using UDBFRaceFlow.Application.DTO;
using UDBFRaceFlow.Domain.Enums;

namespace UDBFRaceFlow.Domain.Entities.Race;

public static class FinishTimeExtension
{
    public static List<TeamRoundResultDto> CanculateRoundSystemLeaderBoard(this RaceCategory category)
    {
        var finalLeaderBoard = category.Races
                .SelectMany(f => f.Lanes)
                .Where(f => f.FinishTime.HasValue && f.FinishTime > TimeSpan.Zero && f.Race.RaceStatus == RaceStatus.Finished)
                .GroupBy(f => f.TeamId)
                .Select(g => new TeamRoundResultDto
                (
                    g.Key,
                    TimeSpan.FromMilliseconds(g.Sum(f => f.FinishTime.Value.Milliseconds)),
                    g.Count()
                ))
                .OrderByDescending(f => f.RacesCount)
                .ThenBy(f => f.TotalTime)
                .ToList();

        return finalLeaderBoard;
    }

    public static List<TeamRoundResultDto> CanculateLongSystemLeaderBoard(this RaceCategory category)
    {
        var finalLeaderBoard = category.Races
                .Where(f => f.RaceType == RaceType.Final && f.RaceStatus == RaceStatus.Finished)
                .SelectMany(f => f.Lanes)
                .Where(f => f.FinishTime.HasValue && f.FinishTime > TimeSpan.Zero)
                .GroupBy(f => f.TeamId)
                .Select(g => new TeamRoundResultDto
                (
                    g.Key,
                    g.Select(f => f.FinishTime).FirstOrDefault() ?? TimeSpan.Zero,
                    g.Count()
                ))
                .OrderByDescending(f => f.RacesCount)
                .ThenBy(f => f.TotalTime)
                .ToList();

        return finalLeaderBoard;
    }
}


