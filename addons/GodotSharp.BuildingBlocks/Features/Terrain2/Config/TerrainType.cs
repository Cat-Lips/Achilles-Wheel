using Godot;

namespace GodotSharp.BuildingBlocks.Terrain2
{
    [Tool, GlobalClass]
    public partial class TerrainType : CustomResource
    {
        private string _name;

        private Color _tint;
        private Texture2D _texture;
        private float _gradient = 1;

        private float _minSlope = 0;
        private float _maxSlope = 1;
        private float _minHeight = 0;
        private float _maxHeight = 1;

        private float _textureScale = DefaultTextureScale;
        private float _tintStrength = DefaultTintStrength;
        private float _blendStrength = DefaultBlendStrength;

        [Export] public string Name { get => _name; set => this.Set(ref _name, value); }

        [Export(PropertyHint.ColorNoAlpha)] public Color Tint { get => _tint; set => this.Set(ref _tint, value); }
        [Export] public Texture2D Texture { get => _texture; set => this.Set(ref _texture, value); }
        [Export(PropertyHint.Range, "0,1")] public float Gradient { get => _gradient; set => this.Set(ref _gradient, value); }

        [Export(PropertyHint.Range, "0,1")] public float MinSlope { get => _minSlope; set => this.Set(ref _minSlope, Math.Clamp(value, 0, MaxSlope)); }
        [Export(PropertyHint.Range, "0,1")] public float MaxSlope { get => _maxSlope; set => this.Set(ref _maxSlope, Math.Clamp(value, MinSlope, 1)); }
        [Export(PropertyHint.Range, "0,1")] public float MinHeight { get => _minHeight; set => this.Set(ref _minHeight, Math.Clamp(value, 0, MaxHeight)); }
        [Export(PropertyHint.Range, "0,1")] public float MaxHeight { get => _maxHeight; set => this.Set(ref _maxHeight, Math.Clamp(value, MinHeight, 1)); }

        [Export] public float TextureScale { get => _textureScale; set => this.Set(ref _textureScale, value); }
        [Export(PropertyHint.Range, "0,1")] public float TintStrength { get => _tintStrength; set => this.Set(ref _tintStrength, value); }
        [Export(PropertyHint.Range, "0,1,exp")] public float BlendStrength { get => _blendStrength; set => this.Set(ref _blendStrength, value); }
    }
}
