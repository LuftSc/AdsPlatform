namespace TestTask_AdsPlatform.Models
{
    public class Platform
    {
        public string Name { get; set; } = string.Empty;
        public Dictionary<int, List<string>> LocationsWithEnclosureLevels { get; set; } = [];
        private Platform(string name, Dictionary<int, List<string>> locationsWithEnclosureLevels)
        {
            Name = name;
            LocationsWithEnclosureLevels = locationsWithEnclosureLevels;
        }
        public static Platform Create(string name, Dictionary<int, List<string>> locationsWithEnclosureLevels)
        {
            return new Platform(name, locationsWithEnclosureLevels);
        }
    }
}
