using System.ComponentModel.DataAnnotations;
using UDBFRaceFlow.Domain.Enums;

namespace UDBFRaceFlow.Domain.Entities.Race
{
    public class RaceData
    {
        [Key]
        public Guid Id { get; set; }

        public string? BoatSize { get; set; }

        [Range(200, 2000)]
        public int Distance { get; set; }

        public GenderCategory GenderCategory { get; set; }

        public RaceStatus RaceStatus { get; set; }

        public RaceType RaceType { get; set; }

        public List<RaceEntry> RaceEntries = [];
    }
}
