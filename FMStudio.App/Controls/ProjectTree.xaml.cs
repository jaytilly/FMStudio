using FMStudio.App.Interfaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FMStudio.App.Controls
{
    public partial class ProjectTree : UserControl
    {
        public ProjectTree()
        {
            InitializeComponent();
        }

        private void trvProjects_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var dependencyObject = (DependencyObject)e.OriginalSource;
                var treeViewItem = dependencyObject.FindAncestor<TreeViewItem>();

                if (treeViewItem != null)
                {
                    var projectItem = treeViewItem.Header as ICanBeDragged;

                    if (projectItem != null)
                    {
                        var dragData = new DataObject(EventConstants.DRAGDROP_DATA, projectItem);
                        DragDrop.DoDragDrop(treeViewItem, dragData, DragDropEffects.Move);
                    }
                }
            }
        }

        private void trvProjects_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(EventConstants.DRAGDROP_DATA))
            {
                var draggable = e.Data.GetData(EventConstants.DRAGDROP_DATA) as ICanBeDragged;
                var dependencyObject = (DependencyObject)e.OriginalSource;
                var treeViewItem = dependencyObject.FindAncestor<TreeViewItem>();

                if (treeViewItem != null)
                {
                    var dropTarget = treeViewItem.Header as ICanBeDroppedUpon;
                    if (dropTarget != null)
                        dropTarget.Drop(draggable);
                }
            }
        }

        private void trvProjects_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var treeViewItem = ((DependencyObject)e.OriginalSource).FindAncestor<TreeViewItem>();

            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
            }
        }
    }
}