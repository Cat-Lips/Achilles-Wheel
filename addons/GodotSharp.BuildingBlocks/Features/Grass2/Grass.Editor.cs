#if TOOLS
using Godot.Collections;

namespace GodotSharp.BuildingBlocks.Grass2
{
    public partial class Grass
    {
        public override void _ValidateProperty(Dictionary property)
        {
            if (Editor.Hide(property, PropertyName.Multimesh)) return;
        }
    }
}
#endif
