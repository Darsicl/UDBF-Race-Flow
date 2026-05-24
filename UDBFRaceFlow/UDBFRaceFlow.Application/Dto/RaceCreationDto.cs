using UDBFRaceFlow.Domain.Enums;

namespace UDBFRaceFlow.Application.Dto
{
    public record RaceCreationDto(int RaceNumber, DateTime RaceTime, RaceType RaceType, List<LaneAssignmentDto> Lanes);
}
