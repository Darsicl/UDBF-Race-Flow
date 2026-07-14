using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;
using UDBFRaceFlow.Application.Dto.Request.Team;
using UDBFRaceFlow.Application.Extensions.ValidatorExtensions;
using UDBFRaceFlow.Application.Interfaces.RepositoryContracts;
using UDBFRaceFlow.Application.Interfaces.ServiceContracts.Team;
using UDBFRaceFlow.Domain.Entities.Team;
using UDBFRaceFlow.Domain.Resources;

namespace UDBFRaceFlow.Application.Services.Team.Update
{
    public class UpdateTeamService : IUpdateTeamService
    {
        private readonly ITeamDataRepository _teamDataRepository;
        private readonly ILogger<UpdateTeamService> _logger;
        private readonly IValidator<UpdateTeamDto> _validator;

        public UpdateTeamService(ITeamDataRepository teamDataRepository, ILogger<UpdateTeamService> logger, IValidator<UpdateTeamDto> validator)
        {
            _teamDataRepository = teamDataRepository;
            _logger = logger;
            _validator = validator;
        }

        public async Task<Result> UpdateTeamName(UpdateTeamDto teamDto, CancellationToken cancellationToken = default)
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

            var teamExists = await _teamDataRepository.IsTeamExistsAsync(teamDto.Name, team.Id, cancellationToken);

            if (teamExists)
            {
                string errorMsg = string.Format(Messages.Error_PropertyNotUnique, teamDto.Name);
                _logger.LogError(errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            team.UpdateName(teamDto.Name);

            await _teamDataRepository.SaveChangesAsync(cancellationToken);

            return Result.Ok();

        }
    }
}
