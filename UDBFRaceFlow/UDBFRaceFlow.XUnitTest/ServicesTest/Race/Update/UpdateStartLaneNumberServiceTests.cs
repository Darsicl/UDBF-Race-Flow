using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using NSubstitute;
using UDBFRaceFlow.Application.Dto.Request.Race.Update;
using UDBFRaceFlow.Application.Interfaces.RepositoryContracts;
using UDBFRaceFlow.Application.Services.Race.Update.StartLane;
using UDBFRaceFlow.Domain.Entities.Race;
using UDBFRaceFlow.Domain.Enums;
using Xunit;

namespace UDBFRaceFlow.XUnitTest.ServicesTest.Race.Update
{
    public class UpdateStartLaneNumberServiceTests
    {
        private readonly IRaceDataRepository _raceDataRepositoryMock;
        private readonly ILogger<UpdateStartLaneNumberService> _loggerMock;
        private readonly IValidator<UpdateStartLaneDto> _validatorMock;
        private readonly UpdateStartLaneNumberService _sut;

        public UpdateStartLaneNumberServiceTests()
        {
            _raceDataRepositoryMock = Substitute.For<IRaceDataRepository>();
            _loggerMock = Substitute.For<ILogger<UpdateStartLaneNumberService>>();
            _validatorMock = Substitute.For<IValidator<UpdateStartLaneDto>>();

            _sut = new UpdateStartLaneNumberService(_raceDataRepositoryMock, _loggerMock, _validatorMock);
        }

