using NSubstitute;
using UDBFRaceFlow.Application.Interfaces.ServiceContracts.Race.Create;
using UDBFRaceFlow.Application.Services.Race;
using UDBFRaceFlow.Domain.Enums;
using Xunit;

namespace UDBFRaceFlow.XUnitTest.ServicesTest.Race.Create
{
    public class GridSystemBuilderTests
    {
        private readonly ISystemGenerator _matchingGeneratorMock;
        private readonly ISystemGenerator _nonMatchingGeneratorMock;
        private readonly GridSystemBuilder _sut;

        public GridSystemBuilderTests()
        {
            _matchingGeneratorMock = Substitute.For<ISystemGenerator>();
            _nonMatchingGeneratorMock = Substitute.For<ISystemGenerator>();

            _matchingGeneratorMock.ApplyParametrs(1, RaceSystem.Long).Returns(true);
            _nonMatchingGeneratorMock.ApplyParametrs(1, RaceSystem.Long).Returns(false);
        }

        [Fact]
        public void GetGenerator_WhenMatchingGeneratorExists_ReturnsCorrectGenerator()
        {
            // Arrange
            var generators = new List<ISystemGenerator> { _nonMatchingGeneratorMock, _matchingGeneratorMock };
            var builder = new GridSystemBuilder(generators);

            // Act
            var result = builder.GetGenerator(RaceSystem.Long, 1);

            // Assert
            Assert.NotNull(result);
            Assert.Same(_matchingGeneratorMock, result);
        }

        [Fact]
        public void GetGenerator_WhenGeneratorNotFound_ThrowsArgumentException()
        {
            // Arrange
            var generators = new List<ISystemGenerator> { _nonMatchingGeneratorMock };
            var builder = new GridSystemBuilder(generators);

            // Act
            var exception = Assert.Throws<ArgumentException>(() =>
                builder.GetGenerator(RaceSystem.Long, 1)
            );

            //Assert

            Assert.Equal("System not found", exception.Message);
        }
    }
}
