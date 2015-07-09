using FMStudio.App.Interfaces;
using FMStudio.App.Utility;
using FMStudio.Configuration;
using FMStudio.Utility.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FMStudio.App.ViewModels
{
    public class CategoryViewModel : HierarchicalBaseViewModel, IHaveAName, ICanBeDragged, ICanBeDroppedUpon
    {
        private ILog _log;

        public RootViewModel RootVM { get; private set; }

        public IEnumerable<CategoryViewModel> Categories
        {
            get { return Children.OfType<CategoryViewModel>(); }
        }

        public IEnumerable<ProjectViewModel> Projects
        {
            get { return Children.OfType<ProjectViewModel>(); }
        }

        public Binding<bool> HasPendingMigrations { get; private set; }

        public ICommand DeleteCategoryCommand { get; private set; }

        public ICommand FullUpdateAllUnderlyingProjectsCommand { get; private set; }

        public ICommand RefreshAllUnderlyingProjectsCommand { get; private set; }

        public CategoryViewModel(ILog log, RootViewModel root, CategoryConfiguration categoryConfiguration)
        {
            _log = log;

            RootVM = root;

            Name.Value = categoryConfiguration.Name;
            IsNodeExpanded.Value = categoryConfiguration.IsExpanded;
            HasPendingMigrations = new Binding<bool>();

            DeleteCategoryCommand = new RelayCommand(param => DeleteCategory());
            FullUpdateAllUnderlyingProjectsCommand = new RelayCommand(async param => await FullUpdateAllUnderlyingProjectsAsync());
            RefreshAllUnderlyingProjectsCommand = new RelayCommand(async param => await RefreshAllUnderlyingProjectsAsync());

            categoryConfiguration.Categories.ForEach(c => Add(new CategoryViewModel(_log, RootVM, c)));
            categoryConfiguration.Projects.ForEach(p => Add(new ProjectViewModel(_log, RootVM, p)));
        }

        public override int CompareTo(object obj)
        {
            var projectVM = obj as ProjectViewModel;
            if (projectVM != null)
                return -1;

            return base.CompareTo(obj);
        }

        public void Drop(ICanBeDragged draggable)
        {
            var childVM = draggable as HierarchicalBaseViewModel;
            if (childVM != null && !Children.Contains(childVM))
                Add(childVM);

            Task.Run(async () => await RootVM.UpdateHasPendingMigrations());
        }

        public void DeleteCategory()
        {
            Parent.Remove(this);
        }

        public async Task FullUpdateAllUnderlyingProjectsAsync()
        {
            await Task.WhenAll(Projects.Select(p => p.FullUpdateAsync()));
            await Task.WhenAll(Categories.Select(c => c.FullUpdateAllUnderlyingProjectsAsync()));
        }

        public async Task RefreshAllUnderlyingProjectsAsync()
        {
            await Task.WhenAll(Categories.Select(c => c.RefreshAllUnderlyingProjectsAsync()));
            await Task.WhenAll(Projects.Select(p => p.InitializeAsync()));
        }

        public async Task UpdateHasPendingMigrations()
        {
            await Task.WhenAll(Categories.Select(c => c.UpdateHasPendingMigrations()));

            HasPendingMigrations.Value = Categories.Any(c => c.HasPendingMigrations.Value) || Projects.Any(p => p.HasPendingMigrations.Value);
        }

        public CategoryConfiguration ToConfiguration()
        {
            return new CategoryConfiguration()
            {
                Categories = Categories.Select(c => c.ToConfiguration()).ToList(),
                Name = Name.Value,
                IsExpanded = IsNodeExpanded.Value,
                Projects = Projects.Select(c => c.ToConfiguration()).ToList()
            };
        }
    }
}