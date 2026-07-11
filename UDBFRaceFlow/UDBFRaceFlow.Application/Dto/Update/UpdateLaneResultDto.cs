using UDBFRaceFlow.Domain.Enums;

namespace UDBFRaceFlow.Application.Dto.Update
{
    public record UpdateLaneResultDto(Guid RaceId, List<LaneResultDto> LaneResults, RaceStatus RaceStatus);
}
