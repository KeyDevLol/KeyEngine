using System.Text.Json.Serialization;

namespace KeyEngine_Workshop.Projects
{
    public class Project
    {
        [JsonInclude]
        [JsonPropertyName("path")]
        public string Path;

        public Project(string path)
        {
            Path = path;
        }
    }
}
