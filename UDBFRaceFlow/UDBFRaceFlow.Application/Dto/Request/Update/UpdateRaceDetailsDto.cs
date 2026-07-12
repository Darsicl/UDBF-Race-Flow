namespace UDBFRaceFlow.Application.Dto.Request.Update
{
    public record UpdateRaceDetailsDto(Guid RaceId, int RaceNumber, DateTime OriginalDateTime);
}