using FMStudio.App.Utility;

namespace FMStudio.App.ViewModels
{
    public class PreferencesViewModel : BaseViewModel
    {
        public RootViewModel RootVM { get; set; }

        public Binding<bool> StartMaximized { get; set; }

        public PreferencesViewModel(RootViewModel root)
        {
            RootVM = root;

            StartMaximized = new Binding<bool>(root.Configuration.Preferences.StartMaximized);
        }
    }
}