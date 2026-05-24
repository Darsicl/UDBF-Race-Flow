using UDBFRaceFlow.Domain.Entities.Race;
using UDBFRaceFlow.Domain.Enums;

namespace UDBFRaceFlow.Application.Dto
{
    public record CreateSystemDto(int RaceNumber, string BoatSize, int Distance, GenderCategory GenderCategory, RaceType RaceType, List<RaceEntry> RaceEntries);
}
