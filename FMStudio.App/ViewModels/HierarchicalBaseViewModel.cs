using FMStudio.App.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FMStudio.App.ViewModels
{
    public class HierarchicalBaseViewModel : BaseViewModel, IComparable
    {
        public HierarchicalBaseViewModel Parent { get; set; }

        public Binding<string> Name { get; set; }

        public Binding<bool> IsNodeExpanded { get; set; }

        public ObservableCollection<HierarchicalBaseViewModel> Children { get; set; }

        public ICommand CollapseAllCommand { get; private set; }

        public ICommand ExpandAllCommand { get; private set; }

        public HierarchicalBaseViewModel()
        {
            Name = new Binding<string>();
            IsNodeExpanded = new Binding<bool>();
            Children = new ObservableCollection<HierarchicalBaseViewModel>();

            CollapseAllCommand = new RelayCommand(param => CollapseAll());
            ExpandAllCommand = new RelayCommand(param => ExpandAll());
        }

        public virtual async Task InitializeAsync()
        {
            var projectVMs = Children.OfType<ProjectViewModel>().ToList();

            await Task.WhenAll(projectVMs.Where(p => p.IsLoadedOnStart.Value).Select(c => c.InitializeAsync()));

            await Task.WhenAll(Children.Where(c => !projectVMs.Contains(c)).Select(c => c.InitializeAsync()));
        }

        #region Collection

        public virtual void Add(HierarchicalBaseViewModel child)
        {
            if (child.Parent != null)
                child.Parent.Remove(child);

            child.Parent = this;

            if (!Children.Contains(child))
                Children.AddOnDispatcher(child);

            Children.SortBy(c => c);
        }

        public virtual void Remove(HierarchicalBaseViewModel child)
        {
            child.Parent = null;
            Children.RemoveOnDispatcher(child);
        }

        public virtual void Sort()
        {
            Children.SortBy(a => a);
        }

        public virtual int CompareTo(object obj)
        {
            var vm = obj as HierarchicalBaseViewModel;

            if (vm != null)
                return string.Compare(Name.Value, vm.Name.Value);

            return 0;
        }

        #endregion Collection

        public void CollapseAll()
        {
            IsNodeExpanded.Value = false;

            Children.ForEachOnDispatcher(c => c.CollapseAll());
        }

        public void ExpandAll()
        {
            IsNodeExpanded.Value = true;

            Children.ForEachOnDispatcher(c => c.ExpandAll());
        }
    }
}