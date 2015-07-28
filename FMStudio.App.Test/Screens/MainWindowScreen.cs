using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestStack.White;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.WindowItems;

namespace FMStudio.App.Test.Screens
{
    public class MainWindowScreen : IDisposable
    {
        private Application _app;
        
        public MainWindowScreen()
        {
            var path = Path.GetFullPath("../../FMStudio.App/bin/FMStudio.exe");
            _app = Application.Launch(path);
        }

        public void Dispose()
        {
            _app.Dispose();
        }

        public Window GetWindow()
        {
            return _app.GetWindows().First();
        }

        public ProjectTreeScreen ProjectTreeScreen()
        {
            return new ProjectTreeScreen(_app);
        }

        public MainWindowScreen ClickAddProject()
        {
            var btnAddProject = GetWindow().Get(SearchCriteria.ByAutomationId("btnAddProject"));
            btnAddProject.Click();

            return this;
        }
    }
}
