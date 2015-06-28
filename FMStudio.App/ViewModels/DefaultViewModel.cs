using FMStudio.App.Utility;
using FMStudio.Utility;
using System.Reflection;

namespace FMStudio.App.ViewModels
{
    public class DefaultViewModel : BaseViewModel
    {
        public Binding<string> VersionString { get; set; }

        public DefaultViewModel()
        {
            var buildDate = Build.GetBuildDate(Assembly.GetCallingAssembly());
            var versionString = string.Format("Build {0}", buildDate.ToString("dd.MM.yyyy"));
            VersionString = new Binding<string>(versionString);
        }
    }
}