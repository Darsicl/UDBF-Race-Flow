namespace UDBFRaceFlow.Application.Dto.Request.Race.Update
{
    public record UpdateRaceDetailsDto(Guid RaceId, int RaceNumber, DateTime OriginalDateTime);
}