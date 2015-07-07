using FMStudio.Utility.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FMStudio.App.ViewModels
{
    public class ProfilesViewModel : HierarchicalBaseViewModel
    {
        private ILog _log;

        public ProjectViewModel ProjectVM { get; private set; }

        public ProfilesViewModel(ILog log, ProjectViewModel projectVM)
        {
            _log = log;

            ProjectVM = projectVM;
        }

        public override async Task InitializeAsync()
        {
            Children.ClearOnDispatcher();
            foreach (var profile in ProjectVM.ProjectInfo.Profiles.OrderBy(p => p.Name))
            {
                var profileVM = new ProfileViewModel(_log, this, profile);
                Add(profileVM);
                await profileVM.InitializeAsync();
            }

            Sort();
        }
    }
}