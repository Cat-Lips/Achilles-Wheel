using Godot;

namespace GodotSharp.BuildingBlocks
{
    public static class MathV
    {
        public static Vector2 Vec2(float v) => new(v, v);
        public static Vector3 Vec3(float v) => new(v, v, v);
        public static Vector3 Vec3(float x, float z) => new(x, 0, z);

        private static T Min<T>(params T[] values) => values.Min();
        public static float Min(this in Vector2 source) => Min(source.X, source.Y);
        public static float Min(this in Vector3 source) => Min(source.X, source.Y, source.Z);
        public static int Min(this in Vector2I source) => Min(source.X, source.Y);
        public static int Min(this in Vector3I source) => Min(source.X, source.Y, source.Z);

        private static T Max<T>(params T[] values) => values.Max();
        public static float Max(this in Vector2 source) => Max(source.X, source.Y);
        public static float Max(this in Vector3 source) => Max(source.X, source.Y, source.Z);
        public static int Max(this in Vector2I source) => Max(source.X, source.Y);
        public static int Max(this in Vector3I source) => Max(source.X, source.Y, source.Z);

        public static Vector2 Min(this in Vector2 source, in Vector2 other) => new(Min(source.X, other.X), Min(source.Y, other.Y));
        public static Vector3 Min(this in Vector3 source, in Vector3 other) => new(Min(source.X, other.X), Min(source.Y, other.Y), Min(source.Z, other.Z));
        public static Vector2I Min(this in Vector2I source, in Vector2I other) => new(Min(source.X, other.X), Min(source.Y, other.Y));
        public static Vector3I Min(this in Vector3I source, in Vector3I other) => new(Min(source.X, other.X), Min(source.Y, other.Y), Min(source.Z, other.Z));

        public static Vector2 Max(this in Vector2 source, in Vector2 other) => new(Max(source.X, other.X), Max(source.Y, other.Y));
        public static Vector3 Max(this in Vector3 source, in Vector3 other) => new(Max(source.X, other.X), Max(source.Y, other.Y), Max(source.Z, other.Z));
        public static Vector2I Max(this in Vector2I source, in Vector2I other) => new(Max(source.X, other.X), Max(source.Y, other.Y));
        public static Vector3I Max(this in Vector3I source, in Vector3I other) => new(Max(source.X, other.X), Max(source.Y, other.Y), Max(source.Z, other.Z));

        public static Vector2 Clamp(this in Vector2 source, in Vector2 max) => source.Clamp(Vector2.Zero, max);
        public static Vector3 Clamp(this in Vector3 source, in Vector3 max) => source.Clamp(Vector3.Zero, max);
        public static Vector2I Clamp(this in Vector2I source, in Vector2I max) => source.Clamp(Vector2I.Zero, max);
        public static Vector3I Clamp(this in Vector3I source, in Vector3I max) => source.Clamp(Vector3I.Zero, max);

        public static Vector2 XZ(this in Vector3 source) => new(source.X, source.Z);
        public static Vector3 XZ(this in Vector3 source, float y) => new(source.X, y, source.Z);

        public static IEnumerable<(Vector2I Cell, int Ring)> Spiral(int radius) => Vector2I.Zero.Spiral(radius);
        public static IEnumerable<(Vector2I Cell, int Ring)> Spiral(int x, int z, int radius) => new Vector2I(x, z).Spiral(radius);
        public static IEnumerable<(Vector2I Cell, int Ring)> Spiral(this Vector2I origin, int radius)
        {
            var x = 0;
            var y = 0;
            var leg = 0;
            var layer = 1;

            if (radius is 0) yield break;
            yield return (origin, 0);
            var absOrigin = origin.Abs();

            while (true)
            {
                var next = Next();
                if (x == radius) break;
                yield return (next, Ring(next));
            }

            Vector2I Next()
            {
                switch (leg)
                {
                    case 0:
                        if (++x == layer) ++leg;
                        break;
                    case 1:
                        if (++y == layer) ++leg;
                        break;
                    case 2:
                        if (--x == -layer) ++leg;
                        break;
                    case 3:
                        if (--y == -layer) { leg = 0; ++layer; }
                        break;
                }

                return origin + new Vector2I(x, y);
            }

            int Ring(in Vector2I cell)
                => (cell.Abs() - absOrigin).Max();
        }
    }
}
