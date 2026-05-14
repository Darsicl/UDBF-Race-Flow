using System.ComponentModel.DataAnnotations;
using UDBFRaceFlow.Domain.Enums;
using UDBFRaceFlow.Domain.Resources;

namespace UDBFRaceFlow.Domain.Entities.Race
{
    public class RaceData
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.Error_PropertyIsRequired))]
        public string BoatSize { get; set; } = string.Empty;

        [Range(200, 2000, ErrorMessageResourceName = nameof(Messages.Error_Range), ErrorMessageResourceType = typeof(Messages)]
        public int Distance { get; set; }

        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.Error_PropertyIsRequired))]
        public GenderCategory GenderCategory { get; set; }

        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.Error_PropertyIsRequired))]
        public RaceStatus RaceStatus { get; set; }

        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.Error_PropertyIsRequired))]
        public RaceType RaceType { get; set; }

        public List<RaceEntry> RaceEntries { get; set; }
    }
}
