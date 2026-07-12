using UDBFRaceFlow.Domain.Enums;

namespace UDBFRaceFlow.Application.Dto.Request.Update
{
    public record UpdateLaneResultDto(Guid RaceId, List<RaceResultDto> LaneResults, RaceStatus RaceStatus);
}
