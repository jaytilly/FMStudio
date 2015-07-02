using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

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
            ProjectVM.ProjectInfo.Profiles.OrderBy(p => p.Name).ToList().ForEach(p => Add(new ProfileViewModel(this, p)));

            await base.InitializeAsync();
        }
    }
}