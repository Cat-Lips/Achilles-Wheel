namespace GodotSharp.BuildingBlocks
{
    public static partial class LinqExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action(item);
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            var i = -1;
            foreach (var item in source)
                action(item, ++i);
        }

        public static IEnumerable<T> ForAll<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
                yield return item;
            }
        }

        public static IEnumerable<T> ForAll<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            var i = -1;
            foreach (var item in source)
            {
                action(item, ++i);
                yield return item;
            }
        }

        public static IEnumerable<T> SelectRecursive<T>(this T root, Func<T, IEnumerable<T>> select)
        {
            foreach (var child in select(root))
            {
                yield return child;

                foreach (var sub in child.SelectRecursive(select))
                    yield return sub;
            }
        }
    }
}
