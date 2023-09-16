using Godot;

namespace GodotSharp.BuildingBlocks.Terrain2
{
    public partial class TerrainData
    {
        private Curve DefaultHeightCurve()
        {
            return Default.Curve(Points());

            Vector2[] Points()
            {
                var (points, maxHeight) = Points();
                return Normalise(points, maxHeight);

                (Vector2[] Points, float MaxHeight) Points()
                {
                    var maxHeight = 0f;
                    return (Points().ToArray(), maxHeight);

                    IEnumerable<Vector2> Points()
                    {
                        yield return Vector2.Zero;

                        var lastHeight = 0f;
                        foreach (var (height, gradient) in Heights()
                            .DistinctBy(x => x.Height)
                            .OrderBy(x => x.Height))
                        {
                            var newHeight = lastHeight + height * gradient;
                            yield return new(height, newHeight);
                            lastHeight = newHeight;

                            maxHeight = Math.Max(maxHeight, newHeight);
                        }

                        IEnumerable<(float Height, float Gradient)> Heights()
                            => Regions.Select(x => (x.MaxHeight, x.Gradient));
                    }
                }

                Vector2[] Normalise(Vector2[] points, float maxHeight)
                {
                    return points.Select(p => new Vector2(p.X, Normalise(p.Y))).ToArray();

                    float Normalise(float y)
                        => y is 0 ? 0 : y / maxHeight;
                }
            }
        }
    }
}
