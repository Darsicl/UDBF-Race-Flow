using UDBFRaceFlow.Domain.Enums;

namespace UDBFRaceFlow.Application.Dto.Create
{
    public record CreateCategoryDto(RaceSystem RaceSystem, int SystemType, RaceAge RaceAge, int Distance, BoatSize BoatSize, GenderCategory Gender, List<CreateRaceDto> Races);
}
