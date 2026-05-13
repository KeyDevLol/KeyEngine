using System.Text.Json.Serialization;

namespace KeyEngine_Workshop.Projects
{
    public struct ProjectTemplateInfo
    {
        public string DisplayName { get; set; }
        public string Description { get; set; }
        [JsonIgnore]
        public string Path { get; set; }

        public ProjectTemplateInfo(string displayName, string description, string path)
        {
            DisplayName = displayName;
            Description = description;
            Path = path;
        }
    }
}
