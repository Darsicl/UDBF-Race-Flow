using System.ComponentModel.DataAnnotations;
using UDBFRaceFlow.Domain.Entities.Team;
using UDBFRaceFlow.Domain.Resources;

namespace UDBFRaceFlow.Domain.Entities.Race
{

    public class RaceEntry
    {
        [Key]
        public Guid Id { get; set; }

        [Range(1, 7, ErrorMessageResourceName = Messages.Error_StartLaneRange, ErrorMessageResourceType = typeof(Messages))]
        public int StartLane { get; set; }

        public int FinishPlace { get; set; }

        public TimeSpan FinishTime { get; set; }

        public Guid RaceId { get; set; }

        public RaceData Race { get; set; } = null!;

        public Guid TeamId { get; set; }

        public TeamData Team { get; set; } = null!;
    }
}
