using TestTask_AdsPlatform.Models;
using TestTask_AdsPlatform.Services;

namespace TestTask_AdsPlatform.Tests.Services.AdsPlatformServiceTests
{
    public class PrivateMethodTests
    {
        private readonly AdsPlatformService _service = new();
        [Fact]
        public void ExtractPlatforms_ValidInput_ReturnsPlatforms()
        {
            //Act
            var platforms = _service.ExtractPlatforms("Ревдинский рабочий:/ru/svrd/revda,/ru/svrd/pervik");

            //Assert
            var expectedLocations = new[] { "/ru/svrd/revda", "/ru/svrd/pervik" };

            Assert.Equal("Ревдинский рабочий", platforms[0].Name);
            Assert.Equal(expectedLocations, platforms[0].LocationsWithEnclosureLevels[3]);
        }

        [Fact]
        public void ExtractPlatforms_InvalidInput_ThrowsException()
        {
            Assert.Throws<IndexOutOfRangeException>(() =>
                _service.ExtractPlatforms("Яндекс.Директ /ru"));
        }

        [Fact]
        public void ExtractPlatforms_InvalidInputLocationFormat_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() => 
                _service.ExtractPlatforms("Яндекс.Директ:ru"));
        }

        [Fact]
        public void GetEnclosureLevelsFromLocations_ValidLocations_ReturnsLevels()
        {
            //Arragne
            var validLocations = new[] { "/ru", "/ru/msk", "/ru/svrd/pervik" };
            
            //Act
            var resultDict = _service.GetEnclosureLevelsFromLocations(validLocations);

            //Assert
            var expectedDict = new Dictionary<int, List<string>>()
            { {1, ["/ru"]}
            , {2, ["/ru/msk"] }
            , {3, ["/ru/svrd/pervik"] } };

            Assert.Equal(expectedDict, resultDict); 
        }

        [Fact]
        public void GetEnclosureLevelsFromLocations_ValidLocationsWithDuplicates_ReturnsLevels()
        {
            //Arragne
            var validLocationsWithDuplicates = new[] { "/ru", "/ru/msk", "/ru/svrd/pervik", "/ru", "/ru/svrd/pervik", "/ru" };

            //Act
            var resultDict = _service.GetEnclosureLevelsFromLocations(validLocationsWithDuplicates);

            //Assert
            var expectedDict = new Dictionary<int, List<string>>()
            { {1, ["/ru"]}
            , {2, ["/ru/msk"] }
            , {3, ["/ru/svrd/pervik"] } };

            Assert.Equal(expectedDict, resultDict);
        }

        [Fact]
        public void GetEnclosureLevelsFromLocations_InvalidLocations_ThrowsException()
        {
            var invalidLocations = new[] { "ru", "/pervik/", "//svrd//" };

            Assert.Throws<InvalidOperationException>(() =>
                _service.GetEnclosureLevelsFromLocations(invalidLocations));
        }

        [Fact]
        public void GroupPlatformsByEnclosureLevels_ValidInput_ReturnsGroupedPlatformsDict()
        {
            //Arrange
            var platformsWithFirstLevelEnclosure = new[] 
                { Platform.Create("Яндекс.Директ", new Dictionary<int, List<string>>() { { 1, ["/ru"] } }) };
            
            var platformsWithSecondLevelEnclosure = new[] 
                { Platform.Create("Газета уральских москвичей", new Dictionary<int, List<string>>() { {2, ["/ru/msk", "/ru/permobl", "/ru/chelobl"] } })
                , Platform.Create("Крутая реклама", new Dictionary<int, List<string>>() { {2, ["/ru/svrd"] } })};
            
            var platformsWithThirdLevelEnclosure = new[] 
                { Platform.Create("Ревдинский рабочий", new Dictionary<int, List<string>>() { { 3, ["/ru/svrd/revda", "/ru/svrd/pervik"] } }) };

            var allPlatforms = platformsWithFirstLevelEnclosure
                .Union(platformsWithSecondLevelEnclosure)
                .Union(platformsWithThirdLevelEnclosure)
                .ToArray();

            //Act
            var enclosureLevelsDict = _service.GroupPlatformsByEnclosureLevels(allPlatforms);

            //Assert
            var expectedDict = new Dictionary<int, List<Platform>>()
            {
                  { 3, platformsWithThirdLevelEnclosure.ToList() } 
                , { 2, platformsWithSecondLevelEnclosure.ToList() }
                , { 1, platformsWithFirstLevelEnclosure.ToList() }
            };

            Assert.Equal(enclosureLevelsDict, expectedDict);
        }

        [Fact]
        public void GroupPlatformsByEnclosureLevels_InvalidInput_ThrowsException()
        {
            var invalidPlatforms = new Platform[] { null, null, null};

            Assert.Throws<ArgumentNullException>(() =>
                _service.GroupPlatformsByEnclosureLevels(invalidPlatforms));
        }
    }
}
