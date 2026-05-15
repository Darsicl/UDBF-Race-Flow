using System.ComponentModel.DataAnnotations;
using UDBFRaceFlow.Domain.Entities.Race;
using UDBFRaceFlow.Domain.Resources;

namespace UDBFRaceFlow.Domain.Entities.Team
{
    public class TeamData
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.Error_PropertyIsRequired))]
        [MaxLength(50, ErrorMessageResourceName = nameof(Messages.Error_MaxLenght), ErrorMessageResourceType = typeof(Messages))]
        public string Name { get; set; } = string.Empty;

        public List<RaceEntry> RaceEntries { get; set; }
    }
}
