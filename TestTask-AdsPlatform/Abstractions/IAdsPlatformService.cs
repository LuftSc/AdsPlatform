using TestTask_AdsPlatform.Result;

namespace TestTask_AdsPlatform.Abstractions
{
    public interface IAdsPlatformService
    {
        Result.Result InitializePlatformsDictionary(string content);
        Result<IReadOnlyList<string>> GetPlatformsByLocation(string location);
    }
}