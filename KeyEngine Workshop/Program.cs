using KeyEngine_Workshop.GUI;
using KeyEngine_Workshop.Hub;
using KeyEngine_Workshop.Projects;
using KeyEngine_Workshop.Windowing;

namespace KeyEngine_Workshop
{
    internal class Program
    {
        private static HubWindow mainGui = null!;

        private static void Main()
        {
            ApplicationWindow.Initialize();
            ApplicationWindow.Instance.RegisterHandler(WindowManager.GetInstance());
            ApplicationWindow.Instance.RegisterHandler(new ProjectListSaver());
            ProjectManager.Initialize();
            HubWindow.GetInstance();
            ApplicationWindow.Run();
        }
    }
}
