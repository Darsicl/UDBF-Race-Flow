using UDBFRaceFlow.Domain.Enums;

namespace UDBFRaceFlow.Application.Dto.Request.Race.Create
{
    public record CreateRaceDto(int RaceNumber, DateTime RaceTime, int SequenceNumber, RaceType RaceType, List<CreateLaneDto> Lanes);
}
