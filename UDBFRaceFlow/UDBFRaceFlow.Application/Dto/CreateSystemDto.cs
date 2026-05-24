using UDBFRaceFlow.Domain.Entities.Race;

namespace UDBFRaceFlow.Application.Dto
{
    public record CreateSystemDto(RaceData Race, List<RaceEntry> RaceEntries);
}
