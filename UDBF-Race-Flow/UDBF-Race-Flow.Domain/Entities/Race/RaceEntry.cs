using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UDBFRaceFlow.Domain.Entities.Team;

namespace UDBFRaceFlow.Domain.Entities.Race
{

    public class RaceEntry
    {
        [Key]
        public Guid Id { get; set; }

        [Range(1, 7)]
        public int StartLane { get; set; }

        public int FinishPlace { get; set; }

        public TimeSpan FinishTime { get; set; }

        [ForeignKey]
        public Guid RaceId { get; set; }

        public RaceData Race { get; set; } = null!;

        [ForeignKey]
        public Guid TeamId { get; set; }

        public TeamData Team { get; set; } = null!;
    }
}
