using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using UDBFRaceFlow.Application.Dto.Request.Race.Update;
using UDBFRaceFlow.Application.Dto.Request.Update;
using UDBFRaceFlow.Application.Interfaces.RepositoryContracts;
using UDBFRaceFlow.Application.Services.Race.Update;
using UDBFRaceFlow.Domain.Entities.Race;
using UDBFRaceFlow.Domain.Enums;
using Xunit;

namespace UDBFRaceFlow.XUnitTest.ServicesTest.Update
{
    public class RaceResultServiceTests
    {
        private readonly IRaceDataRepository _raceDataRepositoryMock;
        private readonly ILogger<RaceResultService> _loggerMock;
        private readonly RaceResultService _sut;

        public RaceResultServiceTests()
        {
            _raceDataRepositoryMock = Substitute.For<IRaceDataRepository>();
            _loggerMock = Substitute.For<ILogger<RaceResultService>>();

            _sut = new RaceResultService(_raceDataRepositoryMock, _loggerMock);
        }

        [Fact]
        public async Task UpdateLaneResult_ShouldCalculatePlacesCorrectly_WhenRaceStatusIsFinished()
        {
            // Arrange
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
                    new LaneData { Id = laneId1, FinishTime = TimeSpan.Zero},
                    new LaneData { Id = laneId2, FinishTime = TimeSpan.Zero},
                    new LaneData { Id = laneId3, FinishTime = TimeSpan.Zero }
                }
            };

            var laneResults = new List<RaceResultDto>
            {
                new RaceResultDto(laneId1, TimeSpan.FromSeconds(75), FinishStatus.Confirmed),
                new RaceResultDto(laneId2, TimeSpan.FromSeconds(70), FinishStatus.Confirmed),
                new RaceResultDto(laneId3, TimeSpan.FromSeconds(80), FinishStatus.Confirmed)
            };

            var dto = new UpdateLaneResultDto(raceId, laneResults, RaceStatus.Finished);

            _raceDataRepositoryMock
                .GetRaceWithLanesAsync(raceId, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(existingRace));

            // Act
            var result = await _sut.UpdateLaneResult(dto, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            existingRace.RaceStatus.Should().Be(RaceStatus.Finished);

            existingRace.Lanes.First(l => l.Id == laneId2).FinishPlace.Should().Be(1);
            existingRace.Lanes.First(l => l.Id == laneId1).FinishPlace.Should().Be(2);
            existingRace.Lanes.First(l => l.Id == laneId3).FinishPlace.Should().Be(3);

            await _raceDataRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task UpdateLaneResult_ShouldIgnoreUnconfirmedOrZeroTimeLanes_WhenCalculatingPlaces()
        {
            // Arrange
            var raceId = Guid.NewGuid();
            var confirmedLaneId = Guid.NewGuid();
            var zeroTimeLaneId = Guid.NewGuid();
            var dnsLaneId = Guid.NewGuid();

            var existingRace = new RaceData
            {
                Id = raceId,
                Lanes = new List<LaneData>
                {
                    new LaneData { Id = confirmedLaneId, FinishTime = TimeSpan.Zero},
                    new LaneData { Id = zeroTimeLaneId, FinishTime = TimeSpan.Zero},
                    new LaneData { Id = dnsLaneId, FinishTime = TimeSpan.Zero}
                }
            };

            var laneResults = new List<RaceResultDto>
            {
                new RaceResultDto(confirmedLaneId, TimeSpan.FromMinutes(2), FinishStatus.Confirmed),
                new RaceResultDto(zeroTimeLaneId, TimeSpan.Zero, FinishStatus.Confirmed),
                new RaceResultDto(dnsLaneId, TimeSpan.FromMinutes(1), FinishStatus.DNS)
            };

            var dto = new UpdateLaneResultDto(raceId, laneResults, RaceStatus.Finished);

            _raceDataRepositoryMock
                .GetRaceWithLanesAsync(raceId, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(existingRace));

            // Act
            var result = await _sut.UpdateLaneResult(dto, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            existingRace.Lanes.First(l => l.Id == confirmedLaneId).FinishPlace.Should().Be(1);
            existingRace.Lanes.First(l => l.Id == zeroTimeLaneId).FinishPlace.Should().BeNull();
            existingRace.Lanes.First(l => l.Id == dnsLaneId).FinishPlace.Should().BeNull();
        }

        [Fact]
        public async Task UpdateLaneResult_ShouldReturnFail_WhenRaceNotFound()
        {
            // Arrange
            var raceId = Guid.NewGuid();
            var dto = new UpdateLaneResultDto(raceId, new List<RaceResultDto>(), RaceStatus.Finished);

            _raceDataRepositoryMock
                .GetRaceWithLanesAsync(raceId, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<RaceData>(null));

            // Act
            var result = await _sut.UpdateLaneResult(dto, CancellationToken.None);

            // Assert
            result.IsFailed.Should().BeTrue();
            await _raceDataRepositoryMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
