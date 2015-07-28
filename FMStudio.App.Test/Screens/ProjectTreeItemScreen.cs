using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using TestStack.White;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.WindowItems;

namespace FMStudio.App.Test.Screens
{
    public class ProjectTreeItemScreen
    {
        private Application _app;

        private string _projectName;

        public ProjectTreeItemScreen(Application app, string projectName)
        {
            _app = app;
            _projectName = projectName;
        }

        public Window GetWindow()
        {
            return _app.GetWindows().First();
        }

        public ProjectTreeItemScreen VerifyExists()
        {
            var projects = GetWindow().GetMultiple(SearchCriteria.ByClassName("ProjectViewer"));

            foreach (var p in projects)
            {
                var lblProjectName = p.GetElement(SearchCriteria.ByAutomationId("lblProjectName"));

                if (lblProjectName != null)
                {
                    var currentLabel = lblProjectName.Current;
                    var name = currentLabel.Name;

                    if (name == _projectName) return this;
                }
            }

            Assert.Fail("Could not find project named '" + _projectName + "'");

            return null;
        }
    }
}