using FluentAssertions;
using UDBFRaceFlow.Application.Services.RaceSystems.SystemRound.RoundTypes;
using UDBFRaceFlow.Domain.Entities.Race;
using UDBFRaceFlow.Domain.Enums;
using Xunit;

namespace UDBFRaceFlow.XUnitTest.ServicesTest.RaceSystemsTest.RoundTypesTest
{
    public class RoundForFiveTeamsTests
    {
        private readonly RoundForFiveTeams _sut;

        public RoundForFiveTeamsTests()
        {
            _sut = new RoundForFiveTeams();
        }

        [Theory]
        [InlineData(5, true)]
        [InlineData(4, false)]
        [InlineData(6, false)]
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
            var teamThreeId = Guid.NewGuid();
            var teamFourId = Guid.NewGuid();
            var teamFiveId = Guid.NewGuid();

            var heatRace = new RaceData { Id = Guid.NewGuid(), RaceType = RaceType.Heat };
            heatRace.Lanes.Add(new LaneData { StartLane = 1, TeamId = teamOneId, RaceId = heatRace.Id });
            heatRace.Lanes.Add(new LaneData { StartLane = 2, TeamId = teamTwoId, RaceId = heatRace.Id });
            heatRace.Lanes.Add(new LaneData { StartLane = 3, TeamId = teamThreeId, RaceId = heatRace.Id });
            heatRace.Lanes.Add(new LaneData { StartLane = 4, TeamId = teamFourId, RaceId = heatRace.Id });
            heatRace.Lanes.Add(new LaneData { StartLane = 5, TeamId = teamFiveId, RaceId = heatRace.Id });

            var semiRace = new RaceData { Id = Guid.NewGuid(), RaceType = RaceType.Semifinal };
            var finalRace = new RaceData { Id = Guid.NewGuid(), RaceType = RaceType.Final };

            category.Races.AddRange(new[] { heatRace, semiRace, finalRace });

            // Act
            var result = _sut.CreateRestRound(category);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);

            var actualSemi = result.Single(r => r.RaceType == RaceType.Semifinal);
            actualSemi.Lanes.Should().HaveCount(5);

            actualSemi.Lanes.Single(l => l.StartLane == 1).TeamId.Should().Be(teamThreeId);
            actualSemi.Lanes.Single(l => l.StartLane == 2).TeamId.Should().Be(teamFourId);
            actualSemi.Lanes.Single(l => l.StartLane == 3).TeamId.Should().Be(teamOneId);
            actualSemi.Lanes.Single(l => l.StartLane == 4).TeamId.Should().Be(teamFiveId);
            actualSemi.Lanes.Single(l => l.StartLane == 5).TeamId.Should().Be(teamTwoId);

            var actualFinal = result.Single(r => r.RaceType == RaceType.Final);
            actualFinal.Lanes.Should().HaveCount(5);

            actualFinal.Lanes.Single(l => l.StartLane == 1).TeamId.Should().Be(teamFourId);
            actualFinal.Lanes.Single(l => l.StartLane == 2).TeamId.Should().Be(teamThreeId);
            actualFinal.Lanes.Single(l => l.StartLane == 3).TeamId.Should().Be(teamFiveId);
            actualFinal.Lanes.Single(l => l.StartLane == 4).TeamId.Should().Be(teamOneId);
            actualFinal.Lanes.Single(l => l.StartLane == 5).TeamId.Should().Be(teamTwoId);
        }

        [Fact]
        public void CreateRestRound_WhenSemifinalIsMissing_ShouldThrowArgumentNullException()
        {
            // Arrange
            var category = new RaceCategory { Id = Guid.NewGuid() };
            var heatRace = new RaceData { Id = Guid.NewGuid(), RaceType = RaceType.Heat };
            var finalRace = new RaceData { Id = Guid.NewGuid(), RaceType = RaceType.Final };

            category.Races.AddRange(new[] { heatRace, finalRace });

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

            category.Races.AddRange(new[] { heatRace, semiRace });

            // Act
            Action act = () => _sut.CreateRestRound(category);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void CreateRestRound_WhenAnyTeamIsMissingInHeatLanes_ShouldThrowArgumentNullException(int missingLaneNumber)
        {
            // Arrange
            var category = new RaceCategory { Id = Guid.NewGuid() };
            var heatRace = new RaceData { Id = Guid.NewGuid(), RaceType = RaceType.Heat };

            if (missingLaneNumber != 1)
            {
                heatRace.Lanes.Add(new LaneData { StartLane = 1, TeamId = Guid.NewGuid(), RaceId = heatRace.Id });
            }

            if (missingLaneNumber != 2)
            {
                heatRace.Lanes.Add(new LaneData { StartLane = 2, TeamId = Guid.NewGuid(), RaceId = heatRace.Id });
            }

            if (missingLaneNumber != 3)
            {
                heatRace.Lanes.Add(new LaneData { StartLane = 3, TeamId = Guid.NewGuid(), RaceId = heatRace.Id });
            }

            if (missingLaneNumber != 4)
            {
                heatRace.Lanes.Add(new LaneData { StartLane = 4, TeamId = Guid.NewGuid(), RaceId = heatRace.Id });
            }

            if (missingLaneNumber != 5)
            {
                heatRace.Lanes.Add(new LaneData { StartLane = 5, TeamId = Guid.NewGuid(), RaceId = heatRace.Id });
            }

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
