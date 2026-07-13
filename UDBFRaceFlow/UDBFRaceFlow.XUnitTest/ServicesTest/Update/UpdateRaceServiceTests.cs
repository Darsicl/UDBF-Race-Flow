using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using UDBFRaceFlow.Application.Dto.Request.Update;
using UDBFRaceFlow.Application.Interfaces.RepositoryContracts;
using UDBFRaceFlow.Application.Services.Race.Update;
using UDBFRaceFlow.Domain.Entities.Race;
using UDBFRaceFlow.Domain.Enums;
using Xunit;

namespace UDBFRaceFlow.XUnitTest.ServicesTest.Update
{
    public class UpdateRaceServiceTests
    {
        private readonly IRaceDataRepository _raceDataRepositoryMock;
        private readonly ILogger<UpdateRaceDelayService> _loggerMock;
        private readonly UpdateRaceDelayService _sut;

        public UpdateRaceServiceTests()
        {
            _raceDataRepositoryMock = Substitute.For<IRaceDataRepository>();
            _loggerMock = Substitute.For<ILogger<UpdateRaceDelayService>>();

            _sut = new UpdateRaceDelayService(_loggerMock, _raceDataRepositoryMock);
        }

        [Fact]
        public async Task UpdateRaceDelay_ShouldReturnOkImmediately_WhenNoRacesFoundForDay()
        {
            // Arrange
            var token = TestContext.Current.CancellationToken;
            var delayDay = new DateOnly(2026, 7, 12);
            var expectedDate = delayDay.ToDateTime(TimeOnly.MinValue);
            var dto = new RaceDelayDto(TimeSpan.FromMinutes(15), delayDay);

            _raceDataRepositoryMock
                .GetRacesByDayForDelayAsync(expectedDate, token)
                .Returns(Task.FromResult(new List<RaceData>()));

            // Act
            var result = await _sut.UpdateRaceDelay(dto, token);

            // Assert
            result.IsSuccess.Should().BeTrue();
            await _raceDataRepositoryMock.DidNotReceive().SaveChangesAsync(token);
        }

        [Fact]
        public async Task UpdateRaceDelay_ShouldShiftRaceTimeAndSave_WhenRacesExist()
        {
            // Arrange
            var token = TestContext.Current.CancellationToken;
            var delayDay = new DateOnly(2026, 7, 12);
            var expectedDate = delayDay.ToDateTime(TimeOnly.MinValue);
            var dto = new RaceDelayDto(TimeSpan.FromMinutes(20), delayDay);

            var originalTime1 = new DateTime(2026, 7, 12, 10, 0, 0);
            var originalTime2 = new DateTime(2026, 7, 12, 10, 30, 0);

            var races = new List<RaceData>
            {
                new RaceData { OriginalDateTime = originalTime1, RaceTime = originalTime1 },
                new RaceData { OriginalDateTime = originalTime2, RaceTime = originalTime2 }
            };

            _raceDataRepositoryMock
                .GetRacesByDayForDelayAsync(expectedDate, token)
                .Returns(Task.FromResult(races));

            // Act
            var result = await _sut.UpdateRaceDelay(dto, token);

            // Assert
            result.IsSuccess.Should().BeTrue();
            races[0].RaceTime.Should().Be(originalTime1.AddMinutes(20));
            races[1].RaceTime.Should().Be(originalTime2.AddMinutes(20));

            await _raceDataRepositoryMock.Received(1).SaveChangesAsync(token);
        }

        [Fact]
        public async Task UpdateRaceDetails_ShouldReturnFail_WhenRaceNotFound()
        {
            // Arrange
            var token = TestContext.Current.CancellationToken;
            var raceId = Guid.NewGuid();
            var dto = new UpdateRaceDetailsDto(raceId, 1, DateTime.Now);

            _raceDataRepositoryMock
                .GetByIdAsync(raceId, token)
                .Returns(Task.FromResult<RaceData>(null));

            // Act
            var result = await _sut.UpdateRaceDetails(dto, token);

            // Assert
            result.IsFailed.Should().BeTrue();
            await _raceDataRepositoryMock.DidNotReceive().SaveChangesAsync(token);
        }

        [Fact]
        public async Task UpdateRaceDetails_ShouldReturnFail_WhenRaceDateIsNotUnique()
        {
            // Arrange
            var token = TestContext.Current.CancellationToken;
            var raceId = Guid.NewGuid();
            var existingRace = new RaceData { Id = raceId, RaceNumber = 5, OriginalDateTime = DateTime.Now };
            var dto = new UpdateRaceDetailsDto(raceId, 10, new DateTime(2026, 7, 12, 14, 0, 0));

            _raceDataRepositoryMock
                .GetByIdAsync(raceId, token)
                .Returns(Task.FromResult(existingRace));

            _raceDataRepositoryMock
                .IsRaceDateUniqueAsync(existingRace.OriginalDateTime, raceId, token)
                .Returns(Task.FromResult(true));

            _raceDataRepositoryMock
                .IsRaceDateUniqueAsync(dto.OriginalDateTime, raceId, token)
                .Returns(Task.FromResult(true));

            _raceDataRepositoryMock
                .GetAllRacesAsync(token)
                .Returns(Task.FromResult(new List<RaceData>()));

            // Act
            var result = await _sut.UpdateRaceDetails(dto, token);

            // Assert
            result.IsFailed.Should().BeTrue();
            await _raceDataRepositoryMock.DidNotReceive().SaveChangesAsync(token);
        }

