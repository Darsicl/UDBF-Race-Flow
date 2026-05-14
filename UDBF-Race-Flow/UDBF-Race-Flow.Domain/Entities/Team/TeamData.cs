using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UDBFRaceFlow.Domain.Entities.Race;

namespace UDBFRaceFlow.Domain.Entities.Team
{
    public class TeamData
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        [ForeignKey]
        public List<RaceEntry> RaceEntries = [];
    }
}
