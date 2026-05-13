using KeyEngine_Workshop.Windowing;
using System.ComponentModel;

namespace KeyEngine_Workshop.Projects
{
    public class ProjectListSaver : OpenTKWindowHandlerBase
    {
        public override void OnWindowClosing(CancelEventArgs e)
        {
            ProjectManager.SaveProjectList();
        }
    }
}
