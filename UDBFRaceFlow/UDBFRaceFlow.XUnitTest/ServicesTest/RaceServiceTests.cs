using AutoFixture;
using FluentResults;
using Microsoft.Extensions.Logging;
using NSubstitute;
using UDBFRaceFlow.Application.Interfaces.RepositoryContracts;
using UDBFRaceFlow.Application.Interfaces.ServiceContracts;
using UDBFRaceFlow.Application.Services;
using UDBFRaceFlow.Domain.Entities.Race;
using UDBFRaceFlow.Domain.Enums;
using Xunit;

namespace UDBFRaceFlow.XUnitTest.Services
{
    public class RaceServiceTests
    {
        private readonly IRaceCategoryRepository _raceRepoMock;
        private readonly ISystemGenerator _generatorMock;
        private readonly ILogger<RaceService> _loggerMock;
        private readonly Fixture _fixture;
        private readonly RaceService _sut; // SUT = System Under Test (Тестируемый объект)

        public RaceServiceTests()
        {
            _raceRepoMock = Substitute.For<IRaceCategoryRepository>();
            _generatorMock = Substitute.For<ISystemGenerator>();
            _loggerMock = Substitute.For<ILogger<RaceService>>();

            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var generators = new List<ISystemGenerator> { _generatorMock };
            _sut = new RaceService(_raceRepoMock, generators, _loggerMock);
        }

        [Fact]
        public async Task CheckFinishOfHeats_WhenCategoryDoesNotExist_ReturnsFail()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            _raceRepoMock.GetCategoryWithRacesAndLanesAsync(categoryId, Arg.Any<CancellationToken>())
                .Returns((RaceCategory)null);

            // Act
            var result = await _sut.CheckFinishOfHeats(categoryId, TestContext.Current.CancellationToken);

