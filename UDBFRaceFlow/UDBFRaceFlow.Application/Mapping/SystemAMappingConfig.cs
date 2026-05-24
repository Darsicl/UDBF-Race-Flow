using Mapster;
using UDBFRaceFlow.Application.Dto;
using UDBFRaceFlow.Domain.Entities.Race;

namespace UDBFRaceFlow.Application.Mapping
{
    public class SystemAMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<RaceCreationDto, RaceData>()
                .Map(dest => dest.Id, src => Guid.NewGuid())
                .Map(dest => dest.RaceTime, src => src.RaceTime)
                .Map(dest => dest.RaceEntries, src => src.Lanes);

            config.NewConfig<LaneAssignmentDto, RaceEntry>()
                .Map(dest => dest.Id, src => Guid.NewGuid())
                .Map(dest => dest.TeamId, src => src.TeamId)
                .Map(dest => dest.StartLane, src => src.StartLane);
        }
    }
}
