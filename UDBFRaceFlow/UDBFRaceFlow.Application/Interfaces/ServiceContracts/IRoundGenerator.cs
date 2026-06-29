using UDBFRaceFlow.Domain.Entities.Race;

namespace UDBFRaceFlow.Application.Interfaces.ServiceContracts
{
    public interface IRoundGenerator
    {
        bool ApplyParametrs(int CountOfTeams);
        List<RaceData> CreateRestRound(RaceCategory category);
    }
}
