namespace UDBFRaceFlow.Application.Dto.Request.Update
{
    public record RaceDelayDto(TimeSpan Delay, DateOnly DelayDay);
}
