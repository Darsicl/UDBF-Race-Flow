using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using NSubstitute;
using UDBFRaceFlow.Application.Dto.Request.Team;
using UDBFRaceFlow.Application.Interfaces.RepositoryContracts;
using UDBFRaceFlow.Application.Services.Team.Create;
using UDBFRaceFlow.Domain.Entities.Team;
using Xunit;

namespace UDBFRaceFlow.XUnitTest.ServicesTest.Team
{
    public class CreateTeamServiceTests
    {
        private readonly ITeamDataRepository _teamDataRepositoryMock;
        private readonly ILogger<CreateTeamService> _loggerMock;
        private readonly IValidator<CreateTeamDto> _validatorMock;
        private readonly CreateTeamService _sut;

        public CreateTeamServiceTests()
        {
            _teamDataRepositoryMock = Substitute.For<ITeamDataRepository>();
            _loggerMock = Substitute.For<ILogger<CreateTeamService>>();
            _validatorMock = Substitute.For<IValidator<CreateTeamDto>>();

            _sut = new CreateTeamService(_teamDataRepositoryMock, _loggerMock, _validatorMock);
        }

        [Fact]
        public async Task CreateNewTeam_ShouldReturnFail_WhenValidationFails()
        {
            // Arrange
            var token = TestContext.Current.CancellationToken;
            var dto = new CreateTeamDto(string.Empty);

            var validationResult = new FluentValidation.Results.ValidationResult(new[]
            {
                new FluentValidation.Results.ValidationFailure("Name", "Team name is required")
            });

            _validatorMock.ValidateAsync(dto, token).Returns(Task.FromResult(validationResult));

            // Act
            var result = await _sut.CreateNewTeam(dto, token);

            // Assert
            result.IsFailed.Should().BeTrue();
            await _teamDataRepositoryMock.DidNotReceive().IsTeamExistsAsync(Arg.Any<string>(), token);
            await _teamDataRepositoryMock.DidNotReceive().AddAsync(Arg.Any<TeamData>(), token);
            await _teamDataRepositoryMock.DidNotReceive().SaveChangesAsync(token);
        }

        [Fact]
        public async Task CreateNewTeam_ShouldReturnFail_WhenTeamNameAlreadyExists()
        {
            // Arrange
            var token = TestContext.Current.CancellationToken;
            var dto = new CreateTeamDto("Existing Team");

            _validatorMock.ValidateAsync(dto, token).Returns(Task.FromResult(new FluentValidation.Results.ValidationResult()));

            _teamDataRepositoryMock.IsTeamExistsAsync(dto.Name, token).Returns(Task.FromResult(true));

            // Act
            var result = await _sut.CreateNewTeam(dto, token);

            // Assert
            result.IsFailed.Should().BeTrue();
            await _teamDataRepositoryMock.DidNotReceive().AddAsync(Arg.Any<TeamData>(), token);
            await _teamDataRepositoryMock.DidNotReceive().SaveChangesAsync(token);
        }

        [Fact]
        public async Task CreateNewTeam_ShouldCreateAndSave_WhenValidationsPass()
        {
            // Arrange
            var token = TestContext.Current.CancellationToken;
            var dto = new CreateTeamDto("New Unique Team");

            _validatorMock.ValidateAsync(dto, token).Returns(Task.FromResult(new FluentValidation.Results.ValidationResult()));

            _teamDataRepositoryMock.IsTeamExistsAsync(dto.Name, token).Returns(Task.FromResult(false));

            // Act
            var result = await _sut.CreateNewTeam(dto, token);

            // Assert
            result.IsSuccess.Should().BeTrue();

            await _teamDataRepositoryMock.Received(1).AddAsync(Arg.Is<TeamData>(t => t.Name == dto.Name), token);
            await _teamDataRepositoryMock.Received(1).SaveChangesAsync(token);
        }
    }
}
