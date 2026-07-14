using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using NSubstitute;
using UDBFRaceFlow.Application.Dto.Request.Race.Update;
using UDBFRaceFlow.Application.Interfaces.RepositoryContracts;
using UDBFRaceFlow.Application.Services.Race.Update.ChangeRaceStatus;
using UDBFRaceFlow.Domain.Entities.Race;
using UDBFRaceFlow.Domain.Enums;
using Xunit;

namespace UDBFRaceFlow.XUnitTest.ServicesTest.Race.Update
{
    public class UpdateRaceStatusServiceTests
    {
        private readonly IRaceDataRepository _raceDataRepositoryMock;
        private readonly ILogger<UpdateRaceStatusService> _loggerMock;
        private readonly IValidator<UpdateStatusDto> _validatorMock;
        private readonly UpdateRaceStatusService _sut;

        public UpdateRaceStatusServiceTests()
        {
            _raceDataRepositoryMock = Substitute.For<IRaceDataRepository>();
            _loggerMock = Substitute.For<ILogger<UpdateRaceStatusService>>();
            _validatorMock = Substitute.For<IValidator<UpdateStatusDto>>();

            _sut = new UpdateRaceStatusService(_raceDataRepositoryMock, _loggerMock, _validatorMock);
        }

        [Fact]
        public async Task UpdateRaceStatus_ShouldReturnFail_WhenValidationFails()
        {
            // Arrange
            var token = TestContext.Current.CancellationToken;
            var dto = new UpdateStatusDto(Guid.NewGuid(), RaceStatus.Finished);

            var validationResult = new FluentValidation.Results.ValidationResult(new[]
            {
                new FluentValidation.Results.ValidationFailure("RaceStatus", "Invalid status")
            });

            _validatorMock.ValidateAsync(dto, token).Returns(Task.FromResult(validationResult));

            // Act
            var result = await _sut.UpdateRaceStatus(dto, token);

            // Assert
            result.IsFailed.Should().BeTrue();
            await _raceDataRepositoryMock.DidNotReceive().GetByIdAsync(Arg.Any<Guid>(), token);
            await _raceDataRepositoryMock.DidNotReceive().SaveChangesAsync(token);
        }

        [Fact]
        public async Task UpdateRaceStatus_ShouldReturnFail_WhenRaceDoesNotExist()
        {
            // Arrange
            var token = TestContext.Current.CancellationToken;
            var raceId = Guid.NewGuid();
            var dto = new UpdateStatusDto(raceId, RaceStatus.Finished);

            _validatorMock.ValidateAsync(dto, token).Returns(Task.FromResult(new FluentValidation.Results.ValidationResult()));

            _raceDataRepositoryMock
                .GetByIdAsync(raceId, token)
                .Returns(Task.FromResult<RaceData>(null!));

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

            _validatorMock.ValidateAsync(dto, token).Returns(Task.FromResult(new FluentValidation.Results.ValidationResult()));

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
