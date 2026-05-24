using UDBFRaceFlow.Domain.Enums;

namespace UDBFRaceFlow.Application.Dto
{
    public record RaceCreationDto(int RaceNumber, DateTime RaceTime, int SequenceNumber, RaceType RaceType, List<LaneAssignmentDto> Lanes);
}
