using FMStudio.App.Utility;
using Squirrel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FMStudio.App.ViewModels
{
    public class UpdateViewModel : BaseViewModel
    {
        //public const string UpdateUrl = @"https://fmstudio.azurewebsites.net/release/dev";
        public const string UpdateUrl = @"C:\Users\Marco\Source\Repos\FMStudio\Releases";

        public RootViewModel RootVM { get; private set; }

        public Binding<string> CurrentVersion { get; private set; }

        public Binding<string> LatestVersion { get; private set; }

        public Binding<bool> IsUpdateAvailable { get; private set; }

        public Binding<bool> IsUpdating { get; private set; }

        public Binding<int> UpdateProgress { get; private set; }

        public Binding<bool> IsUpdateComplete { get; private set; }

        public ICommand CheckForUpdatesCommand { get; private set; }

        public ICommand UpdateToLatestVersionCommand { get; private set; }

        public UpdateViewModel(RootViewModel rootVM)
        {
            RootVM = rootVM;

            CurrentVersion = new Binding<string>("current");
            LatestVersion = new Binding<string>("...");
            IsUpdateAvailable = new Binding<bool>();

            IsUpdating = new Binding<bool>();
            UpdateProgress = new Binding<int>();
            IsUpdateComplete = new Binding<bool>();

            CheckForUpdatesCommand = new RelayCommand(async param => await CheckForUpdates());
            UpdateToLatestVersionCommand = new RelayCommand(async param => await UpdateToLatestVersion());
        }

        public async Task CheckForUpdates()
        {
            using (var updateManager = new UpdateManager(UpdateUrl))
            {
                var result = await updateManager.CheckForUpdate();

                CurrentVersion.Value = result.CurrentlyInstalledVersion.Version.ToString();
                LatestVersion.Value = result.FutureReleaseEntry.Version.ToString();
                IsUpdateAvailable.Value = result.ReleasesToApply.Any();
            }
        }

        public async Task UpdateToLatestVersion()
        {
            using (var updateManager = new UpdateManager(UpdateUrl))
            {
                IsUpdating.Value = true;

                var result = await updateManager.UpdateApp(progress => UpdateProgress.Value = progress);

                IsUpdating.Value = false;
                IsUpdateComplete.Value = true;
            }
        }
    }
}