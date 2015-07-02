using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace System.Collections.Generic
{
    public static class CollectionExtensions
    {
        public static List<T> ToList<T>(this IList list)
        {
            var result = new List<T>();

            foreach (var item in list)
            {
                result.Add((T)item);
            }

            return result;
        }

        public static void AddOnDispatcher<T>(this Collection<T> list, T item)
        {
            if (Application.Current != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    list.Add(item);
                });
            }
            else
            {
                list.Add(item);
            }
        }

        public static void ClearOnDispatcher<T>(this Collection<T> list)
        {
            if (Application.Current != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    list.Clear();
                });
            }
            else
            {
                list.Clear();
            }
        }

        public static Collection<T> ForEachOnDispatcher<T>(this Collection<T> list, Action<T> action)
        {
            if (Application.Current != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    foreach (var item in list)
                    {
                        action(item);
                    }
                });
            }
            else
            {
                foreach (var item in list)
                {
                    action(item);
                }
            }
            return list;
        }

        public static void InsertOnDispatcher<T>(this Collection<T> list, int index, T item)
        {
            if (Application.Current != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    list.Insert(index, item);
                });
            }
            else
            {
                list.Insert(index, item);
            }
        }

        public static void MoveOnDispatcher<T>(this ObservableCollection<T> list, int oldIndex, int newIndex)
        {
            if (Application.Current != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    list.Move(oldIndex, newIndex);
                });
            }
            else
            {
                list.Move(oldIndex, newIndex);
            }
        }

        public static void RemoveOnDispatcher<T>(this Collection<T> list, T item)
        {
            if (Application.Current != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    list.Remove(item);
                });
            }
            else
            {
                list.Remove(item);
            }
        }

        public static void SortBy<T, TKey>(this ObservableCollection<T> collection, Func<T, TKey> predicate)
        {
            var sorted = collection.OrderBy(predicate).ToList();
            for (int i = 0; i < sorted.Count; i++)
                collection.MoveOnDispatcher(collection.IndexOf(sorted[i]), i);
        }
    }
}