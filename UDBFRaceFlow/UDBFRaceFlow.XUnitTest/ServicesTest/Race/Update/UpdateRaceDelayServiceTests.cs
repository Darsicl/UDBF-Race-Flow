using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using NSubstitute;
using UDBFRaceFlow.Application.Dto.Request.Race.Update;
using UDBFRaceFlow.Application.Interfaces.RepositoryContracts;
using UDBFRaceFlow.Application.Services.Race.Update.RaceDelay;
using UDBFRaceFlow.Domain.Entities.Race;
using Xunit;

namespace UDBFRaceFlow.XUnitTest.ServicesTest.Race.Update
{
    public class UpdateRaceDelayServiceTests
    {
        private readonly IRaceDataRepository _raceDataRepositoryMock;
        private readonly ILogger<UpdateRaceDelayService> _loggerMock;
        private readonly IValidator<RaceDelayDto> _validatorMock;
        private readonly UpdateRaceDelayService _sut;

        public UpdateRaceDelayServiceTests()
        {
            _raceDataRepositoryMock = Substitute.For<IRaceDataRepository>();
            _loggerMock = Substitute.For<ILogger<UpdateRaceDelayService>>();
            _validatorMock = Substitute.For<IValidator<RaceDelayDto>>();

            _sut = new UpdateRaceDelayService(_loggerMock, _raceDataRepositoryMock, _validatorMock);
        }

        [Fact]
        public async Task UpdateRaceDelay_ShouldReturnFail_WhenValidationFails()
        {
            // Arrange
            var token = TestContext.Current.CancellationToken;
            var dto = new RaceDelayDto(TimeSpan.FromMinutes(15), new DateOnly(2026, 7, 12));

            var validationResult = new FluentValidation.Results.ValidationResult(new[]
            {
                new FluentValidation.Results.ValidationFailure("DelayDay", "Invalid date")
            });

            _validatorMock.ValidateAsync(dto, token).Returns(Task.FromResult(validationResult));

            // Act
            var result = await _sut.UpdateRaceDelay(dto, token);

            // Assert
            result.IsFailed.Should().BeTrue();
            await _raceDataRepositoryMock.DidNotReceive().GetRacesByDayForDelayAsync(Arg.Any<DateTime>(), token);
            await _raceDataRepositoryMock.DidNotReceive().SaveChangesAsync(token);
        }

        [Fact]
        public async Task UpdateRaceDelay_ShouldReturnOkImmediately_WhenNoRacesFoundForDay()
        {
            // Arrange
            var token = TestContext.Current.CancellationToken;
            var delayDay = new DateOnly(2026, 7, 12);
            var expectedDate = delayDay.ToDateTime(TimeOnly.MinValue);
            var dto = new RaceDelayDto(TimeSpan.FromMinutes(15), delayDay);

            _validatorMock.ValidateAsync(dto, token).Returns(Task.FromResult(new FluentValidation.Results.ValidationResult()));

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

            _validatorMock.ValidateAsync(dto, token).Returns(Task.FromResult(new FluentValidation.Results.ValidationResult()));

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
    }
}
