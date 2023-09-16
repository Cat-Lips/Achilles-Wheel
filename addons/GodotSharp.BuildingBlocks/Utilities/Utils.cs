using Godot;

namespace GodotSharp.BuildingBlocks
{
    public static partial class Utils
    {
        public static int TrueRound(float value) => (int)Math.Floor(value + .5);
        public static Vector2 TrueRound(this Vector2 source) => new(TrueRound(source.X), TrueRound(source.Y));
        public static Vector3 TrueRound(this Vector3 source) => new(TrueRound(source.X), TrueRound(source.Y), TrueRound(source.Z));
    }
}
