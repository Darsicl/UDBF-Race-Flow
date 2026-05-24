using UDBFRaceFlow.Domain.Enums;

namespace UDBFRaceFlow.Application.Dto
{
    public record RaceCreationDto(int RaceNumber, int Distance, BoatSize BoatSize, GenderCategory Gender, RaceType RaceType, List<LaneAssignmentDto> Lanes);
}
