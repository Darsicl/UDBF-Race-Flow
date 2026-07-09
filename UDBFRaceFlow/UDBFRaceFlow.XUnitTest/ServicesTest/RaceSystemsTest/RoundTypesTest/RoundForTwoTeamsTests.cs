using FluentAssertions;
using UDBFRaceFlow.Application.Services.RaceSystems.SystemRound.RoundTypes;
using UDBFRaceFlow.Domain.Entities.Race;
using UDBFRaceFlow.Domain.Enums;
using Xunit;

namespace UDBFRaceFlow.XUnitTest.ServicesTest.RaceSystemsTest.RoundTypesTest
{
    public class RoundForTwoTeamsTests
    {
        private readonly RoundForTwoTeams _sut;

        public RoundForTwoTeamsTests()
        {
            _sut = new RoundForTwoTeams();
        }

        [Theory]
        [InlineData(2, true)]
        [InlineData(1, false)]
        [InlineData(3, false)]
        [InlineData(0, false)]
        public void ApplyParametrs_ShouldReturnExpectedResult_DependingOnCountOfTeams(int countOfTeams, bool expected)
        {
            // Act
            var result = _sut.ApplyParametrs(countOfTeams);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void CreateRestRound_WhenDataIsValid_ShouldPopulateSemifinalAndFinalLanesCorrectly()
        {
            // Arrange
            var category = new RaceCategory { Id = Guid.NewGuid() };

            var teamOneId = Guid.NewGuid();
            var teamTwoId = Guid.NewGuid();

            var heatRace = new RaceData { Id = Guid.NewGuid(), RaceType = RaceType.Heat };
            heatRace.Lanes.Add(new LaneData { StartLane = 1, TeamId = teamOneId, RaceId = heatRace.Id });
            heatRace.Lanes.Add(new LaneData { StartLane = 2, TeamId = teamTwoId, RaceId = heatRace.Id });

            var semiRace = new RaceData { Id = Guid.NewGuid(), RaceType = RaceType.Semifinal };
            var finalRace = new RaceData { Id = Guid.NewGuid(), RaceType = RaceType.Final };

            category.Races.AddRange(new[] { heatRace, semiRace, finalRace });

            // Act
            var result = _sut.CreateRestRound(category);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);

            var actualSemi = result.Single(r => r.RaceType == RaceType.Semifinal);
            actualSemi.Lanes.Should().HaveCount(2);

            var semiLane1 = actualSemi.Lanes.Single(l => l.StartLane == 1);
            semiLane1.TeamId.Should().Be(teamTwoId);
            semiLane1.RaceId.Should().Be(semiRace.Id);

            var semiLane2 = actualSemi.Lanes.Single(l => l.StartLane == 2);
            semiLane2.TeamId.Should().Be(teamOneId);
            semiLane2.RaceId.Should().Be(semiRace.Id);

            var actualFinal = result.Single(r => r.RaceType == RaceType.Final);
            actualFinal.Lanes.Should().HaveCount(2);

            var finalLane1 = actualFinal.Lanes.Single(l => l.StartLane == 1);
            finalLane1.TeamId.Should().Be(teamOneId);
            finalLane1.RaceId.Should().Be(finalRace.Id);

            var finalLane2 = actualFinal.Lanes.Single(l => l.StartLane == 2);
            finalLane2.TeamId.Should().Be(teamTwoId);
            finalLane2.RaceId.Should().Be(finalRace.Id);
        }

        [Fact]
        public void CreateRestRound_WhenSemifinalIsMissing_ShouldThrowArgumentNullException()
        {
            // Arrange
            var category = new RaceCategory { Id = Guid.NewGuid() };
            var heatRace = new RaceData { Id = Guid.NewGuid(), RaceType = RaceType.Heat };
            var finalRace = new RaceData { Id = Guid.NewGuid(), RaceType = RaceType.Final };

            category.Races.AddRange(new[] { heatRace, finalRace }); // Нет полуфинала

            // Act
            Action act = () => _sut.CreateRestRound(category);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateRestRound_WhenFinalIsMissing_ShouldThrowArgumentNullException()
        {
            // Arrange
            var category = new RaceCategory { Id = Guid.NewGuid() };
            var heatRace = new RaceData { Id = Guid.NewGuid(), RaceType = RaceType.Heat };
            var semiRace = new RaceData { Id = Guid.NewGuid(), RaceType = RaceType.Semifinal };

            category.Races.AddRange(new[] { heatRace, semiRace }); // Нет финала

            // Act
            Action act = () => _sut.CreateRestRound(category);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateRestRound_WhenTeamOneIsMissingInHeat_ShouldThrowArgumentNullException()
        {
            // Arrange
            var category = new RaceCategory { Id = Guid.NewGuid() };

            var heatRace = new RaceData { Id = Guid.NewGuid(), RaceType = RaceType.Heat };
            heatRace.Lanes.Add(new LaneData { StartLane = 2, TeamId = Guid.NewGuid(), RaceId = heatRace.Id });

            var semiRace = new RaceData { Id = Guid.NewGuid(), RaceType = RaceType.Semifinal };
            var finalRace = new RaceData { Id = Guid.NewGuid(), RaceType = RaceType.Final };

            category.Races.AddRange(new[] { heatRace, semiRace, finalRace });

            // Act
            Action act = () => _sut.CreateRestRound(category);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateRestRound_WhenTeamTwoIsMissingInHeat_ShouldThrowArgumentNullException()
        {
            // Arrange
            var category = new RaceCategory { Id = Guid.NewGuid() };

            var heatRace = new RaceData { Id = Guid.NewGuid(), RaceType = RaceType.Heat };
            heatRace.Lanes.Add(new LaneData { StartLane = 1, TeamId = Guid.NewGuid(), RaceId = heatRace.Id });

            var semiRace = new RaceData { Id = Guid.NewGuid(), RaceType = RaceType.Semifinal };
            var finalRace = new RaceData { Id = Guid.NewGuid(), RaceType = RaceType.Final };

            category.Races.AddRange(new[] { heatRace, semiRace, finalRace });

            // Act
            Action act = () => _sut.CreateRestRound(category);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }
    }
}
