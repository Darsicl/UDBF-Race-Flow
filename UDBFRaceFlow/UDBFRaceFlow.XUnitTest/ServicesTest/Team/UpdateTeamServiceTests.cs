using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using NSubstitute;
using UDBFRaceFlow.Application.Dto.Request.Team;
using UDBFRaceFlow.Application.Interfaces.RepositoryContracts;
using UDBFRaceFlow.Application.Services.Team.Update;
using UDBFRaceFlow.Domain.Entities.Team;
using Xunit;

namespace UDBFRaceFlow.XUnitTest.ServicesTest.Team
{
    public class UpdateTeamServiceTests
    {
        private readonly ITeamDataRepository _teamDataRepositoryMock;
        private readonly ILogger<UpdateTeamService> _loggerMock;
        private readonly IValidator<UpdateTeamDto> _validatorMock;
        private readonly UpdateTeamService _sut;

        public UpdateTeamServiceTests()
        {
            _teamDataRepositoryMock = Substitute.For<ITeamDataRepository>();
            _loggerMock = Substitute.For<ILogger<UpdateTeamService>>();
            _validatorMock = Substitute.For<IValidator<UpdateTeamDto>>();

            _sut = new UpdateTeamService(_teamDataRepositoryMock, _loggerMock, _validatorMock);
        }

        [Fact]
        public async Task UpdateTeamName_ShouldReturnFail_WhenValidationFails()
        {
            // Arrange
            var token = TestContext.Current.CancellationToken;
            var dto = new UpdateTeamDto(Guid.NewGuid(), string.Empty);

            var validationResult = new FluentValidation.Results.ValidationResult(new[]
            {
                new FluentValidation.Results.ValidationFailure("Name", "Team name is required")
            });

            _validatorMock.ValidateAsync(dto, token).Returns(Task.FromResult(validationResult));

            // Act
            var result = await _sut.UpdateTeamName(dto, token);

            // Assert
            result.IsFailed.Should().BeTrue();
            await _teamDataRepositoryMock.DidNotReceive().GetByIdAsync(Arg.Any<Guid>(), token);
            await _teamDataRepositoryMock.DidNotReceive().SaveChangesAsync(token);
        }

        [Fact]
        public async Task UpdateTeamName_ShouldReturnFail_WhenTeamDoesNotExist()
        {
            // Arrange
            var token = TestContext.Current.CancellationToken;
            var teamId = Guid.NewGuid();
            var dto = new UpdateTeamDto(teamId, "New Name");

            _validatorMock.ValidateAsync(dto, token).Returns(Task.FromResult(new FluentValidation.Results.ValidationResult()));
            _teamDataRepositoryMock.GetByIdAsync(teamId, token).Returns(Task.FromResult<TeamData>(null!));

            // Act
            var result = await _sut.UpdateTeamName(dto, token);

            // Assert
            result.IsFailed.Should().BeTrue();
            await _teamDataRepositoryMock.DidNotReceive().IsTeamExistsAsync(Arg.Any<string>(), Arg.Any<Guid>(), token);
            await _teamDataRepositoryMock.DidNotReceive().SaveChangesAsync(token);
        }

        [Fact]
        public async Task UpdateTeamName_ShouldReturnFail_WhenNewNameAlreadyExistsForAnotherTeam()
        {
            // Arrange
            var token = TestContext.Current.CancellationToken;
            var teamId = Guid.NewGuid();
            var existingTeam = TeamData.Create("Old Name");
            // Подменяем Id у созданной сущности, если это необходимо для прохождения дальнейшей логики (или полагаемся на поведение доменной модели)

            var dto = new UpdateTeamDto(teamId, "Existing Team Name");

            _validatorMock.ValidateAsync(dto, token).Returns(Task.FromResult(new FluentValidation.Results.ValidationResult()));
            _teamDataRepositoryMock.GetByIdAsync(teamId, token).Returns(Task.FromResult(existingTeam));

            _teamDataRepositoryMock.IsTeamExistsAsync(dto.Name, existingTeam.Id, token).Returns(Task.FromResult(true));

            // Act
            var result = await _sut.UpdateTeamName(dto, token);

            // Assert
            result.IsFailed.Should().BeTrue();
            await _teamDataRepositoryMock.DidNotReceive().SaveChangesAsync(token);
        }

        [Fact]
        public async Task UpdateTeamName_ShouldUpdateNameAndSave_WhenValidationsPass()
        {
            // Arrange
            var token = TestContext.Current.CancellationToken;
            var teamId = Guid.NewGuid();
            var existingTeam = TeamData.Create("Old Name");
            var dto = new UpdateTeamDto(teamId, "Perfect New Name");

            _validatorMock.ValidateAsync(dto, token).Returns(Task.FromResult(new FluentValidation.Results.ValidationResult()));
            _teamDataRepositoryMock.GetByIdAsync(teamId, token).Returns(Task.FromResult(existingTeam));
            _teamDataRepositoryMock.IsTeamExistsAsync(dto.Name, existingTeam.Id, token).Returns(Task.FromResult(false));

            // Act
            var result = await _sut.UpdateTeamName(dto, token);

            // Assert
            result.IsSuccess.Should().BeTrue();
            existingTeam.Name.Should().Be(dto.Name);

            await _teamDataRepositoryMock.Received(1).SaveChangesAsync(token);
        }
    }
}
