using Mapster;
using Microsoft.Extensions.Logging;
using NSubstitute;
using UDBFRaceFlow.Application.Dto.Request.Race.Create;
using UDBFRaceFlow.Application.Interfaces.RepositoryContracts;
using UDBFRaceFlow.Application.Interfaces.ServiceContracts.Race.Create;
using UDBFRaceFlow.Application.Mapping;
using UDBFRaceFlow.Application.Services.Race.Create.RaceSystems;
using UDBFRaceFlow.Application.Services.Race.Create.RaceSystems.SystemRound;
using UDBFRaceFlow.Domain.Entities.Race;
using UDBFRaceFlow.Domain.Enums;
using Xunit;

namespace UDBFRaceFlow.XUnitTest.ServicesTest.Race.Create.RaceSystemsTest
{
    public class SystemRoundGeneratorTests
    {
        private readonly IRaceCategoryRepository _raceCategoryRepoMock;
        private readonly IRaceDataRepository _raceRepoMock;
        private readonly ILogger<SystemRoundGenerator> _loggerMock;
        private readonly IRoundGenerator _roundGeneratorMock;
        private readonly SystemRoundDtoValidator _validator;
        private readonly SystemRoundGenerator _sut;

        public SystemRoundGeneratorTests()
        {
            _raceCategoryRepoMock = Substitute.For<IRaceCategoryRepository>();
            _raceRepoMock = Substitute.For<IRaceDataRepository>();
            _loggerMock = Substitute.For<ILogger<SystemRoundGenerator>>();
            _roundGeneratorMock = Substitute.For<IRoundGenerator>();

            var baseRaceValidator = new BaseRaceValidator(_raceRepoMock);
            _validator = new SystemRoundDtoValidator(baseRaceValidator);

            var roundGenerators = new List<IRoundGenerator> { _roundGeneratorMock };

            _sut = new SystemRoundGenerator(
                _raceCategoryRepoMock,
                _loggerMock,
                roundGenerators,
                _raceRepoMock,
                _validator
            );
        }

        [Fact]
        public void ApplyParametrs_WhenValidArgs_ReturnsTrue()
        {
            // Act
            var result = _sut.ApplyParametrs(1, RaceSystem.Round);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ApplyParametrs_WhenInvalidArgs_ReturnsFalse()
        {
            // Act
            var result = _sut.ApplyParametrs(2, RaceSystem.Long);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task BuildGridAsync_WhenValidationFails_ReturnsFail()
        {
            // Arrange
            var dto = new CreateCategoryDto(
                RaceSystem: RaceSystem.Round,
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
        public async Task BuildGridAsync_WhenRoundGeneratorNotFound_ReturnsFail()
        {
            // Arrange
            var raceDto = new CreateRaceDto(
                1,
                DateTime.UtcNow,
                1,
                RaceType.Heat,
                new List<CreateLaneDto>
                {
                    new CreateLaneDto(Guid.NewGuid(), 1)
                }
            );

            var dto = new CreateCategoryDto(RaceSystem.Round, 1, RaceAge.Premier, 200, BoatSize.D12, GenderCategory.Mix, new() { raceDto });
            var token = TestContext.Current.CancellationToken;

            _roundGeneratorMock.ApplyParametrs(1).Returns(false);

            _raceRepoMock.GetAllRacesAsync(token).Returns(new List<RaceData>());

            // Act
            var result = await _sut.BuildGridAsync(dto, token);

            // Assert
            Assert.True(result.IsFailed);
            _roundGeneratorMock.DidNotReceiveWithAnyArgs().CreateRestRound(Arg.Any<RaceCategory>());
            await _raceCategoryRepoMock.DidNotReceiveWithAnyArgs().AddAsync(Arg.Any<RaceCategory>(), token);
        }

        [Fact]
        public async Task BuildGridAsync_WhenDataIsValidAndGeneratorFound_CallsCreateRestRoundAndSaves()
        {
            // Arrange
            TypeAdapterConfig.GlobalSettings.Scan(typeof(CreateRaceMappingConfig).Assembly);

            var futureRaceTime = DateTime.UtcNow.AddDays(1);

            var heatRace = new CreateRaceDto(
                RaceNumber: 1,
                RaceTime: futureRaceTime,
                SequenceNumber: 1,
                RaceType: RaceType.Heat,
                Lanes: new List<CreateLaneDto>
                {
            new CreateLaneDto(Guid.NewGuid(), 1),
            new CreateLaneDto(Guid.NewGuid(), 2)
                }
            );

            var semiRace = new CreateRaceDto(
                RaceNumber: 2,
                RaceTime: futureRaceTime.AddMinutes(30),
                SequenceNumber: 1,
                RaceType: RaceType.Semifinal,
                Lanes: new List<CreateLaneDto> { }
            );

            var finalRace = new CreateRaceDto(
                RaceNumber: 3,
                RaceTime: futureRaceTime.AddMinutes(60),
                SequenceNumber: 1,
                RaceType: RaceType.Final,
                Lanes: new List<CreateLaneDto> { }
            );

            var dto = new CreateCategoryDto(
                RaceSystem.Round,
                1,
                RaceAge.Premier,
                200,
                BoatSize.D12,
                GenderCategory.Mix,
                new() { heatRace, semiRace, finalRace }
            );

            var token = TestContext.Current.CancellationToken;

            _roundGeneratorMock.ApplyParametrs(2).Returns(true);
            _raceRepoMock.GetAllRacesAsync(token).Returns(new List<RaceData>());

            // Act
            var result = await _sut.BuildGridAsync(dto, token);

            // Assert
            Assert.True(result.IsSuccess, result.Errors.FirstOrDefault()?.Message);

            _roundGeneratorMock.Received(1).CreateRestRound(Arg.Any<RaceCategory>());
            await _raceCategoryRepoMock.Received(1).AddAsync(Arg.Any<RaceCategory>(), token);
            await _raceCategoryRepoMock.Received(1).SaveChangesAsync(token);
        }

        [Fact]
        public async Task BuildSemifinalAsync_Always_ReturnsOk()
        {
            var result = await _sut.BuildSemifinalAsync(Guid.NewGuid(), TestContext.Current.CancellationToken);
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task BuildFinalAsync_Always_ReturnsOk()
        {
            var result = await _sut.BuildFinalAsync(Guid.NewGuid(), TestContext.Current.CancellationToken);
            Assert.True(result.IsSuccess);
        }
    }
}
