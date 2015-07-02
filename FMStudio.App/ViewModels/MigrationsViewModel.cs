using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FMStudio.App.ViewModels
{
    public class MigrationsViewModel : HierarchicalBaseViewModel
    {
        public ProjectViewModel ProjectVM { get; set; }

        public MigrationsViewModel(ProjectViewModel projectVM)
        {
            ProjectVM = projectVM;
        }

        public override Task InitializeAsync()
        {
            Children.ClearOnDispatcher();
            ProjectVM.ProjectInfo.Migrations.OrderByDescending(m => m.Version).ToList().ForEach(m => Add(new MigrationViewModel(this, m)));

            return base.InitializeAsync();
        }
    }
}