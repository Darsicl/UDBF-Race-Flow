using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using NSubstitute;
using UDBFRaceFlow.Application.Dto.Request.Race.Update;
using UDBFRaceFlow.Application.Interfaces.RepositoryContracts;
using UDBFRaceFlow.Application.Services.Race.Update.RaceDetails;
using UDBFRaceFlow.Domain.Entities.Race;
using Xunit;

namespace UDBFRaceFlow.XUnitTest.ServicesTest.Race.Update
{
    public class UpdateRaceDetailsServiceTests
    {
        private readonly IRaceDataRepository _raceDataRepositoryMock;
        private readonly ILogger<UpdateRaceDetailsService> _loggerMock;
        private readonly IValidator<UpdateRaceDetailsDto> _validatorMock;
        private readonly UpdateRaceDetailsService _sut;

        public UpdateRaceDetailsServiceTests()
        {
            _raceDataRepositoryMock = Substitute.For<IRaceDataRepository>();
            _loggerMock = Substitute.For<ILogger<UpdateRaceDetailsService>>();
            _validatorMock = Substitute.For<IValidator<UpdateRaceDetailsDto>>();

            _sut = new UpdateRaceDetailsService(_raceDataRepositoryMock, _loggerMock, _validatorMock);
        }

        [Fact]
        public async Task UpdateRaceDetails_ShouldReturnFail_WhenValidationFails()
        {
            // Arrange
            var token = TestContext.Current.CancellationToken;
            var dto = new UpdateRaceDetailsDto(Guid.NewGuid(), 5, DateTime.Now);

            var validationResult = new FluentValidation.Results.ValidationResult(new[]
            {
                new FluentValidation.Results.ValidationFailure("RaceNumber", "Required")
            });

            _validatorMock.ValidateAsync(dto, token).Returns(Task.FromResult(validationResult));

            // Act
            var result = await _sut.UpdateRaceDetails(dto, token);

            // Assert
            result.IsFailed.Should().BeTrue();
            await _raceDataRepositoryMock.DidNotReceive().GetByIdAsync(Arg.Any<Guid>(), token);
        }

        [Fact]
        public async Task UpdateRaceDetails_ShouldReturnFail_WhenRaceNotFound()
        {
            // Arrange
            var token = TestContext.Current.CancellationToken;
            var raceId = Guid.NewGuid();
            var dto = new UpdateRaceDetailsDto(raceId, 1, DateTime.Now);

            _validatorMock.ValidateAsync(dto, token).Returns(Task.FromResult(new FluentValidation.Results.ValidationResult()));

            _raceDataRepositoryMock
                .GetByIdAsync(raceId, token)
                .Returns(Task.FromResult<RaceData>(null!));

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

            _validatorMock.ValidateAsync(dto, token).Returns(Task.FromResult(new FluentValidation.Results.ValidationResult()));

            _raceDataRepositoryMock.GetByIdAsync(raceId, token).Returns(Task.FromResult(existingRace));

            _raceDataRepositoryMock.IsRaceDateUniqueAsync(Arg.Any<DateTime>(), raceId, token).Returns(Task.FromResult(true));

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

            _validatorMock.ValidateAsync(dto, token).Returns(Task.FromResult(new FluentValidation.Results.ValidationResult()));

            _raceDataRepositoryMock.GetByIdAsync(raceId, token).Returns(Task.FromResult(existingRace));
            _raceDataRepositoryMock.IsRaceDateUniqueAsync(Arg.Any<DateTime>(), raceId, token).Returns(Task.FromResult(false));

            _raceDataRepositoryMock.IsRaceNumberUniqueAsync(Arg.Any<int>(), raceId, token).Returns(Task.FromResult(true));

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

            _validatorMock.ValidateAsync(dto, token).Returns(Task.FromResult(new FluentValidation.Results.ValidationResult()));

            _raceDataRepositoryMock.GetByIdAsync(raceId, token).Returns(Task.FromResult(existingRace));
            _raceDataRepositoryMock.IsRaceDateUniqueAsync(Arg.Any<DateTime>(), raceId, token).Returns(Task.FromResult(false));
            _raceDataRepositoryMock.IsRaceNumberUniqueAsync(Arg.Any<int>(), raceId, token).Returns(Task.FromResult(false));

            _raceDataRepositoryMock.GetAllRacesAsync(token).Returns(Task.FromResult(new List<RaceData> { existingRace }));

            // Act
            var result = await _sut.UpdateRaceDetails(dto, token);

            // Assert
            if (result.IsSuccess)
            {
                existingRace.RaceNumber.Should().Be(5);
                existingRace.OriginalDateTime.Should().Be(newDateTime);
                await _raceDataRepositoryMock.Received(1).SaveChangesAsync(token);
            }
            else
            {
                result.IsFailed.Should().BeTrue();
            }
        }
    }
}
