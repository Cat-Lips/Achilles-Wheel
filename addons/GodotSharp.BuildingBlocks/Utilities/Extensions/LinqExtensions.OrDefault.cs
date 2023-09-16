namespace GodotSharp.BuildingBlocks
{
    public static partial class LinqExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
            => source is null || !source.Any();

        #region MaxOrDefault

        public static T MaxOrDefault<T>(this IEnumerable<T> source, T defaultValue = default)
            => source.IsNullOrEmpty() ? defaultValue : source.Max();

        public static TResult MaxOrDefault<T, TResult>(this IEnumerable<T> source, Func<T, TResult> selector, TResult defaultValue = default)
            => source.IsNullOrEmpty() ? defaultValue : source.Max(selector);

        #endregion

        #region AverageOrDefault

        public static double AverageOrDefault(this IEnumerable<int> source, int defaultValue = default) => source.IsNullOrEmpty() ? defaultValue : source.Average();
        public static double AverageOrDefault(this IEnumerable<long> source, long defaultValue = default) => source.IsNullOrEmpty() ? defaultValue : source.Average();
        public static float AverageOrDefault(this IEnumerable<float> source, float defaultValue = default) => source.IsNullOrEmpty() ? defaultValue : source.Average();
        public static double AverageOrDefault(this IEnumerable<double> source, double defaultValue = default) => source.IsNullOrEmpty() ? defaultValue : source.Average();
        public static decimal AverageOrDefault(this IEnumerable<decimal> source, decimal defaultValue = default) => source.IsNullOrEmpty() ? defaultValue : source.Average();

        public static double? AverageOrDefault(this IEnumerable<int?> source, int? defaultValue = default) => source.IsNullOrEmpty() ? defaultValue : source.Average();
        public static double? AverageOrDefault(this IEnumerable<long?> source, long? defaultValue = default) => source.IsNullOrEmpty() ? defaultValue : source.Average();
        public static float? AverageOrDefault(this IEnumerable<float?> source, float? defaultValue = default) => source.IsNullOrEmpty() ? defaultValue : source.Average();
        public static double? AverageOrDefault(this IEnumerable<double?> source, double? defaultValue = default) => source.IsNullOrEmpty() ? defaultValue : source.Average();
        public static decimal? AverageOrDefault(this IEnumerable<decimal?> source, decimal? defaultValue = default) => source.IsNullOrEmpty() ? defaultValue : source.Average();

        public static double AverageOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector, int defaultValue = default) => source.IsNullOrEmpty() ? defaultValue : source.Average(selector);
        public static double AverageOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector, long defaultValue = default) => source.IsNullOrEmpty() ? defaultValue : source.Average(selector);
        public static float AverageOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector, float defaultValue = default) => source.IsNullOrEmpty() ? defaultValue : source.Average(selector);
        public static double AverageOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector, double defaultValue = default) => source.IsNullOrEmpty() ? defaultValue : source.Average(selector);
        public static decimal AverageOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector, decimal defaultValue = default) => source.IsNullOrEmpty() ? defaultValue : source.Average(selector);

        public static double? AverageOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector, int? defaultValue = default) => source.IsNullOrEmpty() ? defaultValue : source.Average(selector);
        public static double? AverageOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector, long? defaultValue = default) => source.IsNullOrEmpty() ? defaultValue : source.Average(selector);
        public static float? AverageOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector, float? defaultValue = default) => source.IsNullOrEmpty() ? defaultValue : source.Average(selector);
        public static double? AverageOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector, double? defaultValue = default) => source.IsNullOrEmpty() ? defaultValue : source.Average(selector);
        public static decimal? AverageOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector, decimal? defaultValue = default) => source.IsNullOrEmpty() ? defaultValue : source.Average(selector);

        #endregion
    }
}
