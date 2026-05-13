using System.Diagnostics;
using System.Text.Json;
using KeyEngine_Workshop.Core;
using KeyEngine_Workshop.Extensions;

namespace KeyEngine_Workshop.Projects
{
    public static class ProjectManager
    {
        public const string PROJECTS_LIST_FILE_NAME = "projects.json";
        public const string PROJECT_TEMPLATES_FOLDER_NAME = "ProjectTemplates";
        public const string PROJECTS_FOLDER_NAME = "CreatedProjects";
        public const string PROJECT_TEMPLATE_FILE_EXTENSION = "projtemplate";

        public readonly static string ProjectTemplatesFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PROJECT_TEMPLATES_FOLDER_NAME);
        public readonly static string ProjectsFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PROJECTS_FOLDER_NAME);

        private static List<Project> loadedProjects = [];
        private readonly static List<ProjectTemplateInfo> projectTemplates = [];
        public static IEnumerable<Project> LoadedProjects => loadedProjects;
        public static IEnumerable<ProjectTemplateInfo> LoadedProjectTemplates => projectTemplates;

        private static readonly JsonSerializerOptions jsonSerializerOptions;

        static ProjectManager()
        {
            jsonSerializerOptions = new()
            {
                WriteIndented = true,
                IncludeFields = true
            };

            if (!Directory.Exists(ProjectsFolderPath))
                Directory.CreateDirectory(ProjectsFolderPath);
        }

        public static void Initialize()
        {
            LoadProjectList();
            LoadProjectTemplates();
        }

        public static void CreateProject(ProjectTemplateInfo template, string name, string destination)
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
                List<Project>? jsonProjects = JsonSerializer.Deserialize<List<Project>>(File.ReadAllText(PROJECTS_LIST_FILE_NAME)) ?? throw new NullReferenceException($"Failed to parse {PROJECTS_LIST_FILE_NAME}.");

                for (int i = 0; i < jsonProjects.Count; i++)
                {
                    Project project = jsonProjects[i];

                    if (!Directory.Exists(project.Path))
                    {
                        Log.Print($"Project at path: {project.Path}; was not found.", LogType.Warning);
                        jsonProjects.RemoveAt(i);
                    }
                }

                loadedProjects = jsonProjects;
            }
            else
            {
                Log.Print($"{PROJECTS_LIST_FILE_NAME} does not exists.", LogType.Warning);
            }
        }

        public static void LoadProjectTemplates()
        {
            foreach (string file in Directory.GetFiles(PROJECT_TEMPLATES_FOLDER_NAME, $"*.{PROJECT_TEMPLATE_FILE_EXTENSION}", SearchOption.AllDirectories))
            {
                ProjectTemplateInfo templateInfo = JsonSerializer.Deserialize<ProjectTemplateInfo>(File.ReadAllText(file), jsonSerializerOptions);

                if (templateInfo.DisplayName == null || templateInfo.Description == null)
                    throw new NullReferenceException($"Failed to parse project template json. Template path: {file}");

                templateInfo.Path = Path.GetDirectoryName(file) ?? throw new DirectoryNotFoundException("Path.GetDirectoryName returned null.");
                projectTemplates.Add(templateInfo);

            }
        }
    }
}
