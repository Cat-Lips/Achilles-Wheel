#if TOOLS
using Godot.Collections;

namespace GodotSharp.BuildingBlocks.Terrain1.Lague
{
    public partial class LagueTerrain
    {
        public override void _ValidateProperty(Dictionary property)
        {
            if (Editor.Hide(property, PropertyName.ChunkSize)) return;
            if (Editor.Hide(property, PropertyName.IsleSlope, @if: !Islands)) return;
            if (Editor.Hide(property, PropertyName.IsleWater, @if: !Islands)) return;
            if (Editor.Hide(property, PropertyName.GlobalNormalMultiplier, @if: NormaliseMode is not NormaliseMode.Global)) return;
        }
    }
}
#endif
