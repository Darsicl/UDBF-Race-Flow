using System.ComponentModel.DataAnnotations;
using UDBFRaceFlow.Domain.Entities.Race;

namespace UDBFRaceFlow.Domain.Entities.Team
{
    public class TeamData
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        public List<RaceEntry> RaceEntries { get; set; } = new List<RaceEntry>();
    }
}
