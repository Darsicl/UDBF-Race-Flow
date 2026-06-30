using UDBFRaceFlow.Application.Interfaces.ServiceContracts;
using UDBFRaceFlow.Domain.Enums;

namespace UDBFRaceFlow.Application.Services
{
    public class GridSystemBuilder
    {
        private readonly IEnumerable<ISystemGenerator> _generators;

        public GridSystemBuilder(IEnumerable<ISystemGenerator> generators)
        {
            _generators = generators;
        }

        public ISystemGenerator GetGenerator(RaceSystems raceSystems, int systemType)
        {
            return _generators.FirstOrDefault(g => g.ApplyParametrs(systemType, raceSystems))
                   ?? throw new ArgumentException("System not found");
        }
    }
}
