using FMStudio.App.Utility;
using FMStudio.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FMStudio.App.ViewModels
{
    public class RootViewModel : NotifyPropertyChanged
    {
        public ObservableCollection<IHaveAName> Children { get; private set; }

        public Binding<BaseViewModel> ActiveEntity { get; set; }

        public ICommand AddCategoryCommand { get; private set; }

        public ICommand AddProjectCommand { get; private set; }

        public ICommand EditPreferencesCommand { get; private set; }

        public ICommand FullUpdateCommand { get; private set; }

        public ICommand SelectActiveEntityCommand { get; private set; }

        public FMConfiguration Configuration { get; set; }

        public List<FMStudio.Lib.ProjectInfo> LibProjects { get; set; }

        public Binding<string> Output { get; set; }

        public OutputViewModel OutputVM { get; set; }

        public RootViewModel(FMConfiguration configuration)
        {
            ActiveEntity = new Binding<BaseViewModel>(new DefaultViewModel());
            Output = new Binding<string>();

            AddCategoryCommand = new RelayCommand(param => AddCategory());
            AddProjectCommand = new RelayCommand(param => AddProject());
            EditPreferencesCommand = new RelayCommand(param => EditPreferences());
            FullUpdateCommand = new RelayCommand(async param => await FullUpdateAsync());
            SelectActiveEntityCommand = new RelayCommand(param => SelectActiveEntity(param));

            Configuration = configuration;
            Children = new ObservableCollection<IHaveAName>();

            OutputVM = new OutputViewModel();

            AppendOutput("Loaded local FluentMigrator assembly version " + Lib.Utility.References.GetFluentMigratorAssemblyVersion());
        }

        public async Task InitializeAsync()
        {
            Children.Clear();

            foreach (var categoryConfiguration in Configuration.Categories)
            {
                Children.Add(LoadCategory(categoryConfiguration));
            }

            foreach(CategoryViewModel category in Children)
            {
                //await category.InitializeAsync();
            }
        }

        private CategoryViewModel LoadCategory(CategoryConfiguration categoryConfiguration)
        {
            var categoryVM = new CategoryViewModel(this, categoryConfiguration);

            foreach (var subCategoryConfiguration in categoryConfiguration.Categories)
            {
                categoryVM.Add(LoadCategory(subCategoryConfiguration));
            }

            foreach (var projectConfiguration in categoryConfiguration.Projects)
            {
                var projectVM = new ProjectViewModel(this, projectConfiguration);
                categoryVM.Add(projectVM);

                categoryVM.Children.SortBy(p => p.Name.Value);
            }

            return categoryVM;
        }

        public void AppendOutput(string format, params object[] args)
        {
            OutputVM.Write(format, args);
        }

        private void AddCategory()
        {
            var categoryVM = new CategoryViewModel(this, new CategoryConfiguration());

            var activeCategoryVM = ActiveEntity.Value as CategoryViewModel;

            if (activeCategoryVM != null)
                activeCategoryVM.Children.Add(categoryVM);
            else
                Children.Add(categoryVM);

            ActiveEntity.Value = categoryVM;
        }

        private void AddProject()
        {
            var categoryVM = ActiveEntity.Value as CategoryViewModel;
            if (categoryVM != null)
            {
                var projectVM = new ProjectViewModel(this, new ProjectConfiguration());
                projectVM.IsNew.Value = true;

                projectVM.MoveTo(categoryVM);

                ActiveEntity.Value = projectVM;
            }
        }

        private void EditPreferences()
        {
            ActiveEntity.Value = new PreferencesViewModel(this);
        }

        private async Task FullUpdateAsync()
        {
            foreach (ProjectViewModel project in Children)
            {
                try
                {
                    await project.ProjectInfo.FullUpdateAsync();
                }
                catch (Exception e)
                {
                    AppendOutput("Could not run a full update on project '{0}': {1}", project.Name.Value, e.GetFullMessage());
                }
            }
        }

        private void SelectActiveEntity(object param)
        {
            var migrationEntity = param as BaseViewModel;
            if (migrationEntity != null)
            {
                ActiveEntity.Value = migrationEntity;
            }
        }
    }
}