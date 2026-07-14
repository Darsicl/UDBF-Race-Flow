using UDBFRaceFlow.Domain.Enums;

namespace UDBFRaceFlow.Application.Dto.Request.Race.Update
{
    public record UpdateStatusDto(Guid RaceId, RaceStatus RaceStatus);
}
