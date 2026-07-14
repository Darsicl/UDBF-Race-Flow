using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using NSubstitute;
using UDBFRaceFlow.Application.Dto.Request.Team;
using UDBFRaceFlow.Application.Interfaces.RepositoryContracts;
using UDBFRaceFlow.Application.Services.Team.Delete;
using UDBFRaceFlow.Domain.Entities.Team;
using Xunit;

namespace UDBFRaceFlow.XUnitTest.ServicesTest.Team
{
    public class DeleteTeamServiceTests
    {
        private readonly ITeamDataRepository _teamDataRepositoryMock;
        private readonly IRaceDataRepository _raceDataRepositoryMock;
        private readonly ILogger<DeleteTeamService> _loggerMock;
        private readonly IValidator<DeleteTeamDto> _validatorMock;
        private readonly DeleteTeamService _sut;

        public DeleteTeamServiceTests()
        {
            _teamDataRepositoryMock = Substitute.For<ITeamDataRepository>();
            _raceDataRepositoryMock = Substitute.For<IRaceDataRepository>();
            _loggerMock = Substitute.For<ILogger<DeleteTeamService>>();
            _validatorMock = Substitute.For<IValidator<DeleteTeamDto>>();

            _sut = new DeleteTeamService(
                _teamDataRepositoryMock,
                _loggerMock,
                _validatorMock,
                _raceDataRepositoryMock
            );
        }

        [Fact]
        public async Task DeleteTeam_ShouldReturnFail_WhenValidationFails()
        {
            // Arrange
            var token = TestContext.Current.CancellationToken;
            var dto = new DeleteTeamDto(Guid.Empty);

            var validationResult = new FluentValidation.Results.ValidationResult(new[]
            {
                new FluentValidation.Results.ValidationFailure("Id", "Id is required")
            });

            _validatorMock.ValidateAsync(dto, token).Returns(Task.FromResult(validationResult));

            // Act
            var result = await _sut.DeleteTeam(dto, token);

            // Assert
            result.IsFailed.Should().BeTrue();
            await _teamDataRepositoryMock.DidNotReceive().GetByIdAsync(Arg.Any<Guid>(), token);
            await _teamDataRepositoryMock.DidNotReceive().DeleteAsync(Arg.Any<TeamData>());
            await _teamDataRepositoryMock.DidNotReceive().SaveChangesAsync(token);
        }

        [Fact]
        public async Task DeleteTeam_ShouldReturnFail_WhenTeamDoesNotExist()
        {
            // Arrange
            var token = TestContext.Current.CancellationToken;
            var teamId = Guid.NewGuid();
            var dto = new DeleteTeamDto(teamId);

            _validatorMock.ValidateAsync(dto, token).Returns(Task.FromResult(new FluentValidation.Results.ValidationResult()));
            _teamDataRepositoryMock.GetByIdAsync(teamId, token).Returns(Task.FromResult<TeamData>(null!));

            // Act
            var result = await _sut.DeleteTeam(dto, token);

            // Assert
            result.IsFailed.Should().BeTrue();
            await _raceDataRepositoryMock.DidNotReceive().IsRacesHasActiveTeam(Arg.Any<Guid>(), token);
            await _teamDataRepositoryMock.DidNotReceive().DeleteAsync(Arg.Any<TeamData>());
            await _teamDataRepositoryMock.DidNotReceive().SaveChangesAsync(token);
        }

        [Fact]
        public async Task DeleteTeam_ShouldReturnFail_WhenTeamIsActiveInRaces()
        {
            // Arrange
            var token = TestContext.Current.CancellationToken;
            var teamId = Guid.NewGuid();
            var existingTeam = TeamData.Create("Active Team");
            var dto = new DeleteTeamDto(teamId);

            _validatorMock.ValidateAsync(dto, token).Returns(Task.FromResult(new FluentValidation.Results.ValidationResult()));
            _teamDataRepositoryMock.GetByIdAsync(teamId, token).Returns(Task.FromResult(existingTeam));

            _raceDataRepositoryMock.IsRacesHasActiveTeam(existingTeam.Id, token).Returns(Task.FromResult(true));

            // Act
            var result = await _sut.DeleteTeam(dto, token);

            // Assert
            result.IsFailed.Should().BeTrue();
            await _teamDataRepositoryMock.DidNotReceive().DeleteAsync(Arg.Any<TeamData>());
            await _teamDataRepositoryMock.DidNotReceive().SaveChangesAsync(token);
        }

        [Fact]
        public async Task DeleteTeam_ShouldDeleteAndSave_WhenValidationsPassAndTeamIsNotActive()
        {
            // Arrange
            var token = TestContext.Current.CancellationToken;
            var teamId = Guid.NewGuid();
            var existingTeam = TeamData.Create("Team to Delete");
            var dto = new DeleteTeamDto(teamId);

            _validatorMock.ValidateAsync(dto, token).Returns(Task.FromResult(new FluentValidation.Results.ValidationResult()));
            _teamDataRepositoryMock.GetByIdAsync(teamId, token).Returns(Task.FromResult(existingTeam));
            _raceDataRepositoryMock.IsRacesHasActiveTeam(existingTeam.Id, token).Returns(Task.FromResult(false));

            // Act
            var result = await _sut.DeleteTeam(dto, token);

            // Assert
            result.IsSuccess.Should().BeTrue();

            await _teamDataRepositoryMock.Received(1).DeleteAsync(existingTeam);
            await _teamDataRepositoryMock.Received(1).SaveChangesAsync(token);
        }
    }
}
