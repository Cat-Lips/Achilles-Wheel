namespace GodotSharp.BuildingBlocks
{
    public static class RandomExtensions
    {
        public static T Next<T>(this Random source, T[] array)
            => array[source.Next() % array.Length];
    }
}
