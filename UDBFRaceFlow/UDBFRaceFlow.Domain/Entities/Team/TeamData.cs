using System.ComponentModel.DataAnnotations;
using UDBFRaceFlow.Domain.Entities.Race;
using UDBFRaceFlow.Domain.Resources;

namespace UDBFRaceFlow.Domain.Entities.Team
{
    public class TeamData
    {
        private TeamData() { }

        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        public List<LaneData> Lanes { get; set; } = new List<LaneData>();

        public static TeamData Create(string name)
        {
            return new TeamData
            {
                Id = Guid.CreateVersion7(),
                Name = name
            };
        }

        public void UpdateName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
            {
                throw new ArgumentNullException(Messages.Error_PropertyIsRequired, newName);
            }

            Name = newName;
        }
    }
}
