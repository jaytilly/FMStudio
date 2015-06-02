using FMStudio.Lib;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace FMStudio.App.ViewModels
{
    public class MigrationsViewModel : BaseViewModel
    {
        public ProjectInfo ProjectInfo { get; set; }

        public ProjectViewModel ProjectVM { get; set; }

        public ObservableCollection<MigrationViewModel> Migrations { get; private set; }

        public MigrationsViewModel(ProjectViewModel projectVM, ProjectInfo project)
        {
            ProjectVM = projectVM;
            ProjectInfo = project;

            Migrations = new ObservableCollection<MigrationViewModel>();
        }

        public async Task InitializeAsync()
        {
            Migrations.Clear();

            foreach (var migration in ProjectInfo.Migrations.OrderByDescending(m => m.Version))
            {
                var migrationVM = new MigrationViewModel(this, migration);
                Migrations.Add(migrationVM);

                await migrationVM.InitializeAsync();
            }
        }
    }
}