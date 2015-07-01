using FMStudio.Lib;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace FMStudio.App.ViewModels
{
    public class ProfilesViewModel : HierarchicalBaseViewModel
    {
        public ProjectViewModel ProjectVM { get; private set; }

        public ProjectInfo ProjectInfo { get; private set; }
        
        public ProfilesViewModel(ProjectViewModel projectVM, ProjectInfo projectInfo)
        {
            ProjectVM = projectVM;
            ProjectInfo = projectInfo;
        }

        public async Task InitializeAsync()
        {
            Children.Clear();

            foreach (var profile in ProjectInfo.Profiles)
            {
                var profileVM = new ProfileViewModel(this, profile);
                Children.Add(profileVM);

                await profileVM.InitializeAsync();
            }
        }
    }
}