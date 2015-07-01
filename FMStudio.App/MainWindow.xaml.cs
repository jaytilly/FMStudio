using FMStudio.App.ViewModels;
using FMStudio.Configuration;
using MahApps.Metro.Controls;

namespace FMStudio.App
{
    public partial class MainWindow : MetroWindow
    {
        private RootViewModel _root;

        public MainWindow()
        {
            InitializeComponent();

            var config = FMConfiguration.Load();
            _root = new RootViewModel(config);
            _root.InitializeAsync();

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