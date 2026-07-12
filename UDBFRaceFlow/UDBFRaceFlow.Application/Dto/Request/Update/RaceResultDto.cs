using UDBFRaceFlow.Domain.Enums;

namespace UDBFRaceFlow.Application.Dto.Request.Update
{
    public record RaceResultDto(Guid LaneId, TimeSpan? FinishTime, FinishStatus FinishStatus);
}
