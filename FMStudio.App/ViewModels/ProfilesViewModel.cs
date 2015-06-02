using FMStudio.Lib;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace FMStudio.App.ViewModels
{
    public class ProfilesViewModel : BaseViewModel
    {
        public ProjectViewModel ProjectVM { get; private set; }

        public ProjectInfo ProjectInfo { get; private set; }

        public ObservableCollection<ProfileViewModel> Profiles { get; private set; }

        public ProfilesViewModel(ProjectViewModel projectVM, ProjectInfo projectInfo)
        {
            ProjectVM = projectVM;
            ProjectInfo = projectInfo;

            Profiles = new ObservableCollection<ProfileViewModel>();
        }

        public async Task InitializeAsync()
        {
            Profiles.Clear();

            foreach (var profile in ProjectInfo.Profiles)
            {
                var profileVM = new ProfileViewModel(this, profile);
                Profiles.Add(profileVM);

                await profileVM.InitializeAsync();
            }
        }
    }
}