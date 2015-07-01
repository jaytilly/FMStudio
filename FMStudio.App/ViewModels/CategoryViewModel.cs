using FMStudio.App.Interfaces;
using FMStudio.App.Utility;
using FMStudio.Configuration;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Linq;

namespace FMStudio.App.ViewModels
{
    public class CategoryViewModel : HierarchicalBaseViewModel, IHaveAName, ICanBeDragged, ICanBeDroppedUpon
    {
        public RootViewModel RootVM { get; private set; }

        public CategoryConfiguration CategoryConfiguration { get; private set; }

        public ICommand RemoveCategoryCommand { get; private set; }

        public ICommand SaveCategoryCommand { get; private set; }

        public CategoryViewModel(RootViewModel root, CategoryConfiguration categoryConfiguration)
        {
            RootVM = root;
            CategoryConfiguration = categoryConfiguration;

            Name.Value = categoryConfiguration.Name;

            RemoveCategoryCommand = new RelayCommand(param => Remove());
            SaveCategoryCommand = new RelayCommand(param => Save());
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

        public override async Task InitializeAsync()
        {
            Children.Clear();

            foreach (var subCategory in CategoryConfiguration.Categories)
                Add(new CategoryViewModel(RootVM, subCategory));

            foreach (var project in CategoryConfiguration.Projects)
                Add(new ProjectViewModel(RootVM, project));

            await base.InitializeAsync();
        }

        #region Command implementations

        private void Remove()
        {
            Parent.Remove(this);
        }

        private void Save()
        {
            if (!RootVM.Configuration.Categories.Contains(CategoryConfiguration))
                RootVM.Configuration.Categories.Add(CategoryConfiguration);

            CategoryConfiguration.Name = Name.Value;
            RootVM.Configuration.Save();
        }

        #endregion Command implementations

        public CategoryConfiguration ToConfiguration()
        {
            return new CategoryConfiguration()
            {
                Categories = Children.OfType<CategoryViewModel>().Select(c => c.ToConfiguration()).ToList(),
                Name = Name.Value,
                Projects = Children.OfType<ProjectViewModel>().Select(c => c.ToConfiguration()).ToList()
            };
        }
    }
}