        [Fact]
        public async Task ChangeTeamStartLane_ShouldReturnFail_WhenValidationFails()
        {
            // Arrange
            var token = TestContext.Current.CancellationToken;
            var dto = new UpdateStartLaneDto(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            var validationFailures = new List<FluentValidation.Results.ValidationFailure>
            {
                new("raceId", "Property is required")
            };
            var validationResult = new FluentValidation.Results.ValidationResult(validationFailures);

            _validatorMock
                .ValidateAsync(dto, token)
                .Returns(Task.FromResult(validationResult));

            // Act
            var result = await _sut.ChangeTeamStartLane(dto, token);

            // Assert
            result.IsFailed.Should().BeTrue();
            await _raceDataRepositoryMock.DidNotReceive().GetRaceWithLanesAsync(Arg.Any<Guid>(), token);
            await _raceDataRepositoryMock.DidNotReceive().SaveChangesAsync(token);
        }

        [Fact]
        public async Task ChangeTeamStartLane_ShouldReturnFail_WhenRaceNotFound()
        {
            // Arrange
            var token = TestContext.Current.CancellationToken;
            var dto = new UpdateStartLaneDto(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            _validatorMock
                .ValidateAsync(dto, token)
                .Returns(Task.FromResult(new FluentValidation.Results.ValidationResult()));

            _raceDataRepositoryMock
                .GetRaceWithLanesAsync(dto.raceId, token)
                .Returns(Task.FromResult<RaceData>(null));

            // Act
            var result = await _sut.ChangeTeamStartLane(dto, token);

            // Assert
            result.IsFailed.Should().BeTrue();
            await _raceDataRepositoryMock.DidNotReceive().SaveChangesAsync(token);
        }

        [Fact]
        public async Task ChangeTeamStartLane_ShouldReturnFail_WhenRaceIsNotScheduled()
        {
            // Arrange
            var token = TestContext.Current.CancellationToken;
            var raceId = Guid.NewGuid();
            var dto = new UpdateStartLaneDto(raceId, Guid.NewGuid(), Guid.NewGuid());

            var existingRace = new RaceData
            {
                Id = raceId,
                RaceStatus = RaceStatus.InProgress,
                RaceNumber = 12
            };

            _validatorMock
                .ValidateAsync(dto, token)
                .Returns(Task.FromResult(new FluentValidation.Results.ValidationResult()));

            _raceDataRepositoryMock
                .GetRaceWithLanesAsync(raceId, token)
                .Returns(Task.FromResult(existingRace));

            // Act
            var result = await _sut.ChangeTeamStartLane(dto, token);

            // Assert
            result.IsFailed.Should().BeTrue();
            await _raceDataRepositoryMock.DidNotReceive().SaveChangesAsync(token);
        }

        [Fact]
        public async Task ChangeTeamStartLane_ShouldReturnFail_WhenLanesAreNullOrMissing()
        {
            // Arrange
            var token = TestContext.Current.CancellationToken;
            var raceId = Guid.NewGuid();
            var dto = new UpdateStartLaneDto(raceId, Guid.NewGuid(), Guid.NewGuid()); // Случайные ID дорожек

            var existingRace = new RaceData
            {
                Id = raceId,
                RaceStatus = RaceStatus.Scheduled,
                Lanes = new List<LaneData>()
            };

            _validatorMock
                .ValidateAsync(dto, token)
                .Returns(Task.FromResult(new FluentValidation.Results.ValidationResult()));

            _raceDataRepositoryMock
                .GetRaceWithLanesAsync(raceId, token)
                .Returns(Task.FromResult(existingRace));

            // Act
            var result = await _sut.ChangeTeamStartLane(dto, token);

            // Assert
            result.IsFailed.Should().BeTrue();
            await _raceDataRepositoryMock.DidNotReceive().SaveChangesAsync(token);
        }

        [Fact]
        public async Task ChangeTeamStartLane_ShouldReturnOkImmediately_WhenStartLaneEqualsTargetLane()
        {
            // Arrange
            var token = TestContext.Current.CancellationToken;
            var raceId = Guid.NewGuid();
            var laneId = Guid.NewGuid();

            var dto = new UpdateStartLaneDto(raceId, laneId, laneId);

            var lane = new LaneData { Id = laneId, StartLane = 1, TeamId = Guid.NewGuid() };
            var existingRace = new RaceData
            {
                Id = raceId,
                RaceStatus = RaceStatus.Scheduled,
                Lanes = new List<LaneData> { lane }
            };

            _validatorMock
                .ValidateAsync(dto, token)
                .Returns(Task.FromResult(new FluentValidation.Results.ValidationResult()));

            _raceDataRepositoryMock
                .GetRaceWithLanesAsync(raceId, token)
                .Returns(Task.FromResult(existingRace));

            // Act
            var result = await _sut.ChangeTeamStartLane(dto, token);

            // Assert
            result.IsSuccess.Should().BeTrue();
            await _raceDataRepositoryMock.DidNotReceive().SaveChangesAsync(token);
        }

        [Fact]
        public async Task ChangeTeamStartLane_ShouldShiftLanesCorrectly_WhenDraggingDown()
        {
            // Arrange
            var token = TestContext.Current.CancellationToken;
            var raceId = Guid.NewGuid();

            var team1 = Guid.NewGuid();
            var team2 = Guid.NewGuid();
            var team3 = Guid.NewGuid();
            var team4 = Guid.NewGuid();

            var lane1 = new LaneData { Id = Guid.NewGuid(), StartLane = 1, TeamId = team1 };
            var lane2 = new LaneData { Id = Guid.NewGuid(), StartLane = 2, TeamId = team2 };
            var lane3 = new LaneData { Id = Guid.NewGuid(), StartLane = 3, TeamId = team3 };
            var lane4 = new LaneData { Id = Guid.NewGuid(), StartLane = 4, TeamId = team4 };

            var existingRace = new RaceData
            {
                Id = raceId,
                RaceStatus = RaceStatus.Scheduled,
                Lanes = new List<LaneData> { lane1, lane2, lane3, lane4 }
            };

            var dto = new UpdateStartLaneDto(raceId, lane4.Id, lane1.Id);

            _validatorMock
                .ValidateAsync(dto, token)
                .Returns(Task.FromResult(new FluentValidation.Results.ValidationResult()));

            _raceDataRepositoryMock
                .GetRaceWithLanesAsync(raceId, token)
                .Returns(Task.FromResult(existingRace));

            // Act
            var result = await _sut.ChangeTeamStartLane(dto, token);

            // Assert
            result.IsSuccess.Should().BeTrue();

            lane1.TeamId.Should().Be(team2);
            lane2.TeamId.Should().Be(team3);
            lane3.TeamId.Should().Be(team4);
            lane4.TeamId.Should().Be(team1);

            await _raceDataRepositoryMock.Received(1).SaveChangesAsync(token);
        }

        [Fact]
        public async Task ChangeTeamStartLane_ShouldShiftLanesCorrectly_WhenDraggingUp()
        {
            // Arrange
            var token = TestContext.Current.CancellationToken;
            var raceId = Guid.NewGuid();

            var team1 = Guid.NewGuid();
            var team2 = Guid.NewGuid();
            var team3 = Guid.NewGuid();
            var team4 = Guid.NewGuid();

            var lane1 = new LaneData { Id = Guid.NewGuid(), StartLane = 1, TeamId = team1 };
            var lane2 = new LaneData { Id = Guid.NewGuid(), StartLane = 2, TeamId = team2 };
            var lane3 = new LaneData { Id = Guid.NewGuid(), StartLane = 3, TeamId = team3 };
            var lane4 = new LaneData { Id = Guid.NewGuid(), StartLane = 4, TeamId = team4 };

            var existingRace = new RaceData
            {
                Id = raceId,
                RaceStatus = RaceStatus.Scheduled,
                Lanes = new List<LaneData> { lane1, lane2, lane3, lane4 }
            };

            var dto = new UpdateStartLaneDto(raceId, lane1.Id, lane4.Id);

            _validatorMock
                .ValidateAsync(dto, token)
                .Returns(Task.FromResult(new FluentValidation.Results.ValidationResult()));

            _raceDataRepositoryMock
                .GetRaceWithLanesAsync(raceId, token)
                .Returns(Task.FromResult(existingRace));

            // Act
            var result = await _sut.ChangeTeamStartLane(dto, token);

            // Assert
            result.IsSuccess.Should().BeTrue();

            lane1.TeamId.Should().Be(team4);
            lane2.TeamId.Should().Be(team1);
            lane3.TeamId.Should().Be(team2);
            lane4.TeamId.Should().Be(team3);

            await _raceDataRepositoryMock.Received(1).SaveChangesAsync(token);
        }
    }
}
