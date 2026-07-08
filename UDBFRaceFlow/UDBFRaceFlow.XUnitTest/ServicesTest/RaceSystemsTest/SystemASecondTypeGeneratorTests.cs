using Microsoft.Extensions.Logging;
using NSubstitute;
using UDBFRaceFlow.Application.Dto;
using UDBFRaceFlow.Application.Interfaces.RepositoryContracts;
using UDBFRaceFlow.Application.Services.RaceSystems.SystemA;
using UDBFRaceFlow.Domain.Entities.Race;
using UDBFRaceFlow.Domain.Enums;
using Xunit;

namespace UDBFRaceFlow.XUnitTest.Services.RaceSystems
{
    public class SystemASecondTypeGeneratorTests
    {
        private readonly IRaceCategoryRepository _raceCategoryRepoMock;
        private readonly IRaceDataRepository _raceRepoMock;
        private readonly ILogger<SystemASecondTypeGenerator> _loggerMock;
        private readonly SystemADtoValidator _validatorMock;
        private readonly SystemASecondTypeGenerator _sut;

        public SystemASecondTypeGeneratorTests()
        {
            _raceCategoryRepoMock = Substitute.For<IRaceCategoryRepository>();
            _raceRepoMock = Substitute.For<IRaceDataRepository>();
            _loggerMock = Substitute.For<ILogger<SystemASecondTypeGenerator>>();

            var raceCreationValidatorMock = Substitute.For<FluentValidation.IValidator<RaceCreationDto>>();
            _validatorMock = new SystemADtoValidator(raceCreationValidatorMock);

            _sut = new SystemASecondTypeGenerator(
                _raceCategoryRepoMock,
                _loggerMock,
                _raceRepoMock,
                _validatorMock);
        }

        private RaceCategory CreateBaseCategory(Guid id)
        {
            return new RaceCategory
            {
                Id = id,
                Distance = 1000,
                RaceSystem = RaceSystem.SystemA,
                Races = new List<RaceData>()
            };
        }

        [Fact]
        public void ApplyParametrs_WhenValidArgs_ReturnsTrue()
        {
            var result = _sut.ApplyParametrs(2, RaceSystem.SystemA);
            Assert.True(result);
        }

        [Fact]
        public void ApplyParametrs_WhenInvalidArgs_ReturnsFalse()
        {
            var result = _sut.ApplyParametrs(1, RaceSystem.Long);
            Assert.False(result);
        }

        [Fact]
        public async Task BuildGridAsync_WhenValidationFails_ReturnsFail()
        {
            // Arrange
            var dto = new CreateFullGridDto(
                RaceSystem.SystemA,
                1,
                RaceAge.Premier,
                200,
                BoatSize.D12,
                GenderCategory.Mix,
                new());
            var token = TestContext.Current.CancellationToken;

            // Act
            var result = await _sut.BuildGridAsync(dto, token);

            // Assert
            Assert.True(result.IsFailed);
            await _raceCategoryRepoMock.DidNotReceiveWithAnyArgs().AddAsync(Arg.Any<RaceCategory>(), token);
        }

        [Fact]
        public async Task BuildSemifinalAsync_WhenCategoryNotFound_ReturnsFail()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var token = TestContext.Current.CancellationToken;
            _raceCategoryRepoMock.GetCategoryWithRacesAndLanesAsync(categoryId, token).Returns((RaceCategory?)null);

            // Act
            var result = await _sut.BuildSemifinalAsync(categoryId, token);

            // Assert
            Assert.True(result.IsFailed);
        }

        [Fact]
        public async Task BuildSemifinalAsync_WhenSemiRaceMissingInGrid_ReturnsFail()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var token = TestContext.Current.CancellationToken;
            var category = CreateBaseCategory(categoryId);

            _raceCategoryRepoMock.GetCategoryWithRacesAndLanesAsync(categoryId, token).Returns(category);

            // Act
            var result = await _sut.BuildSemifinalAsync(categoryId, token);

