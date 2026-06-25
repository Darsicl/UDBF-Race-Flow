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

        public ISystemGenerator GetGenerator(RaceSystems raceSystems, int distance)
        {
            return _generators.FirstOrDefault(g => g.ApplyParametrs(distance, raceSystems))
                   ?? throw new ArgumentException("System not found");
        }
    }
}
