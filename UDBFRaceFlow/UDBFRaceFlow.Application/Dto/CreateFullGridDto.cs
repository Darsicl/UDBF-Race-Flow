using UDBFRaceFlow.Domain.Enums;

namespace UDBFRaceFlow.Application.Dto
{
    public record CreateFullGridDto(RaceSystems RaceSystem, int Distance, BoatSize BoatSize, GenderCategory Gender, List<RaceCreationDto> Races);
}
