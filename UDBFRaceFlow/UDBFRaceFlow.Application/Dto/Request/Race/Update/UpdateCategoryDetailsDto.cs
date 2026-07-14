using UDBFRaceFlow.Domain.Enums;

namespace UDBFRaceFlow.Application.Dto.Request.Race.Update
{
    public record UpdateCategoryDetailsDto(Guid CategoryId, RaceAge RaceAge, int Distance, BoatSize BoatSize, GenderCategory GenderCategory);
}



