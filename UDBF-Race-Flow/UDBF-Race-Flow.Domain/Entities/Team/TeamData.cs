using System.ComponentModel.DataAnnotations;
using UDBFRaceFlow.Domain.Entities.Race;
using UDBFRaceFlow.Domain.Resources;

namespace UDBFRaceFlow.Domain.Entities.Team
{
    public class TeamData
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = Messages.Error_TeamNameIsRequired)]
        public string Name { get; set; } = string.Empty;

        public List<RaceEntry> RaceEntries = [];
    }
}
