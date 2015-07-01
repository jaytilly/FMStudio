using FMStudio.App.Interfaces;
using FMStudio.App.ViewModels;
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
                //var listView = sender as ListView;
                var dependencyObject = (DependencyObject)e.OriginalSource;
                var treeViewItem = dependencyObject.FindAncestor<TreeViewItem>();

                if (treeViewItem != null)
                {
                    //var itemsToMove = new List<Message>();
                    //foreach (var item in lstContents.SelectedItems)
                    //{
                    //    var message = item as Message;

                    //    if (message != null)
                    //    {
                    //        itemsToMove.Add(message);
                    //    }
                    //}

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
                var itemsToMove = e.Data.GetData(EventConstants.DRAGDROP_DATA) as ICanBeDragged;
                
                var treeView = (TreeView)sender;
                var dependencyObject = (DependencyObject)e.OriginalSource;
                var treeViewItem = dependencyObject.FindAncestor<TreeViewItem>();

                if (treeViewItem != null)
                {
                    var dropTarget = treeViewItem.Header as ICanBeDroppedUpon;
                    if(dropTarget != null)
                    {
                        dropTarget.Drop(itemsToMove);
                    }

                    //var targetQueue = treeViewItem.Header as Queue;

                    //if (targetQueue != null)
                    //{
                    //    QueueModel.ActiveQueue.MoveMessages(itemsToMove, targetQueue);
                    //}
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