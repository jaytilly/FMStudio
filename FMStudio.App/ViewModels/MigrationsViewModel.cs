using FMStudio.Lib;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace FMStudio.App.ViewModels
{
    public class MigrationsViewModel : HierarchicalBaseViewModel
    {
        public ProjectInfo ProjectInfo { get; set; }

        public ProjectViewModel ProjectVM { get; set; }
        
        public MigrationsViewModel(ProjectViewModel projectVM, ProjectInfo project)
        {
            ProjectVM = projectVM;
            ProjectInfo = project;
        }

        public override async Task InitializeAsync()
        {
            Children.Clear();

            foreach (var migration in ProjectInfo.Migrations.OrderByDescending(m => m.Version))
            {
                var migrationVM = new MigrationViewModel(this, migration);
                Add(migrationVM);

                await migrationVM.InitializeAsync();
            }

            await base.InitializeAsync();
        }
    }
}