using UDBFRaceFlow.Domain.Enums;

namespace UDBFRaceFlow.Application.Dto.Request.Update
{
    public record UpdateStatusDto(Guid RaceId, RaceStatus RaceStatus);
}
