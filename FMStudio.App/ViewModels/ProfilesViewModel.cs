using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FMStudio.App.ViewModels
{
    public class ProfilesViewModel : HierarchicalBaseViewModel
    {
        public ProjectViewModel ProjectVM { get; private set; }

        public ProfilesViewModel(ProjectViewModel projectVM)
        {
            ProjectVM = projectVM;
        }

        public override async Task InitializeAsync()
        {
            Children.ClearOnDispatcher();
            foreach (var profile in ProjectVM.ProjectInfo.Profiles.OrderBy(p => p.Name))
            {
                var profileVM = new ProfileViewModel(this, profile);
                Add(profileVM);
                await profileVM.InitializeAsync();
            }
        }
    }
}