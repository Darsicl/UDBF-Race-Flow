using FluentResults;
using UDBFRaceFlow.Application.Dto.Request.Race.Update;

namespace UDBFRaceFlow.Application.Interfaces.ServiceContracts.Race.Update
{
    public interface IUpdateRaceStatusService
    {
        Task<Result> UpdateRaceStatus(UpdateStatusDto statusDto, CancellationToken cancellationToken);
    }
}
