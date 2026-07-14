using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;
using UDBFRaceFlow.Application.Dto.Request.Team;
using UDBFRaceFlow.Application.Extensions.ValidatorExtensions;
using UDBFRaceFlow.Application.Interfaces.RepositoryContracts;
using UDBFRaceFlow.Application.Interfaces.ServiceContracts.Team;
using UDBFRaceFlow.Domain.Entities.Team;
using UDBFRaceFlow.Domain.Resources;

namespace UDBFRaceFlow.Application.Services.Team.Delete
{
    public class DeleteTeamService : IDeleteTeamService
    {
        private readonly ITeamDataRepository _teamDataRepository;
        private readonly ILogger<DeleteTeamService> _logger;
        private readonly IValidator<DeleteTeamDto> _validator;
        private readonly IRaceDataRepository _raceDataRepository;

        public DeleteTeamService(ITeamDataRepository teamDataRepository, ILogger<DeleteTeamService> logger, IValidator<DeleteTeamDto> validator, IRaceDataRepository raceDataRepository)
        {
            _teamDataRepository = teamDataRepository;
            _logger = logger;
            _validator = validator;
            _raceDataRepository = raceDataRepository;
        }

        public async Task<Result> DeleteTeam(DeleteTeamDto teamDto, CancellationToken cancellationToken)
        {

            var validationResult = await _validator.ValidateDtoAsync(teamDto, cancellationToken);

            if (validationResult.IsFailed)
            {
                return validationResult;
            }

            var team = await _teamDataRepository.GetByIdAsync(teamDto.Id, cancellationToken);

            if (team is null)
            {
                string errorMsg = string.Format(Messages.Error_EntityWithIdNotFound, nameof(TeamData), teamDto.Id);
                _logger.LogError(errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var isTeamActive = await _raceDataRepository.IsRacesHasActiveTeam(team.Id, cancellationToken);

            if (isTeamActive)
            {
                string errorMsg = string.Format(Messages.Error_DeleteIsForbidden, team.Name);
                _logger.LogError(errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            await _teamDataRepository.DeleteAsync(team);
            await _teamDataRepository.SaveChangesAsync(cancellationToken);

            return Result.Ok();

        }
    }
}
