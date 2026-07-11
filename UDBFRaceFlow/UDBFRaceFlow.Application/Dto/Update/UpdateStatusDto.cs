using UDBFRaceFlow.Domain.Enums;

namespace UDBFRaceFlow.Application.Dto.Update
{
    public record UpdateStatusDto(Guid RaceId, RaceStatus RaceStatus);
}
