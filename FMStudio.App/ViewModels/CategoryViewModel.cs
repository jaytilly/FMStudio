using FMStudio.App.Interfaces;
using FMStudio.App.Utility;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using FMStudio.Configuration;
using System.Windows.Input;

namespace FMStudio.App.ViewModels
{
    public class CategoryViewModel : BaseViewModel, IHaveAName, ICanBeDragged, ICanBeDroppedUpon
    {
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
                projectVM.Category.Children.Remove(projectVM);
                Children.Add(projectVM);
                projectVM.Category = this;
                
                CategoryConfiguration.Projects.Add(projectVM.ProjectConfiguration);
                RootVM.Configuration.Save();
            }
        }

        private void Save()
        {
            if (!RootVM.Configuration.Categories.Contains(CategoryConfiguration))
                RootVM.Configuration.Categories.Add(CategoryConfiguration);
            
            CategoryConfiguration.Name = Name.Value;
            RootVM.Configuration.Save();
        }
    }
}