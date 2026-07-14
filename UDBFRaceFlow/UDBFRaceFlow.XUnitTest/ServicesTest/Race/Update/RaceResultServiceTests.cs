using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using NSubstitute;
using UDBFRaceFlow.Application.Dto.Request.Race.Update;
using UDBFRaceFlow.Application.Interfaces.RepositoryContracts;
using UDBFRaceFlow.Application.Services.Race.Update.RaceResult;
using UDBFRaceFlow.Domain.Entities.Race;
using UDBFRaceFlow.Domain.Enums;
using Xunit;

namespace UDBFRaceFlow.XUnitTest.ServicesTest.Race.Update
{
    public class RaceResultServiceTests
    {
        private readonly IRaceDataRepository _raceDataRepositoryMock;
        private readonly ILogger<RaceResultService> _loggerMock;
        private readonly IValidator<UpdateLaneResultDto> _validatorMock;
        private readonly RaceResultService _sut;

        public RaceResultServiceTests()
        {
            _raceDataRepositoryMock = Substitute.For<IRaceDataRepository>();
            _loggerMock = Substitute.For<ILogger<RaceResultService>>();
            _validatorMock = Substitute.For<IValidator<UpdateLaneResultDto>>();

            _sut = new RaceResultService(_raceDataRepositoryMock, _loggerMock, _validatorMock);
        }

        [Fact]
        public async Task UpdateLaneResult_ShouldReturnFail_WhenValidationFails()
        {
            // Arrange
            var token = TestContext.Current.CancellationToken;
            var dto = new UpdateLaneResultDto(Guid.NewGuid(), new List<RaceResultDto>(), RaceStatus.Finished);

            var validationResult = new FluentValidation.Results.ValidationResult(new[]
            {
                new FluentValidation.Results.ValidationFailure("RaceId", "Race ID is required")
            });

            _validatorMock.ValidateAsync(dto, token).Returns(Task.FromResult(validationResult));

            // Act
            var result = await _sut.UpdateLaneResult(dto, token);

            // Assert
            result.IsFailed.Should().BeTrue();
            await _raceDataRepositoryMock.DidNotReceive().GetRaceWithLanesAsync(Arg.Any<Guid>(), token);
            await _raceDataRepositoryMock.DidNotReceive().SaveChangesAsync(token);
        }

        [Fact]
        public async Task UpdateLaneResult_ShouldReturnFail_WhenRaceNotFound()
        {
            // Arrange
            var token = TestContext.Current.CancellationToken;
            var raceId = Guid.NewGuid();
            var dto = new UpdateLaneResultDto(raceId, new List<RaceResultDto>(), RaceStatus.Finished);

            _validatorMock.ValidateAsync(dto, token).Returns(Task.FromResult(new FluentValidation.Results.ValidationResult()));
            _raceDataRepositoryMock.GetRaceWithLanesAsync(raceId, token).Returns(Task.FromResult<RaceData>(null!));

            // Act
            var result = await _sut.UpdateLaneResult(dto, token);

            // Assert
            result.IsFailed.Should().BeTrue();
            await _raceDataRepositoryMock.DidNotReceive().SaveChangesAsync(token);
        }

        [Fact]
        public async Task UpdateLaneResult_ShouldCalculatePlacesCorrectly_WhenRaceStatusIsFinished()
        {
            // Arrange
            var token = TestContext.Current.CancellationToken;
            var raceId = Guid.NewGuid();
            var laneId1 = Guid.NewGuid();
            var laneId2 = Guid.NewGuid();
            var laneId3 = Guid.NewGuid();

            var existingRace = new RaceData
            {
                Id = raceId,
                RaceStatus = RaceStatus.InProgress,
                Lanes = new List<LaneData>
                {
                    new LaneData { Id = laneId1 },
                    new LaneData { Id = laneId2 },
                    new LaneData { Id = laneId3 }
                }
            };

            var laneResults = new List<RaceResultDto>
            {
                new RaceResultDto(laneId1, TimeSpan.FromSeconds(75), FinishStatus.Confirmed),
                new RaceResultDto(laneId2, TimeSpan.FromSeconds(70), FinishStatus.Confirmed),
                new RaceResultDto(laneId3, TimeSpan.FromSeconds(80), FinishStatus.Confirmed)
            };

            var dto = new UpdateLaneResultDto(raceId, laneResults, RaceStatus.Finished);

            _validatorMock.ValidateAsync(dto, token).Returns(Task.FromResult(new FluentValidation.Results.ValidationResult()));
            _raceDataRepositoryMock.GetRaceWithLanesAsync(raceId, token).Returns(Task.FromResult(existingRace));

            // Act
            var result = await _sut.UpdateLaneResult(dto, token);

            // Assert
            result.IsSuccess.Should().BeTrue();
            existingRace.RaceStatus.Should().Be(RaceStatus.Finished);

            existingRace.Lanes.First(l => l.Id == laneId2).FinishPlace.Should().Be(1);
            existingRace.Lanes.First(l => l.Id == laneId1).FinishPlace.Should().Be(2);
            existingRace.Lanes.First(l => l.Id == laneId3).FinishPlace.Should().Be(3);

            await _raceDataRepositoryMock.Received(1).SaveChangesAsync(token);
        }

        [Fact]
        public async Task UpdateLaneResult_ShouldIgnoreUnconfirmedOrZeroTimeLanes_WhenCalculatingPlaces()
        {
            // Arrange
            var token = TestContext.Current.CancellationToken;
            var raceId = Guid.NewGuid();
            var confirmedLaneId = Guid.NewGuid();
            var zeroTimeLaneId = Guid.NewGuid();
            var dnsLaneId = Guid.NewGuid();

            var existingRace = new RaceData
            {
                Id = raceId,
                RaceStatus = RaceStatus.InProgress,
                Lanes = new List<LaneData>
                {
                    new LaneData { Id = confirmedLaneId },
                    new LaneData { Id = zeroTimeLaneId },
                    new LaneData { Id = dnsLaneId }
                }
            };

            var laneResults = new List<RaceResultDto>
            {
                new RaceResultDto(confirmedLaneId, TimeSpan.FromMinutes(2), FinishStatus.Confirmed),
                new RaceResultDto(zeroTimeLaneId, TimeSpan.Zero, FinishStatus.Confirmed),
                new RaceResultDto(dnsLaneId, TimeSpan.FromMinutes(1), FinishStatus.DNS)
            };

            var dto = new UpdateLaneResultDto(raceId, laneResults, RaceStatus.Finished);

            _validatorMock.ValidateAsync(dto, token).Returns(Task.FromResult(new FluentValidation.Results.ValidationResult()));
            _raceDataRepositoryMock.GetRaceWithLanesAsync(raceId, token).Returns(Task.FromResult(existingRace));

            // Act
            var result = await _sut.UpdateLaneResult(dto, token);

            // Assert
            result.IsSuccess.Should().BeTrue();

            existingRace.Lanes.First(l => l.Id == confirmedLaneId).FinishPlace.Should().Be(1);
            existingRace.Lanes.First(l => l.Id == zeroTimeLaneId).FinishPlace.Should().BeNull();
            existingRace.Lanes.First(l => l.Id == dnsLaneId).FinishPlace.Should().BeNull();

            await _raceDataRepositoryMock.Received(1).SaveChangesAsync(token);
        }
    }
}
