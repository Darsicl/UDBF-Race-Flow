namespace UDBFRaceFlow.Application.Dto.Request.Race.Update
{
    public record UpdateStartLaneDto(Guid raceId, Guid targetTeamId, Guid draggetLaneId);
}