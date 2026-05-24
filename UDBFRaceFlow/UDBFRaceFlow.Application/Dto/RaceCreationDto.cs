using UDBFRaceFlow.Domain.Enums;

namespace UDBFRaceFlow.Application.Dto
{
    public record RaceCreationDto(int RaceNumber, RaceType RaceType, List<LaneAssignmentDto> Lanes);
}
