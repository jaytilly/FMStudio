using FMStudio.Utility.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FMStudio.App.ViewModels
{
    public class MigrationsViewModel : HierarchicalBaseViewModel
    {
        private ILog _log;

        public ProjectViewModel ProjectVM { get; set; }

        public MigrationsViewModel(ILog log, ProjectViewModel projectVM)
        {
            _log = log;

            ProjectVM = projectVM;
        }

        public override async Task InitializeAsync()
        {
            Children.ClearOnDispatcher();
            foreach (var migration in ProjectVM.ProjectInfo.Migrations.OrderByDescending(m => m.Version))
            {
                var migrationVM = new MigrationViewModel(_log, this, migration);
                Add(migrationVM);
                await migrationVM.InitializeAsync();
            }
        }
    }
}