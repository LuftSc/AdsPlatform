using TestTask_AdsPlatform.Result;
using TestTask_AdsPlatform.Services;
using TestTask_AdsPlatform.Tests.Extensions;

namespace TestTask_AdsPlatform.Tests.Services.AdsPlatformServiceTests
{
    public class GetPlatformsByLocationTests
    {
        private readonly AdsPlatformService _service = new();

        [Fact]
        public void NotInitializedDictionary_ReturnsFailureResult()
        {
            //Arrange
            //Act
            var result = _service.GetPlatformsByLocation("/ru");

            //Assert
            var expected = Result<IReadOnlyList<string>>
                .Failure(ErrorResponse.Create("PLATFORMS_NOT_LOADED"
                    , "Список платформ пуст или не был загружен на эндпоинт [POST /ads-platforms]. Сначала загрузите файл со списком платформ,а затем используйте данный эндпоинт"));
            
            Assert.False(result.IsSuccessfull);
            Assert.NotNull(result.Error);

            result.Error.ShouldBeEquivalentTo(expected.Error!);
        }

        [Fact]
        public void InvalidLocation_ReturnsFailureResult()
        {
            //Arrange
            _service.InitializePlatformsDictionary(TestData.DEFAULT_INITIAL_TEXT);

            //Act
            var result = _service.GetPlatformsByLocation("ru/svrd");

            //Assert
            var expected = Result<IReadOnlyList<string>>
                .Failure(ErrorResponse.Create("INVALID_LOCATION_FORMAT",
                        "Некорректный формат локации"));

            Assert.False(result.IsSuccessfull);
            Assert.NotNull(result.Error);
            
            result.Error.ShouldBeEquivalentTo(expected.Error!);
        }

        [Fact]
        public void ValidLocation_ReturnsOnePlatform()
        {
            //Arrange
            _service.InitializePlatformsDictionary(TestData.DEFAULT_INITIAL_TEXT);

            //Act
            var result = _service.GetPlatformsByLocation("/ru");

            //Assert
            Assert.True(result.IsSuccessfull);
            Assert.Single(result.Value);
        }

        [Fact]
        public void ValidLocation_ReturnsPlatforms()
        {
            //Arrange
            _service.InitializePlatformsDictionary(TestData.DEFAULT_INITIAL_TEXT);

            //Act
            var result = _service.GetPlatformsByLocation("/ru/svrd/revda");

            //Assert
            var expected = new string[] { "Ревдинский рабочий", "Крутая реклама", "Яндекс.Директ" };
           
            Assert.True(result.IsSuccessfull);
            Assert.Equal(expected, result.Value);
        }

        [Fact]
        public void UnknownLocation_ReturnsFailureResult()
        {
            //Arrange
            _service.InitializePlatformsDictionary(TestData.DEFAULT_INITIAL_TEXT);

            //Act
            var result = _service.GetPlatformsByLocation("/kz");

            //Assert
            var expected = Result<IReadOnlyList<string>>
                .Failure(ErrorResponse.Create("LOCATION_NOT_FOUND",
                        "По запрашиваемой локации не работает ни одна платформа"));

            Assert.False(result.IsSuccessfull);
            Assert.NotNull(result.Error);

            result.Error.ShouldBeEquivalentTo(expected.Error!);
        }
    }
}
