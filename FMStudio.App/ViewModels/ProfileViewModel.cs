using FMStudio.App.Utility;
using FMStudio.Lib;
using FMStudio.Utility.Logging;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FMStudio.App.ViewModels
{
    public class ProfileViewModel : HierarchicalBaseViewModel
    {
        private ILog _log;

        public ProfilesViewModel ProfilesVM { get; set; }

        public ProfileInfo ProfileInfo { get; set; }
        
        public Binding<string> Sql { get; private set; }

        public ICommand RunProfileCommand { get; set; }

        public ProfileViewModel(ILog log, ProfilesViewModel profilesVM, ProfileInfo profileInfo)
        {
            _log = log;

            ProfilesVM = profilesVM;
            ProfileInfo = profileInfo;
            
            Sql = new Binding<string>(() => ProfileInfo.GetSqlAsync().Result); // TODO: Make nicer

            RunProfileCommand = new RelayCommand(async param => await RunProfileAsync());
        }

        public override async Task InitializeAsync()
        {
            try
            {
                await ProfileInfo.InitializeAsync();

                Name.Value = ProfileInfo.Name;
            }
            catch (Exception e)
            {
                _log.Error("Could not load profile '{0}': {1}", ProfileInfo.Name, e.GetFullMessage());
            }
        }

        private async Task RunProfileAsync()
        {
            try
            {
                await ProfileInfo.RunAsync();

                await InitializeAsync();
            }
            catch (Exception e)
            {
                _log.Error("Could not run profile '{0}': {1}", ProfileInfo.Name, e.GetFullMessage());
            }
        }
    }
}