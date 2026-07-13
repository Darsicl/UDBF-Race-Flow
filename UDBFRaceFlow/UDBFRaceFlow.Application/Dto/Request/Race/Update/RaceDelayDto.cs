namespace UDBFRaceFlow.Application.Dto.Request.Race.Update
{
    public record RaceDelayDto(TimeSpan Delay, DateOnly DelayDay);
}
