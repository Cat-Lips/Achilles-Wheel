#if TOOLS
using Godot.Collections;

namespace GodotSharp.BuildingBlocks.Terrain2
{
    public partial class TerrainData
    {
        public override void _ValidateProperty(Dictionary property)
        {
            if (Editor.Show(property, PropertyName.HeightMap, @if: Noise is null)) return;
            if (Editor.Show(property, PropertyName.NormalMap, @if: Noise is null)) return;
            if (Editor.Show(property, PropertyName.Noise, @if: HeightMap is null)) return;

            if (Editor.SetDisplayOnly(property, PropertyName.RegionGradients)) return;
            if (Editor.SetDisplayOnly(property, PropertyName.RegionTintStrength)) return;
            if (Editor.SetDisplayOnly(property, PropertyName.RegionTextureScale)) return;
            if (Editor.SetDisplayOnly(property, PropertyName.RegionBlendStrength)) return;

            if (Editor.SetDisplayOnly(property, PropertyName.RegionDefaultType)) return;
            if (Editor.SetDisplayOnly(property, PropertyName.RegionTintFromTexture)) return;
        }
    }
}
#endif
