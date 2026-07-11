namespace UDBFRaceFlow.Application.Dto.Update
{
    public record UpdateRaceDetailsDto(Guid RaceId, int RaceNumber, DateTime OriginalDateTime);
}