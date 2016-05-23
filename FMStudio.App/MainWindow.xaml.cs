using FMStudio.App.Services;
using FMStudio.App.ViewModels;
using FMStudio.Configuration;
using MahApps.Metro.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FMStudio.App
{
    public partial class MainWindow : MetroWindow
    {
        public static MainWindow Instance { get; private set; }

        public RootViewModel Root { get; private set; }

        public MainWindow(string pathToConfigFile)
        {
            Instance = this;

            FMConfiguration config = null;

            if (!string.IsNullOrEmpty(pathToConfigFile))
                config = FMConfiguration.Load(pathToConfigFile);
            else
                config = FMConfiguration.Load();

            LoadPreferences(config.Preferences);

            InitializeComponent();

            var dialogService = new DialogService(this);

            Root = new RootViewModel(dialogService, config);
            Task.Run(() => Root.InitializeAsync());

            DataContext = Root;

            Closing += (s, e) => Root.SaveConfiguration();
        }

        public void SetTheme(ThemeViewModel theme)
        {
            var rd = new System.Windows.ResourceDictionary() { Source = new Uri(@"/FMStudio;component/Themes/{0}.xaml".FormatInvariant(theme.ResourceName), UriKind.Relative) };
            System.Windows.Application.Current.Resources.MergedDictionaries.Clear();
            System.Windows.Application.Current.Resources.MergedDictionaries.Add(rd);
        }

        private void LoadPreferences(Preferences preferences)
        {
            var theme = ThemeViewModel.GetThemesList().FirstOrDefault(t => t.Name == preferences.Theme);

            if (theme == null)
                theme = ThemeViewModel.GetThemesList()[0];

            SetTheme(theme);

            if (preferences.StartMaximized)
            {
                WindowState = System.Windows.WindowState.Maximized;
            }
        }
    }
}