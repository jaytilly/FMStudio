using FMStudio.App.Interfaces;
using FMStudio.App.Utility;
using FMStudio.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FMStudio.App.ViewModels
{
    public class CategoryViewModel : HierarchicalBaseViewModel, IHaveAName, ICanBeDragged, ICanBeDroppedUpon
    {
        public RootViewModel RootVM { get; private set; }
        
        public ICommand DeleteCategoryCommand { get; private set; }

        public ICommand FullUpdateAllUnderlyingProjectsCommand { get; private set; }

        public CategoryViewModel(RootViewModel root, CategoryConfiguration categoryConfiguration)
        {
            RootVM = root;

            Name.Value = categoryConfiguration.Name;
            IsNodeExpanded.Value = categoryConfiguration.IsExpanded;

            DeleteCategoryCommand = new RelayCommand(param => DeleteCategory());
            FullUpdateAllUnderlyingProjectsCommand = new RelayCommand(async param => await FullUpdateAllUnderlyingProjectsAsync());

            categoryConfiguration.Categories.ForEach(c => Add(new CategoryViewModel(RootVM, c)));
            categoryConfiguration.Projects.ForEach(p => Add(new ProjectViewModel(RootVM, p)));
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
        }

        public void DeleteCategory()
        {
            Parent.Remove(this);
        }

        public async Task FullUpdateAllUnderlyingProjectsAsync()
        {
            await Task.WhenAll(Children.OfType<ProjectViewModel>().Select(p => p.FullUpdateAsync()));
            await Task.WhenAll(Children.OfType<CategoryViewModel>().Select(c => c.FullUpdateAllUnderlyingProjectsAsync()));
        }

        public CategoryConfiguration ToConfiguration()
        {
            return new CategoryConfiguration()
            {
                Categories = Children.OfType<CategoryViewModel>().Select(c => c.ToConfiguration()).ToList(),
                Name = Name.Value,
                IsExpanded = IsNodeExpanded.Value,
                Projects = Children.OfType<ProjectViewModel>().Select(c => c.ToConfiguration()).ToList()
            };
        }
    }
}