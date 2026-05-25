using Mapster;
using UDBFRaceFlow.Application.Dto;
using UDBFRaceFlow.Domain.Entities.Race;

namespace UDBFRaceFlow.Application.Mapping
{
    public class SystemAMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<CreateFullGridDto, RaceCategory>()
                .Map(dest => dest.Id, src => Guid.NewGuid())
                .Map(dest => dest.Distance, src => src.Distance);

            config.NewConfig<RaceCreationDto, RaceData>()
                .Map(dest => dest.Id, src => Guid.NewGuid())
                .Map(dest => dest.RaceTime, src => src.RaceTime)
                .Map(dest => dest.Lanes, src => src.Lanes);

            config.NewConfig<LaneAssignmentDto, LaneData>()
                .Map(dest => dest.Id, src => Guid.NewGuid())
                .Map(dest => dest.TeamId, src => src.TeamId)
                .Map(dest => dest.StartLane, src => src.StartLane);
        }
    }
}
