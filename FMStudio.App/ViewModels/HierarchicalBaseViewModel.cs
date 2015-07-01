using FMStudio.App.Utility;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace FMStudio.App.ViewModels
{
    public class HierarchicalBaseViewModel : BaseViewModel, IComparable
    {
        public HierarchicalBaseViewModel Parent { get; set; }

        public Binding<string> Name { get; set; }

        public Binding<bool> IsNodeExpanded { get; set; }

        public ObservableCollection<HierarchicalBaseViewModel> Children { get; set; }

        public HierarchicalBaseViewModel()
        {
            Name = new Binding<string>();
            IsNodeExpanded = new Binding<bool>();
            Children = new ObservableCollection<HierarchicalBaseViewModel>();
        }

        public virtual void Add(HierarchicalBaseViewModel child)
        {
            if (child.Parent != null)
                child.Parent.Remove(child);

            child.Parent = this;

            if (!Children.Contains(child))
                Children.Add(child);

            Children.SortBy(c => c);
        }

        public virtual async Task InitializeAsync()
        {
            foreach(var child in Children)
            {
                await child.InitializeAsync();
            }
        }

        public virtual void Remove(HierarchicalBaseViewModel child)
        {
            child.Parent = null;
            Children.Remove(child);
        }

        public virtual int CompareTo(object obj)
        {
            var vm = obj as HierarchicalBaseViewModel;

            if (vm != null)
                return string.Compare(Name.Value, vm.Name.Value);

            return 0;
        }
    }
}