using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;
using UDBFRaceFlow.Application.Dto.Request.Team;
using UDBFRaceFlow.Application.Extensions.ValidatorExtensions;
using UDBFRaceFlow.Application.Interfaces.RepositoryContracts;
using UDBFRaceFlow.Application.Interfaces.ServiceContracts.Team;
using UDBFRaceFlow.Domain.Entities.Team;
using UDBFRaceFlow.Domain.Resources;

namespace UDBFRaceFlow.Application.Services.Team.Create
{
    public class CreateTeamService : ICreateTeamService
    {
        private readonly ITeamDataRepository _teamDataRepository;
        private readonly IValidator<CreateTeamDto> _validator;
        private readonly ILogger<CreateTeamService> _logger;

        public CreateTeamService(ITeamDataRepository teamDataRepository, ILogger<CreateTeamService> logger, IValidator<CreateTeamDto> validator)
        {
            _teamDataRepository = teamDataRepository;
            _logger = logger;
            _validator = validator;
        }

        public async Task<Result> CreateNewTeam(CreateTeamDto teamDto, CancellationToken cancellationToken = default)
        {
            var validate = await _validator.ValidateDtoAsync(teamDto, cancellationToken);
            if (validate.IsFailed)
            {
                return validate;
            }

            var teamExists = await _teamDataRepository.IsTeamExistsAsync(teamDto.Name, cancellationToken);
            if (teamExists)
            {
                string errorMsg = string.Format(Messages.Error_PropertyNotUnique, nameof(teamDto.Name));
                _logger.LogError(errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var newTeam = TeamData.Create(teamDto.Name);

            await _teamDataRepository.AddAsync(newTeam, cancellationToken);
            await _teamDataRepository.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}
