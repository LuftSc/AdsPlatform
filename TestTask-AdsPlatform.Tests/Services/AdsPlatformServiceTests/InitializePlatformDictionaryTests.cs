using TestTask_AdsPlatform.Result;
using TestTask_AdsPlatform.Services;
using TestTask_AdsPlatform.Tests.Extensions;

namespace TestTask_AdsPlatform.Tests.Services.AdsPlatformServiceTests
{
    public class InitializePlatformDictionaryTests
    {
        private readonly AdsPlatformService _service = new();
        [Fact]
        public void InvalidTextContent_ReturnsFailureResult()
        {
            //Arrange
            var invalidInitialData = @"////ru\\\\\svrd";
            //Act
            var result = _service.InitializePlatformsDictionary(invalidInitialData);
            //Assert
            var expected = Result.Result
                    .Failure(ErrorResponse.Create("ERROR_IN_PLATFORM_PARSING"
                        , "Ошибка при парсинге списка платформ: некорректный формат записи списка платформ"));

            Assert.False(result.IsSuccessfull);
            Assert.NotNull(result.Error);

            result.Error.ShouldBeEquivalentTo(expected.Error!);
        }

        [Fact]
        public void ValidTextContent_ReturnsSuccessResult()
        {
            //Arrange
            var validInitialData = TestData.DEFAULT_INITIAL_TEXT;

            //Act
            var result = _service.InitializePlatformsDictionary(validInitialData);

            //Assert
            var expected = Result.Result.Success();

            var expectedDictionary = new Dictionary<string, List<string>>()
            { { "/ru/svrd/revda", ["Ревдинский рабочий", "Крутая реклама", "Яндекс.Директ"]}
            , { "/ru/svrd/pervik", ["Ревдинский рабочий", "Крутая реклама", "Яндекс.Директ"]}
            , { "/ru/msk", ["Газета уральских москвичей", "Яндекс.Директ"]}
            , { "/ru/permobl", ["Газета уральских москвичей", "Яндекс.Директ"]}
            , { "/ru/chelobl", ["Газета уральских москвичей", "Яндекс.Директ"]}
            , { "/ru/svrd", ["Крутая реклама", "Яндекс.Директ"]}
            , { "/ru", ["Яндекс.Директ"]}};

            Assert.True(result.IsSuccessfull);
            Assert.Null(result.Error);

            Assert.Equal(_service.PlatformDictionary, expectedDictionary.AsReadOnly());
        }
    }
}