        [Fact]
        public async Task UpdateRaceDetails_ShouldReturnFail_WhenRaceNumberIsNotUnique()
        {
            // Arrange
            var token = TestContext.Current.CancellationToken;
            var raceId = Guid.NewGuid();
            var existingRace = new RaceData { Id = raceId, RaceNumber = 5, OriginalDateTime = DateTime.Now };
            var dto = new UpdateRaceDetailsDto(raceId, 99, DateTime.Now);

            _raceDataRepositoryMock
                .GetByIdAsync(raceId, token)
                .Returns(Task.FromResult(existingRace));

            _raceDataRepositoryMock
                .IsRaceDateUniqueAsync(existingRace.OriginalDateTime, raceId, token)
                .Returns(Task.FromResult(false));

            _raceDataRepositoryMock
                .IsRaceNumberUniqueAsync(dto.RaceNumber, raceId, token)
                .Returns(Task.FromResult(true));

            _raceDataRepositoryMock
                .GetAllRacesAsync(token)
                .Returns(Task.FromResult(new List<RaceData>()));

            // Act
            var result = await _sut.UpdateRaceDetails(dto, token);

            // Assert
            result.IsFailed.Should().BeTrue();
            await _raceDataRepositoryMock.DidNotReceive().SaveChangesAsync(token);
        }

        [Fact]
        public async Task UpdateRaceDetails_ShouldUpdateAndSave_WhenAllValidationsPass()
        {
            // Arrange
            var token = TestContext.Current.CancellationToken;
            var raceId = Guid.NewGuid();
            var existingRace = new RaceData { Id = raceId, RaceNumber = 1, OriginalDateTime = DateTime.Now };
            var newDateTime = new DateTime(2026, 7, 12, 16, 0, 0);
            var dto = new UpdateRaceDetailsDto(raceId, 5, newDateTime);

            _raceDataRepositoryMock
                .GetByIdAsync(raceId, token)
                .Returns(Task.FromResult(existingRace));

            _raceDataRepositoryMock
                .IsRaceDateUniqueAsync(newDateTime, raceId, token)
                .Returns(Task.FromResult(false));

            _raceDataRepositoryMock
                .IsRaceNumberUniqueAsync(5, raceId, token)
                .Returns(Task.FromResult(false));

            _raceDataRepositoryMock
                .GetAllRacesAsync(token)
                .Returns(Task.FromResult(new List<RaceData> { existingRace }));

            // Act
            var result = await _sut.UpdateRaceDetails(dto, token);

            // Assert
            result.IsSuccess.Should().BeTrue();
            existingRace.RaceNumber.Should().Be(5);
            existingRace.OriginalDateTime.Should().Be(newDateTime);

            await _raceDataRepositoryMock.Received(1).SaveChangesAsync(token);
        }

        [Fact]
        public async Task UpdateRaceStatus_ShouldReturnFail_WhenRaceDoesNotExist()
        {
            // Arrange
            var token = TestContext.Current.CancellationToken;
            var raceId = Guid.NewGuid();
            var dto = new UpdateStatusDto(raceId, RaceStatus.Finished);

            _raceDataRepositoryMock
                .GetByIdAsync(raceId, token)
                .Returns(Task.FromResult<RaceData>(null));

            // Act
            var result = await _sut.UpdateRaceStatus(dto, token);

            // Assert
            result.IsFailed.Should().BeTrue();
            await _raceDataRepositoryMock.DidNotReceive().SaveChangesAsync(token);
        }

        [Fact]
        public async Task UpdateRaceStatus_ShouldUpdateStatusAndSave_WhenRaceExists()
        {
            // Arrange
            var token = TestContext.Current.CancellationToken;
            var raceId = Guid.NewGuid();
            var existingRace = new RaceData { Id = raceId, RaceStatus = RaceStatus.InProgress };
            var dto = new UpdateStatusDto(raceId, RaceStatus.Finished);

            _raceDataRepositoryMock
                .GetByIdAsync(raceId, token)
                .Returns(Task.FromResult(existingRace));

            // Act
            var result = await _sut.UpdateRaceStatus(dto, token);

            // Assert
            result.IsSuccess.Should().BeTrue();
            existingRace.RaceStatus.Should().Be(RaceStatus.Finished);

            await _raceDataRepositoryMock.Received(1).SaveChangesAsync(token);
        }
    }
}
