using FMStudio.App.Utility;
using Squirrel;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FMStudio.App.ViewModels
{
    public class UpdateViewModel : BaseViewModel
    {
        public const string UpdateUrl = @"http://builds.flyingpie.nl/fm-studio/squirrel/dev/";

        public RootViewModel RootVM { get; private set; }

        public Binding<string> CurrentVersion { get; private set; }

        public Binding<string> LatestVersion { get; private set; }

        public Binding<bool> IsUpdateAvailable { get; private set; }

        public Binding<bool> IsUpdating { get; private set; }

        public Binding<int> UpdateProgress { get; private set; }

        public Binding<bool> IsUpdateComplete { get; private set; }

        public ICommand CheckForUpdatesCommand { get; private set; }

        public ICommand UpdateToLatestVersionCommand { get; private set; }

        public ICommand RestartApplicationCommand { get; private set; }

        public UpdateViewModel(RootViewModel rootVM)
        {
            RootVM = rootVM;

            CurrentVersion = new Binding<string>("<unknown>");
            LatestVersion = new Binding<string>("<unknown>");
            IsUpdateAvailable = new Binding<bool>();

            IsUpdating = new Binding<bool>();
            UpdateProgress = new Binding<int>();
            IsUpdateComplete = new Binding<bool>();

            CheckForUpdatesCommand = new RelayCommand(async param => await CheckForUpdates());
            UpdateToLatestVersionCommand = new RelayCommand(async param => await UpdateToLatestVersion());
            RestartApplicationCommand = new RelayCommand(param => RestartApplication());
        }

        public async Task CheckForUpdates()
        {
            try
            {
                using (var updateManager = new UpdateManager(UpdateUrl))
                {
                    var result = await updateManager.CheckForUpdate();

                    CurrentVersion.Value = result.CurrentlyInstalledVersion.Version.ToString();
                    LatestVersion.Value = result.FutureReleaseEntry.Version.ToString();
                    IsUpdateAvailable.Value = result.ReleasesToApply.Any();
                }
            }
            catch (Exception e)
            {
                RootVM.OutputVM.Error("Cannot check for updates: {0}", e.GetFullMessage());
            }
        }

        public async Task UpdateToLatestVersion()
        {
            try
            {
                using (var updateManager = new UpdateManager(UpdateUrl))
                {
                    IsUpdating.Value = true;

                    var result = await updateManager.UpdateApp(progress => UpdateProgress.Value = progress);

                    IsUpdating.Value = false;
                    IsUpdateComplete.Value = true;
                }
            }
            catch (Exception e)
            {
                RootVM.OutputVM.Error("Cannot update to latest version: {0}", e.GetFullMessage());
            }
        }

        public void RestartApplication()
        {
            try
            {
                UpdateManager.RestartApp();
            }
            catch (Exception e)
            {
                RootVM.OutputVM.Error("Cannot restart the application: {0}", e.GetFullMessage());
            }
        }
    }
}