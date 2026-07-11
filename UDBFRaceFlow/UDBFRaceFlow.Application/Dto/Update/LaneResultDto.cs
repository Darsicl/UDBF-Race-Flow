using UDBFRaceFlow.Domain.Enums;

namespace UDBFRaceFlow.Application.Dto.Update
{
    public record LaneResultDto(Guid LaneId, TimeSpan? FinishTime, FinishStatus FinishStatus);
}
