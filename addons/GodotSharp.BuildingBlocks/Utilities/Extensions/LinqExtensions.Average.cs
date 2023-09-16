using Godot;

namespace GodotSharp.BuildingBlocks
{
    public static partial class LinqExtensions
    {
        public static TResult MostCommon<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector) => source.Select(selector).MostCommon();
        public static T MostCommon<T>(this IEnumerable<T> source)
        {
            return source
                .GroupBy(item => item)
                .OrderByDescending(group => group.Count())
            .First().Key;
        }

        public static Color Average<T>(this IEnumerable<T> source, Func<T, Color> selector) => source.Select(selector).Average();
        public static Color Average(this IEnumerable<Color> source)
        {
            var count = 0;
            float r = 0, g = 0, b = 0;

            foreach (var c in source)
            {
                r += c.R;
                g += c.G;
                b += c.B;
                ++count;
            }

            return new(r / count, g / count, b / count);
        }
    }
}
