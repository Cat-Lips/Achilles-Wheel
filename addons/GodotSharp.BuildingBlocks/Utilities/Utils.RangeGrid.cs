using Godot;

namespace GodotSharp.BuildingBlocks
{
    public static partial class Utils
    {
        public static IEnumerable<(int x, int y)> RangeGrid(int end) => RangeGrid(0, end, 1);
        public static IEnumerable<(int x, int y)> RangeGrid(int start, int end) => RangeGrid(start, end, 1);
        public static IEnumerable<(int x, int y)> RangeGrid(int start, int end, int step)
        {
            ++end; // Inclusive
            foreach (var x in GD.Range(start, end, step))
            {
                foreach (var y in GD.Range(start, end, step))
                    yield return (x, y);
            }
        }
    }
}
