#if TOOLS

using Godot.Collections;

namespace GodotSharp.BuildingBlocks
{
    public partial class PhysicsSync
    {
        public override void _ValidateProperty(Dictionary property)
        {
            if (Editor.Hide(property, PropertyName.Position)) return;
            if (Editor.Hide(property, PropertyName.Rotation)) return;
            if (Editor.Hide(property, PropertyName.LinearVelocity)) return;
            if (Editor.Hide(property, PropertyName.AngularVelocity)) return;
        }
    }
}
#endif
