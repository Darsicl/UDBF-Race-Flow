using UDBF_Race_Flow.Domain.Enums;

namespace UDBF_Race_Flow.Domain.Entities.Race
{
    public class RaceData
    {
        public Guid Id { get; set; }

        public string? BoatSize { get; set; }

        public int Distance { get; set; }

        public GenderCategory GenderCategory { get; set; }

        public RaceStatus RaceStatus { get; set; }
        public RaceType RaceType { get; set; }

    }
}
