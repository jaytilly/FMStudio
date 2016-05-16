using FMStudio.App.Utility;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace FMStudio.App.ViewModels
{
    public class PreferencesViewModel : BaseViewModel
    {
        public RootViewModel RootVM { get; set; }

        public Binding<bool> StartMaximized { get; set; }

        public ObservableCollection<ThemeViewModel> Themes { get; set; }

        public Binding<ThemeViewModel> Theme { get; set; }

        public PreferencesViewModel(RootViewModel root)
        {
            RootVM = root;

            StartMaximized = new Binding<bool>(root.Configuration.Preferences.StartMaximized);
            StartMaximized.PropertyChanged += (s, e) =>
            {
                root.Configuration.Preferences.StartMaximized = true;
                root.SaveConfiguration();
            };

            Themes = new ObservableCollection<ThemeViewModel>(ThemeViewModel.GetThemesList());

            var currentThemeName = root.Configuration.Preferences.Theme;
            var theme = Themes.FirstOrDefault(t => t.Name == currentThemeName);
            if (theme == null)
                theme = Themes[0];

            Theme = new Binding<ThemeViewModel>(theme);

            Theme.PropertyChanged += (s, e) =>
            {
                MainWindow.Instance.SetTheme(Theme.Value);

                var prefs = root.Configuration.Preferences;
                prefs.Theme = Theme.Value.Name;

                root.SaveConfiguration();
            };
        }
    }
}