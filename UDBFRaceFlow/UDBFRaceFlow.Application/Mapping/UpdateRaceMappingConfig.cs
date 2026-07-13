using Mapster;
using UDBFRaceFlow.Application.Dto.Request.Race.Update;
using UDBFRaceFlow.Domain.Entities.Race;

namespace UDBFRaceFlow.Application.Mapping
{
    public class UpdateRaceMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<UpdateCategoryDetailsDto, RaceCategory>();

            config.NewConfig<RaceResultDto, LaneData>();

            config.NewConfig<UpdateRaceDetailsDto, RaceData>();
        }
    }
}
