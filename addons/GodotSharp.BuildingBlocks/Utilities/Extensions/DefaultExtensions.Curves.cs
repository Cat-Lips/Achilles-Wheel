using Godot;
using static Godot.Curve;
using static Godot.CurveTexture;

namespace GodotSharp.BuildingBlocks
{
    public static partial class Default
    {
        public static Curve Curve(IEnumerable<Vector2> points)
        {
            var curve = new Curve();
            points.ForEach(x => curve.AddPoint(x, leftMode: TangentMode.Linear, rightMode: TangentMode.Linear));
            return curve;
        }

        public static Curve Curve(IEnumerable<(Vector2 Point, float Tangent)> points)
        {
            var curve = new Curve();
            points.ForEach(x => curve.AddPoint(x.Point, x.Tangent, x.Tangent));
            return curve;
        }

        public static Curve Curve(IEnumerable<(Vector2 Point, float LeftTangent, float RightTangent)> points)
        {
            var curve = new Curve();
            points.ForEach(x => curve.AddPoint(x.Point, x.LeftTangent, x.RightTangent));
            return curve;
        }

        public static CurveTexture CurveTexture(Curve curve) => new()
        {
            Curve = curve,
            TextureMode = TextureModeEnum.Red,
        };

        public static CurveXyzTexture CurveTexture(Curve x, Curve y, Curve z = null) => new()
        {
            CurveX = x,
            CurveY = y,
            CurveZ = z,
        };
    }
}
