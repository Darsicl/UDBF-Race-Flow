using Mapster;
using UDBFRaceFlow.Application.Dto;
using UDBFRaceFlow.Domain.Entities.Race;

namespace UDBFRaceFlow.Application.Mapping
{
    public class RaceMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<CreateFullGridDto, RaceCategory>()
                .Map(dest => dest.Id, src => Guid.CreateVersion7())
                .Map(dest => dest.Distance, src => src.Distance)
                .Ignore(dest => dest.Races);

            config.NewConfig<RaceCreationDto, RaceData>()
                .Map(dest => dest.Id, src => Guid.CreateVersion7())
                .Map(dest => dest.RaceTime, src => src.RaceTime)
                .Ignore(dest => dest.Lanes);

            config.NewConfig<LaneAssignmentDto, LaneData>()
                .Map(dest => dest.TeamId, src => src.TeamId)
                .Map(dest => dest.StartLane, src => src.StartLane);
        }
    }
}
