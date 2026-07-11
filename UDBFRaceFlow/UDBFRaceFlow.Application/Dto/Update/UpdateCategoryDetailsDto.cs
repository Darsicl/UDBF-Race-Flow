using UDBFRaceFlow.Domain.Enums;

namespace UDBFRaceFlow.Application.Dto.Update
{
    public record UpdateCategoryDetailsDto(Guid CategoryId, RaceAge RaceAge, int Distance, BoatSize BoatSize, GenderCategory Gender);
}
