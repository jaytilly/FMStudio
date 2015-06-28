using System.Linq;

namespace System.Collections.ObjectModel
{
    public static class ObservableCollectionExtensions
    {
        public static void SortBy<T, TKey>(this ObservableCollection<T> collection, Func<T, TKey> predicate)
        {
            var sorted = collection.OrderBy(predicate).ToList();
            for (int i = 0; i < sorted.Count; i++)
                collection.Move(collection.IndexOf(sorted[i]), i);
        }
    }
}