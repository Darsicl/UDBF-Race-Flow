namespace UDBFRaceFlow.Application.DTO
{
    public record TeamRoundResultDto(Guid TeamId, TimeSpan TotalTime, int RacesCount);
}
