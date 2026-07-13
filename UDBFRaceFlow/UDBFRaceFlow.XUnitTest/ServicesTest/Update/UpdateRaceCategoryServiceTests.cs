using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using UDBFRaceFlow.Application.Dto.Request.Update;
using UDBFRaceFlow.Application.Interfaces.RepositoryContracts;
using UDBFRaceFlow.Application.Services.Race.Update;
using UDBFRaceFlow.Domain.Entities.Race;
using UDBFRaceFlow.Domain.Enums;
using Xunit;

namespace UDBFRaceFlow.XUnitTest.ServicesTest.Update
{
    public class UpdateRaceCategoryServiceTests
    {
        private readonly IRaceCategoryRepository _raceCategoryRepositoryMock;
        private readonly ILogger<UpdateRaceCategoryService> _loggerMock;
        private readonly UpdateRaceCategoryService _sut;

        public UpdateRaceCategoryServiceTests()
        {
            _raceCategoryRepositoryMock = Substitute.For<IRaceCategoryRepository>();
            _loggerMock = Substitute.For<ILogger<UpdateRaceCategoryService>>();

            _sut = new UpdateRaceCategoryService(_raceCategoryRepositoryMock, _loggerMock);
        }

        [Fact]
        public async Task UpdateCategoryDetails_ShouldReturnFail_WhenCategoryDoesNotExist()
        {
            // Arrange
            var token = TestContext.Current.CancellationToken;
            var categoryId = Guid.NewGuid();
            var dto = new UpdateCategoryDetailsDto(categoryId, RaceAge.Senior40, 2000, BoatSize.D10, GenderCategory.Open);

            _raceCategoryRepositoryMock
                .GetByIdAsync(categoryId, token)
                .Returns(Task.FromResult<RaceCategory>(null));

            // Act
            var result = await _sut.UpdateCategoryDetails(dto, token);

            // Assert
            result.IsFailed.Should().BeTrue();

            await _raceCategoryRepositoryMock.DidNotReceiveWithAnyArgs().IsCategoryUnique(Arg.Any<RaceCategory>(), token);
            await _raceCategoryRepositoryMock.DidNotReceive().SaveChangesAsync(token);
        }

        [Fact]
        public async Task UpdateCategoryDetails_ShouldReturnFail_WhenCategoryIsNotUniqueAfterMapping()
        {
            // Arrange
            var token = TestContext.Current.CancellationToken;
            var categoryId = Guid.NewGuid();

            var existingCategory = new RaceCategory
            {
                Id = categoryId,
                RaceAge = RaceAge.U24,
                GenderCategory = GenderCategory.Mix,
                BoatSize = BoatSize.D22,
                Distance = 500
            };

            var dto = new UpdateCategoryDetailsDto(categoryId, RaceAge.Premier, 1000, BoatSize.D10, GenderCategory.Open);

            _raceCategoryRepositoryMock
                .GetByIdAsync(categoryId, token)
                .Returns(Task.FromResult(existingCategory));

            _raceCategoryRepositoryMock
                .IsCategoryUnique(existingCategory, token)
                .Returns(Task.FromResult(true));

            // Act
            var result = await _sut.UpdateCategoryDetails(dto, token);

            // Assert
            result.IsFailed.Should().BeTrue();

            existingCategory.CategoryName.Should().Be("Premier Open D10 1000m");

            await _raceCategoryRepositoryMock.DidNotReceive().SaveChangesAsync(token);
        }

        [Fact]
        public async Task UpdateCategoryDetails_ShouldUpdateDetailsAndSave_WhenParametersAreValidAndUnique()
        {
            // Arrange
            var token = TestContext.Current.CancellationToken;
            var categoryId = Guid.NewGuid();
            var existingCategory = new RaceCategory
            {
                Id = categoryId,
                RaceAge = RaceAge.U24,
                GenderCategory = GenderCategory.Mix,
                BoatSize = BoatSize.D22,
                Distance = 500
            };

            var dto = new UpdateCategoryDetailsDto(categoryId, RaceAge.Premier, 2000, BoatSize.D10, GenderCategory.Open);

            _raceCategoryRepositoryMock
                .GetByIdAsync(categoryId, token)
                .Returns(Task.FromResult(existingCategory));

            _raceCategoryRepositoryMock
                .IsCategoryUnique(existingCategory, token)
                .Returns(Task.FromResult(false));

            // Act
            var result = await _sut.UpdateCategoryDetails(dto, token);

            // Assert
            result.IsSuccess.Should().BeTrue();

            existingCategory.Distance.Should().Be(2000);
            existingCategory.RaceAge.Should().Be(RaceAge.Premier);
            existingCategory.CategoryName.Should().Be("Premier Open D10 2000m");

            await _raceCategoryRepositoryMock.Received(1).SaveChangesAsync(token);
        }
    }
}
