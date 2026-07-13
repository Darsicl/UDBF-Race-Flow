using System.ComponentModel.DataAnnotations;
using UDBFRaceFlow.Domain.Entities.Team;
using UDBFRaceFlow.Domain.Enums;

namespace UDBFRaceFlow.Domain.Entities.Race
{

    public class LaneData
    {
        public LaneData()
        {
            Id = Guid.CreateVersion7();
        }

        [Key]
        public Guid Id { get; set; }

        [Range(1, 7)]
        public int StartLane { get; set; }

        [Range(1, 7)]
        public int? FinishPlace { get; set; } = null;

        public TimeSpan? FinishTime { get; set; } = null;

        public FinishStatus FinishStatus { get; set; }

        [Required]
        public Guid RaceId { get; set; }

        public RaceData Race { get; set; } = null!;

        [Required]
        public Guid TeamId { get; set; }

        public TeamData Team { get; set; } = null!;

        public void ChangeTeam(Guid teamId)
        {
            TeamId = teamId;
        }
    }
}
