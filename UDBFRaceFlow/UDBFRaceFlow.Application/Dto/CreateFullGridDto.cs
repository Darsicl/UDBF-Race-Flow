using UDBFRaceFlow.Domain.Enums;

namespace UDBFRaceFlow.Application.Dto
{
    public record CreateFullGridDto(RaceSystems RaceSystem, List<RaceCreationDto> Races);
}
