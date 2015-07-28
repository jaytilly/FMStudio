using TestStack.White;

namespace FMStudio.App.Test.Screens
{
    public class ProjectTreeScreen
    {
        private Application _app;

        public ProjectTreeScreen(Application app)
        {
            _app = app;
        }

        public ProjectTreeItemScreen WithProject(string projectName)
        {
            return new ProjectTreeItemScreen(_app, projectName);
        }
    }
}