using UDBFRaceFlow.Domain.Entities.Race;

namespace UDBFRaceFlow.Application.Interfaces.ServiceContracts
{
    public interface IRoundGenerator
    {
        List<RaceData> CreateRestRound(RaceCategory category);
    }
}
