using FMStudio.App.ViewModels;
using FMStudio.Configuration;
using MahApps.Metro.Controls;
using System.Threading.Tasks;

namespace FMStudio.App
{
    public partial class MainWindow : MetroWindow
    {
        private RootViewModel _root;

        public MainWindow(string pathToConfigFile)
        {
            InitializeComponent();

            FMConfiguration config = null;

            if (!string.IsNullOrEmpty(pathToConfigFile))
                config = FMConfiguration.Load(pathToConfigFile);
            else
                config = FMConfiguration.Load();

            _root = new RootViewModel(config);
            Task.Run(() => _root.InitializeAsync());

            LoadPreferences(config.Preferences);

            DataContext = _root;

            Closing += (s, e) => _root.SaveConfiguration();
        }

        private void LoadPreferences(Preferences preferences)
        {
            if (preferences.StartMaximized)
            {
                WindowState = System.Windows.WindowState.Maximized;
            }
        }
    }
}