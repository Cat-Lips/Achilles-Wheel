using Godot;

namespace GodotSharp.BuildingBlocks.Terrain1.Lague
{
    [Tool, GlobalClass]
    public partial class TerrainType : CustomResource
    {
        private string _name;

        private Texture2D _texture;
        private Color _tint;
        private float _height;
        private SlopeType _slope;
        private float _shaderHeight;

        private float _textureScale = 10;
        private float _tintStrength = 0;
        private float _blendStrength = .5f;

        [Export] public string Name { get => _name; set => this.Set(ref _name, value); }

        [Export] public Texture2D Texture { get => _texture; set => this.Set(ref _texture, value); }
        [Export(PropertyHint.ColorNoAlpha)] public Color Tint { get => _tint; set => this.Set(ref _tint, value); }
        [Export(PropertyHint.Range, "0,1")] public float Height { get => _height; set => this.Set(ref _height, value); }
        [Export(PropertyHint.Range, "0,1")] public float ShaderHeight { get => _shaderHeight; set => this.Set(ref _shaderHeight, value); }
        [Export] public SlopeType Slope { get => _slope; set => this.Set(ref _slope, value); }

        [Export] public float TextureScale { get => _textureScale; set => this.Set(ref _textureScale, value); }
        [Export(PropertyHint.Range, "0,1")] public float TintStrength { get => _tintStrength; set => this.Set(ref _tintStrength, value); }
        [Export(PropertyHint.Range, "0,1")] public float BlendStrength { get => _blendStrength; set => this.Set(ref _blendStrength, value); }

        public static TerrainType Default() => new()
        {
            Name = "Grass",
            Tint = Colors.LawnGreen,
            Slope = SlopeType.Flat,
        };

        private static TerrainType[] _defaultTerrainTypes;
        public static TerrainType[] DefaultTerrainTypes()
        {
            return _defaultTerrainTypes ??= DefaultTerrainTypes().Select(x =>
            {
                x.Texture = LoadTexture(x.Name);
                return x;
            }).ToArray();

            static IEnumerable<TerrainType> DefaultTerrainTypes()
            {
                yield return new() { Name = "DeepWater", Tint = Colors.DarkBlue, Height = .3f, Slope = SlopeType.Steep, ShaderHeight = 0, };
                yield return new() { Name = "ShallowWater", Tint = Colors.LightSeaGreen, Height = .4f, Slope = SlopeType.Gentle, ShaderHeight = .3f, };
                yield return new() { Name = "Sand", Tint = Colors.SandyBrown, Height = .45f, Slope = SlopeType.Gentle, ShaderHeight = .4f, };
                yield return new() { Name = "Grass", Tint = Colors.LawnGreen, Height = .55f, Slope = SlopeType.Gentle, ShaderHeight = .45f, };
                yield return new() { Name = "Forest", Tint = Colors.ForestGreen, Height = .6f, Slope = SlopeType.Medium, ShaderHeight = .55f, };
                yield return new() { Name = "LowRock", Tint = Colors.SaddleBrown, Height = .7f, Slope = SlopeType.Medium, ShaderHeight = .6f, };
                yield return new() { Name = "HighRock", Tint = Colors.SaddleBrown.Darkened(.3f), Height = .9f, Slope = SlopeType.Steep, ShaderHeight = .7f, };
                yield return new() { Name = "Snow", Tint = Colors.Snow, Height = 1, Slope = SlopeType.Steep, ShaderHeight = .9f, };
            }

            static Texture2D LoadTexture(string name)
                => GD.Load<Texture2D>(App.GetScriptDir<TerrainType>().PathJoin($"../Textures/{name}.png"));
        }

        public static Curve CreateTerrainCurve(TerrainType[] regions)
        {
            var curve = new Curve();
            curve.AddPoint(Vector2.Zero);
            GetPoints().ForEach(x => curve.AddPoint(x.Point, x.Tangent, x.Tangent));
            return curve;

            IEnumerable<(Vector2 Point, float Tangent)> GetPoints()
            {
                var maxHeight = 0f;
                var points = GetPoints().ToArray();
                return GetTangentsAndNormaliseHeights().ToArray();

                IEnumerable<Vector2> GetPoints()
                {
                    var lastHeight = 0f;
                    foreach (var region in regions)
                    {
                        var newHeight = lastHeight + region.Height * Gradient(region.Slope);
                        yield return new(region.Height, newHeight);
                        if (newHeight > maxHeight) maxHeight = newHeight;
                        lastHeight = newHeight;
                    }

                    static float Gradient(SlopeType slope) => slope switch
                    {
                        SlopeType.Flat => 0,
                        SlopeType.Gentle => .1f,
                        SlopeType.Medium => .25f,
                        SlopeType.Steep => 1,
                        _ => throw new NotImplementedException(),
                    };
                }

                IEnumerable<(Vector2 Point, float Tangent)> GetTangentsAndNormaliseHeights()
                {
                    var lastHeight = 0f;
                    var baseTangent = Mathf.DegToRad(45);
                    foreach (var p in points)
                    {
                        var normalisedHeight = p.Y is 0 ? 0 : p.Y / maxHeight;
                        var tangent = baseTangent * (normalisedHeight - lastHeight);
                        yield return (new(p.X, normalisedHeight), tangent);
                        lastHeight = normalisedHeight;
                    }
                }
            }
        }
    }
}
