using System.Data;
using UDBFRaceFlow.Domain.Resources;

namespace UDBFRaceFlow.Domain.Entities.Race
{
    public static class CheckIntervalTimeExtension
    {
        private static Dictionary<int, TimeSpan> IntervalRules = new()
        {
            {200, TimeSpan.FromMinutes(10) },
            {500, TimeSpan.FromMinutes(15) },
            {2000, TimeSpan.FromMinutes(30) }
        };

        public static bool CheckInterval(this List<RaceData> races)
        {
            var sortedRaces = races.OrderBy(s => s.RaceNumber)
                .ToList();

            for (int i = 0; i < sortedRaces.Count - 1; i++)
            {
                var currentRace = sortedRaces[i];
                var nextRace = sortedRaces[i + 1];

                if (!IntervalRules.TryGetValue(currentRace.Category.Distance, out var minRequiredInterval))
                {
                    throw new Exception(string.Format(Messages.Error_UnknownDistance, currentRace.Category.Distance));
                }

                var actualInterval = nextRace.RaceTime - currentRace.RaceTime;

                if (actualInterval < minRequiredInterval)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
