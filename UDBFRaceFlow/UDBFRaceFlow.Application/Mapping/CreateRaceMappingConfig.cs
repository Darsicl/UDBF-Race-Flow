using Mapster;
using UDBFRaceFlow.Application.Dto.Request.Create;
using UDBFRaceFlow.Domain.Entities.Race;

namespace UDBFRaceFlow.Application.Mapping
{
    public class CreateRaceMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<CreateCategoryDto, RaceCategory>()
                .Map(dest => dest.Id, src => Guid.CreateVersion7())
                .Map(dest => dest.Distance, src => src.Distance)
                .Ignore(dest => dest.Races);

            config.NewConfig<CreateRaceDto, RaceData>()
                .Map(dest => dest.Id, src => Guid.CreateVersion7())
                .Map(dest => dest.RaceTime, src => src.RaceTime)
                .Ignore(dest => dest.Lanes);

            config.NewConfig<CreateLaneDto, LaneData>()
                .Map(dest => dest.TeamId, src => src.TeamId)
                .Map(dest => dest.StartLane, src => src.StartLane);
        }
    }
}
