using KeyEngine_Workshop.Core;
using KeyEngine_Workshop.Extensions;
using System.Text.Json;

namespace KeyEngine_Workshop.Projects
{
    public static class ProjectManager
    {
        public const string PROJECTS_LIST_FILE_NAME = "projects.json";
        public const string PROJECT_TEMPLATES_FOLDER_NAME = "ProjectTemplates";

        public readonly static string ProjectTemplatesFolderPath = Path.Combine(Environment.CurrentDirectory, PROJECT_TEMPLATES_FOLDER_NAME);
        private readonly static List<ProjectTemplate> projectTemplates = [];

        private static List<Project> loadedProjects = [];
        public static IEnumerable<Project> LoadedProjects => loadedProjects;

        private static readonly JsonSerializerOptions jsonSerializerOptions;

        static ProjectManager()
        {
            jsonSerializerOptions = new()
            {
                WriteIndented = true,
                IncludeFields = true
            };
        }

        public static void CreateProject(ProjectTemplate template, string name, string destination)
        {
            string projectPath = Path.Combine(destination, name);

            Directory.CloneDirectory(template.Path, projectPath);
            loadedProjects.Add(new Project(projectPath));
        }

        public static void SaveProjectList()
        {
            File.WriteAllText(PROJECTS_LIST_FILE_NAME, JsonSerializer.Serialize(loadedProjects, jsonSerializerOptions));
        }

        public static void LoadProjectList()
        {
            if (File.Exists(PROJECTS_LIST_FILE_NAME))
            {
                List<Project>? jsonProjects = JsonSerializer.Deserialize<List<Project>>(File.ReadAllText(PROJECTS_LIST_FILE_NAME));

                if (jsonProjects != null)
                    loadedProjects = jsonProjects;
                else
                    Log.Print($"Failed to parse {PROJECTS_LIST_FILE_NAME}.", LogType.Error);
            }
            else
            {
                Log.Print($"{PROJECTS_LIST_FILE_NAME} does not exists.", LogType.Warning);
            }
        }
    }
}
