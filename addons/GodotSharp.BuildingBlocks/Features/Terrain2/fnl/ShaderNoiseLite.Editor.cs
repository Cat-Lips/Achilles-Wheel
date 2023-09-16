#if TOOLS
using Godot.Collections;

namespace GodotSharp.BuildingBlocks.Terrain2.Internal
{
    public partial class ShaderNoiseLite
    {
        public override void _ValidateProperty(Dictionary property)
        {
            if (Editor.Hide(property, PropertyName.Octaves, @if: FractalType is FractalType.None)) return;
            if (Editor.Hide(property, PropertyName.Lacunarity, @if: FractalType is FractalType.None)) return;
            if (Editor.Hide(property, PropertyName.WeightedStrength, @if: FractalType is FractalType.None)) return;
            if (Editor.Show(property, PropertyName.PingPongStrength, @if: FractalType is FractalType.PingPong)) return;
        }
    }
}
#endif
