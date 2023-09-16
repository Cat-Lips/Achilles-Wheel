#if TOOLS
using Godot;
using GodotSharp.BuildingBlocks.Terrain2;

namespace GodotSharp.BuildingBlocks
{
    [Tool]
    public partial class Plugin : EditorPlugin
    {
        public override void _EnterTree()
        {
            TerrainPosition.Add();
            PhysicsImport.Add(this);
        }

        public override void _ExitTree()
        {
            TerrainPosition.Remove();
            PhysicsImport.Remove(this);
        }
    }
}
#endif
