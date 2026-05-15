using System.ComponentModel.DataAnnotations;
using UDBFRaceFlow.Domain.Entities.Team;

namespace UDBFRaceFlow.Domain.Entities.Race
{

    public class RaceEntry
    {
        [Key]
        public Guid Id { get; set; }

        [Range(1, 7)]
        public int StartLane { get; set; }

        [Range(1, 7)]
        public int FinishPlace { get; set; }

        public TimeSpan FinishTime { get; set; }

        [Required]
        public Guid RaceId { get; set; }

        public RaceData Race { get; set; } = null!;

        [Required]
        public Guid TeamId { get; set; }

        public TeamData Team { get; set; } = null!;
    }
}
