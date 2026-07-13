using UDBFRaceFlow.Domain.Enums;

namespace UDBFRaceFlow.Application.Dto.Request.Race.Update
{
    public record RaceResultDto(Guid LaneId, TimeSpan? FinishTime, FinishStatus FinishStatus);
}
