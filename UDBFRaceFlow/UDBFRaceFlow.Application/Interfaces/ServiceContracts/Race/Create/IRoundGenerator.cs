using UDBFRaceFlow.Domain.Entities.Race;

namespace UDBFRaceFlow.Application.Interfaces.ServiceContracts.Race.Create
{
    public interface IRoundGenerator
    {
        bool ApplyParametrs(int CountOfTeams);
        List<RaceData> CreateRestRound(RaceCategory category);
    }
}
