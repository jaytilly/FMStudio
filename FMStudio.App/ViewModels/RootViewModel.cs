using FMStudio.App.Interfaces;
using FMStudio.App.Utility;
using FMStudio.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FMStudio.App.ViewModels
{
    public class RootViewModel : HierarchicalBaseViewModel, ICanBeDroppedUpon
    {
        public Binding<BaseViewModel> ActiveEntity { get; set; }

        public ICommand AddCategoryCommand { get; private set; }

        public ICommand AddProjectCommand { get; private set; }

        public ICommand EditPreferencesCommand { get; private set; }

        public ICommand FullUpdateCommand { get; private set; }

        public ICommand SaveConfigurationCommand { get; private set; }

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
            SaveConfigurationCommand = new RelayCommand(param => SaveConfiguration());
            SelectActiveEntityCommand = new RelayCommand(param => SelectActiveEntity(param));

            Configuration = configuration;

            OutputVM = new OutputViewModel();

            OutputVM.Write("Loaded local FluentMigrator assembly version " + Lib.Utility.References.GetFluentMigratorAssemblyVersion());

            Configuration.Categories.ForEach(c => Add(new CategoryViewModel(OutputVM, this, c)));
            Configuration.Projects.ForEach(p => Add(new ProjectViewModel(OutputVM, this, p)));
        }
        
        public void Drop(ICanBeDragged draggable)
        {
            var childVM = draggable as HierarchicalBaseViewModel;
            if (childVM != null)
                Add(childVM);
        }

        public void SaveConfiguration()
        {
            Configuration.Categories.Clear();
            Configuration.Categories.AddRange(Children.OfType<CategoryViewModel>().Select(c => c.ToConfiguration()));

            Configuration.Projects.Clear();
            Configuration.Projects.AddRange(Children.OfType<ProjectViewModel>().Select(c => c.ToConfiguration()));

            Configuration.Save();
        }

        private void AddCategory()
        {
            var categoryVM = new CategoryViewModel(OutputVM, this, new CategoryConfiguration() { Name = "New category" });

            var selectedCategoryVM = ActiveEntity.Value as CategoryViewModel;
            if (selectedCategoryVM != null)
                selectedCategoryVM.Add(categoryVM);
            else
                Add(categoryVM);

            ActiveEntity.Value = categoryVM;
        }

        private void AddProject()
        {
            var projectVM = new ProjectViewModel(OutputVM, this, new ProjectConfiguration());
            projectVM.IsNew.Value = true;

            var currentCategoryVM = ActiveEntity.Value as CategoryViewModel;
            var currentProjectVM = ActiveEntity.Value as ProjectViewModel;

            if (currentCategoryVM != null)
            {
                currentCategoryVM.IsNodeExpanded.Value = true;
                currentCategoryVM.Add(projectVM);
            }
            else if (currentProjectVM != null)
            {
                currentProjectVM.Parent.IsNodeExpanded.Value = true;
                currentProjectVM.Parent.Add(projectVM);
            }
            else
            {
                IsNodeExpanded.Value = true;
                Add(projectVM);
            }

            ActiveEntity.Value = projectVM;
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
                    OutputVM.Error("Could not run a full update on project '{0}': {1}", project.Name.Value, e.GetFullMessage());
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

        public async Task UpdateHasPendingMigrations()
        {
            await Task.WhenAll(Children.OfType<CategoryViewModel>().Select(c => c.UpdateHasPendingMigrations()));
        }
    }
}