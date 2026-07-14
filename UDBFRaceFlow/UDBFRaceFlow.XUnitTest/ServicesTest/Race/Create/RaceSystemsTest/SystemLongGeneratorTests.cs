using Microsoft.Extensions.Logging;
using NSubstitute;
using UDBFRaceFlow.Application.Dto.Request.Race.Create;
using UDBFRaceFlow.Application.Interfaces.RepositoryContracts;
using UDBFRaceFlow.Application.Services.Race.Create.RaceSystems;
using UDBFRaceFlow.Application.Services.Race.Create.RaceSystems.SystemLong;
using UDBFRaceFlow.Domain.Entities.Race;
using UDBFRaceFlow.Domain.Enums;
using Xunit;

namespace UDBFRaceFlow.XUnitTest.ServicesTest.Race.Create.RaceSystemsTest
{
    public class SystemLongGeneratorTests
    {
        private readonly IRaceCategoryRepository _raceCategoryRepoMock;
        private readonly IRaceDataRepository _raceDataRepoMock;
        private readonly ILogger<SystemLongGenerator> _loggerMock;
        private readonly SystemLongDtoValidator _validator;
        private readonly SystemLongGenerator _sut;

        public SystemLongGeneratorTests()
        {
            _raceCategoryRepoMock = Substitute.For<IRaceCategoryRepository>();
            _raceDataRepoMock = Substitute.For<IRaceDataRepository>();
            _loggerMock = Substitute.For<ILogger<SystemLongGenerator>>();

            var baseRaceValidator = new BaseRaceValidator(_raceDataRepoMock);
            _validator = new SystemLongDtoValidator(baseRaceValidator);

            _sut = new SystemLongGenerator(
                _raceCategoryRepoMock,
                _loggerMock,
                _validator,
                _raceDataRepoMock
            );
        }

        [Fact]
        public void ApplyParametrs_WhenValidArgs_ReturnsTrue()
        {
            // Act
            var result = _sut.ApplyParametrs(1, RaceSystem.Long);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ApplyParametrs_WhenInvalidArgs_ReturnsFalse()
        {
            // Act
            var result = _sut.ApplyParametrs(2, RaceSystem.SystemA);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task BuildGridAsync_WhenValidationFails_ReturnsFail()
        {
            // Arrange
            var dto = new CreateCategoryDto(
                RaceSystem: RaceSystem.Long,
                SystemType: 1,
                RaceAge: RaceAge.Premier,
                Distance: 200,
                BoatSize: BoatSize.D12,
                Gender: GenderCategory.Mix,
                Races: new()
            );
            var token = TestContext.Current.CancellationToken;

            // Act
            var result = await _sut.BuildGridAsync(dto, token);

            // Assert
            Assert.True(result.IsFailed);
            await _raceCategoryRepoMock.DidNotReceiveWithAnyArgs().AddAsync(Arg.Any<RaceCategory>(), token);
        }

        [Fact]
        public async Task BuildSemifinalAsync_Always_ReturnsOk()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var token = TestContext.Current.CancellationToken;

            // Act
            var result = await _sut.BuildSemifinalAsync(categoryId, token);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task BuildFinalAsync_Always_ReturnsOk()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var token = TestContext.Current.CancellationToken;

            // Act
            var result = await _sut.BuildFinalAsync(categoryId, token);

            // Assert
            Assert.True(result.IsSuccess);
        }
    }
}
