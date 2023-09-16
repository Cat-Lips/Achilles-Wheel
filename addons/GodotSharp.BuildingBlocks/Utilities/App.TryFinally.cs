namespace GodotSharp.BuildingBlocks
{
    public static partial class App
    {
        public static void Try(Action @try, Action @finally)
        {
            try { @try(); }
            finally { @finally(); }
        }

        public static void Try<_>(Action @try, Func<_> @finally)
        {
            try { @try(); }
            finally { @finally(); }
        }

        public static T Try<T>(Func<T> @try, Action @finally)
        {
            try { return @try(); }
            finally { @finally(); }
        }

        public static T Try<T, _>(Func<T> @try, Func<_> @finally)
        {
            try { return @try(); }
            finally { @finally(); }
        }
    }
}
