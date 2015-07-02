using FMStudio.App.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        public virtual async Task InitializeAsync()
        {
            await Task.WhenAll(Children.Select(c => c.InitializeAsync()));
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
    }
}