            // Assert
            Assert.True(result.IsFailed);
        }

        [Fact]
        public async Task BuildSemifinalAsync_WhenNotEnoughTeamsFromHeats_ReturnsFail()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var token = TestContext.Current.CancellationToken;
            var category = CreateBaseCategory(categoryId);

            var semiRace = new RaceData { Id = Guid.NewGuid(), RaceType = RaceType.Semifinal, Lanes = new() };
            var heatRace = new RaceData
            {
                RaceType = RaceType.Heat,
                Lanes = new List<LaneData> { new() { TeamId = Guid.NewGuid(), FinishPlace = 2, FinishTime = TimeSpan.FromSeconds(50) } }
            };

            category.Races.Add(semiRace);
            category.Races.Add(heatRace);

            _raceCategoryRepoMock.GetCategoryWithRacesAndLanesAsync(categoryId, token).Returns(category);

            // Act
            var result = await _sut.BuildSemifinalAsync(categoryId, token);

            // Assert
            Assert.True(result.IsFailed);
        }

        [Fact]
        public async Task BuildSemifinalAsync_WhenDataIsValid_BuildsSemifinalAndSaves()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var token = TestContext.Current.CancellationToken;
            var category = CreateBaseCategory(categoryId);

            var semiRace = new RaceData { Id = Guid.NewGuid(), RaceType = RaceType.Semifinal, Lanes = new() };

            var heat1 = new RaceData
            {
                RaceType = RaceType.Heat,
                Lanes = new List<LaneData>
                {
                    new() { TeamId = Guid.NewGuid(), FinishPlace = 2, FinishTime = TimeSpan.FromSeconds(51) },
                    new() { TeamId = Guid.NewGuid(), FinishPlace = 3, FinishTime = TimeSpan.FromSeconds(61) }
                }
            };
            var heat2 = new RaceData
            {
                RaceType = RaceType.Heat,
                Lanes = new List<LaneData>
                {
                    new() { TeamId = Guid.NewGuid(), FinishPlace = 2, FinishTime = TimeSpan.FromSeconds(52) },
                    new() { TeamId = Guid.NewGuid(), FinishPlace = 3, FinishTime = TimeSpan.FromSeconds(62) }
                }
            };

            category.Races.AddRange(new[] { semiRace, heat1, heat2 });
            _raceCategoryRepoMock.GetCategoryWithRacesAndLanesAsync(categoryId, token).Returns(category);

            // Act
            var result = await _sut.BuildSemifinalAsync(categoryId, token);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(4, semiRace.Lanes.Count);
            Assert.Equal(1, semiRace.Lanes.First(l => l.TeamId == heat1.Lanes.First(x => x.FinishPlace == 3).TeamId).StartLane);
            await _raceCategoryRepoMock.Received(1).SaveChangesAsync(token);
        }

        [Fact]
        public async Task BuildFinalAsync_WhenCategoryNotFound_ReturnsFail()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var token = TestContext.Current.CancellationToken;
            _raceCategoryRepoMock.GetCategoryWithRacesAndLanesAsync(categoryId, token).Returns((RaceCategory?)null);

            // Act
            var result = await _sut.BuildFinalAsync(categoryId, token);

            // Assert
            Assert.True(result.IsFailed);
        }

        [Fact]
        public async Task BuildFinalAsync_WhenFinalRaceMissing_ReturnsFail()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var token = TestContext.Current.CancellationToken;
            var category = CreateBaseCategory(categoryId);

            _raceCategoryRepoMock.GetCategoryWithRacesAndLanesAsync(categoryId, token).Returns(category);

            // Act
            var result = await _sut.BuildFinalAsync(categoryId, token);

            // Assert
            Assert.True(result.IsFailed);
        }

        [Fact]
        public async Task BuildFinalAsync_WhenMissingSemisResults_ReturnsFail()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var token = TestContext.Current.CancellationToken;
            var category = CreateBaseCategory(categoryId);

            var finalRace = new RaceData { Id = Guid.NewGuid(), RaceType = RaceType.Final, Lanes = new() };
            var heat1 = new RaceData { RaceType = RaceType.Heat, Lanes = new List<LaneData> { new() { TeamId = Guid.NewGuid(), FinishPlace = 1 } } };
            var heat2 = new RaceData { RaceType = RaceType.Heat, Lanes = new List<LaneData> { new() { TeamId = Guid.NewGuid(), FinishPlace = 1 } } };

            var semiRace = new RaceData { RaceType = RaceType.Semifinal, Lanes = new List<LaneData>() };

            category.Races.AddRange(new[] { finalRace, heat1, heat2, semiRace });
            _raceCategoryRepoMock.GetCategoryWithRacesAndLanesAsync(categoryId, token).Returns(category);

            // Act
            var result = await _sut.BuildFinalAsync(categoryId, token);

            // Assert
            Assert.True(result.IsFailed);
        }

        [Fact]
        public async Task BuildFinalAsync_WhenDataIsValid_BuildsFinalAndSaves()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var token = TestContext.Current.CancellationToken;
            var category = CreateBaseCategory(categoryId);

            var finalRace = new RaceData { Id = Guid.NewGuid(), RaceType = RaceType.Final, Lanes = new() };

            var heat1 = new RaceData { RaceType = RaceType.Heat, Lanes = new List<LaneData> { new() { TeamId = Guid.NewGuid(), FinishPlace = 1, FinishTime = TimeSpan.FromSeconds(40) } } };
            var heat2 = new RaceData { RaceType = RaceType.Heat, Lanes = new List<LaneData> { new() { TeamId = Guid.NewGuid(), FinishPlace = 1, FinishTime = TimeSpan.FromSeconds(41) } } };

            var semiTeam1 = Guid.NewGuid();
            var semiTeam2 = Guid.NewGuid();
            var semiRace = new RaceData
            {
                RaceType = RaceType.Semifinal,
                Lanes = new List<LaneData>
                {
                    new() { TeamId = semiTeam1, FinishPlace = 1 },
                    new() { TeamId = semiTeam2, FinishPlace = 2 }
                }
            };

            category.Races.AddRange(new[] { finalRace, heat1, heat2, semiRace });
            _raceCategoryRepoMock.GetCategoryWithRacesAndLanesAsync(categoryId, token).Returns(category);

            // Act
            var result = await _sut.BuildFinalAsync(categoryId, token);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(4, finalRace.Lanes.Count);

            Assert.Equal(semiTeam1, finalRace.Lanes.First(l => l.StartLane == 1).TeamId);
            Assert.Equal(heat1.Lanes[0].TeamId, finalRace.Lanes.First(l => l.StartLane == 2).TeamId);

            await _raceCategoryRepoMock.Received(1).SaveChangesAsync(token);
        }
    }
}