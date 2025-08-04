using TestTask_AdsPlatform.Abstractions;
using TestTask_AdsPlatform.Models;
using TestTask_AdsPlatform.Result;

namespace TestTask_AdsPlatform.Services
{
    public class AdsPlatformService : IAdsPlatformService
    {
        private readonly Dictionary<string, List<string>> _platformDictionary = [];
        internal IReadOnlyDictionary<string, List<string>> PlatformDictionary 
            => _platformDictionary.AsReadOnly();

        public Result<IReadOnlyList<string>> GetPlatformsByLocation(string location)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(location) || !location.StartsWith('/'))
                    return Result<IReadOnlyList<string>>.Failure(ErrorResponse.Create("INVALID_LOCATION_FORMAT",
                        "Некорректный формат локации"));

                if (_platformDictionary.Count == 0)
                    return Result<IReadOnlyList<string>>.Failure(ErrorResponse.Create("PLATFORMS_NOT_LOADED"
                            , "Список платформ пуст или не был загружен на эндпоинт [POST /ads-platforms]. Сначала загрузите файл со списком платформ,а затем используйте данный эндпоинт"));

                if (!_platformDictionary.TryGetValue(location, out var platforms))
                    return Result<IReadOnlyList<string>>.Failure(ErrorResponse.Create("LOCATION_NOT_FOUND",
                        "По запрашиваемой локации не работает ни одна платформа"));

                return Result<IReadOnlyList<string>>.Success(platforms.AsReadOnly());
            }
            catch (Exception error)
            {
                return Result<IReadOnlyList<string>>.Failure(ErrorResponse.Create("INVALID_INITIAL_DATA_OR_INVALID_REQUEST",
                        "Были загружены некорректные данные или локация была написана некорректно"
                        , new Dictionary<string, string> { { "innerException", error.Message } }));
            }
        }
        public Result.Result InitializePlatformsDictionary(string content)
        {
            try
            {
                var platforms = ExtractPlatforms(content);
                var enclosureLevelsDict = GroupPlatformsByEnclosureLevels(platforms);

                AddAllPlatformsFromDictionary(enclosureLevelsDict);

                return Result.Result.Success();
            }
            catch (Exception error)
            {
                return Result.Result
                    .Failure(ErrorResponse.Create("ERROR_IN_PLATFORM_PARSING"
                        , "Ошибка при парсинге списка платформ: некорректный формат записи списка платформ"
                        , new Dictionary<string, string> { { "innerException", error.Message } }
                    ));
            }
        }
        private void AddAllPlatformsFromDictionary(Dictionary<int, List<Platform>> enclosureLevelsDict)
        {
            var levels = enclosureLevelsDict.Keys.OrderByDescending(x => x);
            var maxLevel = levels.Max();

            foreach (var level in levels)
            {
                if (level == maxLevel)
                    AddPlatformsOnlyByTheirLocations(enclosureLevelsDict, level);
                else
                    AddPlatformsByTheirLocationsAndByAllEnclosured(enclosureLevelsDict, level);
            }
        }
        private void AddPlatformsByTheirLocationsAndByAllEnclosured
            (Dictionary<int, List<Platform>> enclosureLevelsDict
            , int level)
        {
            foreach (var platform in enclosureLevelsDict[level])
            {
                foreach (var location in platform.LocationsWithEnclosureLevels[level])
                {
                    var enclosureLocations = _platformDictionary.Keys
                        .Select(key => key)
                        .Where(key => key.Contains(location))
                        .ToArray();

                    foreach (var enclosureLocation in enclosureLocations)
                    {
                        _platformDictionary[enclosureLocation].Add(platform.Name);
                    }

                    if (!_platformDictionary.TryGetValue(location, out var _))
                        _platformDictionary[location] = [];

                    _platformDictionary[location].Add(platform.Name);
                }
            }
        }
        private void AddPlatformsOnlyByTheirLocations
            (Dictionary<int, List<Platform>> enclosureLevelsDict
            , int level)
        {
            foreach (var platform in enclosureLevelsDict[level])
            {
                foreach (var location in platform.LocationsWithEnclosureLevels[level])
                {
                    if (!_platformDictionary.TryGetValue(location, out var _))
                        _platformDictionary[location] = [];

                    _platformDictionary[location].Add(platform.Name);
                }
            }
        }

        internal Dictionary<int, List<Platform>> GroupPlatformsByEnclosureLevels(Platform[] platforms)
        {
            var enclosureLevelsDict = new Dictionary<int, List<Platform>>();

            foreach (var platform in platforms)
            {
                ArgumentNullException.ThrowIfNull(platform);

                var uniqueEnclosureLevels = platform.LocationsWithEnclosureLevels.Keys
                    .Distinct()
                    .ToArray();

                foreach (var level in uniqueEnclosureLevels)
                {
                    if (!enclosureLevelsDict.TryGetValue(level, out var _))
                        enclosureLevelsDict[level] = [];

                    enclosureLevelsDict[level].Add(platform);
                }
            }

            return enclosureLevelsDict;
        }
        internal Platform[] ExtractPlatforms(string fileContent)
        {
            var result = fileContent.Split("\r\n")
                .Select(x =>
                {
                    var platformAndLocations = x.Split(':');
                    var locations = platformAndLocations[1].Split(',');

                    var invalidLocations = locations
                        .Where(l => !l.StartsWith('/'))
                        .ToArray();

                    if (invalidLocations.Length > 0) 
                        throw new InvalidOperationException("Некорекктный формат локаций");

                    var locationsWithEnclosureLevels = GetEnclosureLevelsFromLocations(locations);

                    return Platform.Create(platformAndLocations[0], locationsWithEnclosureLevels);
                })
                .ToArray();

            return result;
        }
        internal Dictionary<int, List<string>> GetEnclosureLevelsFromLocations(string[] locations)
        {
            var result = new Dictionary<int, List<string>>();

            foreach (var location in locations)
            {
                if (!location.StartsWith('/') || location.EndsWith('/'))
                    throw new InvalidOperationException($"Некорректный формат локации: {location}");

                var level = location.Split('/').Length - 1;

                if (!result.TryGetValue(level, out var _))
                    result[level] = [];

                if (!result[level].Contains(location))
                    result[level].Add(location);
            }

            return result;
        }
    }
}
