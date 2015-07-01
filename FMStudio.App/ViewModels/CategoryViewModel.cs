using FMStudio.App.Interfaces;
using FMStudio.App.Utility;
using FMStudio.Configuration;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FMStudio.App.ViewModels
{
    public class CategoryViewModel : BaseViewModel, IHaveAName, ICanBeDragged, ICanBeDroppedUpon
    {
        public CategoryViewModel ParentCategory { get; private set; }

        public RootViewModel RootVM { get; private set; }

        public CategoryConfiguration CategoryConfiguration { get; private set; }

        public Binding<string> Name { get; set; }

        public Binding<bool> IsNodeExpanded { get; set; }

        public ObservableCollection<IHaveAName> Children { get; set; }

        public ICommand SaveCategoryCommand { get; private set; }

        public CategoryViewModel(RootViewModel root, CategoryConfiguration categoryConfiguration)
        {
            RootVM = root;
            CategoryConfiguration = categoryConfiguration;

            Name = new Binding<string>(categoryConfiguration.Name);
            IsNodeExpanded = new Binding<bool>(true);

            Children = new ObservableCollection<IHaveAName>();

            SaveCategoryCommand = new RelayCommand(param => Save());
        }

        public void Drop(ICanBeDragged draggable)
        {
            var projectVM = draggable as ProjectViewModel;
            if (projectVM != null && !Children.Contains(projectVM))
            {
                projectVM.MoveTo(this);
                RootVM.Configuration.Save();
            }

            var categoryVM = draggable as CategoryViewModel;
            if (categoryVM != null)
            {
                categoryVM.MoveTo(this);
                RootVM.Configuration.Save();
            }
        }

        public async Task InitializeAsync()
        {
            foreach (CategoryViewModel category in Children)
            {
                await category.InitializeAsync();
            }

            foreach (ProjectViewModel project in Children)
            {
                await project.InitializeAsync();
            }
        }

        #region Collection

        public void Add(CategoryViewModel category)
        {
            category.ParentCategory = this;

            if (!Children.Contains(category))
                Children.Add(category);

            CategoryConfiguration.Add(category.CategoryConfiguration);

            RootVM.Configuration.Save();
        }

        public void Add(ProjectViewModel project)
        {
            project.ParentCategory = this;

            if (!Children.Contains(project))
                Children.Add(project);

            CategoryConfiguration.Add(project.ProjectConfiguration);

            RootVM.Configuration.Save();
        }

        public void MoveTo(CategoryViewModel category)
        {
            ParentCategory.Remove(this);
            category.Add(this);

            RootVM.Configuration.Save();
        }

        public void Remove(CategoryViewModel category)
        {
            category.ParentCategory = null;
            Children.Remove(category);

            CategoryConfiguration.Remove(category.CategoryConfiguration);

            RootVM.Configuration.Save();
        }

        public void Remove(ProjectViewModel project)
        {
            project.ParentCategory = null;
            Children.Remove(project);

            CategoryConfiguration.Remove(project.ProjectConfiguration);

            RootVM.Configuration.Save();
        }

        #endregion

        private void Save()
        {
            if (!RootVM.Configuration.Categories.Contains(CategoryConfiguration))
                RootVM.Configuration.Categories.Add(CategoryConfiguration);

            CategoryConfiguration.Name = Name.Value;
            RootVM.Configuration.Save();

            RootVM.Configuration.Save();
        }
    }
}