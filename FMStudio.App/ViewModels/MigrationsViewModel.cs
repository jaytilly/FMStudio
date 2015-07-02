using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FMStudio.App.ViewModels
{
    public class MigrationsViewModel : HierarchicalBaseViewModel
    {
        public ProjectViewModel ProjectVM { get; set; }

        public MigrationsViewModel(ProjectViewModel projectVM)
        {
            ProjectVM = projectVM;
        }

        public override async Task InitializeAsync()
        {
            Children.ClearOnDispatcher();
            foreach (var migration in ProjectVM.ProjectInfo.Migrations.OrderByDescending(m => m.Version))
            {
                var migrationVM = new MigrationViewModel(this, migration);
                Add(migrationVM);
                await migrationVM.InitializeAsync();
            }
        }
    }
}