            // Assert
            Assert.True(result.IsFailed);
            await _generatorMock.DidNotReceiveWithAnyArgs().BuildSemifinalAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task CheckFinishOfHeats_WhenNotAllHeatsAreFinished_ReturnsOkWithoutBuildingSemis()
        {
            // Arrange
            var category = _fixture.Create<RaceCategory>();

            category.Races = new List<RaceData>
            {
                new() { RaceType = RaceType.Heat, RaceStatus = RaceStatus.Finished },
                new() { RaceType = RaceType.Heat, RaceStatus = RaceStatus.InProgress }
            };

            _raceRepoMock.GetCategoryWithRacesAndLanesAsync(category.Id, Arg.Any<CancellationToken>())
                .Returns(category);

            // Act
            var result = await _sut.CheckFinishOfHeats(category.Id, TestContext.Current.CancellationToken);

            // Assert
            Assert.True(result.IsSuccess);
            await _generatorMock.DidNotReceiveWithAnyArgs().BuildSemifinalAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task CheckFinishOfHeats_WhenHeatsFinishedButGeneratorNotFound_ReturnsFail()
        {
            // Arrange
            var category = _fixture.Create<RaceCategory>();
            category.Races = new List<RaceData>
            {
                new() { RaceType = RaceType.Heat, RaceStatus = RaceStatus.Finished }
            };

            _raceRepoMock.GetCategoryWithRacesAndLanesAsync(category.Id, Arg.Any<CancellationToken>())
                .Returns(category);

            _generatorMock.ApplyParametrs(category.Distance, category.RaceSystem).Returns(false);

            // Act
            var result = await _sut.CheckFinishOfHeats(category.Id, TestContext.Current.CancellationToken);

            // Assert
            Assert.True(result.IsFailed);
            await _generatorMock.DidNotReceiveWithAnyArgs().BuildSemifinalAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task CheckFinishOfHeats_WhenGeneratorFailsToBuildSemis_ReturnsFail()
        {
            // Arrange
            var category = _fixture.Create<RaceCategory>();
            category.Races = new List<RaceData>
            {
                new() { RaceType = RaceType.Heat, RaceStatus = RaceStatus.Finished }
            };

            _raceRepoMock.GetCategoryWithRacesAndLanesAsync(category.Id, Arg.Any<CancellationToken>())
                .Returns(category);

            _generatorMock.ApplyParametrs(category.Distance, category.RaceSystem).Returns(true);

            _generatorMock.BuildSemifinalAsync(category.Id, Arg.Any<CancellationToken>())
                .Returns(Result.Fail("Some internal generation error"));

            // Act
            var result = await _sut.CheckFinishOfHeats(category.Id, TestContext.Current.CancellationToken);

            // Assert
            Assert.True(result.IsFailed);
        }

        [Fact]
        public async Task CheckFinishOfHeats_WhenAllHeatsFinishedAndSemisBuildSuccessfully_ReturnsOk()
        {
            // Arrange
            var cancellationToken = new CancellationToken();
            var category = _fixture.Create<RaceCategory>();
            category.Races = new List<RaceData>
            {
                new() { RaceType = RaceType.Heat, RaceStatus = RaceStatus.Finished },
                new() { RaceType = RaceType.Heat, RaceStatus = RaceStatus.Finished }
            };

            _raceRepoMock.GetCategoryWithRacesAndLanesAsync(category.Id, cancellationToken)
                .Returns(category);

            _generatorMock.ApplyParametrs(category.Distance, category.RaceSystem).Returns(true);
            _generatorMock.BuildSemifinalAsync(category.Id, cancellationToken).Returns(Result.Ok());

            // Act
            var result = await _sut.CheckFinishOfHeats(category.Id, cancellationToken);

            // Assert
            Assert.True(result.IsSuccess);
            await _generatorMock.Received(1).BuildSemifinalAsync(category.Id, cancellationToken);
        }

        [Fact]
        public async Task CheckFinishOfSemis_WhenCategoryDoesNotExist_ReturnsFail()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            _raceRepoMock.GetCategoryWithRacesAndLanesAsync(categoryId, Arg.Any<CancellationToken>())
                .Returns((RaceCategory)null);

            // Act
            var result = await _sut.CheckFinishOfSemis(categoryId, TestContext.Current.CancellationToken);

            // Assert
            Assert.True(result.IsFailed);
            await _generatorMock.DidNotReceiveWithAnyArgs().BuildFinalAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task CheckFinishOfSemis_WhenNotAllSemisAreFinished_ReturnsOkWithoutBuildingFinals()
        {
            // Arrange
            var category = _fixture.Create<RaceCategory>();
            category.Races = new List<RaceData>
            {
                new() { RaceType = RaceType.Semifinal, RaceStatus = RaceStatus.Finished },
                new() { RaceType = RaceType.Semifinal, RaceStatus = RaceStatus.Scheduled }
            };

            _raceRepoMock.GetCategoryWithRacesAndLanesAsync(category.Id, Arg.Any<CancellationToken>())
                .Returns(category);

            // Act
            var result = await _sut.CheckFinishOfSemis(category.Id, TestContext.Current.CancellationToken);

            // Assert
            Assert.True(result.IsSuccess);
            await _generatorMock.DidNotReceiveWithAnyArgs().BuildFinalAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task CheckFinishOfSemis_WhenSemisFinishedButGeneratorNotFound_ReturnsFail()
        {
            // Arrange
            var category = _fixture.Create<RaceCategory>();
            category.Races = new List<RaceData>
            {
                new() { RaceType = RaceType.Semifinal, RaceStatus = RaceStatus.Finished }
            };

            _raceRepoMock.GetCategoryWithRacesAndLanesAsync(category.Id, Arg.Any<CancellationToken>())
                .Returns(category);

            _generatorMock.ApplyParametrs(category.Distance, category.RaceSystem).Returns(false);

            // Act
            var result = await _sut.CheckFinishOfSemis(category.Id, TestContext.Current.CancellationToken);

            // Assert
            Assert.True(result.IsFailed);
            await _generatorMock.DidNotReceiveWithAnyArgs().BuildFinalAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task CheckFinishOfSemis_WhenGeneratorFailsToBuildFinals_ReturnsFail()
        {
            // Arrange
            var category = _fixture.Create<RaceCategory>();
            category.Races = new List<RaceData>
            {
                new() { RaceType = RaceType.Semifinal, RaceStatus = RaceStatus.Finished }
            };

            _raceRepoMock.GetCategoryWithRacesAndLanesAsync(category.Id, Arg.Any<CancellationToken>())
                .Returns(category);

            _generatorMock.ApplyParametrs(category.Distance, category.RaceSystem).Returns(true);
            _generatorMock.BuildFinalAsync(category.Id, Arg.Any<CancellationToken>())
                .Returns(Result.Fail("Error building finals"));

            // Act
            var result = await _sut.CheckFinishOfSemis(category.Id, TestContext.Current.CancellationToken);

            // Assert
            Assert.True(result.IsFailed);
        }

        [Fact]
        public async Task CheckFinishOfSemis_WhenAllSemisFinishedAndFinalsBuildSuccessfully_ReturnsOk()
        {
            // Arrange
            var cancellationToken = new CancellationToken();
            var category = _fixture.Create<RaceCategory>();
            category.Races = new List<RaceData>
            {
                new() { RaceType = RaceType.Semifinal, RaceStatus = RaceStatus.Finished }
            };

            _raceRepoMock.GetCategoryWithRacesAndLanesAsync(category.Id, cancellationToken)
                .Returns(category);

            _generatorMock.ApplyParametrs(category.Distance, category.RaceSystem).Returns(true);
            _generatorMock.BuildFinalAsync(category.Id, cancellationToken).Returns(Result.Ok());

            // Act
            var result = await _sut.CheckFinishOfSemis(category.Id, cancellationToken);

            // Assert
            Assert.True(result.IsSuccess);
            await _generatorMock.Received(1).BuildFinalAsync(category.Id, cancellationToken);
        }
    }
}
