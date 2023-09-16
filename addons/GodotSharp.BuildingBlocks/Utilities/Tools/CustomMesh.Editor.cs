#if TOOLS

using Godot.Collections;

namespace GodotSharp.BuildingBlocks
{
    public partial class CustomMesh
    {
        public override void _ValidateProperty(Dictionary property)
        {
            if (Editor.Hide(property, "mesh")) return;
            if (Editor.Hide(property, "Mesh")) return;
        }
    }
}
#endif
