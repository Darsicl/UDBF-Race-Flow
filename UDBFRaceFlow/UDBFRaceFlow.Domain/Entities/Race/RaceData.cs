using System.ComponentModel.DataAnnotations;
using UDBFRaceFlow.Domain.Enums;

namespace UDBFRaceFlow.Domain.Entities.Race
{
    public class RaceData
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public int RaceNumber { get; set; }

        [Required]
        public RaceStatus RaceStatus { get; set; }

        [Required]
        public RaceType RaceType { get; set; }

        [Required]
        public DateTime RaceTime { get; set; }

        [Required]
        public DateTime OriginalDateTime { get; set; }

        [Required]
        public int SequenceNumber { get; set; }

        public RaceCategory Category { get; set; } = null!;

        [Required]
        public Guid CategoryId { get; set; }

        public List<LaneData> Lanes { get; set; } = new List<LaneData>();
    }
}
