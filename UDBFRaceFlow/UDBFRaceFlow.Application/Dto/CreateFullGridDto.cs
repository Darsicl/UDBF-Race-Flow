using UDBFRaceFlow.Domain.Enums;

namespace UDBFRaceFlow.Application.Dto
{
    public record CreateFullGridDto(RaceSystem RaceSystem, int SystemType, RaceAge RaceAge, int Distance, BoatSize BoatSize, GenderCategory Gender, List<RaceCreationDto> Races);
}
